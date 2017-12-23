using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Web;
using System.Text;
using Coralcode.Framework.Contexts;
using Coralcode.Framework.Exceptions;
using Coralcode.Framework.Log;
using Coralcode.Framework.Models;
using Coralcode.Framework.WCF.Exception;

namespace Coralcode.Framework.WCF.Aspect
{
    public class AuthorizationAttribute : Attribute, IOperationBehavior, IParameterInspector
    {


        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription operationDescription, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {

        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(this);
        }

        public void Validate(OperationDescription operationDescription)
        {

        }

        #endregion

        #region IParameterInspector Members

        public void AfterCall(string operationName, object[] outputs, object returnValue, object correlationState)
        {
        }

        public object BeforeCall(string operationName, object[] inputs)
        {
            if (WebOperationContext.Current == null)
            {
                throw new WcfException(new ResultErrorMessage(ResultState.Fail, "请使用 http 协议请求"), HttpStatusCode.Unauthorized);

            }


            if (WebOperationContext.Current.IncomingRequest.Headers.Count == 0
                || !WebOperationContext.Current.IncomingRequest.Headers.AllKeys.Contains("Authorization", StringComparer.CurrentCultureIgnoreCase))
            {
                throw new WcfException(new ResultErrorMessage(ResultState.Fail, "请求需携带认证参数 Basic"), HttpStatusCode.Unauthorized);
            }

            var credentials = AnalysisAuthenticationHeaderValue();
            string appKey = credentials.Key;
            string appSecret = credentials.Value;

            //验证数据
            AppModel appModel = PlatformContext.Current.Get<AppModel>(appKey);
            if (appModel == null)
            {
                throw new WcfException(new ResultErrorMessage(ResultState.Fail, "平台身份认证失败"), HttpStatusCode.Unauthorized);
            }
            if (appSecret != appModel.Secret)
            {
                throw new WcfException(new ResultErrorMessage(ResultState.Fail, "AppToken 已过期"), HttpStatusCode.Unauthorized);
            }


            LoggerFactory.Instance.Info(" {0} 调用Caad.Service ", appModel.Name);


            return null;
        }

        #endregion



        private KeyValuePair<string, string> AnalysisAuthenticationHeaderValue()
        {
            if (WebOperationContext.Current == null)
                throw new WcfException(new ResultErrorMessage(ResultState.Fail, "无效的请求"), HttpStatusCode.BadRequest);
            var auth = WebOperationContext.Current.IncomingRequest.Headers["Authorization"];
            if (string.IsNullOrEmpty(auth) || !auth.StartsWith("Basic "))
            {
                throw new WcfException(new ResultErrorMessage(ResultState.Fail, "无效的 Basic"),
                    HttpStatusCode.Unauthorized);
            }
            auth = auth.Replace("Basic ", "");

            try
            {
                string credentials = Encoding.UTF8.GetString(Convert.FromBase64String(auth));
                var pairs = credentials.Split(':');
                if (pairs.Length != 2)
                {
                    throw new WcfException(new ResultErrorMessage(ResultState.Fail, "无效的 Basic"),
                        HttpStatusCode.Unauthorized);
                }
                return new KeyValuePair<string, string>(pairs[0], pairs[1]);
            }
            catch (System.Exception ex)
            {
                LoggerFactory.Instance.Error(" 解析 Basic 参数出现异常，Param:{0}  ", ex,
                    WebOperationContext.Current.IncomingRequest.Headers["Authorization"]);
                throw new WcfException(new ResultErrorMessage(ResultState.Fail, "解析 Basic 参数出现异常"),
                    HttpStatusCode.Unauthorized);
            }

        }

    }
}