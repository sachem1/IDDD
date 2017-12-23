using System;
using System.Threading;
using System.Web;
using Coralcode.Framework.Cache;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Data;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Models;

namespace Coralcode.Framework.Contexts
{
    public static class ContextBuilder
    {
        public static PlatformContext BuildPlatform()
        {
             return PlatformContext.Current= new PlatformContext("Caad", CacheFactory.GetRedisCache(sectionName: StaticString.PlatformContextString));
        }

        public static AppContext BuildAppContext(this PlatformContext platformContext)
        {
            var app = new AppModel
            {
                Id = CoralConvert.Convert<long>(AppConfig.AppSettings["AppId"]),
                Key = AppConfig.AppSettings["AppKey"],
                Secret = AppConfig.AppSettings["AppSecret"],
                Name = AppConfig.AppSettings["AppName"],
                Code = AppConfig.AppSettings["AppCode"],
                Domain = AppConfig.DllConfigs["Host"]["Servers"]["AppDomain"]
            };

            if (string.IsNullOrEmpty(app.Domain))
            {
                throw new ArgumentNullException("Domain", "请在Configs文件夹中Host配置文件的Servers节点配置AppDomain项的值");
            }
            return platformContext.CreateAppContext(app);
        }
    }
}