using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Integrations.Ollama.Embed;
using RpWeave.Server.Integrations.Qdrant;
using RpWeave.Server.Integrations.Qdrant.Requests;
using RpWeave.Server.Mcp;
using RpWeave.Server.Mcp.Orchestrators;
using RpWeave.Server.Mcp.Tools;
using RpWeave.Server.Orchestrations.BookBreakdown;
using RpWeave.Server.Orchestrations.BookBreakdown.Modules.RelationshipExtraction;

namespace RpWeave.Server.Api;

[ApiController]
public class TestController(
    BookBreakdownOrchestrator bookBreakdownOrchestrator,
    OllamaEmbedClient embedClient,
    VectorDbClient vectorDbClient,
    RelationshipExtractionModule relationshipExtractionModule) : ControllerBase
{
    [HttpGet("api/test/queryvector")]
    [AllowAnonymous]
    public async Task<IActionResult> QueryVector(string collectionName, string query)
    {
        var searchVector = await embedClient.GenerateEmbeddingsAsync(query);

        var response = await vectorDbClient.SearchAsync(new VectorDbSearchRequest(
            collectionName,
            searchVector));
        
        return Ok(response);
    }

    [HttpGet("api/test/extractrelationships")]
    [AllowAnonymous]
    public async Task<IActionResult> ExtractRelationships(string collectionName, string entitiesString)
    {
        var entities = entitiesString.Split(",");
        var entityObjects = new List<RelationshipExtractionEntity>();
        foreach (var entity in entities)
        {
            var split = entity.Split("-");
            entityObjects.Add(new RelationshipExtractionEntity(split[0].Trim(), split[1].Trim()));
        }

        var result =
            await relationshipExtractionModule.ProcessAsync(
                new RelationshipExtractionRequest(collectionName, entityObjects));
        
        return Ok(result);
    }
}