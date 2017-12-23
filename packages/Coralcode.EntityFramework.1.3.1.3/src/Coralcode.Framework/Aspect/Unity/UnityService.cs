using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Coralcode.Framework.Aspect.UnityExtension;
using Coralcode.Framework.Extensions;
using Coralcode.Framework.MessageBus.Event;
using Coralcode.Framework.Reflection;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;

namespace Coralcode.Framework.Aspect.Unity
{
    /// <summary>
    /// IOC容器
    /// </summary>
    public class UnityService
    {

        static UnityService()
        {
            //注册标记
            Current = new UnityContainer();
            LifetimeManager = new ConcurrentDictionary<LifetimeManagerType, Type>(new Dictionary<LifetimeManagerType, Type> {
                {LifetimeManagerType.Transient, typeof (TransientLifetimeManager)},
                {LifetimeManagerType.ContainerControlled, typeof (ContainerControlledLifetimeManager)},
                {LifetimeManagerType.Hierarchica, typeof (HierarchicalLifetimeManager)},
                {LifetimeManagerType.Externally, typeof (ExternallyControlledLifetimeManager)},
                {LifetimeManagerType.PerThread, typeof (PerThreadLifetimeManager)},
                {LifetimeManagerType.PerResolve, typeof (PerResolveLifetimeManager)},
                {LifetimeManagerType.PerHttp, typeof (PerHttpLifetimeManager)}
            });
            Current.AddNewExtension<Interception>();
            MetaDataManager.Type.GetAll().ForEach(item =>
            {
                if (item == null)
                    return;
                var registers = item.GetCustomAttributes<InjectAttribute>().ToList();
                if (registers.Count == 0)
                    return;
                registers.ForEach(register =>
                {
                    if (register.RegisterType != null)
                        RegisterType(register.Name, register.RegisterType, item, GetLifetimeManager(register.LifetimeManagerType), GetInjectionMembers(register.AopType, item));
                    else
                        RegisterType(register.Name, item, GetLifetimeManager(register.LifetimeManagerType), GetInjectionMembers(register.AopType, item));
                });
            });
        }

        #region 属性

        /// <summary>
        ///     Get the current configured container
        /// </summary>
        /// <returns>Configured container</returns>
        private static IUnityContainer Current { get; set; }

        private static readonly ConcurrentDictionary<LifetimeManagerType, Type> LifetimeManager;

        #endregion


        /// <summary>
        /// 在当前模块中注册接口的实现
        /// </summary>
        protected virtual void Regist()
        {
        }
        /// <summary>
        /// 在当前模块中注册应用程序启动事件
        /// </summary>
        protected virtual void RegistComplete()
        {
        }

        public static void SetLifetimeManagerDict<T>(LifetimeManagerType lifetimeManagerType)
        {
            LifetimeManager.AddOrUpdate(lifetimeManagerType, key => typeof(T), (key, value) => typeof(T));
        }

        #region 注册相关的方法
        /// <summary>
        /// 获取生命周期
        /// </summary>
        /// <param name="lifetimeManagerType"></param>
        /// <returns></returns>
        public static LifetimeManager GetLifetimeManager(LifetimeManagerType lifetimeManagerType)
        {
            Current.RegisterType<IEventBus, EventBus>(new TransientLifetimeManager());
            //Current.RegisterType<IEventBus, EventBus>(new PerHttpLifetimeManager());
            Type type;
            if (!LifetimeManager.TryGetValue(lifetimeManagerType, out type))
                return new TransientLifetimeManager();
            return (LifetimeManager)Activator.CreateInstance(type);

            //switch (lifetimeManagerType)
            //{
            //    case LifetimeManagerType.Transient:
            //        return new TransientLifetimeManager();
            //    case LifetimeManagerType.ContainerControlled:
            //        return new ContainerControlledLifetimeManager();
            //    case LifetimeManagerType.Hierarchica:
            //        return new HierarchicalLifetimeManager();
            //    case LifetimeManagerType.Externally:
            //        return new ExternallyControlledLifetimeManager();
            //    case LifetimeManagerType.PerThread:
            //        return new PerThreadLifetimeManager();
            //    case LifetimeManagerType.PerResolve:
            //        return new PerResolveLifetimeManager();
            //    default:
            //        return new PerResolveLifetimeManager();
            //}
        }

