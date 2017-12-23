using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using Coralcode.Framework.Data;
using Coralcode.Framework.Data.Core;
using Coralcode.Framework.Data.DynamicRepository;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Reflection;
using System.Diagnostics;

namespace Coralcode.EntityFramework.ModelConfiguration
{
    public class EntityConfigManager
    {

        /// <summary>
        /// 配置数据库的表和实体之间的映射
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="context"></param>
        public static void ConfigDbTypes(DbModelBuilder modelBuilder, DbInitContext context)
        {
            var configs = GetTypeAndConfigedTypesMapping(context);

            context.Types.ForEach(item =>
            {
                Type type;
                dynamic config;
                if (!configs.TryGetValue(item, out type))
                    config = GeneralDefautConfig(item);
                else
                    config = Activator.CreateInstance(type);
                modelBuilder.Configurations.Add(config);
            });
            ConfigConvertions(modelBuilder.Conventions);
        }

        /// <summary>
        /// 获取类型和已配置类型的映射
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Dictionary<Type, Type> GetTypeAndConfigedTypesMapping(DbInitContext context)
        {
            var map = new Dictionary<Type, Type>();

            LinqExtensions.ForEach(MetaDataManager.Type.GetAll(), item =>
            {
                if (item.BaseType == null || !item.BaseType.IsGenericType || item.BaseType.IsAbstract)
                    return;
                if (!IsDefined(item))
                    return;
                var genericType =
                    Enumerable.FirstOrDefault(item.BaseType.GetGenericArguments(), data => data.IsSubclassOf(typeof(Entity)));
                if (genericType == null)
                    return;
                //如果是抽象类或者标记成忽略则无需加载
                if (IgnoreAttribute.IsDefined(genericType)||genericType.IsAbstract)
                    return;
                if (!context.Types.Contains(genericType))
                    return;
                map.Add(genericType, item);
            });
            return map;

        }

        /// <summary>
        /// 是否是自定义配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsDefined(Type type)
        {
            return type.GetInterfaces().Contains(typeof(IEntityConfiguration));
        }

        /// <summary>
        /// 生成默认配置
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static dynamic GeneralDefautConfig(Type type)
        {
            if (type.GetInterfaces().Contains(typeof(IDynamicRouter)))
                return Activator.CreateInstance(typeof(DynamicEntityTypeConfig<>).MakeGenericType(type));
            else
                return Activator.CreateInstance(typeof(StaticEntityTypeConfig<>).MakeGenericType(type));
        }

        /// <summary>
        /// 配置数据校验和类型转换
        /// </summary>
        /// <param name="convertions"></param>
        public static void ConfigConvertions(ConventionsConfiguration convertions)
        {
            convertions.Add(new DecimalAttributeConvention());

        }

    }
}
