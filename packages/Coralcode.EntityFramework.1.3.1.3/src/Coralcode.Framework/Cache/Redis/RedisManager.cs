using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Coralcode.Framework.Common;
using Coralcode.Framework.ConfigManager;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Coralcode.Framework.Cache.Redis
{
    /// <summary>
    /// redis 管理
    /// </summary>
    public class RedisManager
    {
        /// <summary>
        /// redis配置文件信息
        /// </summary>
        private static ConcurrentDictionary<string, RedisSetting> _settings = new ConcurrentDictionary<string, RedisSetting>();
        /// <summary>
        /// redis链接信息
        /// </summary>
        private static ConcurrentDictionary<string, ConnectionMultiplexer> _connnections = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        private static TextWriter _log;

        public static void SetRedisLog(TextWriter writer)
        {
            _log = writer;
        }

        /// <summary>
        /// 客户端缓存操作对象
        /// 加锁同步执行
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static IDatabase GetClient(RedisSetting setting)
        {
            var config = _settings.GetOrAdd(setting.ClientName, item => setting);
            var connection = _connnections.AddOrUpdate(setting.ClientName, key =>
                            {
                                return ConnectionMultiplexer.Connect(Convert(config), _log);
                            },
                            (key, old) =>
                            {
                                if (!old.IsConnected)
                                {
                                    old.Close();
                                    old.Dispose();
                                    old = ConnectionMultiplexer.Connect(Convert(config), _log);

                                }
                                return old;
                            });
            return connection.GetDatabase();
        }

        private static ConfigurationOptions Convert(RedisSetting setting)
        {
            string[] writeServerList = setting.ServerList.Split(',');
            var sdkOptions = new ConfigurationOptions
            {
                ClientName = setting.ClientName,
                KeepAlive = 5 * 60,
                DefaultVersion = new Version(setting.Version),
                AbortOnConnectFail = false,
                DefaultDatabase = setting.DefaultDb,
                ConnectRetry = 10,
                Password = setting.Password,
            };
            foreach (var s in writeServerList)
            {
                sdkOptions.EndPoints.Add(s);
            };
            return sdkOptions;
        }
    }
}
