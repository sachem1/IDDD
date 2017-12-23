using System;
using System.Threading;
using Coralcode.Framework.Cache;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;

namespace Coralcode.Framework.Contexts
{
    /// <summary>
    /// 上下文
    /// </summary>
    public abstract class Context : IDisposable
    {
        /// <summary>
        /// 销毁的时候发出的事件
        /// </summary>
        internal event Action<Context> Dispised;

        /// <summary>
        /// 清空触发事件
        /// </summary>
        internal event Action<Context> ClearEventHandle;

        /// <summary>
        /// 缓存
        /// </summary>
        protected ICache Cache { get; set; }

        protected virtual int TimeOut
        {
            get
            {
                return
                    CoralConvert.Convert<int>(
                        AppConfig.DllConfigs.Current[StaticString.DefaultContextString][StaticString.TimeoutString]);
            }
        }

        /// <summary>
        /// hashset的key
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            Dispised?.Invoke(this);
            Clear();
        }

        #region 缓存

        /// <summary>
        /// 使用方式
        ///  Cache.Instance.GetFormCache(MethodBase.GetCurrentMethod().ToString(), Action);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataKey"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual T Get<T>(string dataKey, Func<T> action)
        {
            return Cache.Get(CacheKey, dataKey, action, TimeOut);
        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public virtual T Get<T>(string dataKey)
        {
            return Cache.Get<T>(CacheKey, dataKey);
        }

        public virtual bool Exist(string dataKey)
        {
            return Cache.Exist(CacheKey, dataKey);

        }

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public virtual void Remove(string dataKey)
        {
            Cache.Remove(CacheKey, dataKey);
        }

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataKey"></param>
        /// <param name="value"></param>
        public virtual void Set<T>(string dataKey, T value)
        {
            Cache.Set(CacheKey, dataKey, value, TimeOut);
        }

        /// <summary>
        /// 清空
        /// </summary>
        public virtual void Clear()
        {
            Cache.Remove(CacheKey);

            ClearEventHandle?.Invoke(this);
        }

        #endregion
    }
}
