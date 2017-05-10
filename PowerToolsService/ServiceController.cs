using System;
using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;
using PowerToolsService.Logging;
using PowerToolsService.Models;

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

			PowerToolServices serviceList = this.GetServices(configFile);
			this.ValidateContainers(serviceList);
			this.CreateProcesses(serviceList);
		}

		private void CreateProcesses(PowerToolServices serviceContainers)
		{
			foreach (PowerToolsServiceContainer serviceContainer in serviceContainers.Containers)
			{
				ParentService.Logger.Log(String.Format("Starting service: {0}", serviceContainer.DisplayName), LogType.Info);
				ProcessController pc = new ProcessController(this, serviceContainer);
				pc.Start();
				_serviceProcesses.Add(pc);
			}
		}

		private PowerToolServices GetServices(string configFile)
		{
			PowerToolServices services;

			using (StreamReader r = new StreamReader(configFile))
			{
				string json = r.ReadToEnd();
				services = JsonConvert.DeserializeObject<PowerToolServices>(json);
			}

			return services;
		}

		private void ValidateContainers(PowerToolServices serviceList)
		{
			foreach (PowerToolsServiceContainer serviceContainer in serviceList.Containers)
			{
				if (serviceContainer.ExeType.ToLower().Equals("nodejs"))
				{
					try
					{
						string containerValidationError = serviceContainer.Validate();

						if (string.IsNullOrWhiteSpace(containerValidationError))
						{
							ParentService.Logger.Log(String.Format("Found service '{0}' in config file.", serviceContainer.DisplayName), LogType.Info);
						}
						else
						{
							ParentService.Logger.Log(String.Format("Could not parse service '{0}' because: '{0}'", containerValidationError), LogType.Info);
						}
					}
					catch (Exception e)
					{
						ParentService.Logger.Log(String.Format("Failed during validation. Message: {0}", e.Message), LogType.Error);
					}
				}
				else
				{
					ParentService.Logger.Log("Only services of 'ExecType' equal to 'nodejs' are currently supported.", LogType.Error);
					throw new NotImplementedException("Only services of 'ExecType' equal to 'nodejs' are currently supported.");
				}
			}
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
