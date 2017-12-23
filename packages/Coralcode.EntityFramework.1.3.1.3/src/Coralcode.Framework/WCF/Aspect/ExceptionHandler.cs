using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;
using Coralcode.Framework.Exceptions;
using Coralcode.Framework.Models;
using Coralcode.Framework.WCF.Exception;
using Coralcode.Framework.WCF.Json;
using Newtonsoft.Json;

namespace Coralcode.Framework.WCF.Aspect
{
    public class ExceptionHandler : Attribute, IErrorHandler, IServiceBehavior
    {
        public void Validate(ServiceDescription description,
            ServiceHostBase serviceHostBase)
        {
        }

        public void AddBindingParameters(ServiceDescription description, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection parameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceDescription description, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase dispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                var channelDispatcher = dispatcherBase as ChannelDispatcher;
                if (channelDispatcher != null)
                    channelDispatcher.ErrorHandlers.Add(this);
            }
        }

        public void ProvideFault(System.Exception error, MessageVersion version, ref Message fault)
        {
            var errorMessage = new ResultErrorMessage(CoralException.ThrowException<CoralErrorCode>(item => item.SystemError, innerException: error));
            fault = Message.CreateMessage(version,error.Source,new RawBodyWriter(errorMessage));
            fault.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Raw));
            HttpRequestMessageProperty reqProp = new HttpRequestMessageProperty();
            reqProp.Headers[HttpRequestHeader.ContentType] = "application/json";
            fault.Properties.Add(HttpRequestMessageProperty.Name, reqProp);
        }

        public bool HandleError(System.Exception error)
        {
            return true;
        }
    }
}
