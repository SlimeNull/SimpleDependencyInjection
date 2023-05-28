# SimpleDependencyInjection

一个简单的依赖注入框架. 仿照 Microsoft.Extensions.DependencyInjection 所制作.

## 使用方式

创建一个服务容器.

```csharp
ServiceCollection serviceCollection = new ServiceCollection();
```

注册一些服务.

```csharp
serviceCollection.AddSingleton<ServiceA>();
serviceCollection.AddSingleton<ServiceB>();
serviceCollection.AddSingleton<ServiceC>();
serviceCollection.AddScoped<ScopedServiceA>();
serviceCollection.AddSingleton<PropertyInjectServiceA>();
serviceCollection.AddSingleton<PropertyInjectServiceB>();
```

构建服务提供者.

```csharp
IServiceProvider services = serviceCollection.BuildServiceProvider();
```

获取服务.

```csharp
ServiceA serviceA = services.GetService<ServiceA>();
```

获取 Scoped 服务

```csharp
using (IServiceScope scope = services.CreateScope())
{
	ScopedServiceA scopedServiceA = services.GetService<ScopedServiceA>();
}
```

显式指定服务容器应该使用的服务构造函数.

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

指定服务中的某个字段或属性应该被注入.

```csharp
class PropertyInjectServiceA
{
    [ServiceInject]
    public ServiceA? ServiceA { get; set; }

    [ServiceInject]
    public PropertyInjectServiceB? PropertyInjectServiceB { get; set; }
}
```

## 注意

**IServiceProvider 在本库中的实现不是线程安全的.**

这意味着, 尽管字段和属性注入的循环依赖可以被框架所解决, 但是当你的程序涉及到并发, 例如当 A 线程获取一个存在循环依赖的服务 A, 并且在对应操作未返回服务实例的时候 B 线程也尝试获取该服务, 那么 B 线程可能会拿到未完全初始化的服务.
或者也有可能两个线程会获取到不同的服务实例(尽管它是单例服务)