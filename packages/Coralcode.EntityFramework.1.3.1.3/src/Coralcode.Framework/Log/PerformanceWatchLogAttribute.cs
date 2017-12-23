using System;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;
using Microsoft.Practices.Unity.InterceptionExtension;
using Newtonsoft.Json;

namespace Coralcode.Framework.Log
{
    /// <summary>
    /// 性能监控日志输出属性类
    /// </summary>
    public class PerformanceWatchLogAttribute : UnityAopAttribute
    {
        private DateTime _onBeforeDateTime = DateTime.Now;
        private readonly bool _debug = Convert.ToBoolean(AppConfig.DllConfigs.Current[StaticString.LogContextString][StaticString.LogEnableTraceTimeConsumingString] ?? "True");
        private const string LogTag = "TimeConsuming";
        private static readonly ILogger _logger = LoggerFactory.GetLogger(LogTag);
        

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

                _logger.Debug("性能监控->完整方法：{3},方法：{0},参数：{1},耗时：{2}",
                    input.MethodBase.Name, JsonConvert.SerializeObject(input.Arguments), ts.TotalSeconds,input.MethodBase);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}