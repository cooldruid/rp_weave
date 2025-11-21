namespace RpWeave.Server.Api.Features.Campaign.Create;

public record CampaignCreateRequest(CampaignCreateData Data, IFormFile? Pdf);

public record CampaignCreateData(
    string Name, 
    string Description, 
    bool CreateEmbeddings,
    int? ChapterFontSize,
    int? SubChapterFontSize,
    int? HeaderFontSize,
    bool? IgnoreFooter);