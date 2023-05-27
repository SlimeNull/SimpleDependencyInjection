using System;

namespace SimpleDependencyInjection
{
    internal class ServiceScope : IServiceScope, IDisposable
    {
        public ServiceScope(ServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            scopedServices = new Dictionary<Type, object>();

            serviceProvider.OpenScope(this);
        }

        ~ServiceScope()
        {
            Dispose(false);
        }

        private readonly ServiceProvider serviceProvider;
        private readonly Dictionary<Type, object> scopedServices;

        IServiceProvider IServiceScope.ServiceProvider => serviceProvider;
        internal object? GetScopedService(Type serviceType, out bool createNew)
        {
            if (scopedServices.TryGetValue(serviceType, out var service))
            {
                createNew = false;
                return service;
            }
            else
            {
                service = ServiceUtils.CreateServiceInstance(serviceProvider, serviceType);
                scopedServices.Add(serviceType, service);
                createNew = true;
                return service;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                return;

            serviceProvider.CloseScope(this);
        }
    }
}