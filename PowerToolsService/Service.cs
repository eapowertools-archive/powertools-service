using System.Collections;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace PowerToolsService
{
	public partial class Service : ServiceBase
	{
		public Service()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			LoadServicesFile();
		}

		private void LoadServicesFile()
		{
			string configDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string configPath = Path.Combine(configDir, "services.conf");
			string configText = File.ReadAllText(configPath);

			IEnumerable<IserviceCon> >
			
		}

		protected override void OnStop()
		{
		}
	}
}
