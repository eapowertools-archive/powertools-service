using System;

namespace PowerToolsService.Models
{
	public class NodeServiceContainer : IPowerToolsServiceContainer
	{
		public NodeServiceContainer()
		{
			Random rand = new Random(DateTime.UtcNow.Millisecond);
			ServiceID = rand.Next(1, 1000);
		}

		public int ServiceID { get; set; }
		public string ServiceContainerName { get; set; }
		public string Identity { get; set; }
		public bool Enabled { get; set; }
		public string DisplayName { get; set; }
		public string ExecutionPath { get; set; }
		public string FilePath { get; set; }
	}
}
