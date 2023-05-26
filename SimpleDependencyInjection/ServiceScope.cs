using System;

namespace SimpleDependencyInjection
{
    internal class ServiceScope : IServiceScope, IDisposable
    {
        public ServiceScope(ServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            scopedServices = new Dictionary<Type, object>();

            serviceProvider.currentScope = this;
        }

        ~ServiceScope()
        {
            Dispose(false);
        }

        private readonly ServiceProvider serviceProvider;
        private readonly Dictionary<Type, object> scopedServices;

        IServiceProvider IServiceScope.ServiceProvider => serviceProvider;
        public object? GetService(Type serviceType) =>
            scopedServices.GetOrAdd(serviceType, () => ServiceUtils.CreateServiceInstance(serviceProvider, serviceType));

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
                return;

            if (serviceProvider.currentScope == this)
                serviceProvider.currentScope = null;
        }
    }
}