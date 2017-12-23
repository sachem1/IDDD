using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Cache.Local;
using Coralcode.Framework.Cache.Redis;
using Coralcode.Framework.Exceptions;
using Coralcode.Framework.Utils;
using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Common;
using StackExchange.Redis;

namespace Coralcode.Framework.Cache
{
    /// <summary>
    /// 缓存工厂
    /// </summary>
    public static class CacheFactory
    {
        /// <summary>
        /// 获取本地缓存
        /// </summary>
        /// <returns></returns>
        public static ICache GetLocalCache()
        {
            return LocalCache.Default;
        }

        /// <summary>
        /// 获取redis缓存组件
        /// </summary>
        /// <param name="dllName"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static ICache GetRedisCache(string dllName = StaticString.DefaultRedisConfigDll, string sectionName = StaticString.DefautlRedisConfigSection)
        {

            var setting = GetSetting(dllName, sectionName);
            return GetRedisCache(setting);

        }
        public static ICache GetRedisCache(RedisSetting setting)
        {
            try
            {
                return new RedisCache(setting);
            }
            catch (NullReferenceException)
            {
                throw CoralException.ThrowException(item => item.ConfigError);
            }
        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="dllName"></param>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public static RedisSetting GetSetting(string dllName = StaticString.DefaultRedisConfigDll, string sectionName = StaticString.DefautlRedisConfigSection)
        {

            if (!AppConfig.IsExists(dllName))
                throw new NotFoundException("未找到对应的配置文件");
            dllName = Assembly.GetCallingAssembly().GetName().Name;
            if (!AppConfig.DllConfigs[dllName].IsExists(sectionName))
                throw new NotFoundException("未找到对应的配置文件下的配置节点");
            var Setting = new RedisSetting();
            try
            {
                Setting.ClientName = StringUtil.FormatKey(dllName, sectionName);
                Setting.LocalCacheEnable = CoralConvert.Convert<bool>(AppConfig.DllConfigs[dllName][sectionName]["LocalCacheEnable"]);
                Setting.LocalCacheTimeout = CoralConvert.Convert<int>(AppConfig.DllConfigs[dllName][sectionName]["LocalCacheTimeout"]);
                Setting.ServerList = AppConfig.DllConfigs[dllName][sectionName]["ServerList"];
                Setting.Version = AppConfig.DllConfigs[dllName][sectionName]["Version"];
                Setting.DefaultDb = Convert.ToInt32(AppConfig.DllConfigs[dllName][sectionName]["DefaultDb"] ?? "0");
                Setting.Password = AppConfig.DllConfigs[dllName][sectionName]["Password"];
                return Setting;
            }
            catch (Exception ex)
            {
                throw CoralException.ThrowException(item => item.ConfigError, innerException: ex, param: Setting);
            }
        }
    }
}
