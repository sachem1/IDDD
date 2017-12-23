using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Coralcode.Framework.Aspect.InstanceProviders
{
    /// <summary>
    /// Service behavior for adding Unity instance provider in each
    /// endpoint dispatcher
    /// </summary>
    public class UnityInstanceProviderServiceBehavior : Attribute, IServiceBehavior
    {
        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints,
                                         BindingParameterCollection bindingParameters) {
            
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (var item in serviceHostBase.ChannelDispatchers)
            {
                var dispatcher = item as ChannelDispatcher;
                if (dispatcher != null) // add new instance provider for each end point dispatcher
                {
                    dispatcher.Endpoints.ToList().ForEach(endpoint =>
                    {
                        endpoint.DispatchRuntime.InstanceProvider = new UnityInstanceProvider(serviceDescription.ServiceType);
                    });
                }
            }
            
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }


        #endregion
    }

}