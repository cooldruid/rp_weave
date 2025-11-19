using Microsoft.AspNetCore.Mvc;
using RpWeave.Server.Api.Extensions;

namespace RpWeave.Server.Api.Features.Campaign.List;

[ApiController]
public class CampaignListController(CampaignListHandler handler) : ControllerBase
{
    [HttpGet("api/campaign")]
    public async Task<IActionResult> ListAsync()
    {
        var result = await handler.HandleAsync();

        return result.ToActionResult();
    }
}