using System.Collections.Generic;
using System.Linq;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Cache;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Models;

namespace Coralcode.Framework.Contexts
{
    /// <summary>
    /// 平台上下文
    /// </summary>
    public class PlatformContext : Context
    {

        public static PlatformContext Current { get; set; }


        internal PlatformContext(string key, ICache cache)
        {
            CacheKey = key;
            Cache = cache;
        }

        protected override int TimeOut
        {
            get
            {
                return
                    CoralConvert.Convert<int>(
                        AppConfig.DllConfigs.Current[StaticString.PlatformContextString][StaticString.TimeoutString]);
            }
        }

        public AppContext CreateAppContext(AppModel app)
        {
            Set(app.Key, app);
            //更新当前App
            var apps = Get<List<AppModel>>(StaticString.AppsString) ?? new List<AppModel>();
            var current = apps.FirstOrDefault(item => item.Key == app.Key);
            if (current != null)
                apps.Remove(current);
            apps.Add(app);
            Set(StaticString.AppsString, apps);
            AppContext.Current = new AppContext(this, app, CacheFactory.GetRedisCache(sectionName: StaticString.AppContextString));
            AppContext.Current.App = app;
            return AppContext.Current;
        }
    }
}
