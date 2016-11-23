using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace PowerToolsService
{
	public partial class Service : ServiceBase
	{
		public static string ASSEMBLY_DIRECTORY = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		public ServiceController _serviceController;

		public Service()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			_serviceController = new ServiceController(Path.Combine(ASSEMBLY_DIRECTORY, "services.conf"));
		}

		protected override void OnStop() {}
	}
}
