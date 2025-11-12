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
using RpWeave.Server.Orchestrations.BookBreakdown.Pdf;

namespace RpWeave.Server.Api;

[ApiController]
public class TestController(
    BookBreakdownOrchestrator bookBreakdownOrchestrator,
    OllamaEmbedClient embedClient,
    VectorDbClient vectorDbClient) : ControllerBase
{
    [HttpGet("api/test/extractpdf")]
    [AllowAnonymous]
    public IActionResult ExtractPdf(string filePath)
    {
        //new PdfProcessor().Process(filePath);
        return Ok();
    }
    
    [HttpGet("api/test/orchestrate")]
    [AllowAnonymous]
    public async Task<IActionResult> Orchestrate(string fileLocation)
    {
        var collectionName = await bookBreakdownOrchestrator.ProcessBookBreakdown(fileLocation);
        
        return Ok(collectionName);
    }

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