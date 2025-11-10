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
    private static string CollectionName = "";
    
    [HttpGet("api/test/extractpdf")]
    [AllowAnonymous]
    public IActionResult ExtractPdf()
    {
        return Ok();
    }
    
    [HttpGet("api/test/prompt/broken")]
    [AllowAnonymous]
    public async Task<IActionResult> ExtractPdf(string prompt)
    {
        var thing = new OllamaClientWithFunctions();
        return Ok();
    }
    
    [HttpGet("api/test/orchestrate")]
    [AllowAnonymous]
    public async Task<IActionResult> Orchestrate(string fileLocation)
    {
        CollectionName = await bookBreakdownOrchestrator.ProcessBookBreakdown(fileLocation);
        
        return Ok();
    }

    [HttpGet("api/test/queryvector")]
    [AllowAnonymous]
    public async Task<IActionResult> QueryVector(string query)
    {
        var searchVector = await embedClient.GenerateEmbeddingsAsync(query);

        var response = await vectorDbClient.SearchAsync(new VectorDbSearchRequest(
            string.IsNullOrEmpty(CollectionName) ? "test1" : CollectionName,
            searchVector));
        
        return Ok(response);
    }
}