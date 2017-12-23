using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using Coralcode.Framework.Aspect.Unity;

namespace Coralcode.Framework.Aspect.InstanceProviders
{
    /// <summary>
    /// The unity instance provider. This class provides
    /// an extensibility point for creating instances of wcf
    /// service.
    /// <remarks>
    /// The goal is to inject dependencies from the inception point
    /// </remarks>
    /// </summary>
    public class UnityInstanceProvider : IInstanceProvider
    {
        #region Members

        readonly Type _serviceType;

        #endregion

        #region Constructor

        /// <summary>
        /// Create a new instance of unity instance provider
        /// </summary>
        /// <param name="serviceType">The service where we apply the instance provider</param>
        public UnityInstanceProvider(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            _serviceType = serviceType;
        }

        #endregion

        #region IInstance Provider Members

        /// <summary>
        /// <see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/>
        /// </summary>
        /// <param name="instanceContext"><see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/></param>
        /// <param name="message"><see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/></param>
        /// <returns><see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/></returns>
        public object GetInstance(InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
        {
            //This is the only call to UNITY container in the whole solution
            var parentInterface = _serviceType.GetInterfaces().FirstOrDefault(item =>
            {
                var attrs = item.GetCustomAttributes(typeof(ServiceContractAttribute), false);
                return attrs.Length > 0;
            });
            return UnityService.Resolve(parentInterface ?? _serviceType);
        }


        /// <summary>
        /// <see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/>
        /// </summary>
        /// <param name="instanceContext"><see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/></param>
        /// <returns><see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/></returns>
        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        /// <summary>
        /// <see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/>
        /// </summary>
        /// <param name="instanceContext"><see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/></param>
        /// <param name="instance"><see cref="System.ServiceModel.Dispatcher.IInstanceProvider"/></param>
        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
            var disposable = instance as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        #endregion

    }
}