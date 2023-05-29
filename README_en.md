# SimpleDependencyInjection

A simple dependency injection framework modeled after Microsoft.Extensions.DependencyInjection.

## Usage

Create a service container.

```csharp
ServiceCollection serviceCollection = new ServiceCollection();
```

Register some services.

```csharp
serviceCollection.AddSingleton<ServiceA>();
serviceCollection.AddSingleton<ServiceB>();
serviceCollection.AddSingleton<ServiceC>();
serviceCollection.AddScoped<ScopedServiceA>();
serviceCollection.AddSingleton<PropertyInjectServiceA>();
serviceCollection.AddSingleton<PropertyInjectServiceB>();
```

Build the service provider.

```csharp
IServiceProvider services = serviceCollection.BuildServiceProvider();
```

Get services.

```csharp
ServiceA serviceA = services.GetService<ServiceA>();
```

Get scoped services.

```csharp
using (IServiceScope scope = services.CreateScope())
{
    ScopedServiceA scopedServiceA = services.GetService<ScopedServiceA>();
}
```

Explicitly specify the service constructor that the service container should use.

```csharp
class ServiceA
{
    [ServiceConstructor]
    public ServiceA(
        ServiceB serviceB)
    {
        ServiceB = serviceB;
    }

    public ServiceB ServiceB { get; }
}
```

Specify that a field or property in the service should be injected.

```csharp
class PropertyInjectServiceA
{
    [ServiceInject]
    public ServiceA? ServiceA { get; set; }

    [ServiceInject]
    public PropertyInjectServiceB? PropertyInjectServiceB { get; set; }
}
```

## Note

**The implementation of IServiceProvider in this library is not thread-safe.**

This means that while circular dependencies in field and property injection can be resolved by the framework, if your program involves concurrency, such as when thread A gets a service A with a circular dependency and thread B also tries to get that service before the corresponding operation returns the service instance, then thread B may get an incompletely initialized service. It is also possible that two threads will get different service instances even though it is a singleton service.