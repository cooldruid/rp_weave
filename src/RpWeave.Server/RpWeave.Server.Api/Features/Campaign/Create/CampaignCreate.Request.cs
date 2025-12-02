namespace RpWeave.Server.Api.Features.Campaign.Create;

public record CampaignCreateRequest(string Data, IFormFile? File);

public record CampaignCreateData(
    string Name, 
    string Description, 
    bool CreateEmbeddings,
    int? ChapterFontSize,
    int? SubChapterFontSize,
    int? HeaderFontSize,
    bool? IgnoreFooter);