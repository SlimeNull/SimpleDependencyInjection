// See https://aka.ms/new-console-template for more information
using SimpleDependencyInjection;

//using Microsoft.Extensions.DependencyInjection;

class ServiceA
{
    public ServiceA(
        ServiceB serviceB)
    {
        ServiceB = serviceB;
    }

    public ServiceB ServiceB { get; }
}

class ServiceB
{
    public ServiceB(
        ServiceC serviceC)
    {
        ServiceC = serviceC;
    }

    public ServiceC ServiceC { get; }
}

class ServiceC
{
    
}

class ScopedServiceA
{

}

class PropertyInjectServiceA
{
    [ServiceInject]
    public ServiceA? ServiceA { get; set; }

    [ServiceInject]
    public PropertyInjectServiceB? PropertyInjectServiceB { get; set; }
}

class PropertyInjectServiceB
{
    // 循环依赖注入
    [ServiceInject]
    public PropertyInjectServiceA? PropertyInjectServiceA { get; set; }
}

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");

        ServiceCollection serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<ServiceA>();
        serviceCollection.AddSingleton<ServiceB>();
        serviceCollection.AddSingleton<ServiceC>();
        serviceCollection.AddScoped<ScopedServiceA>();
        serviceCollection.AddSingleton<PropertyInjectServiceA>();
        serviceCollection.AddSingleton<PropertyInjectServiceB>();

        IServiceProvider services = serviceCollection.BuildServiceProvider();

        var serviceA = services.GetService<ServiceA>();
        var serviceA2 = services.GetService<ServiceA>();

        Console.WriteLine(object.ReferenceEquals(serviceA, serviceA2));

        Console.WriteLine("Scoped services test:");
        using (IServiceScope scope = services.CreateScope())
        {
            var scopedService = services.GetRequiredService<ScopedServiceA>();
            var scopedService2 = services.GetRequiredService<ScopedServiceA>();
            Console.WriteLine(object.ReferenceEquals(scopedService, scopedService2));
            Console.WriteLine(scopedService.GetHashCode());
        }

        using (IServiceScope scope = services.CreateScope())
        {
            var scopedService = services.GetRequiredService<ScopedServiceA>();
            var scopedService2 = services.GetRequiredService<ScopedServiceA>();
            Console.WriteLine(object.ReferenceEquals(scopedService, scopedService2));
            Console.WriteLine(scopedService.GetHashCode());
        }

        Console.WriteLine("Property inject test");
        var propInjectService = services.GetRequiredService<PropertyInjectServiceA>();

        if (propInjectService.PropertyInjectServiceB != null)
            Console.WriteLine("属性成功注入");
    }
}