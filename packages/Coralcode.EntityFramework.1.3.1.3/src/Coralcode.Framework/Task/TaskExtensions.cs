using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Services;

namespace Coralcode.Framework.Task
{
    public static class TaskExtensions
    {
        /// <summary>
        /// 调用示例
        /// 
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        public  static void AsyncExecuteServiceAction<TService>(this TService service,string actionName,params  object[] parameters)
            where TService:CoralService
        {
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                using (var newservice = UnityService.Resolve<TService>())
                {
                    newservice.InitContext(service.AppContext,service.UserContext,service.SessionContext,service.PageContext);
                    newservice.ExecuteMethod<object>(actionName, parameters);
                }
            });
        }

        ///// <summary>
        ///// 调用示例
        ///// 
        ///// </summary>
        ///// <typeparam name="TService"></typeparam>
        //public static void AsyncExecuteServiceAction<TService>(this TService service, Action<object[]> action, params object[] parameters)
        //    where TService : CoralService
        //{
        //    System.Threading.Tasks.Task.Factory.StartNew(() =>
        //    {
        //        using (var newservice = UnityService.Resolve<TService>())
        //        {
        //            newservice.InitContext(service.AppContext, service.UserContext, service.SessionContext, service.PageContext);
        //            action newservice.ExecuteMethod<object>(actionName, parameters);
        //        }
        //    });
        //}
    }
}
