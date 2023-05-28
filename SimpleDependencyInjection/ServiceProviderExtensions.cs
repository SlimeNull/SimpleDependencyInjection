using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static TService? GetService<TService>(this IServiceProvider serviceProvider) where TService : class =>
            serviceProvider.GetService(typeof(TService)) as TService;

        public static object GetRequiredService(this IServiceProvider serviceProvider, Type serviceType)
        {
            if (serviceProvider is IRequiredServiceProvider requiredServiceProvider)
                return requiredServiceProvider.GetRequiredService(serviceType);

            object? service = serviceProvider.GetService(serviceType);
            if (service == null)
                throw new InvalidOperationException($"Service is not registered. Service: {serviceType}");

            return service;
        }
        public static TService GetRequiredService<TService>(this IServiceProvider serviceProvider) where TService : class =>
            (TService)serviceProvider.GetRequiredService(typeof(TService));

        public static IServiceScope CreateScope(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }
    }
}
