using System;

namespace Coralcode.Framework.Aspect
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class InjectAttribute : Attribute
    {
        /// <summary>
        /// 注册的名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 依赖注入的类型
        /// </summary>
        public Type RegisterType { get; set; }

        /// <summary>
        /// 注册条件
        /// </summary>
        public RegisterCondition Condition { get; set; }

        /// <summary>
        /// aop类型
        /// </summary>
        public AopType AopType { get; set; }

        /// <summary>
        /// 生命周期类型
        /// </summary>
        public LifetimeManagerType LifetimeManagerType { get; set; }
    }

    [Flags]
    public enum RegisterCondition
    {
        /// <summary>
        /// 是否必须
        /// </summary>
        IsRequire = 1,
    }

    /// <summary>
    /// 拦截类型
    /// </summary>
    public enum AopType
    {
        /// <summary>
        /// 不拦截
        /// </summary>
        None,
        /// <summary>
        /// 虚方法拦截
        /// </summary>
        VirtualMethodInterceptor,
        /// <summary>
        /// 接口拦截
        /// </summary>
        InterfaceInterceptor,
        /// <summary>
        /// 动态代理拦截
        /// </summary>
        TransparentProxyInterceptor,
        //这里可以添加自定义
    }

    public enum LifetimeManagerType
    {
        /// <summary>
        /// 每次通过Resolve或ResolveAll调用对象的时候都会重新创建一个新的对象。
        /// </summary>
        Transient,
        /// <summary>
        /// 容器控制生命周期管理，这个生命周期管理器是RegisterInstance默认使用的生命周期管理器，也就是单件实例
        /// </summary>
        ContainerControlled,
        /// <summary>
        /// 分层生命周期管理器，这个管理器类似于ContainerControlledLifetimeManager，
        /// 也是由UnityContainer来管理，也就是单件实例。
        /// 不过与ContainerControlledLifetimeManager不 同的是，
        /// 这个生命周期管理器是分层的，
        /// 因为Unity的容器时可以嵌套的，所以这个生命周期管理器就是针对这种情况，
        /// 当使用了这种生命周期管理器，
        /// 父容器 和子容器所维护的对象的生命周期是由各自的容器来管理
        /// </summary>
        Hierarchica,
        /// <summary>
        /// 外部控制生命周期管理器，这个 生命周期管理允许你使用RegisterType和RegisterInstance来注册对象之间的关系，
        /// 但是其只会对对象保留一个弱引用，
        /// 其生命周期 交由外部控制，也就是意味着你可以将这个对象缓存或者销毁而不用在意UnityContainer，
        /// 而当其他地方没有强引用这个对象时，其会被GC给销毁 掉。
        /// </summary>
        Externally,
        /// <summary>
        /// 每线程生命周期管理器，就是保证每个线程返回同一实例
        /// </summary>
        PerThread,
        /// <summary>
        /// 其类似于 TransientLifetimeManager，但是其不同在于，
        /// 如果应用了这种生命周期管理器，
        /// 则在第一调用的时候会创建一个新的对象，
        /// 而再次通过 循环引用访问到的时候就会返回先前创建的对象实例（单件实例），
        /// </summary>
        PerResolve,
        /// <summary>
        /// 一个Http请求，多次Resolve只会创建一个对象
        /// </summary>
        PerHttp
    }
}
