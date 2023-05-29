namespace SimpleDependencyInjection
{
    public interface IRequiredServiceProvider : IServiceProvider
    {
        object GetRequiredService(Type serviceType);
    }
}