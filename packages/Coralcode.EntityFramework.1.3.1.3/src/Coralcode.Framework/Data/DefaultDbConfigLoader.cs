using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Data.DynamicRepository;
using Coralcode.Framework.Extensions;

namespace Coralcode.Framework.Data
{
    public class DefaultDbConfigLoader : IDbConfigLoader
    {
        protected List<DbConfig> AllConfigs=new List<DbConfig>();
        protected readonly ConcurrentDictionary<string, DbConfig> DynamicCodenConfigsMapping = new ConcurrentDictionary<string, DbConfig>();
        protected readonly ConcurrentDictionary<int, DbConfig> NumberConfigsMapping = new ConcurrentDictionary<int, DbConfig>();
        protected readonly ConcurrentDictionary<string, List<DbConfig>> StaticCodenConfigsMapping = new ConcurrentDictionary<string, List<DbConfig>>();

        public virtual List<DbConfig> BuildStaticConfigs()
        {
            AllConfigs = new List<DbConfig>().LoadFormXml(AppConfig.GetAbsolutePath("DbConfig"));
            AllConfigs.ForEach(config => config.PreAnalysis());
            var configs = AllConfigs.DistinctBy(item => item.StaticCoden).ToList();
            return configs;
        }

        /// <summary>
        /// 根据路由获取数据库连接配置
        /// </summary>
        /// <param name="staticConfig">静态配置</param>
        /// <param name="router">动态路由</param>
        /// <returns></returns>
        public virtual DbConfig GetDynamicDbConfigByRouter(DbConfig staticConfig, IDynamicRouter router)
        {
            return DynamicCodenConfigsMapping.GetOrAdd(router.Coden,
                coden => AllConfigs.Find(item => item.StaticCoden == staticConfig.StaticCoden
                                                  && item.DynamicCoden == router.Coden));
        }

        /// <summary>
        /// 获取所有静态配置相同的动态配置
        /// </summary>
        /// <param name="staticConfig">静态配置</param>
        /// <returns></returns>
        public virtual List<DbConfig> GetDynamicDbConfigs(DbConfig staticConfig)
        {
            return StaticCodenConfigsMapping.GetOrAdd(staticConfig.StaticCoden,
                coden => AllConfigs.FindAll(item => item.StaticCoden == staticConfig.StaticCoden));
        }

        /// <summary>
        /// 根据动态路由编号获取配置
        /// </summary>
        /// <param name="staticConfig">静态配置</param>
        /// <param name="number">动态编号</param>
        /// <returns></returns>
        public virtual DbConfig GetDynamicDbConfigByNumber(DbConfig staticConfig, short number)
        {
            return NumberConfigsMapping.GetOrAdd(number,
                coden => AllConfigs.Find(item => item.StaticCoden == staticConfig.StaticCoden
                                                  && item.Number == number));
        }
    }
}
