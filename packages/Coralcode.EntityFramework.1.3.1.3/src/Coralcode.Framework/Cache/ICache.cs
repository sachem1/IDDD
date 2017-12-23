using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.SS.Formula.Functions;

namespace Coralcode.Framework.Cache
{
    public interface ICache : IDisposable
    {
        /// <summary>
        /// 使用方式
        ///  Cache.Instance.GetFormCache(MethodBase.GetCurrentMethod().ToString(), Action);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="action"></param>
        /// <param name="expirationSeconds"></param>
        /// <returns></returns>
        T Get<T>(string contextKey, string dataKey, Func<T> action, int? expirationSeconds = null);

        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        T Get<T>(string contextKey, string dataKey);


        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="contextKey"></param>
        /// <returns></returns>
        IDictionary<string, T> Get<T>(string contextKey);


        /// <summary>
        /// 获取缓存对象
        /// </summary>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        void Remove(string contextKey, string dataKey);

        /// <summary>
        /// 设置缓存对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="value"></param>
        /// <param name="expirationSeconds"></param>
        void Set<T>(string contextKey, string dataKey, T value, int? expirationSeconds = null);

        /// <summary>
        /// 移除缓存对象
        /// </summary>
        /// <param name="contextKey"></param>
        void Remove(string contextKey);

        /// <summary>
        /// 判断datakey是否存在
        /// </summary>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        bool Exist(string contextKey, string dataKey);

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="contextKey"></param>
        /// <returns></returns>
        bool ExistChildren(string contextKey);
    }
}
