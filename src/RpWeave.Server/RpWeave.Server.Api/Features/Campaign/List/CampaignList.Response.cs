namespace RpWeave.Server.Api.Features.Campaign.List;

public record CampaignListResponse(List<CampaignListItem> Campaigns);

public record CampaignListItem(string Id, string Name, string Description, string? PdfPath, string? VectorCollectionName);