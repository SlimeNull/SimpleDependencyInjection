namespace SimpleDependencyInjection
{

    public partial class ServiceProvider : IServiceProvider
    {
        public ServiceProvider(ServiceCollection serviceCollection)
        {
            if (serviceCollection == null)
                throw new ArgumentNullException(nameof(serviceCollection));

            this.serviceCollection = serviceCollection;
            singletonServices = new Dictionary<Type, object>();
        }

        public object? GetServiceCore(Type serviceType)
        {
            if (serviceCollection.FirstOrDefault(item => item.ServiceType == serviceType) is not ServiceItem registeredService)
                return null;

            bool createNew = false;
            object? service = null;
            if (registeredService.Lifetime == ServiceLifetime.Transient)
            {
                createNew = true;
                service = ServiceUtils.CreateServiceInstance(this, serviceType);
            }
            else if (registeredService.Lifetime == ServiceLifetime.Singleton)
            {
                if (!singletonServices.TryGetValue(serviceType, out service))
                {
                    createNew = true;
                    service = ServiceUtils.CreateServiceInstance(this, serviceType);
                    singletonServices.Add(serviceType, service);
                }
            }
            else if (registeredService.Lifetime == ServiceLifetime.Scoped)
            {
                if (currentScope != null)
                {
                    service = currentScope.GetScopedService(serviceType, out createNew);
                }
            }

            if (createNew && service != null)
                ServiceUtils.InjectServiceMembers(this, service);

            return service;
        }


        public object? GetService(Type serviceType) => GetServiceCore(serviceType);

        internal readonly ServiceCollection serviceCollection;
        internal readonly Dictionary<Type, object> singletonServices;

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