using System;
using System.IO;

namespace PowerToolsService.Models
{
	public abstract class PowerToolsServiceContainer : IPowerToolsServiceContainer
	{
		protected PowerToolsServiceContainer()
		{
			Random rand = new Random(DateTime.UtcNow.Millisecond);
			ServiceID = rand.Next(1, 1000);
		}

		public int ServiceID { get; }
		public string ServiceContainerName { get; set; }
		public string Identity { get; set; }
		public bool Enabled { get; set; }
		public string DisplayName { get; set; }
		public string ExecutionPath { get; set; }
		public string FilePath { get; set; }
		public string[] Arguments { get; set; }

		public string Validate()
		{
			if (!File.Exists(this.ExecutionPath))
			{
				return String.Format("The execution path '{1}' for service '{0}' is not valid (likely does not exist).", this.DisplayName, this.ExecutionPath);
			}
			else if (!File.Exists(this.FilePath))
			{
				return String.Format("The file path '{1}' for service '{0}' is not valid (likely does not exist).", this.DisplayName, this.FilePath);
			}
			return "";
		}
	}

	public interface IPowerToolsServiceContainer
	{
		int ServiceID { get; }
		string ServiceContainerName { get; set; }
		string Identity { get; set; }
		bool Enabled { get; set; }
		string DisplayName { get; set; }
		string ExecutionPath { get; set; }
		string FilePath { get; set; }
		string[] Arguments { get; set; }
	}
}
