namespace SimpleDependencyInjection
{
    public interface IServiceCollection : ICollection<ServiceItem>
    {
        void AddScoped(Type serviceType);
        void AddScoped(Type serviceType, ServiceFactory factory);
        void AddScoped<TService>() where TService : class;
        void AddScoped<TService>(ServiceFactory<TService> factory) where TService : class;
        void AddSingleton(Type serviceType);
        void AddSingleton<TService>() where TService : class;
        void AddSingleton(Type serviceType, ServiceFactory factory);
        void AddSingleton<TService>(ServiceFactory<TService> factory) where TService : class;
        void AddTransient(Type serviceType);
        void AddTransient(Type serviceType, ServiceFactory factory);
        void AddTransient<TService>() where TService : class;
        void AddTransient<TService>(ServiceFactory<TService> factory) where TService : class;

        IServiceProvider BuildServiceProvider();
    }


    public delegate object ServiceFactory(IServiceProvider serviceProvider);

    public delegate TService ServiceFactory<TService>(IServiceProvider serviceProvider) where TService : class;
}