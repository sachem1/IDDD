using System;
using System.Collections.Generic;
using System.Threading;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Cache;
using Coralcode.Framework.Cache.Local;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;

namespace Coralcode.Framework.Contexts
{
    /// <summary>
    /// 会话上下文
    /// </summary>
    public class SessionContext : Context
    {

        internal SessionContext(UserContext userContext, string sessionKey, ICache cache)
        {
            UserContext = userContext;
            ClearEventHandle += item =>
            {
                //userContext.RemoveChild(sessionKey);
            };
            //Dispised += item =>
            //{
            //    userContext.RemoveChild(sessionKey);
            //};
            CacheKey = string.Format("{0}-{1}", userContext.CacheKey, sessionKey);
            Cache = cache;
            SessionKey = sessionKey;
        }

        public string SessionKey { get; private set; }


        private static ThreadLocal<SessionContext> _current = new ThreadLocal<SessionContext>();

        public static SessionContext Current
        {
            get
            {
                if (_current == null)
                    return null;
                return _current.Value;
            }
            set { _current.Value = value; }
        }
                
        public PageContext CreatePageContext(string pageKey)
        {
            //List<string> children = Get<List<string>>(StaticString.ChildrenContexsString) ?? new List<string>();
            //if (!children.Contains(pageKey))
            //{
            //    children.Add(pageKey);
            //    Set(StaticString.ChildrenContexsString, children);
            //}
            //暂时页面上下文用本地缓存
            PageContext.Current = new PageContext(this, pageKey, LocalCache.Default);
            return PageContext.Current;
        }

        //public bool HasChildExists(string pageKey)
        //{
        //    List<string> children = Get<List<string>>(StaticString.ChildrenContexsString);
        //    return children != null && children.Count != 0 && children.Contains(pageKey);
        //}
        
        /// <summary>
        /// 移除PageContext
        /// </summary>
        /// <param name="pageKey"></param>
        //public void RemoveChild(string pageKey)
        //{
        //    List<string> children = Get<List<string>>(StaticString.ChildrenContexsString) ?? new List<string>();
        //    if (!children.Contains(pageKey))
        //    {
        //        children.Remove(pageKey);
        //        Set(StaticString.ChildrenContexsString, children);
        //    }
        //}

        protected override int TimeOut
        {
            get
            {
                return
                    CoralConvert.Convert<int>(
                        AppConfig.DllConfigs.Current[StaticString.SessionContextString][StaticString.TimeoutString]);
            }
        }

        /// <summary>
        /// 用户上下文
        /// </summary>
        public UserContext UserContext { get; private set; }

        /// <summary>
        /// 应用上下文
        /// </summary>
        public AppContext AppContext
        {
            get { return UserContext.AppContext; }
        }
               
    }
}
