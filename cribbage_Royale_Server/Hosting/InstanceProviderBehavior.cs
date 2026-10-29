using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace cribbage_Royale_Server.Hosting
{
    public class InstanceProviderBehavior : IServiceBehavior, IInstanceProvider
    {
        private readonly Func<object> _createInstance;

        public InstanceProviderBehavior(Func<object> createInstance)
        {
            _createInstance = createInstance;
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return _createInstance();
        }

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return _createInstance();
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase dispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = dispatcherBase as ChannelDispatcher;

                if (channelDispatcher != null)
                {
                    foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
                    {
                        endpointDispatcher.DispatchRuntime.InstanceProvider = this;
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }
}