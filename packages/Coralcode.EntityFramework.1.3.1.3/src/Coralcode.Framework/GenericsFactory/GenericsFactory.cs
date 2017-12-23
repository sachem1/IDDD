using System;
using System.Collections.Generic;
using System.Linq;
using Coralcode.Framework.Aspect.Unity;
using Coralcode.Framework.Data;
using Coralcode.Framework.Exceptions;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.Models;
using Coralcode.Framework.Reflection;

namespace Coralcode.Framework.GenericsFactory
{

    /// <summary>
    /// 建议将所有的子类在容器中注册为单例,避免重复创建的性能损耗
    /// 工厂只负责创建，需要外部自己回收(这里可能是个问题)
    /// 一，在自己继承工厂,并且将策略的执行放到工厂执行,完成以后销毁
    /// 例如给出如下方法
    ///  T Execute(T)(name,func(strategy,T))
    /// 二,使用调整容器的默认生命周期,通过默认生命周期来控制
    /// </summary>
    /// <typeparam name="TStrategy"></typeparam>
    public interface IGenericsFactory<TStrategy>
        where TStrategy : IStrategy
    {

        /// <summary>
        /// 可以由外部调用客户来决定如何初始化
        /// </summary>
        /// <param name="creator"></param>
        void SetCreator(Func<Type, TStrategy> creator);

        /// <summary>
        /// 注册策略
        /// </summary>
        /// <param name="type"></param>
        void Regist(Type type);

        /// <summary>
        /// 获取策略
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        TStrategy GetStrategy(string name);

        /// <summary>
        /// 获取策略名称列表
        /// </summary>
        /// <returns></returns>
        List<string> GetStrategys();

        /// <summary>
        /// 获取策略的类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Type GetStrategyType(string name);

        /// <summary>
        /// 获取所有策略的类型
        /// </summary>
        /// <returns></returns>
        List<Type> GetStrategyTypes();
    }

    public class GenericsFactory<TStrategy> : IGenericsFactory<TStrategy>
        where TStrategy : IStrategy
    {
        public GenericsFactory()
        {
            InitStrategys();
            Creator = type => (TStrategy)UnityService.Resolve(type);
        }

        /// <summary>
        /// 初始化策略
        /// </summary>
        protected void InitStrategys()
        {
            if (Strategys == null)
            {
                Strategys = MetaDataManager.Type.Find(item => item.GetInterfaces().Contains(typeof(TStrategy)) || item.IsSubclassOf(typeof(TStrategy)))
                        .Where(item => !item.IsAbstract)
                        .SelectMany(item =>
                        {
                            var descs = item.GetStrategys();
                            if (descs == null || IgnoreAttribute.IsDefined(item))
                                return new List<EditableKeyValuePair<string, Type>>();
                            return descs.Select(desc => new EditableKeyValuePair<string, Type>(desc, item));
                        })
                        .DistinctBy(item => item.Key)
                        .Where(item => !string.IsNullOrEmpty(item.Key))
                        .ToDictionary(item => item.Key, item => item.Value);
            }
        }

        protected Dictionary<string, Type> Strategys;
        protected Func<Type, TStrategy> Creator;

        public void SetCreator(Func<Type, TStrategy> creator)
        {
            Creator = creator ?? throw new ArgumentNullException(nameof(creator));
        }

        /// <summary>
        /// 注册策略
        /// </summary>
        /// <param name="type"></param>
        public virtual void Regist(Type type)
        {
            if (type.GetInterfaces().Contains(typeof(TStrategy)) && type.IsSubclassOf(typeof(TStrategy)))
                throw new TypeLoadException(string.Format("类型不是从{0},继承", typeof(TStrategy).Name));

            var descs = type.GetStrategys();
            descs.ForEach(item =>
            {
                if (Strategys.ContainsKey(item))
                    return;
                Strategys.Add(item, type);
            });
        }


        /// <summary>
        /// 获取策略
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual TStrategy GetStrategy(string name)
        {
            if (!Strategys.ContainsKey(name))
                throw new NotFoundException($"策略名为{name}的{typeof(TStrategy).Name}策略不存在");
            return Creator(Strategys[name]);
        }

        /// <summary>
        /// 获取策略名称列表
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetStrategys()
        {
            return Strategys.Keys.ToList();
        }

        /// <summary>
        /// 获取策略的类型
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Type GetStrategyType(string name)
        {
            if (!Strategys.ContainsKey(name))
                throw new NotFoundException($"策略名为{name}的{typeof(TStrategy).Name}策略不存在");
            return Strategys[name];
        }

        /// <summary>
        /// 获取所有策略的类型
        /// </summary>
        /// <returns></returns>
        public virtual List<Type> GetStrategyTypes()
        {
            return Strategys.Values.ToList();
        }
    }
}
