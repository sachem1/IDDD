using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Task;
using Microsoft.Practices.Unity.InterceptionExtension;
using Newtonsoft.Json;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;

namespace Coralcode.Framework.Log
{
    /// <summary>
    /// 函数执行耗时日志记录，秒为单位
    /// </summary>
    public class TimeConsumingLogAttribute : UnityAopAttribute
    {
        private DateTime _onBeforeDateTime = DateTime.Now;
        private static readonly TaskQueue Queue = TaskQueue.GetQueue(1);
        private readonly bool _debug = Convert.ToBoolean(AppConfig.DllConfigs.Current[StaticString.LogContextString][StaticString.LogEnableTraceTimeConsumingString] ?? "True");

        protected override void OnBefore(IMethodInvocation input)
        {
            if (!_debug) return;
            _onBeforeDateTime = DateTime.Now;
        }

        protected override void OnAfter(IMethodInvocation input)
        {
            if (!_debug) return;
            try
            {
                var currentDateTime = DateTime.Now;
                var ts = currentDateTime.Subtract(_onBeforeDateTime);
                if (input.MethodBase.DeclaringType == null) return;

                Queue.Execute(() =>
                {
                    LoggerFactory.GetLogger("TimeConsuming").Info("接口：{0},方法：{1},参数：{2},耗时：{3}", input.MethodBase.DeclaringType.FullName,
                        input.MethodBase.Name, JsonConvert.SerializeObject(input.Arguments), ts.TotalSeconds);
                    return true;
                });
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
