using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Coralcode.Framework.Cache.Redis
{
    public class RedisSetting : ICloneable
    {
        public string ClientName { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// 默认数据库名称
        /// </summary>
        public int? DefaultDb { get; set; }

        /// <summary>
        /// 服务器列表
        /// </summary>
        public string ServerList { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否开启二级缓存
        /// </summary>
        public bool LocalCacheEnable { get; set; }

        /// <summary>
        /// 超时时间秒
        /// </summary>
        public int LocalCacheTimeout { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
