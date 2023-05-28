using System;
using System.Diagnostics.CodeAnalysis;
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

        public static bool FindRegisteredService(ServiceCollection serviceCollection, Type serviceType, [NotNullWhen(true)] out ServiceItem? registeredServiceItem)
        {
            Type? serviceGenericTypeDefinition = null;
            if (serviceType.IsGenericType)
                serviceGenericTypeDefinition = serviceType.GetGenericTypeDefinition();

            foreach (var serviceItem in serviceCollection)
            {
                if ((serviceItem.ServiceType == serviceType) ||
                    (serviceItem.ServiceType.IsGenericTypeDefinition && serviceItem.ServiceType == serviceGenericTypeDefinition))
                {
                    registeredServiceItem = serviceItem;
                    return true;
                }
            }

            registeredServiceItem = null;
            return false;
        }

        public static object? CreateServiceInstance(ServiceProvider serviceProvider, ServiceItem serviceItem, Type serviceType)
        {
            if (serviceItem.Factory != null)
                return serviceItem.Factory.Invoke(serviceProvider);


            if (GetServiceConstructor(serviceItem.ServiceType) is not ConstructorInfo constructor)
                return null;

            ParameterInfo[] parameters = constructor.GetParameters();
            object[] arguments = new object[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                object? currentArgument =
                    serviceProvider.GetService(parameters[i].ParameterType);

                if (currentArgument == null)
                    return null;

                arguments[i] = currentArgument;
            }

            return constructor.Invoke(arguments);
        }

        public static object CreateRequiredServiceInstance(ServiceProvider serviceProvider, ServiceItem serviceItem, Type serviceType)
        {
            if (serviceItem.Factory != null)
                return serviceItem.Factory.Invoke(serviceProvider) ?? 
                    throw new InvalidOperationException($"Service factory returned null value. Service: {serviceType.FullName}, Factory: {serviceItem.Factory}");


            if (GetServiceConstructor(serviceType) is not ConstructorInfo constructor)
                throw new InvalidOperationException($"Service has no service constructor. Service: {serviceType.FullName}");

            ParameterInfo[] parameterInfos = constructor.GetParameters();
            object[] parameters = new object[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                ParameterInfo parameterInfo = parameterInfos[i];
                object? currentArgument =
                    serviceProvider.GetService(parameterInfo.ParameterType);

                if (currentArgument == null)
                    throw new InvalidOperationException($"Service dependency can't be resolved. Service: {serviceType.FullName}, Dependency: {parameterInfo.ParameterType.FullName}");

                parameters[i] = currentArgument;
            }

            return constructor.Invoke(parameters);
        }

        public static void InjectServiceMembers(ServiceProvider serviceProvider, object serviceInstance)
        {
            Type serviceType = serviceInstance.GetType();

            FieldInfo[] fields = serviceType.GetFields();
            PropertyInfo[] properties = serviceType.GetProperties();

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<ServiceInjectAttribute>() is not ServiceInjectAttribute)
                    continue;

                field.SetValue(serviceInstance, serviceProvider.GetService(field.FieldType));
            }

            foreach (var property in properties)
            {
                if (property.GetCustomAttribute<ServiceInjectAttribute>() is not ServiceInjectAttribute)
                    continue;
                if (!property.CanWrite)
                    throw new InvalidOperationException($"Couldn't inject service for type {serviceType.FullName}, property {property.Name} has no setter");

                property.SetValue(serviceInstance, serviceProvider.GetService(property.PropertyType));
            }
        }
    }
}