using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Coralcode.Framework.ConfigManager;
using log4net;
using log4net.Core;

namespace Coralcode.Framework.Log
{
    [Description("log4net")]
    internal sealed class Log4Net : ILogger
    {
        private readonly ILog _log;

        public Log4Net(ILog log)
        {
            _log = log;
        }

        public void Debug(string message, params object[] args)
        {
            _log.DebugFormat(CultureInfo.InvariantCulture, message, args);
        }

        public void Debug(string message, Exception exception, params object[] args)
        {
            var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);
            _log.Debug(messageToTrace, exception);
        }

        public void Fatal(string message, params object[] args)
        {
            _log.FatalFormat(CultureInfo.InvariantCulture, message, args);
        }

        public void Fatal(string message, Exception exception, params object[] args)
        {
            var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);
            _log.Fatal(messageToTrace, exception);
        }

        public void Info(string message, params object[] args)
        {
            _log.InfoFormat(CultureInfo.InvariantCulture, message, args);
        }

        public void Info(string message, Exception exception, params object[] args)
        {
            var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);
            _log.Info(messageToTrace, exception);
        }

        public void Warning(string message, params object[] args)
        {
            _log.WarnFormat(CultureInfo.InvariantCulture, message, args);
        }

        public void Warning(string message, Exception exception, params object[] args)
        {
            var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);
            _log.Warn(messageToTrace, exception);
        }

        public void Error(string message, params object[] args)
        {
            _log.ErrorFormat(CultureInfo.InvariantCulture, message, args);
        }

        public void Error(string message, Exception exception, params object[] args)
        {
            var messageToTrace = string.Format(CultureInfo.InvariantCulture, message, args);
            _log.Error(messageToTrace, exception); ;
        }
    }
}