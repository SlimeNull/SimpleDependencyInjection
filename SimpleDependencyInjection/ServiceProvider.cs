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

            return registeredService.Lifetime switch
            {
                ServiceLifetime.Transient => ServiceUtils.CreateServiceInstance(this, serviceType),
                ServiceLifetime.Singleton => singletonServices.GetOrAdd(serviceType, () => ServiceUtils.CreateServiceInstance(this, serviceType)),
                ServiceLifetime.Scoped => currentScope != null ? currentScope.GetService(serviceType) : null,
                _ => null
            };
        }


        public object? GetService(Type serviceType) => GetServiceCore(serviceType);

        internal readonly ServiceCollection serviceCollection;
        internal readonly Dictionary<Type, object> singletonServices;

        internal ServiceScope? currentScope;
    }
}