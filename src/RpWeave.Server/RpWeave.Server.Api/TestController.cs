using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Mcp;
using RpWeave.Server.Mcp.Orchestrators;
using RpWeave.Server.Mcp.Tools;

namespace RpWeave.Server.Api;

[ApiController]
public class TestController(TtrpgBookBreakdownOrchestrator orchestrator) : ControllerBase
{
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
        return Ok(await orchestrator.OrchestrateAsync(fileLocation));
    }
}