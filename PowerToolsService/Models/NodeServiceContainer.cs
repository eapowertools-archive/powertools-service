namespace PowerToolsService.Models
{
	public class NodeServiceContainer : IPowerToolsServiceContainer
	{
		public string ServiceContainerName { get; set; }
		public string Identity { get; set; }
		public bool Enabled { get; set; }
		public string DisplayName { get; set; }
		public string ExecutionPath { get; set; }
		public string FilePath { get; set; }
	}
}
