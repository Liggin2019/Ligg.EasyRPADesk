namespace Ligg.Infrastructure.Utility.Ioc
{
    public interface IDependencyResolverFactory
    {
        IDependencyResolver CreateInstance();
    }
}