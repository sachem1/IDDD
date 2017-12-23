using Coralcode.Framework.Aspect;
using Coralcode.Framework.ConfigManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coralcode.Framework.Contexts
{
    /// <summary>
    /// 
    /// </summary>
    [Inject(RegisterType = typeof(IContextOperation), LifetimeManagerType = LifetimeManagerType.ContainerControlled)]
    public class ContextCollection : IContextOperation
    {
        /// <summary>
        /// 
        /// </summary>
        public ContextCollection()
        {
            int.TryParse(periodStr, out _activityTime);
            if (_activityTime == 0)
                _activityTime = _defaultActivityTime;
        }

        private readonly int _activityTime;
        private static Timer _timer;
        private readonly string LastTimeKey = "LastTime";
        private readonly int _defaultActivityTime = 60;
        private string periodStr = AppConfig.AppSettings["ContextInterval"];

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            var firstTime = _activityTime * 1000 * 60;
            if (_timer == null)
                _timer = new Timer(Scan, null, firstTime, firstTime);
        }

        /// <summary>
        /// 定时刷新LastTime
        /// </summary>
        public void Refresh()
        {
            SessionContext.Current.Set(LastTimeKey, DateTime.Now);
        }

        /// <summary>
        /// Context扫描
        /// </summary>
        /// <param name="state"></param>
        public void Scan(object state)
        {
            ScanInternal();
        }

        /// <summary>
        /// 实现Context扫描
        /// </summary>
        internal void ScanInternal()
        {
            //Stopwatch stopwatch = new Stopwatch();
            ////stopwatch.Start();
            ////int times = 0;
            //var timeoutSessionList = new List<SessionContext>();
            //var appContext = AppContext.Current;
            ////times += 1;
            //var userContextList = appContext.GetChildrenList().Select(userKey => appContext.GetUserContext(userKey)).ToList();
            ////times += 1;
            //// 要判断是否有效
            //foreach (UserContext userContext in userContextList)
            //{
            //    //times++;
            //    var sessionKeyList = userContext.GetChildrenList();
            //    if (sessionKeyList == null)
            //        continue;
            //    //times += sessionKeyList.Count;
            //    var sessionList = sessionKeyList.Select(sessionKey => userContext.GetSessionContext(sessionKey)).ToList();
            //    if (sessionList == null)
            //        continue;

            //    timeoutSessionList.AddRange(sessionList.Where(session => session.Get<DateTime>(LastTimeKey).AddMinutes(_activityTime) < DateTime.Now));
            //}

            //timeoutSessionList.ForEach(session =>
            //{
            //    session.Clear();
            //    //times++;
            //});

            //stopwatch.Stop();
            //var seconds = stopwatch.ElapsedMilliseconds / 1000;
            //Debug.WriteLine("redis操作耗时时间：" + seconds + "秒，共读取次数：" + times);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalKey"></param>
        /// <returns></returns>
        private string GetSessionKey(string originalKey)
        {
            var index = originalKey.LastIndexOf('-') + 1;

            return originalKey.Substring(index);
        }
    }
}
