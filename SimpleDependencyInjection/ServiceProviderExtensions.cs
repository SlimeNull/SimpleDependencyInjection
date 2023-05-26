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

        public static object GetRequiredService(this IServiceProvider serviceProvider, Type serviceType) =>
            serviceProvider.GetService(serviceType) ?? throw new InvalidOperationException($"Service or it's dependencies are not registered: {serviceType.FullName}");
        public static TService GetRequiredService<TService>(this IServiceProvider serviceProvider) where TService : class =>
            serviceProvider.GetService(typeof(TService)) as TService ?? throw new InvalidOperationException($"Service or it's dependencies are not registered: {typeof(TService).FullName}");

        public static IServiceScope CreateScope(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        }
    }
}
