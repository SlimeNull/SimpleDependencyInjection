using System.Collections.ObjectModel;
using System.Reflection;

namespace SimpleDependencyInjection
{
    public class ServiceCollection : Collection<ServiceItem>, IServiceCollection
    {
        public void AddSingleton(Type serviceType) =>
            Add(new ServiceItem(serviceType, ServiceLifetime.Singleton));
        public void AddSingleton<TService>() where TService : class =>
            Add(new ServiceItem(typeof(TService), ServiceLifetime.Singleton));

        public void AddSingleton(Type serviceType, ServiceFactory factory) =>
            Add(new ServiceItem(serviceType, ServiceLifetime.Singleton, factory));
        public void AddSingleton<TService>(ServiceFactory<TService> factory) where TService : class =>
            Add(new ServiceItem(typeof(TService), ServiceLifetime.Singleton, ServiceUtils.ToBaseFactory(factory)));


        public void AddTransient(Type serviceType) =>
            Add(new ServiceItem(serviceType, ServiceLifetime.Transient));
        public void AddTransient<TService>() where TService : class =>
            Add(new ServiceItem(typeof(TService), ServiceLifetime.Transient));

        public void AddTransient(Type serviceType, ServiceFactory factory) =>
            Add(new ServiceItem(serviceType, ServiceLifetime.Transient, factory));
        public void AddTransient<TService>(ServiceFactory<TService> factory) where TService : class =>
            Add(new ServiceItem(typeof(TService), ServiceLifetime.Transient, ServiceUtils.ToBaseFactory(factory)));


        public void AddScoped(Type serviceType) =>
            Add(new ServiceItem(serviceType, ServiceLifetime.Scoped));
        public void AddScoped<TService>() where TService : class =>
            Add(new ServiceItem(typeof(TService), ServiceLifetime.Scoped));

        public void AddScoped(Type serviceType, ServiceFactory factory) =>
            Add(new ServiceItem(serviceType, ServiceLifetime.Scoped, factory));
        public void AddScoped<TService>(ServiceFactory<TService> factory) where TService : class =>
            Add(new ServiceItem(typeof(TService), ServiceLifetime.Scoped, ServiceUtils.ToBaseFactory(factory)));

        public bool IsRegistered(Type serviceType)
        {
            foreach (ServiceItem item in this)
                if (item.ServiceType == serviceType)
                    return true;

            return false;
        }

        public bool IsRegistered<TService>() where TService : class
        {
            return IsRegistered(typeof(TService));
        }

        private void EnsureNotAdded(Type serviceType)
        {
            if (IsRegistered(serviceType))
                throw new InvalidOperationException("Service is already added to this collection");
        }

        private void EnsureConstructorDependencies(Type serviceType)
        {
            void EnsureDependenciesCore(Type serviceType, Stack<Type> detectedServices)
            {
                if (detectedServices.Contains(serviceType))
                    throw new InvalidOperationException("Service circle dependency detected");

                if (ServiceUtils.GetServiceConstructor(serviceType) is not ConstructorInfo constructor)
                    throw new InvalidOperationException($"Could'n resolve dependency of Service, {serviceType.Name} has no service constructor");

                detectedServices.Push(serviceType);
                foreach (ParameterInfo parameter in constructor.GetParameters())
                    EnsureDependenciesCore(parameter.ParameterType, detectedServices);
                detectedServices.Pop();
            }

            Stack<Type> detectedServices = new Stack<Type>();
            EnsureDependenciesCore(serviceType, detectedServices);
        }

        protected override void InsertItem(int index, ServiceItem item)
        {
            EnsureNotAdded(item.ServiceType);

            if (item.Factory == null)
                EnsureConstructorDependencies(item.ServiceType);

            base.InsertItem(index, item);
        }

        public IServiceProvider BuildServiceProvider()
        {
            ServiceProvider provider =
                new ServiceProvider(this);

            if (!IsRegistered(typeof(IServiceScopeFactory)))
                AddSingleton<IServiceScopeFactory>(_ => new ServiceScopeFactory(provider));

            return provider;
        }
    }
}