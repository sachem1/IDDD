using System;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Reflection;
using log4net;

namespace Coralcode.Framework.Log
{
    /// <summary>
    /// Log Factory
    /// </summary>
    public static class LoggerFactory
    {
        public static ILogger Instance { get; }
        static LoggerFactory()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(AppConfig.GetAbsolutePath("log4net")));
            var log = LogManager.GetLogger("Default");
            Instance = new Log4Net(log);
        }

        public static ILogger GetLogger(string loggerName)
        {
            var log = LogManager.Exists(loggerName) ?? LogManager.GetLogger(loggerName);
            return new Log4Net(log);
        }

    }
}
