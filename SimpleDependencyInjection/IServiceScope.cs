namespace SimpleDependencyInjection
{
    public interface IServiceScope : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }
    }
}