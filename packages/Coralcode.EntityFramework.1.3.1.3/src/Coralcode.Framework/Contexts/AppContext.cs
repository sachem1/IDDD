using System.Collections.Generic;
using System.Threading;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Cache;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Models;
using System.Linq;

namespace Coralcode.Framework.Contexts
{
    /// <summary>
    /// 应用上下文
    /// </summary>
    public class AppContext : Context
    {
        public AppContext(ICache cache)
        {
            Cache = cache;
        }


        public PlatformContext PlatformContext { get; set; }


        protected override int TimeOut
        {
            get
            {
                return CoralConvert.Convert<int>(AppConfig.DllConfigs.Current[StaticString.AppContextString][StaticString.TimeoutString]);
            }
        }

        public static AppContext Current { get; set; }

        public AppModel App { get; set; }

        internal AppContext(PlatformContext platformContext, AppModel app, ICache cache)
        {
            PlatformContext = platformContext;
            ClearEventHandle += item =>
            {
                // 不清理AppContext
                //PlatformContext.Remove(app.Name);
            };
            CacheKey = string.Format("{0}-{1}", platformContext.CacheKey, app.Name);
            Cache = cache;
        }

        /// <summary>
        /// 创建用户上下文
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public UserContext CreateUserContext(UserModel user)
        {
            //List<string> children = Get<List<string>>(StaticString.ChildrenContexsString) ?? new List<string>();
            //if (!children.Contains(user.Account))
            //{

            //    children.Add(user.Account);
            //    Set(StaticString.ChildrenContexsString, children);
            //    Set(user.Account, user);
            //}
            UserContext.Current = new UserContext(this, user, CacheFactory.GetRedisCache(sectionName: StaticString.UserContextString));

            return UserContext.Current;
        }

        public UserContext GetUserContext(string id)
        {
            var user = Get<UserModel>(id);
            if (user == null)
            {
                throw new KeyNotFoundException("未找到对应数据");
            }
            //这里是获取UserContext，不需要设置当前
            return new UserContext(this, user, CacheFactory.GetRedisCache(sectionName: StaticString.UserContextString));
        }

        /// <summary>
        /// 移除usercontext
        /// </summary>
        /// <param name="userKey"></param>
        //public void RemoveChild(string userKey)
        //{
            //List<string> children = Get<List<string>>(StaticString.ChildrenContexsString) ?? new List<string>();
            //if (children.Contains(userKey))
            //{
            //    //GetUserContext(userKey).Clear();
            //    Remove(userKey);

            //    children.Remove(userKey);
            //    Set(StaticString.ChildrenContexsString, children);
            //}
        //}

        public bool HasChildExists(string userKey)
        {
            return Exist(userKey);
        }

        /// <summary>
        /// 
        /// </summary>
        ///// <returns></returns>
        //public List<string> GetChildrenList()
        //{
        //    return Get<List<string>>(StaticString.ChildrenContexsString) ?? new List<string>();
        //}

    }
}
