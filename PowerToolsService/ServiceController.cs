using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PowerToolsService.Logging;
using PowerToolsService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerToolsService
{
	public class ServiceController
	{
		public static string SERVICE_CONTAINER_KEY = "ServiceContainerName";

		private readonly ConcurrentBag<ProcessController> _serviceProcesses = new ConcurrentBag<ProcessController>();
		public Service ParentService { get; private set; }

		public ServiceController(Service parentService, string configFile)
		{
			ParentService = parentService;

			IList<IDictionary<string, string>> serviceList = this.GetServiceTextList(configFile);
			IList<IPowerToolsServiceContainer> serviceContainers = this.CreateServiceContainers(serviceList);

			this.CreateProcesses(serviceContainers);
		}

		private void CreateProcesses(IList<IPowerToolsServiceContainer> serviceContainers)
		{
			foreach (IPowerToolsServiceContainer serviceContainer in serviceContainers)
			{
				ParentService.Logger.Log(String.Format("Starting service: {0}", serviceContainer.DisplayName), LogType.Info);
				ProcessController pc = new ProcessController(this, serviceContainer);
				pc.Start();
				_serviceProcesses.Add(pc);
			}
		}

		private IList<IDictionary<string, string>> GetServiceTextList(string configFile)
		{
			IList<IDictionary<string, string>> services = new List<IDictionary<string, string>>();

			using (StreamReader reader = new StreamReader(configFile))
			{
				string line = "";
				while ((line = reader.ReadLine()) != null)
				{
					if (string.IsNullOrWhiteSpace(line))
					{
						continue;
					}

					IDictionary<string, string> serviceDictionary = new Dictionary<string, string>();
					while (!string.IsNullOrWhiteSpace(line))
					{
						int indexOfEquals = line.IndexOf("=");
						if (indexOfEquals > 0)
						{
							serviceDictionary.Add(line.Substring(0, indexOfEquals), line.Substring(indexOfEquals + 1, line.Length - indexOfEquals - 1));
						}
						else
						{
							serviceDictionary.Add(SERVICE_CONTAINER_KEY, line.Substring(1, line.Length - 2));
						}
						line = reader.ReadLine();
					}
					services.Add(serviceDictionary);
				}
			}

			return services;
		}

		private IList<IPowerToolsServiceContainer> CreateServiceContainers(IList<IDictionary<string, string>> serviceList)
		{
			IList<IPowerToolsServiceContainer> powertoolServices = new List<IPowerToolsServiceContainer>();
			foreach (IDictionary<string, string> serviceDictionary in serviceList)
			{
				if (serviceDictionary["ExecType"].ToLower().Equals("nodejs"))
				{
					try
					{
						NodeServiceContainer newContainer = new NodeServiceContainer()
						{
							ServiceContainerName = serviceDictionary[SERVICE_CONTAINER_KEY],
							DisplayName = serviceDictionary["DisplayName"],
							Enabled = Boolean.Parse(serviceDictionary["Enabled"]),
							ExecutionPath = Path.IsPathRooted(serviceDictionary["ExePath"]) ? serviceDictionary["ExePath"] : Path.Combine(Service.ASSEMBLY_DIRECTORY, serviceDictionary["ExePath"]),
							FilePath = Path.IsPathRooted(serviceDictionary["Script"]) ? serviceDictionary["Script"] : Path.Combine(Service.ASSEMBLY_DIRECTORY, serviceDictionary["Script"]),
							Identity = serviceDictionary["Identity"],
							Arguments = serviceDictionary.ContainsKey("Args") ?
								JsonConvert.DeserializeObject<JObject>("{\"Arguments\":" + serviceDictionary["Args"] + "}").Children().First().First.ToObject<string[]>() :
								new string[0]
						};

						string containerValidationError = newContainer.Validate();

						if (string.IsNullOrWhiteSpace(containerValidationError))
						{
							ParentService.Logger.Log(String.Format("Found service '{0}' in config file.", newContainer.DisplayName), LogType.Info);
							powertoolServices.Add(newContainer);
						}
						else
						{
							ParentService.Logger.Log(String.Format("Could not parse service '{0}' because: '{0}'", containerValidationError), LogType.Info);
						}
					}
					catch (Exception e)
					{
						ParentService.Logger.Log(String.Format("Tried to parse the config '{0}' as a valid service but failed. Exception message: '{1}'", string.Join(",", serviceDictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray()), e.Message), LogType.Error);
					}
				}
				else
				{
					throw new NotImplementedException("Only services of 'ExecType' equal to 'nodejs' are currently supported.");
				}
			}

			return powertoolServices;
		}

		public void StopAll()
		{
			ParentService.Logger.Log("Stopping all services.", LogType.Info);
			foreach (ProcessController serviceProcess in _serviceProcesses)
			{
				serviceProcess.Stop();
			}
		}
	}
}
