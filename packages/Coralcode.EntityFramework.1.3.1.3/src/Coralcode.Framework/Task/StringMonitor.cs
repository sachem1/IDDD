using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coralcode.Framework.Task
{
    /// <summary>
    /// 字符串粒度的信号量
    /// 用于需要加锁的情况
    /// </summary>
    public static class StringMonitor
    {
        private static readonly ConcurrentDictionary<string, object> Monitors = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 执行加锁的方法，
        /// </summary>
        /// <typeparam name="T">返回结果</typeparam>
        /// <param name="key">字符串标识</param>
        /// <param name="acceptFunc">进入成功后执行的方法</param>
        /// <param name="refuseFunc">拒绝进入后执行的方法</param>
        /// <param name="timeout">尝试时间</param>
        /// <returns></returns>
        public static T Lock<T>(string key, Func<T> acceptFunc, Func<T> refuseFunc, int timeout = 0)
        {
            var monitor = Monitors.GetOrAdd(key, item => new object());

            if (!Monitor.TryEnter(monitor, timeout))
            {
                return refuseFunc();
            }
            try
            {
                return acceptFunc();
            }
            finally
            {
                Monitor.Exit(monitor);
                Monitors.TryRemove(key, out monitor);
            }
        }


        /// <summary>
        /// 执行加锁的方法，
        /// </summary>
        /// <param name="key">字符串标识</param>
        /// <param name="acceptFunc">进入成功后执行的方法，保持方法幂等性</param>
        /// <param name="timeout">尝试时间</param>
        /// <returns></returns>
        public static void Lock(string key, Action acceptFunc, int timeout = 0)
        {
            var monitor = Monitors.GetOrAdd(key, item => new object());

            if (!Monitor.TryEnter(monitor, timeout))
                return;
            try
            {
                acceptFunc();
            }
            finally
            {
                Monitor.Exit(monitor);
                Monitors.TryRemove(key, out monitor);
            }
        }
        /// <summary>
        /// 执行加锁的方法，
        /// </summary>
        /// <param name="key">字符串标识</param>
        /// <param name="acceptAction">进入成功后执行的方法</param>
        /// <param name="refuseAction">拒绝进入后执行的方法</param>
        /// <param name="timeout">尝试时间</param>
        /// <returns></returns>
        public static void Lock(string key, Action acceptAction, Action refuseAction, int timeout = 0)
        {
            var monitor = Monitors.GetOrAdd(key, item => new object());

            if (!Monitor.TryEnter(monitor, timeout))
            {
                refuseAction();
            }
            try
            {
                acceptAction();
            }
            finally
            {
                Monitor.Exit(monitor);
                Monitors.TryRemove(key, out monitor);
            }
        }

    }
}
