using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using Coralcode.Framework.Aspect.Unity;

namespace Coralcode.Framework.WCF
{
    /// <summary>
    /// WebService处理类.
    /// </summary>
    [PermissionSet(SecurityAction.InheritanceDemand, Unrestricted = true)]
    internal class WebServiceHandlerFactory<T> : System.Web.Services.Protocols.WebServiceHandlerFactory, IHttpHandlerFactory
        where T : WebService
    {
        #region 成员变量,构造函数.
        /// <summary>
        /// 核心方法反射调用.
        /// </summary>
        private static readonly MethodInfo CoreGetHandler = typeof(System.Web.Services.Protocols.WebServiceHandlerFactory).GetMethod("CoreGetHandler",
            BindingFlags.NonPublic | BindingFlags.Instance,
            null,
            new[] { typeof(Type), typeof(HttpContext), typeof(HttpRequest), typeof(HttpResponse) },
            null);
        private Type serviceType;
        /// <summary>
        /// 构造函数.
        /// </summary>
        /// <param name="serviceType"></param>
        public WebServiceHandlerFactory(T serviceType)
        {
            this.serviceType = serviceType.GetType();
        }
        #endregion

        


        #region IHttpHandlerFactory 成员
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requestType"></param>
        /// <param name="url"></param>
        /// <param name="pathTranslated"></param>
        /// <returns></returns>
        IHttpHandler IHttpHandlerFactory.GetHandler(HttpContext context, string requestType, string url, string pathTranslated)
        {
            if (this.serviceType == null)
            {
                throw new ArgumentNullException("serviceType", "服务类型为NULL!");
            }
            new AspNetHostingPermission(AspNetHostingPermissionLevel.Minimal).Demand();
            return (IHttpHandler)CoreGetHandler.Invoke(this, new object[] { this.serviceType, context, context.Request, context.Response });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        void IHttpHandlerFactory.ReleaseHandler(IHttpHandler handler)
        {
            base.ReleaseHandler(handler);
        }
        #endregion
    }
}
