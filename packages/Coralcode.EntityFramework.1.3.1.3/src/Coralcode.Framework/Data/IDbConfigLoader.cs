using System.Collections.Generic;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.DynamicRepository;

namespace Coralcode.Framework.Data
{
    /// <summary>
    /// 配置文件加载器
    /// </summary>
    public interface IDbConfigLoader
    {
        /// <summary>
        /// 加载所有的配置文件
        /// </summary>
        /// <returns></returns>
        List<DbConfig> BuildStaticConfigs();

        /// <summary>
        /// 根据路由获取数据库连接配置
        /// </summary>
        /// <param name="staticConfig">静态配置</param>
        /// <param name="router">动态路由</param>
        /// <returns></returns>
        DbConfig GetDynamicDbConfigByRouter(DbConfig staticConfig, IDynamicRouter router);

        /// <summary>
        /// 获取所有静态配置相同的动态配置
        /// </summary>
        /// <param name="staticConfig">静态配置</param>
        /// <returns></returns>
        List<DbConfig> GetDynamicDbConfigs(DbConfig staticConfig);
        /// <summary>
        /// 获取配置
        /// </summary>
        /// <param name="staticConfig">静态配置</param>
        /// <param name="number">动态编号</param>
        /// <returns></returns>
        DbConfig GetDynamicDbConfigByNumber(DbConfig staticConfig, short number);
    }
}
