namespace SimpleDependencyInjection
{
    internal class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly ServiceProvider serviceProvider;

        public ServiceScopeFactory(ServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IServiceScope CreateScope() => new ServiceScope(serviceProvider);
    }
}