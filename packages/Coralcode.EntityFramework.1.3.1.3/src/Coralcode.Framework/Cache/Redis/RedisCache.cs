using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Coralcode.Framework.Cache.Redis
{
    public class RedisCache : ICache
    {
        private readonly bool _localCacheEnable;
        private readonly int _localCacheTimeout;
        private readonly IDatabase _db;
        private readonly ICache _localCache = CacheFactory.GetLocalCache();

        public RedisCache(RedisSetting setting)
        {
            _db = RedisManager.GetClient(setting);
            _localCacheEnable = setting.LocalCacheEnable;
            _localCacheTimeout = setting.LocalCacheTimeout;
            if (_localCacheTimeout == 0)
                _localCacheTimeout = 60;
        }

        /// <summary>
        /// 获取本地数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private T GetLocal<T>(string contextKey, string dataKey, Func<T> action)
        {
            if (_localCacheEnable)
                return _localCache.Get(contextKey, dataKey, action, _localCacheTimeout);
            return action();

        }

        /// <inheritdoc />
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="action"></param>
        /// <param name="expirationSeconds"></param>
        /// <returns></returns>
        public T Get<T>(string contextKey, string dataKey, Func<T> action, int? expirationSeconds = null)
        {
            Func<T> redisAction = () =>
            {
                var key = GeneralContextKey(contextKey);
                if (_db.HashExists(key, dataKey))
                {
                    return ResolveJson<T>(_db.HashGet(key, dataKey));
                }
                T data = action();
                if (data == null)
                    throw new NullReferenceException("不能向缓存中添加为null的数据");
                _db.HashSet(key, dataKey, JsonConvert.SerializeObject(data));
                if (expirationSeconds.HasValue && expirationSeconds != 0)
                    _db.KeyExpire(key, TimeSpan.FromSeconds(expirationSeconds.Value));
                return data;
            };
            return GetLocal(contextKey, dataKey, redisAction);
        }

        /// <inheritdoc />
        /// <summary>
        /// 直接获取数据
        /// 谨慎使用，请使用带委托的获取方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T Get<T>(string contextKey, string dataKey)
        {
            return GetLocal(contextKey, dataKey, () =>
            {
                var key = GeneralContextKey(contextKey);
                return ResolveJson<T>(_db.HashGet(key, dataKey));
            });
        }

        /// <inheritdoc />
        /// <summary>
        /// 直接设置数据
        /// 谨慎使用，请使用带委托的获取方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="value"></param>
        /// <param name="expirationSeconds">如果不设置过期时间则永不过期</param>
        public void Set<T>(string contextKey, string dataKey, T value, int? expirationSeconds = null)
        {
            if (_localCacheEnable)
            {
                _localCache.Set(contextKey, dataKey, value, _localCacheTimeout);
            }
            var key = GeneralContextKey(contextKey);
            _db.HashSet(key, dataKey, JsonConvert.SerializeObject(value));
            if (expirationSeconds.HasValue && expirationSeconds != 0)
                _db.KeyExpire(key, TimeSpan.FromSeconds(expirationSeconds.Value));
        }

        /// <inheritdoc />
        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool Exist(string contextKey, string dataKey)
        {
            if (string.IsNullOrEmpty(contextKey) || string.IsNullOrEmpty(dataKey))
                return false;
            var key = GeneralContextKey(contextKey);
            return _db.HashExists(key, dataKey);
        }

        /// <inheritdoc />
        /// <summary>
        /// 删除set中指定的key
        /// </summary>
        /// <param name="contextKey"></param>
        /// <param name="dataKey"></param>
        public void Remove(string contextKey, string dataKey)
        {
            if (_localCacheEnable)
            {
                _localCache.Remove(contextKey, dataKey);
            }
            var key = GeneralContextKey(contextKey);
            _db.HashDelete(key, dataKey);
        }

        /// <inheritdoc />
        /// <summary>
        /// 获取全部缓存,
        /// 谨慎使用,
        /// 这里的key都是通过set,或者带委托的方法设置进去的
        /// 没有单独设置全部的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contextKey"></param>
        /// <returns></returns>
        public IDictionary<string, T> Get<T>(string contextKey)
        {
            var key = GeneralContextKey(contextKey);
            if (_localCacheEnable)
            {
                var datas = _localCache.Get<T>(contextKey);
                if (datas != null && datas.Count != 0)
                    return datas;
            }
            var redisDatas = _db.HashGetAll(key).ToDictionary(item => (string)item.Name, item => ResolveJson<T>(item.Value));

            if (_localCacheEnable)
            {
                foreach (var item in redisDatas)
                {
                    _localCache.Set(contextKey, item.Key, item.Value, _localCacheTimeout);
                }
            }
            return redisDatas;
        }

        public void Remove(string contextKey)
        {
            if (_localCacheEnable)
            {
                _localCache.Remove(contextKey);
            }
            var key = GeneralContextKey(contextKey);
            _db.KeyDelete(key);
        }

        /// <inheritdoc />
        /// <summary>
        /// 判断是否存在子节点
        /// </summary>
        /// <param name="contextKey"></param>
        /// <returns></returns>
        public bool ExistChildren(string contextKey)
        {
            if (string.IsNullOrEmpty(contextKey))
                return false;
            var key = GeneralContextKey(contextKey);
            //todo 这里的判断带验证,skd的方法是否提供通配符获取
            return _db.KeyExists(key);
        }


        #region 工具方法
        private string GeneralContextKey(string contextKey)
        {
            return contextKey;
        }

        /// <summary>
        /// 解析redisvalue，包含json解析
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        private T ResolveJson<T>(RedisValue value)
        {
            if (!value.HasValue)
                return default(T);

            return JsonConvert.DeserializeObject<T>(value);
        }
        #endregion

        public void Dispose()
        {
        }

    }
}
