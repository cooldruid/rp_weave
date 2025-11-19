using OllamaSharp;
using OllamaSharp.Models;
using RpWeave.Server.Core.Startup;

namespace RpWeave.Server.Integrations.Ollama.Embed;

[ScopedService]
public class OllamaEmbedClient
{
    public async Task<float[]> GenerateEmbeddingsAsync(string input)
    {
        // Create an HTTP client and the OllamaApiClient
        var httpClient = new HttpClient { BaseAddress = new Uri("http://ollama:11434") };
        var ollamaClient = new OllamaApiClient(httpClient);

        var request = new EmbedRequest()
        {
            Model = "mxbai-embed-large",
            Input = [input]
        };

        var embeddings = await ollamaClient.EmbedAsync(request);

        return embeddings.Embeddings.First();
    }
}