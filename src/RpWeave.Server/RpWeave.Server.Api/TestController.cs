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

namespace RpWeave.Server.Api;

[ApiController]
public class TestController(
    BookBreakdownOrchestrator bookBreakdownOrchestrator,
    OllamaEmbedClient embedClient,
    VectorDbClient vectorDbClient) : ControllerBase
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
}