using System.Collections.Generic;
using System.Threading;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Cache;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Models;
using Coralcode.Framework.Services;
using System.Linq;

namespace Coralcode.Framework.Contexts
{
    /// <summary>
    /// 用户上下文
    /// </summary>
    public class UserContext : Context
    {

        private static ThreadLocal<UserContext> _current = new ThreadLocal<UserContext>();
        public static UserContext Current
        {
            get
            {
                if (_current == null)
                    return null;
                return _current.Value;
            }
            set { _current.Value = value; }
        }

        internal UserContext(AppContext appContext, UserModel user, ICache cache)
        {
            AppContext = appContext;
            ClearEventHandle += item =>
            {
                //appContext.RemoveChild(user.Account);
            };
            //Dispised += item =>
            //{
            //    appContext.RemoveChild(user.Account);
            //};
            CacheKey = string.Format("{0}-{1}", appContext.CacheKey, user.Account);
            User = user;
            Cache = cache;
        }

        /// <summary>
        /// 创建一个会话的上下文
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public SessionContext CreateSessionContext(string sessionKey)
        {
            //List<string> children = Get<List<string>>(StaticString.ChildrenContexsString) ?? new List<string>();
            //if (!children.Contains(sessionKey))
            //{
            //    children.Add(sessionKey);
            //    Set(StaticString.ChildrenContexsString, children);
            //}
            SessionContext.Current = new SessionContext(this, sessionKey, CacheFactory.GetRedisCache(sectionName: StaticString.SessionContextString));
            return SessionContext.Current;
        }

        public void RemoveSessionContext(string sessionKey)
        {
            Remove(sessionKey);
        }

        public SessionContext GetSessionContext(string sessionKey)
        {
            //List<string> children = Get<List<string>>(StaticString.ChildrenContexsString) ?? new List<string>();
            //if (!children.Contains(sessionKey))
            //{
            //    throw new KeyNotFoundException("未找到对应数据");
            //}
            //这里是获取SessionContext，不需要设置当前
            return new SessionContext(this, sessionKey, CacheFactory.GetRedisCache(sectionName: StaticString.SessionContextString));
        }

        public bool HasChildExists(string sessionKey)
        {
            List<string> children = Get<List<string>>(StaticString.ChildrenContexsString);
            return children != null && children.Count != 0 && children.Contains(sessionKey);
        }

        /// <summary>
        /// 移除sessioncontext
        /// </summary>
        /// <param name="sessionKey"></param>
        //public void RemoveChild(string sessionKey)
        //{
        //    List<string> children = Get<List<string>>(StaticString.ChildrenContexsString) ?? new List<string>();
        //    if (children.Contains(sessionKey))
        //    {
        //        //GetSessionContext(sessionKey).Clear();
        //        children.Remove(sessionKey);
        //        // UserContext可以重新生成
        //        if (children.Count == 0)
        //            Clear();
        //        else
        //            Set(StaticString.ChildrenContexsString, children);
        //    }
        //}

        /// <summary>
        /// 应用上下文
        /// </summary>
        public AppContext AppContext { get; private set; }

        public UserModel User { get; set; }


        protected override int TimeOut
        {
            get
            {
                return CoralConvert.Convert<int>(AppConfig.DllConfigs.Current[StaticString.UserContextString][StaticString.TimeoutString]);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        //public List<string> GetChildrenList()
        //{
        //    return Get<List<string>>(StaticString.ChildrenContexsString) ?? new List<string>();
        //}

    }
}