        /// <summary>
        /// 注入aop方法
        /// </summary>
        /// <param name="aopType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static InjectionMember[] GetInjectionMembers(AopType aopType, Type type)
        {
            var members = new List<InjectionMember>();
            switch (aopType)
            {
                case AopType.VirtualMethodInterceptor:
                    members.Add(new Interceptor<VirtualMethodInterceptor>());
                    break;
                case AopType.InterfaceInterceptor:
                    members.Add(new Interceptor<InterfaceInterceptor>());
                    break;
                case AopType.TransparentProxyInterceptor:
                    members.Add(new Interceptor<TransparentProxyInterceptor>());
                    break;
                case AopType.None:
                    return members.ToArray();
            }
            members.AddRange(type.GetCustomAttributes()
                   .Where(item => item.GetType().IsSubclassOf(typeof(UnityAopAttribute)))
                   .Cast<UnityAopAttribute>()
                   .Select(item => new InterceptionBehavior(item)));
            return members.ToArray();

        }
        #endregion

        /// <summary>
        ///     注册泛型类型
        /// </summary>
        /// <param name="injectionMembers">构造函数参数</param>
        public static void Register<TTarget, TSource>(string name="", params dynamic[] injectionMembers) where TSource : TTarget
        {
            RegisterType<TTarget, TSource>(name, injectionMembers);
        }
        public static void RegisterInstance(Type type, object instance)
        {
            Current.RegisterInstance(type, instance);
        }

