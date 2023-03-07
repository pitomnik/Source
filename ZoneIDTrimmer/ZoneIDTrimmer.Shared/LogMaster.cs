using log4net;
using log4net.Config;
using System.IO;

namespace ZoneIDTrimmer.Shared
{
    public class LogMaster
    {
        public static readonly ILog Instance;

        static LogMaster()
        {
            if (File.Exists(ConfigManager.Instance.ConfigFile))
            {
                var logFileInfo = new FileInfo(ConfigManager.Instance.ConfigFile);

                XmlConfigurator.ConfigureAndWatch(logFileInfo);
            }
            else
            {
                XmlConfigurator.Configure();
            }

            Instance = LogManager.GetLogger("LogMaster");
        }
    }
}
