using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using RpWeave.Server.Integrations.Ollama.Options;
using Serilog;

namespace RpWeave.Server.Integrations.Ollama.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOllamaIntegration(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var ollamaOptions = configuration.GetSection("Ollama").Get<OllamaOptions>();

        if (ollamaOptions == null)
        {
            Log.Information("Ollama options are not configured. Skipping Ollama integration.");
            return serviceCollection;
        }
        
        var ollama = new OllamaApiClient(new Uri(ollamaOptions.Url))
        {
            SelectedModel = ollamaOptions.Model
        };

        serviceCollection.AddChatClient(ollama).UseFunctionInvocation();

        return serviceCollection;
    }
}