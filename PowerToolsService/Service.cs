using System.IO;
using System.Reflection;
using System.ServiceProcess;
using log4net.Core;
using PowerToolsService.Logging;

namespace PowerToolsService
{
	public partial class Service : ServiceBase
	{
		public static string ASSEMBLY_DIRECTORY = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		public ServiceController _serviceController;
		private readonly FileLogger _logger;

		public Service()
		{
			_logger = new FileLogger();
			_logger.SetLogLevel(Level.Info);
			InitializeComponent();
		}

		public FileLogger Logger
		{
			get { return _logger; }
		}

		protected override void OnStart(string[] args)
		{
			_logger.Log("Starting up the EAPowerTools Service Manager.", LogType.Info);
			if (args.Length > 1)
			{
				_logger.Log("Invalid number of startup parameters. Can only have one of the following log levels for startup parameters: 'OFF','FATAL','ERROR','WARN','INFO','DEBUG','ALL'. Will default to 'Info' level logging.", LogType.Error);
			}
			else if (args.Length == 1)
			{
				switch (args[0].ToUpper())
				{
					case "OFF":
						_logger.SetLogLevel(Level.Off);
						break;
					case "FATAL":
						_logger.SetLogLevel(Level.Fatal);
						break;
					case "ERROR":
						_logger.SetLogLevel(Level.Error);
						break;
					case "WARN":
						_logger.SetLogLevel(Level.Warn);
						break;
					case "INFO":
						_logger.SetLogLevel(Level.Info);
						break;
					case "DEBUG":
						_logger.SetLogLevel(Level.Debug);
						break;
					case "ALL":
						_logger.SetLogLevel(Level.All);
						break;
					default:
						_logger.Log("Invalid startup parameter. The following are valid log level startup parameters: 'OFF','FATAL','ERROR','WARN','INFO','DEBUG','ALL'. Will default to 'Info' level logging.", LogType.Error);
						break;
				}
			}
			else
			{
				_logger.Log("Using default value of 'Info' for log level", LogType.Info);
			}

			_serviceController = new ServiceController(this, Path.Combine(ASSEMBLY_DIRECTORY, "services.conf"));
		}

		protected override void OnStop()
		{
			_serviceController.StopAll();
		}
	}
}
