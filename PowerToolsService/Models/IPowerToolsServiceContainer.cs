using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace PowerToolsService.Models
{
	public class PowerToolServices
	{
		[JsonProperty("configs")]
		public List<PowerToolsServiceContainer> Containers { get; set; }
	}

	public class PowerToolsServiceContainer : IPowerToolsServiceContainer
	{
		private string _filePath;
		private string _executionPath;

		protected PowerToolsServiceContainer()
		{
			Random rand = new Random(DateTime.UtcNow.Millisecond);
			ServiceID = rand.Next(1, 1000);
		}

		public int ServiceID { get; }
		public string Identity { get; set; }
		public bool Enabled { get; set; }
		public string DisplayName { get; set; }

		[JsonProperty("ExePath")]
		public string ExecutionPath
		{
			get { return _executionPath; }
			set { _executionPath = Path.IsPathRooted(value) ? value : Path.Combine(Service.ASSEMBLY_DIRECTORY, value); }
		}

		[JsonProperty("Script")]
		public string FilePath
		{
			get { return _filePath; }
			set { _filePath = Path.IsPathRooted(value) ? value : Path.Combine(Service.ASSEMBLY_DIRECTORY, value); }
		}

		public string ExeType { get; set; }

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
		string Identity { get; set; }
		bool Enabled { get; set; }
		string DisplayName { get; set; }
		string ExecutionPath { get; set; }
		string FilePath { get; set; }
		string ExeType { get; set; }
	}
}
