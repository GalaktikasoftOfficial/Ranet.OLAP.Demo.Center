using System;
using System.Collections.Generic;

namespace Ranet.Demo.Core.Configuration
{
    public sealed class ConnectionServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        private static readonly Lazy<ConnectionServiceProvider> LazyInstance = new Lazy<ConnectionServiceProvider>(() => new ConnectionServiceProvider());
        public static ConnectionServiceProvider Instance
        {
            get { return LazyInstance.Value; }
        }

        private ConnectionServiceProvider()
        {
        }

        public object GetService(Type serviceType)
        {
            object service;
            return _services.TryGetValue(serviceType, out service) ? service : null;
        }

        public void RegisterService(Type serviceType, object service)
        {
            if (service != null)
            {
                if (_services.ContainsKey(serviceType) == false)
                {
                    _services.Add(serviceType, service);
                }
            }
        }

        public void RegisterService<T>(object service)
        {
            RegisterService(typeof(T), service);
        }
    }

    public static class ServiceProviderExtension
    {
        public static T GetServiceExt<T>(this IServiceProvider service)
        {
            return (T)service.GetService(typeof(T));
        }
    }
}
