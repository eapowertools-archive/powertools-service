using System;
using System.Diagnostics;
using PowerToolsService.Logging;
using PowerToolsService.Models;

namespace PowerToolsService
{
	public class ProcessController
	{
		private readonly ServiceController _serviceController;
		private readonly Process _process;
		private readonly IPowerToolsServiceContainer _serviceContainer;

		public ProcessController(ServiceController serviceController, IPowerToolsServiceContainer serviceContainer)
		{
			_serviceController = serviceController;
			_serviceContainer = serviceContainer;

			_process = new Process
			{
				StartInfo =
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					FileName = _serviceContainer.ExecutionPath,
					Arguments = "\"" + _serviceContainer.FilePath + "\""
				}
			};
		}

		public string GetProcessDisplayName()
		{
			return _serviceContainer.DisplayName;
		}

		public void Start()
		{
			_process.Start();
		}

		public void Stop()
		{
			try
			{
				_process.Kill();
				_process.Close();
			}
			catch (Exception e)
			{
				_serviceController.ParentService.Logger.Log(String.Format("Cannot stop service '{0}', exception was thrown: '{1}'", _serviceContainer.DisplayName, e.Message), LogType.Error);
			}
		}
	}
}
