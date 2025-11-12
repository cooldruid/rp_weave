using Microsoft.AspNetCore.Mvc;

namespace RpWeave.Server.Api.Features.Ai.Prompt;

[ApiController]
public class PromptController(PromptHandler handler) : ControllerBase
{
    [HttpPost("api/ai/prompt")]
    public async Task<IActionResult> Prompt(string collectionName, string query)
    {
        var response = await handler.HandleAsync(collectionName, query);

        return Ok(response);
    }
}