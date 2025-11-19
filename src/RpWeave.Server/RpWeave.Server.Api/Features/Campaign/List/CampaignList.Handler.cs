using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Repositories;

namespace RpWeave.Server.Api.Features.Campaign.List;

[ScopedService]
public class CampaignListHandler(ICampaignEntityRepository repository)
{
    public async Task<ValueResult<CampaignListResponse>> HandleAsync()
    {
        var entities = await repository.ListAsync();

        var response = new CampaignListResponse(
            entities.Select(x => 
                new CampaignListItem(x.Id.ToString(), x.Name, x.Description, x.PdfPath, x.VectorCollectionName))
                .ToList());
        
        return ValueResult<CampaignListResponse>.Success(response);
    }
}