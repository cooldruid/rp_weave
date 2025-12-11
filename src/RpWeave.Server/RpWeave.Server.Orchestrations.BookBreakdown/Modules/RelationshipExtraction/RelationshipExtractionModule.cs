using System.Text.Json;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Chat;
using RpWeave.Server.Integrations.Ollama.Embed;
using RpWeave.Server.Integrations.Qdrant;
using RpWeave.Server.Integrations.Qdrant.Requests;
using Serilog;
using Serilog.Core;

namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.RelationshipExtraction;

[ScopedService]
public class RelationshipExtractionModule(
    OllamaEmbedClient embedClient,
    VectorDbClient vectorDbClient,
    OllamaChatClient chatClient)
{
    public async Task<RelationshipExtractionResponse> ProcessAsync(RelationshipExtractionRequest request)
    {
        var relationships = new List<RelationshipExtractionElement>();
        var allEntityNames = request.Entities.Select(x => x.Name).ToList();
        
        foreach (var entity in request.Entities)
        {
            var embedding = await embedClient.GenerateEmbeddingsAsync(entity.Name);
            var vectorSearchRequest = new VectorDbSearchRequest(request.CollectionName, embedding);
            var vectorSearchResponse = await vectorDbClient.SearchAsync(vectorSearchRequest);

            var sourceText = string.Join("\n\n", vectorSearchResponse.Elements.Select(x => x.Text));
            var filteredEntityNames = allEntityNames.Where(x => sourceText.Contains(x, StringComparison.CurrentCultureIgnoreCase)).ToList();

            var chatResponse = "";
            try
            {
                var chatRequest = new OllamaChatRequest(
                    RelationshipExtractionPrompts.SystemPrompt,
                    RelationshipExtractionPrompts.UserPrompt(sourceText, entity.Name, filteredEntityNames),
                    []);
                chatResponse = await chatClient.SendAsync(chatRequest);
                
                if(string.IsNullOrWhiteSpace(chatResponse))
                    continue;

                foreach (var responseLine in chatResponse.Split("\n"))
                {
                    var relationship = JsonSerializer.Deserialize<ModelRelationshipResponse>(responseLine, new JsonSerializerOptions() {PropertyNameCaseInsensitive = true});
                    relationships.Add(new RelationshipExtractionElement(entity.Name, relationship.Entity, relationship.Relationship));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message);
                var chatRequest = new OllamaChatRequest(
                    RelationshipExtractionPrompts.SystemPrompt,
                    RelationshipExtractionPrompts.RepairPrompt(chatResponse),
                    []);
                chatResponse = await chatClient.SendAsync(chatRequest);
                
                if(string.IsNullOrWhiteSpace(chatResponse))
                    continue;

                foreach (var responseLine in chatResponse.Split("\n"))
                {
                    var relationship = JsonSerializer.Deserialize<ModelRelationshipResponse>(responseLine, new JsonSerializerOptions() {PropertyNameCaseInsensitive = true});
                    relationships.Add(new RelationshipExtractionElement(entity.Name, relationship.Entity, relationship.Relationship));
                }
            }
            
            Thread.Sleep(5000);
        }
        
        return new RelationshipExtractionResponse(relationships);
    }
}

public class ModelRelationshipResponse
{
    public string Entity { get; set; }
    public string Relationship { get; set; }
}