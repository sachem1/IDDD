namespace Coralcode.Framework.Common
{
    public static partial class StaticString
    {
        public const string ChildrenContexsString = "ChildrenContexsKey";
        public const string DefaultContextString = "Redis";
        public const string PlatformContextString = "PlatformContext";
        public const string AppContextString = "AppContext";
        public const string UserContextString = "UserContext";
        public const string SessionContextString = "SessionContext";
        public const string PageContextString = "PageContext";
        public const string AppsString = "Apps";

        /// <summary>
        /// 默认的reids配置dll
        /// </summary>
        public const string DefaultRedisConfigDll = "Coralcode.Framework";
        /// <summary>
        /// 默认的reids配置路径
        /// </summary>
        public const string DefautlRedisConfigSection = "Redis";

        #region 日志相关

        public const string LogContextString = "Log";
        /// <summary>
        /// The log enable trace SQL string
        /// </summary>
        public const string LogEnableTraceSQLString = "EnableTraceSQL";
        /// <summary>
        /// The log enable trace time consuming string
        /// </summary>
        public const string LogEnableTraceTimeConsumingString = "EnableTraceTimeConsuming";
        /// <summary>
        /// The log enable trace user behavior string
        /// </summary>
        public const string LogEnableTraceUserBehaviorString = "EnableTraceUserBehavior";

        #endregion
    }
}
