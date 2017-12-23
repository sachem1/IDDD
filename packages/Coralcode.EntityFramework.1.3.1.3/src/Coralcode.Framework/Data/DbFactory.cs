using Coralcode.Framework.ConfigManager;
using Coralcode.Framework.Exceptions;
using Coralcode.Framework.Modules;
using Coralcode.Framework.Security;
using Coralcode.Framework.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Aspect;
using Coralcode.Framework.Data.DynamicRepository;

namespace Coralcode.Framework.Data
{
    [Inject(RegisterType = typeof(DbFactory), LifetimeManagerType = LifetimeManagerType.ContainerControlled)]
    public class DbFactory
    {
        IModuleManager _moduleManager;
        IDbConfigLoader _dbConfigLoader;
        private bool _isInit;
        protected List<DbConfig> Configs = new List<DbConfig>();

        protected List<DbModule> DbModules = new List<DbModule>();

        protected Dictionary<Type, DbModule> TypeDbMapping = new Dictionary<Type, DbModule>();

        protected Dictionary<Type, DbConfig> TypeDbConfigMapping = new Dictionary<Type, DbConfig>();

        public DbFactory(IModuleManager moduleManager, IDbConfigLoader dbConfigLoader)
        {
            _moduleManager = moduleManager;
            _dbConfigLoader = dbConfigLoader;
        }

        public virtual void Init()
        {
            if (_isInit)
                return;

            DbModules = _moduleManager.GetModules(item => DbModule.IsDbModule(item.GetType())).Cast<DbModule>().ToList();
            Configs = _dbConfigLoader.BuildStaticConfigs();
            LoadStaticMapping();
            _isInit = true;
        }

        /// <summary>
        /// 加载静态配置映射
        /// </summary>
        protected virtual void LoadStaticMapping()
        {
            //加载静态模块
            DbModules.ForEach(item =>
            {
                item.EntityTypes.ForEach(type =>
                {
                    if (!TypeDbMapping.ContainsKey(type))
                        TypeDbMapping.Add(type, item);
                    if (TypeDbConfigMapping.ContainsKey(type))
                        return;
                    var config = Configs.FirstOrDefault(c => c.StaticCoden == item.Name);
                    TypeDbConfigMapping.Add(type, config);
                });
            });
        }

        #region 获取配置
        public DbConfig GetStaticDbConfig(Type type)
        {
            if (!TypeDbConfigMapping.ContainsKey(type))
                return new DbConfig
                {
                    NameOrConnectionString = "Default",
                    ReadConnectionString = "ReadDefault",
                    WriteConnectionString = "WriteDefault",
                    CommandTimeout = 30,
                    AutoMerageDataBase = true,
                    StaticCoden = "Default",
                };
            return TypeDbConfigMapping[type];
        }

        /// <summary>
        /// 根据路由获取动态配置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="router"></param>
        /// <returns></returns>
        public DbConfig GetDynamicDbConfig(Type type, IDynamicRouter router)
        {
            var staticDbConfig = GetStaticDbConfig(type);
            return _dbConfigLoader.GetDynamicDbConfigByRouter(staticDbConfig, router);
        }
        /// <summary>
        /// 根据id获取动态配置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public DbConfig GetDynamicDbConfig(Type type, long id)
        {
            var staticDbConfig = GetStaticDbConfig(type);
            return _dbConfigLoader.GetDynamicDbConfigByNumber(staticDbConfig, AnalysisNumber(id));
        }
        /// <summary>
        /// 根据id获取动态配置
        /// </summary>
        /// <param name="type"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public DbConfig GetDynamicDbConfigByNumber(Type type, short number)
        {
            var staticDbConfig = GetStaticDbConfig(type);
            return _dbConfigLoader.GetDynamicDbConfigByNumber(staticDbConfig, number);
        }

        /// <summary>
        /// 根据类型获取动态配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<DbConfig> GetDynamicDbConfigs(Type type)
        {
            var staticDbConfig = GetStaticDbConfig(type);
            return _dbConfigLoader.GetDynamicDbConfigs(staticDbConfig);
        }

        public DbConfig GetDynamicDbConfig(Type type, short number)
        {
            var staticDbConfig = GetStaticDbConfig(type);
            return _dbConfigLoader.GetDynamicDbConfigByNumber(staticDbConfig, number);
        }
        #endregion

        #region 获取数据初始化上下文
        public DbInitContext GetStaticDbInitContext(Type type, DbMode mode)
        {
            var dbConfig = GetStaticDbConfig(type);
            return GetDbInitContext(type, mode, dbConfig);
        }

        public DbInitContext GetDynamicDbInitContext(Type type, DbMode mode, short number)
        {
            var dbConfig = GetDynamicDbConfig(type, number);
            return GetDbInitContext(type, mode, dbConfig);
        }

        public DbInitContext GetDynamicDbInitContext(Type type, DbMode mode, long id)
        {
            var dbConfig = GetDynamicDbConfig(type, id);
            return GetDbInitContext(type, mode, dbConfig);
        }
        public DbInitContext GetDynamicDbInitContext(Type type, DbMode mode, string coden)
        {
            var dbConfig = GetDynamicDbConfig(type, new SampleRouter(coden));
            return GetDbInitContext(type, mode, dbConfig);
        }

        public DbInitContext GetDbInitContext(Type type, DbMode mode, DbConfig dbConfig)
        {
            if (!TypeDbMapping.ContainsKey(type))
                throw new NotFoundException($"未找到{type}对应的数据模块");

            var dbModule = TypeDbMapping[type];
            return new DbInitContext(dbConfig, dbModule, mode);
        }
        #endregion


        /// <summary>
        /// 根据id分析出number
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public short AnalysisNumber(long id)
        {
            var idStr = id.ToString();
            var dbNum = Convert.ToInt16(idStr.Substring(idStr.Length - 2));
            if (idStr.Length == 18)
                return dbNum;

            return Convert.ToInt16(idStr.PadLeft(19, '0').Substring(0, 2)) > 20 ? GetDbNum(id) : dbNum;

        }

        public long GerneralId(short number)
        {
            return IdentityGenerator.NextId(number);
        }

        /// <summary>
        /// 生成静态分库的id
        /// </summary>
        /// <returns></returns>
        public long GeneralId()
        {

            return IdentityGenerator.NextId();
        }
        public long GeneralId<TEntity>(IDynamicRouter router)
        {
            return IdentityGenerator.NextId(GetDynamicDbConfig(typeof(TEntity),router).Number);
        }

        private short GetDbNum(long id)
        {
            var idBinStr = Convert.ToString(id, 2).PadLeft(64, '0');
            return Convert.ToInt16(idBinStr.Substring(idBinStr.Length - 6), 2);
        }
    }
}
