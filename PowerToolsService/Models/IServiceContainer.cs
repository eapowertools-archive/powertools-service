namespace PowerToolsService.Models
{
	public interface IPowerToolsServiceContainer
	{
		string ServiceContainerName { get; set; }
		string Identity { get; set; }
		bool Enabled { get; set; }
		string DisplayName { get; set; }
		string ExecutionPath { get; set; }
		string FilePath { get; set; }
	}
}
