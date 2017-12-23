using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Cache;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Contexts;
using Coralcode.Framework.Data;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Models;

namespace Coralcode.Framework.Web
{
    public class ContextsHttpModule : IHttpModule
    {
        public virtual void Init(HttpApplication context)
        {

            PlatformContext.Current = new PlatformContext("Caad", CacheFactory.GetRedisCache(sectionName: StaticString.PlatformContextString));
            var app = new AppModel
            {
                Id = CoralConvert.Convert<long>(AppConfig.AppSettings["AppId"]),
                Key = AppConfig.AppSettings["AppKey"],
                Secret = AppConfig.AppSettings["AppSecret"],
                Name = AppConfig.AppSettings["AppName"],
                Code = AppConfig.AppSettings["AppCode"],
                Domain = AppConfig.DllConfigs["Host"]["Servers"]["AppDomain"]
            };
            if (!string.IsNullOrEmpty(app.Domain))
                PlatformContext.Current.CreateAppContext(app);
            context.BeginRequest += (obj, arg) =>
            {
                if (!string.IsNullOrEmpty(app.Domain)) return;
                app.Domain = HttpContext.Current.Request.Url.GetUrlHost();
                PlatformContext.Current.CreateAppContext(app);
            };

        }
        public void Dispose()
        {
        }
    }
}
