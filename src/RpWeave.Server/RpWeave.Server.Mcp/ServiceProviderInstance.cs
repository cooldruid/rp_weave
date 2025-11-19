using System.ComponentModel.Design;

namespace RpWeave.Server.Mcp;

public static class ServiceProviderInstance
{
    public static IServiceProvider ServiceProvider { get; private set; } = new ServiceContainer();

    public static void Initialize(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }
}