using System.Diagnostics;
using PowerToolsService.Models;

namespace PowerToolsService
{
	public class ProcessController
	{
		private readonly Process _process;
		private IPowerToolsServiceContainer _serviceContainer;

		public ProcessController(IPowerToolsServiceContainer container)
		{
			_serviceContainer = container;

			_process = new Process
			{
				StartInfo =
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					WindowStyle = ProcessWindowStyle.Hidden,
					FileName = container.ExecutionPath,
					Arguments = "\"" + container.FilePath + "\""
				}
			};
		}

		public void Start()
		{
			_process.Start();
		}

		public void Stop()
		{
			_process.Kill();
			_process.Close();
		}
	}
}
