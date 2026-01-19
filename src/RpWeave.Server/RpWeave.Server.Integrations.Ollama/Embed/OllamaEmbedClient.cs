using OllamaSharp;
using OllamaSharp.Models;
using RpWeave.Server.Core.Startup;
using Serilog;

namespace RpWeave.Server.Integrations.Ollama.Embed;

[ScopedService]
public class OllamaEmbedClient(OllamaSettings ollamaSettings)
{
    public async Task<float[]> GenerateEmbeddingsAsync(string input)
    {
        try
        {
            // Create an HTTP client and the OllamaApiClient
            var httpClient = new HttpClient { BaseAddress = new Uri(ollamaSettings.Url) };
            var ollamaClient = new OllamaApiClient(httpClient);

            var request = new EmbedRequest()
            {
                Model = ollamaSettings.EmbeddingsModel,
                Input = [input]
            };

            var embeddings = await ollamaClient.EmbedAsync(request);

            return embeddings.Embeddings.First();
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message);
            throw;
        }
    }
}