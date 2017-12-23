using System;

namespace Coralcode.Framework.Log
{
    /// <summary>
    /// Common contract for trace instrumentation. You 
    /// can implement this contrat with several frameworks.
    /// .NET Diagnostics API, EntLib, Log4Net,NLog etc.
    /// <remarks>
    /// The use of this abstraction depends on the real needs you have and the specific features  
    /// you want to use of a particular existing implementation. 
    ///  You could also eliminate this abstraction and directly use "any" implementation in your code, 
    /// Logger.Write(new LogEntry()) in EntLib, or LogManager.GetLog("logger-name") with log4net... etc.
    /// </remarks>
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        ///写调试信息
        /// </summary>
        void Debug(string message, params object[] args);

        /// <summary>
        /// 写带异常的Debug信息
        /// </summary>
        void Debug(string message, Exception exception, params object[] args);


        /// <summary>
        ///写崩溃信息
        /// </summary>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// 写带异常崩溃信息
        /// </summary>
        void Fatal(string message, Exception exception, params object[] args);

        /// <summary>
        /// 写基本信息
        /// </summary>
        void Info(string message, params object[] args);

        /// <summary>
        /// 写带异常的基本信息
        /// </summary>
        void Info(string message, Exception exception, params object[] args);

        /// <summary>
        /// 写警告信息
        /// </summary>
        void Warning(string message, params object[] args);

        /// <summary>
        /// 写带异常的基本信息
        /// </summary>
        void Warning(string message, Exception exception, params object[] args);

        /// <summary>
        ///写错误信息
        /// </summary>
        void Error(string message, params object[] args);

        /// <summary>
        ///写带异常的错误信息
        /// </summary>
        void Error(string message, Exception exception, params object[] args);

    }
}
