using System.Collections.Concurrent;
using System.Diagnostics;
using Coralcode.Framework.Log;

namespace Coralcode.Framework.Common
{
    public class PerformanceWatch
    {
        #region Singleton

        public static readonly PerformanceWatch Instance = new PerformanceWatch();

        private PerformanceWatch()
        {
        }

        #endregion

        private readonly ConcurrentDictionary<string, StepWatch> _dictWatchs =
            new ConcurrentDictionary<string, StepWatch>();

        public void Start(string identify)
        {
            if (!_dictWatchs.ContainsKey(identify))
                _dictWatchs.TryAdd(identify, new StepWatch());
            _dictWatchs[identify].Start();
        }


        public void ProgressStep(string identify, string stepName)
        {
            if (!_dictWatchs.ContainsKey(identify))
                return;
            _dictWatchs[identify].Pause();
            LoggerFactory.Instance.Debug("****************************************");
            LoggerFactory.Instance.Debug("{0}过程的{1}步一共耗时{2} 毫秒", identify, stepName, _dictWatchs[identify].ElapsedMilliseconds);
            LoggerFactory.Instance.Debug("{0}的过程到{1}时一共耗时{2} 毫秒", identify, stepName, _dictWatchs[identify].TotelMilliseconds);
            LoggerFactory.Instance.Debug("****************************************");
            _dictWatchs[identify].Restart();
        }


        public void Stop(string identify)
        {
            if (!_dictWatchs.ContainsKey(identify))
                return;
            _dictWatchs[identify].Stop();
            LoggerFactory.Instance.Debug("****************************************");
            LoggerFactory.Instance.Debug("{0}过程的最后一步一共耗时{1} 毫秒", identify, _dictWatchs[identify].ElapsedMilliseconds);
            LoggerFactory.Instance.Debug("{0}的过程一共耗时{1} 毫秒", identify, _dictWatchs[identify].TotelMilliseconds);
            LoggerFactory.Instance.Debug("****************************************");
        }

        private class StepWatch
        {
            private Stopwatch _watch = new Stopwatch();
            private long _totleMilliseconds = 0;

            public long TotelMilliseconds
            {
                get { return _totleMilliseconds; }
            }

            public long ElapsedMilliseconds
            {
                get { return _watch.ElapsedMilliseconds; }
            }

            public void Start()
            {
                _watch.Restart();
                _totleMilliseconds = 0;
            }

            public void Pause()
            {
                _watch.Stop();
                _totleMilliseconds += _watch.ElapsedMilliseconds;
            }

            public void Restart()
            {
                _watch.Restart();
            }

            public void Stop()
            {
                _watch.Stop();
                _totleMilliseconds += _watch.ElapsedMilliseconds;
            }
        }
    }
}