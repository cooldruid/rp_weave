using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RpWeave.Server.Core.Startup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAttributedServices(
        this IServiceCollection services,
        Assembly[] assemblies)
    {
        var types = assemblies
            .SelectMany(x => x.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface);

        foreach (var type in types)
        {
            // Scoped
            var scopedAttr = type.GetCustomAttribute<ScopedServiceAttribute>();
            if (scopedAttr != null)
            {
                var serviceType = scopedAttr.ServiceType ?? type.GetInterfaces().FirstOrDefault() ?? type;
                services.AddScoped(serviceType, type);
                continue;
            }

            // Singleton
            var singletonAttr = type.GetCustomAttribute<SingletonServiceAttribute>();
            if (singletonAttr != null)
            {
                var serviceType = singletonAttr.ServiceType ?? type.GetInterfaces().FirstOrDefault() ?? type;
                services.AddSingleton(serviceType, type);
                continue;
            }

            // Transient
            var transientAttr = type.GetCustomAttribute<TransientServiceAttribute>();
            if (transientAttr != null)
            {
                var serviceType = transientAttr.ServiceType ?? type.GetInterfaces().FirstOrDefault() ?? type;
                services.AddTransient(serviceType, type);
                continue;
            }
        }

        return services;
    }
}
