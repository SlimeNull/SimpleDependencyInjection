namespace SimpleDependencyInjection
{
    public interface IRequiredServiceProvider : IServiceProvider
    {
        object GetRequiredService(Type serviceType);
    }

    public partial class ServiceProvider : IServiceProvider, IRequiredServiceProvider
    {
        public ServiceProvider(ServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            this.serviceCollection = serviceCollection;
            singletonServices = new Dictionary<Type, object>();
        }

        public object? GetService(Type serviceType)
        {
            if (!ServiceUtils.FindRegisteredService(serviceCollection, serviceType, out ServiceItem? registeredService))
                return null;

            bool createNew = false;
            object? service = null;
            if (registeredService.Lifetime == ServiceLifetime.Transient)
            {
                createNew = true;
                service = ServiceUtils.CreateServiceInstance(this, registeredService, serviceType);
            }
            else if (registeredService.Lifetime == ServiceLifetime.Singleton)
            {
                if (!singletonServices.TryGetValue(serviceType, out service))
                {
                    createNew = true;
                    service = ServiceUtils.CreateServiceInstance(this, registeredService, serviceType);

                    if (service != null)
                        singletonServices.Add(serviceType, service);
                }
            }
            else if (registeredService.Lifetime == ServiceLifetime.Scoped)
            {
                if (currentScope != null)
                {
                    service = currentScope.GetScopedService(registeredService, serviceType, out createNew);
                }
            }

            if (service != null && createNew)
                ServiceUtils.InjectServiceMembers(this, service);

            return service;
        }

        public object GetRequiredService(Type serviceType)
        {
            if (!ServiceUtils.FindRegisteredService(serviceCollection, serviceType, out ServiceItem? registeredService))
                throw new InvalidOperationException($"Service is not registered. Service: {serviceType.FullName}");

            bool createNew = false;
            object? service;
            if (registeredService.Lifetime == ServiceLifetime.Transient)
            {
                createNew = true;
                service = ServiceUtils.CreateRequiredServiceInstance(this, registeredService, serviceType);
            }
            else if (registeredService.Lifetime == ServiceLifetime.Singleton)
            {
                if (!singletonServices.TryGetValue(serviceType, out service))
                {
                    createNew = true;
                    service = ServiceUtils.CreateRequiredServiceInstance(this, registeredService, serviceType);
                    singletonServices.Add(serviceType, service);
                }
            }
            else if (registeredService.Lifetime == ServiceLifetime.Scoped)
            {
                if (currentScope == null)
                    throw new InvalidOperationException($"Service scope must be created before getting scoped service. Service: {serviceType}");

                service = currentScope.GetRequiredScopedService(registeredService, serviceType, out createNew);
            }
            else
            {
                throw new InvalidOperationException($"Service lifetime is invalid. Service: {serviceType.FullName}");
            } 

            if (createNew)
                ServiceUtils.InjectServiceMembers(this, service);

            return service;
        }

        private readonly ServiceCollection serviceCollection;
        private readonly Dictionary<Type, object> singletonServices;

        private ServiceScope? currentScope;

        internal void OpenScope(ServiceScope serviceScope)
        {
            currentScope = serviceScope;
        }

        internal void CloseScope(ServiceScope serviceScope)
        {
            if (currentScope == serviceScope)
                currentScope = null;
        }
    }
}