        /// <summary>
        ///     注册泛型类型
        /// </summary>
        /// <param name="name"></param>
        /// <param name="injectionMembers">构造函数参数</param>
        public static void RegisterType<TTarget, TSource>(string name, params dynamic[] injectionMembers) where TSource : TTarget
        {
            var members = new List<InjectionMember>();
            LinqExtensions.ForEach(injectionMembers, item =>
            {
                if (item is InjectionMember)
                    members.Add(item);
                if (item is InjectionMember[])
                    members.AddRange(item);
                else if (item is ConstructorParameter)
                    members.Add(new InjectionConstructor(item.Value));
                else if (item is ConstructorParameter[])
                    members.AddRange((item as ConstructorParameter[]).Select(data => new InjectionConstructor(data.Value)));
            });

            var lifetimeManager = injectionMembers.OfType<LifetimeManager>().FirstOrDefault();
            if (string.IsNullOrEmpty(name))
            {
                if (lifetimeManager == null && injectionMembers == null)
                    Current.RegisterType<TTarget, TSource>();

                else if (lifetimeManager == null)
                    Current.RegisterType<TTarget, TSource>(members.ToArray());
                else if (injectionMembers == null)
                    Current.RegisterType<TTarget, TSource>(lifetimeManager);
                else
                    Current.RegisterType<TTarget, TSource>(lifetimeManager, members.ToArray());

            }
            else
            {
                if (lifetimeManager == null && injectionMembers == null)
                    Current.RegisterType<TTarget, TSource>(name);

                else if (lifetimeManager == null)
                    Current.RegisterType<TTarget, TSource>(name, members.ToArray());
                else if (injectionMembers == null)
                    Current.RegisterType<TTarget, TSource>(name, lifetimeManager);
                else
                    Current.RegisterType<TTarget, TSource>(name, lifetimeManager, members.ToArray());

            }
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <param name="injectionMembers"></param>
        public static void RegisterType(string name, Type target, Type source, params dynamic[] injectionMembers)
        {
            var members = new List<InjectionMember>();
            LinqExtensions.ForEach(injectionMembers, item =>
            {
                if (item is InjectionMember)
                    members.Add(item);
                if (item is InjectionMember[])
                    members.AddRange(item);
                else if (item is ConstructorParameter)
                    members.Add(new InjectionConstructor(item.Value));
                else if (item is ConstructorParameter[])
                    members.AddRange((item as ConstructorParameter[]).Select(data => new InjectionConstructor(data.Value)));
            });
            var lifetimeManager = injectionMembers.OfType<LifetimeManager>().FirstOrDefault();


            if (string.IsNullOrEmpty(name))
            {

                if (lifetimeManager == null && injectionMembers == null)
                    Current.RegisterType(target, source);
                else if (lifetimeManager == null)
                    Current.RegisterType(target, source, members.ToArray());
                else if (injectionMembers == null)
                    Current.RegisterType(target, source, lifetimeManager);
                else
                    Current.RegisterType(target, source, lifetimeManager, members.ToArray());

            }
            else
            {
                if (lifetimeManager == null && injectionMembers == null)
                    Current.RegisterType(target, source, name);
                else if (lifetimeManager == null)
                    Current.RegisterType(target, source, name, members.ToArray());
                else if (injectionMembers == null)
                    Current.RegisterType(target, source, name, lifetimeManager);
                else
                    Current.RegisterType(target, source, name, lifetimeManager, members.ToArray());
            }
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="lifetimeManager"></param>
        /// <param name="injectionMembers"></param>
        public static void RegisterType(Type target, Type source, params dynamic[] injectionMembers)
        {
            var members = new List<InjectionMember>();
            LinqExtensions.ForEach(injectionMembers, item =>
            {
                if (item is InjectionMember)
                    members.Add(item);
                if (item is InjectionMember[])
                    members.AddRange(item);
                else if (item is ConstructorParameter)
                    members.Add(new InjectionConstructor(item.Value));
                else if (item is ConstructorParameter[])
                    members.AddRange((item as ConstructorParameter[]).Select(data => new InjectionConstructor(data.Value)));
            });
            var lifetimeManager = injectionMembers.OfType<LifetimeManager>().FirstOrDefault();




            if (lifetimeManager == null && injectionMembers == null)
                Current.RegisterType(target, source);
            else if (lifetimeManager == null)
                Current.RegisterType(target, source, members.ToArray());
            else if (injectionMembers == null)
                Current.RegisterType(target, source, lifetimeManager);
            else
                Current.RegisterType(target, source, lifetimeManager, members.ToArray());
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="injectionMembers"></param>
        public static void RegisterType(Type type, params dynamic[] injectionMembers)
        {
            var members = new List<InjectionMember>();
            LinqExtensions.ForEach(injectionMembers, item =>
            {
                if (item is InjectionMember)
                    members.Add(item);
                if (item is InjectionMember[])
                    members.AddRange(item);
                else if (item is ConstructorParameter)
                    members.Add(new InjectionConstructor(item.Value));
                else if (item is ConstructorParameter[])
                    members.AddRange((item as ConstructorParameter[]).Select(data => new InjectionConstructor(data.Value)));
            });
            var lifetimeManager = injectionMembers.OfType<LifetimeManager>().FirstOrDefault();

            if (lifetimeManager == null && injectionMembers == null)
                Current.RegisterType(type);
            else if (lifetimeManager == null)
                Current.RegisterType(type, members.ToArray());
            else if (injectionMembers == null)
                Current.RegisterType(type, lifetimeManager);
            else
                Current.RegisterType(type, lifetimeManager, members.ToArray());
        }

        /// <summary>
        /// 注册类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="injectionMembers"></param>
        /// <param name="name"></param>
        public static void RegisterType(string name, Type type, params dynamic[] injectionMembers)
        {
            var members = new List<InjectionMember>();
            LinqExtensions.ForEach(injectionMembers, item =>
            {
                if (item is InjectionMember)
                    members.Add(item);
                if (item is InjectionMember[])
                    members.AddRange(item);
                else if (item is ConstructorParameter)
                    members.Add(new InjectionConstructor(item.Value));
                else if (item is ConstructorParameter[])
                    members.AddRange((item as ConstructorParameter[]).Select(data => new InjectionConstructor(data.Value)));
            });
            var lifetimeManager = injectionMembers.OfType<LifetimeManager>().FirstOrDefault();

            if (string.IsNullOrEmpty(name))
            {
                if (lifetimeManager == null && injectionMembers == null)
                    Current.RegisterType(type);
                else if (lifetimeManager == null)
                    Current.RegisterType(type, members.ToArray());
                else if (injectionMembers == null)
                    Current.RegisterType(type, lifetimeManager);
                else
                    Current.RegisterType(type, lifetimeManager, members.ToArray());
            }
            else
            {
                if (lifetimeManager == null && injectionMembers == null)
                    Current.RegisterType(type, name);
                else if (lifetimeManager == null)
                    Current.RegisterType(type, name, members.ToArray());
                else if (injectionMembers == null)
                    Current.RegisterType(type, name, lifetimeManager);
                else
                    Current.RegisterType(type, name, lifetimeManager, members.ToArray());
            }
        }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static object Resolve(Type source)
        {
            return Current.Resolve(source);
        }

        public static bool HasRegistered(Type type)
        {
            return Current.IsRegistered(type);
        }

        /// <summary>
        /// 创建泛型实例
        /// </summary>
        /// <returns></returns>
        public static T Resolve<T>()
        {
            return Current.Resolve<T>();
        }

        /// <summary>
        /// 创建泛型实例
        /// </summary>
        /// <returns></returns>
        public static T Resolve<T>(string name)
        {
            return Current.Resolve<T>(name);
        }

        public static void Release(object obj)
        {
            Current.Teardown(obj);
        }
    }
}
