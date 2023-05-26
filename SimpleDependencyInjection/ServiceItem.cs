using System.Reflection;

namespace SimpleDependencyInjection
{
    public class ServiceItem
    {
        public ServiceItem(Type serviceType, ServiceLifetime lifetime)
        {
            if (!serviceType.IsClass && !serviceType.IsInterface)
                throw new ArgumentException("Service type must be class or interface");
            if (ServiceUtils.GetServiceConstructor(serviceType) is not ConstructorInfo constructor)
                throw new ArgumentException("Service must has one constructor");

            this.constructor = constructor; 
            ServiceType = serviceType;
            Lifetime = lifetime;
        }

        public ServiceItem(Type serviceType, ServiceLifetime lifetime, ServiceFactory factory)
        {
            if (!serviceType.IsClass && !serviceType.IsInterface)
                throw new ArgumentException("Service type must be class or interface");
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            ServiceType = serviceType;
            Lifetime = lifetime;
            Factory = factory;

        }


        private ConstructorInfo? constructor;

        public Type ServiceType { get; }
        public ServiceLifetime Lifetime { get; }
        public ServiceFactory? Factory { get; }
    }
}