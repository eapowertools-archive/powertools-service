using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PowerToolsService.Models;

namespace PowerToolsService
{
	public class ServiceController
	{
		public static string SERVICE_CONTAINER_KEY = "ServiceContainerName";

		private IEnumerable<Thread> _serviceThreads = new List<Thread>();

		public ServiceController(string configFile)
		{
			IList<IDictionary<string, string>> serviceList = this.GetServiceTextList(configFile);

			IList<IPowerToolsServiceContainer> serviceContainers = this.CreateServiceContainers(serviceList);
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
					powertoolServices.Add(new NodeServiceContainer()
					{
						ServiceContainerName = serviceDictionary[SERVICE_CONTAINER_KEY],
						DisplayName = serviceDictionary["DisplayName"],
						Enabled = Boolean.Parse(serviceDictionary["Enabled"]),
						ExecutionPath = Path.Combine(Service.ASSEMBLY_DIRECTORY, serviceDictionary["ExePath"]),
						FilePath = Path.Combine(Service.ASSEMBLY_DIRECTORY, serviceDictionary["Script"]),
						Identity = serviceDictionary["Identity"]
					});
				}
				else
				{
					throw new NotImplementedException("Only services of 'ExecType' equal to 'nodejs' are currently supported.");
				}
			}

			return powertoolServices;
		}
	}
}
