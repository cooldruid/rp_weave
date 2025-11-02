using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Mcp;
using RpWeave.Server.Mcp.Tools;

namespace RpWeave.Server.Api;

[ApiController]
public class TestController : ControllerBase
{
    [HttpGet("api/test/extractpdf")]
    [AllowAnonymous]
    public IActionResult ExtractPdf()
    {
        return Ok(PdfTools.ExtractTextFromPdf("/src/RpWeave.Server.Mcp/TestFile/CryptsKelemvor.pdf"));
    }
    
    [HttpGet("api/test/prompt")]
    [AllowAnonymous]
    public async Task<IActionResult> ExtractPdf(string prompt)
    {
        var thing = new McpThing();
        return Ok(await thing.SendAsync(prompt));
    }
    
    [HttpGet("api/test/createjson")]
    [AllowAnonymous]
    public async Task<IActionResult> WriteJson(string prompt)
    {
        await JsonTools.WriteJsonToFileAsync("what.json", "{one: 2}");
        return Ok();
    }
}