using System.Reflection;
using Microsoft.AspNetCore.Routing;

namespace RpWeave.Server.Core.Startup;

public static class EndpointRouteBuilderExtensions
{
    public static void MapAttributedEndpoints(
        this IEndpointRouteBuilder app)
    {
        var assembly = Assembly.GetCallingAssembly();

        var endpointTypes = assembly.GetTypes()
            .Where(t => typeof(IEndpoint).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface)
            .ToList();

        foreach (var type in endpointTypes)
        {
            var instance = (IEndpoint)Activator.CreateInstance(type)!;
            instance.MapEndpoint(app);
        }
    }
}