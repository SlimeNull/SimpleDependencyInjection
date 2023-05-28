# SimpleDependencyInjection

һ���򵥵�����ע����. ���� Microsoft.Extensions.DependencyInjection ������.

## ʹ�÷�ʽ

����һ����������.

```csharp
ServiceCollection serviceCollection = new ServiceCollection();
```

ע��һЩ����.

```csharp
serviceCollection.AddSingleton<ServiceA>();
serviceCollection.AddSingleton<ServiceB>();
serviceCollection.AddSingleton<ServiceC>();
serviceCollection.AddScoped<ScopedServiceA>();
serviceCollection.AddSingleton<PropertyInjectServiceA>();
serviceCollection.AddSingleton<PropertyInjectServiceB>();
```

���������ṩ��.

```csharp
IServiceProvider services = serviceCollection.BuildServiceProvider();
```

��ȡ����.

```csharp
ServiceA serviceA = services.GetService<ServiceA>();
```

��ȡ Scoped ����

```csharp
using (IServiceScope scope = services.CreateScope())
{
	ScopedServiceA scopedServiceA = services.GetService<ScopedServiceA>();
}
```

��ʽָ����������Ӧ��ʹ�õķ����캯��.

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

ָ�������е�ĳ���ֶλ�����Ӧ�ñ�ע��.

```csharp
class PropertyInjectServiceA
{
    [ServiceInject]
    public ServiceA? ServiceA { get; set; }

    [ServiceInject]
    public PropertyInjectServiceB? PropertyInjectServiceB { get; set; }
}
```

## ע��

**IServiceProvider �ڱ����е�ʵ�ֲ����̰߳�ȫ��.**

����ζ��, �����ֶκ�����ע���ѭ���������Ա���������, ���ǵ���ĳ����漰������, ���統 A �̻߳�ȡһ������ѭ�������ķ��� A, �����ڶ�Ӧ����δ���ط���ʵ����ʱ�� B �߳�Ҳ���Ի�ȡ�÷���, ��ô B �߳̿��ܻ��õ�δ��ȫ��ʼ���ķ���.
����Ҳ�п��������̻߳��ȡ����ͬ�ķ���ʵ��(�������ǵ�������)