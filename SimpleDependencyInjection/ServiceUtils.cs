using System;
using System.Reflection;

namespace SimpleDependencyInjection
{
    internal static class ServiceUtils
    {
        public static ServiceFactory ToBaseFactory<TService>(ServiceFactory<TService> factory) where TService : class
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            return provider => factory.Invoke(provider);
        }

        public static ConstructorInfo? GetServiceConstructor(Type serviceType)
        {
            ConstructorInfo[] constructors =
                serviceType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

            foreach (ConstructorInfo constructor in constructors)
                if (constructor.GetCustomAttribute<ServiceConstructorAttribute>() is ServiceConstructorAttribute)
                    return constructor;

            return constructors.FirstOrDefault(constructor => constructor.IsPublic);
        }

        public static object CreateServiceInstance(ServiceProvider serviceProvider, Type serviceType)
        {
            ServiceItem registeredService =
                serviceProvider.serviceCollection.First(item => item.ServiceType == serviceType);
            if (registeredService.Factory != null)
                return registeredService.Factory.Invoke(serviceProvider);

            if (GetServiceConstructor(serviceType) is not ConstructorInfo constructor)
                throw new InvalidOperationException($"Service must has one constructor: {serviceType}");

            ParameterInfo[] parameters = constructor.GetParameters();
            object[] arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                arguments[i] = serviceProvider.GetRequiredService(parameters[i].ParameterType);

            return constructor.Invoke(arguments);
        }
    }
}