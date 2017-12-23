using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;

namespace Coralcode.Framework.Cache.Local
{

    public class LocalCache : ICache
    {
        private readonly MemoryCache _cache = MemoryCache.Default;


        internal static readonly ICache Default = new LocalCache();
        private readonly int _timeout;

        public LocalCache()
        {
            if (AppConfig.IsExists(StaticString.DefaultRedisConfigDll) &&
                AppConfig.DllConfigs.Current.IsExists("Local") &&
                AppConfig.DllConfigs.Current["Local"].IsExists("Timeout"))
                _timeout = CoralConvert.Convert<int>(AppConfig.DllConfigs.Current["Local"]["Timeout"]);
            else
                _timeout = 3600;
        }

        public T Get<T>(string contextKey, string dataKey, Func<T> action, int? expirationSeconds = null)
        {
            if (!Exist(contextKey, dataKey))
            {
                var value = action();
                if (value == null)
                    return default(T);
                Set(contextKey, dataKey, value, expirationSeconds);
                return value;
            }
            return Get<T>(contextKey, dataKey);
        }

        public T Get<T>(string contextKey, string dataKey)
        {
            var key = GeneralContextKey(contextKey);
            if (!(_cache.Get(key) is ConcurrentDictionary<string, object> hset))
                return default(T);
            if (hset.TryGetValue(dataKey, out var value))
            {
                return (T)value;
            }
            return default(T);
        }

        public IDictionary<string, T> Get<T>(string contextKey)
        {
            var key = GeneralContextKey(contextKey);
            if (_cache.Get(key) is ConcurrentDictionary<string, object> hset)
                return hset.ToDictionary(item => item.Key, item => (T) item.Value);
            return new Dictionary<string, T>();
        }


        public void Remove(string contextKey, string dataKey)
        {
            var key = GeneralContextKey(contextKey);
            if (!(_cache.Get(key) is ConcurrentDictionary<string, object> hset))
                return;
            hset.TryRemove(dataKey, out _);
        }

        public void Set<T>(string contextKey, string dataKey, T value, int? expirationSeconds = null)
        {
            var key = GeneralContextKey(contextKey);
            int seconds = expirationSeconds ?? _timeout;

            if (!_cache.Contains(key))
            {
                var hset = new ConcurrentDictionary<string, object>(new Dictionary<string, object> { { dataKey, value } });
                _cache.Set(key, hset, DateTimeOffset.Now.AddSeconds(seconds));
            }
            else
            {
                var hset = _cache.Get(key) as ConcurrentDictionary<string, object>;
                hset?.AddOrUpdate(dataKey, k => value, (k, oldvalue) => value);
            }
        }

        public void Remove(string contextKey)
        {
            var key = GeneralContextKey(contextKey);
            if (_cache.Contains(key))
                _cache.Remove(key);
        }

        public bool Exist(string contextKey, string dataKey)
        {
            var key = GeneralContextKey(contextKey);
            return _cache.Get(key) is ConcurrentDictionary<string, object> hset && hset.ContainsKey(dataKey);
        }

        public bool ExistChildren(string contextKey)
        {
            return _cache.Contains(contextKey);
        }

        public void Dispose()
        {
        }
        private static string GeneralContextKey(string contextKey)
        {
            return $"{contextKey}";
        }
    }
}
