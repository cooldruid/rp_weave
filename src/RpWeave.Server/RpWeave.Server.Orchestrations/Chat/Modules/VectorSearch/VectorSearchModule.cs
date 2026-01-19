using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Embed;
using RpWeave.Server.Integrations.Qdrant;
using RpWeave.Server.Integrations.Qdrant.Requests;

namespace RpWeave.Server.Orchestrations.Chat.Modules.VectorSearch;

[ScopedService]
public class VectorSearchModule(
    OllamaEmbedClient embedClient,
    VectorDbClient vectorDbClient)
{
    public async Task<VectorSearchResponse> ProcessAsync(VectorSearchRequest request)
    {
        var vector = await embedClient.GenerateEmbeddingsAsync(request.Query);

        var vectorDbSearchRequest = new VectorDbSearchRequest(request.CollectionName, vector);
        
        var vectorDbSearchResponse = await vectorDbClient.SearchAsync(vectorDbSearchRequest);

        var response = new VectorSearchResponse(
            vectorDbSearchResponse.Elements.Select(x =>
                new VectorSearchResponseElement(x.Id, x.Text, x.Score)).ToList());

        return response;
    }
}