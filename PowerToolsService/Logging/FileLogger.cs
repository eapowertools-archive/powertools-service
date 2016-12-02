using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace PowerToolsService.Logging
{
	public enum LogType
	{
		Info,
		Debug,
		Error,
		Fatal,
		Warn
	}

	public class FileLogger
	{
		public static string LOG_FILE_PATH = Path.Combine(Service.ASSEMBLY_DIRECTORY, "Log/service_log.log");
		private readonly ILog _log;

		public FileLogger()
		{
			RollingFileAppender appender = new RollingFileAppender();
			appender.Name = "File Logger";
			appender.File = LOG_FILE_PATH;
			appender.AppendToFile = false;
			appender.MaxSizeRollBackups = 3;
			appender.MaxFileSize = 10 * 1024 * 1024;
			appender.StaticLogFileName = true;
			appender.PreserveLogFileNameExtension = true;
			appender.DatePattern = "_yyyy-MM-ddTHH.mm.ss.fffZ";

			PatternLayout layout = new PatternLayout();
			layout.Header = "Date\tLog Level\tService\tMessage" + Environment.NewLine;
			layout.ConversionPattern = "%utcdate{yyyy-MM-ddTHH:mm:ss.fffZ}\t%p\t%c\t%m%n";
			layout.ActivateOptions();

			appender.Layout = layout;
			appender.ActivateOptions();

			Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
			hierarchy.Root.AddAppender(appender);
			hierarchy.Configured = true;

			_log = LogManager.GetLogger("EAPowerToolsService");
		}

		public void SetLogLevel(Level logLevel)
		{
			Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
			hierarchy.Root.Level = logLevel;
			hierarchy.Configured = true;
		}

		public void Log(string message, LogType logType)
		{
			switch (logType)
			{
				case LogType.Info:
					_log.Info(message);
					break;
				case LogType.Debug:
					_log.Debug(message);
					break;
				case LogType.Error:
					_log.Error(message);
					break;
				case LogType.Fatal:
					_log.Fatal(message);
					break;
				case LogType.Warn:
					_log.Warn(message);
					break;
				default:
					throw new InvalidEnumArgumentException(string.Format("The enum '{0}' is not supported.", logType.ToString()));
					break;
			}
		}
	}
}
