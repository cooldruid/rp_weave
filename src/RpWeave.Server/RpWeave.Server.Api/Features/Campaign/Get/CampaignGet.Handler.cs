using RpWeave.Server.Api.Features.Campaign.List;
using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Repositories;

namespace RpWeave.Server.Api.Features.Campaign.Get;

[ScopedService]
public class CampaignGetHandler(ICampaignEntityRepository repository)
{
    public async Task<ValueResult<CampaignGetResponse>> HandleAsync(string id)
    {
        var entity = await repository.GetAsync(id);

        var response = new CampaignGetResponse(
                entity.Id.ToString(), entity.Name, entity.Description, entity.PdfPath, entity.VectorCollectionName);
        
        return ValueResult<CampaignGetResponse>.Success(response);
    }
}