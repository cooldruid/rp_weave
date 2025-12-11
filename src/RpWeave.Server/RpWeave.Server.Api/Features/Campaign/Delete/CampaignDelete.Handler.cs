using RpWeave.Server.Core.Results;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Repositories;
using RpWeave.Server.Integrations.Qdrant;

namespace RpWeave.Server.Api.Features.Campaign.Delete;

[ScopedService]
public class CampaignDeleteHandler(
    ICampaignEntityRepository campaignEntityRepository,
    IChapterEntityRepository chapterEntityRepository,
    VectorDbClient vectorDbClient)
{
    public async Task<Result> HandleAsync(string campaignId)
    {
        var campaignEntity = await campaignEntityRepository.GetAsync(campaignId);

        if (campaignEntity == null)
        {
            return Result.Failure(ErrorCodes.NotFound, "Campaign not found");
        }

        if (!string.IsNullOrWhiteSpace(campaignEntity.PdfPath))
        {
            File.Delete(campaignEntity.PdfPath);
        }

        if (!string.IsNullOrWhiteSpace(campaignEntity.VectorCollectionName))
        {
            await vectorDbClient.DeleteCollectionAsync(campaignEntity.VectorCollectionName);
        }
        
        await chapterEntityRepository.DeleteForCampaignAsync(campaignId);
        await campaignEntityRepository.DeleteAsync(campaignId);

        return Result.Success();
    }
}