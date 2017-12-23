using System;
using System.Threading;
using Coralcode.Framework.Cache;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;

namespace Coralcode.Framework.Contexts
{
    /// <summary>
    /// 页面上下文
    /// </summary>
    public class PageContext : Context
    {
        internal PageContext(SessionContext sessionContext, string pageKey, ICache cache)
        {
            SessionContext = sessionContext;
            //ClearEventHandle += item =>
            //{
            //    SessionContext.RemoveChild(pageKey);
            //};
            CacheKey = string.Format("{0}-{1}", sessionContext.CacheKey, pageKey);
            Cache = cache;
            PageKey = pageKey;
        }
        public string PageKey { get; private set; }

        private static readonly ThreadLocal<PageContext> _current = new ThreadLocal<PageContext>();

        public static PageContext Current
        {
            get
            {
                if (_current == null)
                    return null;
                return _current.Value;
            }
            set { _current.Value = value; }
        }

        /// <summary>
        /// 会话上下文
        /// </summary>
        public SessionContext SessionContext { get; private set; }

        /// <summary>
        /// 用户上下文
        /// </summary>
        public UserContext UserContext
        {
            get { return SessionContext.UserContext; }
        }

        /// <summary>
        /// 应用上下文
        /// </summary>
        public AppContext AppContext
        {
            get { return UserContext.AppContext; }
        }


        protected override int TimeOut
        {
            get
            {
                return
                    CoralConvert.Convert<int>(
                        AppConfig.DllConfigs.Current[StaticString.PageContextString][StaticString.TimeoutString]);
            }
        }
    }
}
