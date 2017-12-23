using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;

namespace Coralcode.Framework.WCF
{
    /// <summary>
    /// 基础WebService服务
    /// </summary>
    public class BaseWebService : WebService, IHttpHandler
    {
        /// <summary>
        /// 其他请求是否可以使用此实例
        /// </summary>
        public bool IsReusable
        {
            get { return false; }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {
            IHttpHandlerFactory factory = new WebServiceHandlerFactory<BaseWebService>(this);
            var handler = factory.GetHandler(context, null, null, null);
            handler.ProcessRequest(context);
        }
    }
}
