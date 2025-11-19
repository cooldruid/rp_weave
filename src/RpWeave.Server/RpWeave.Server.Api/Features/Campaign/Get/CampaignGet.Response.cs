namespace RpWeave.Server.Api.Features.Campaign.Get;

public record CampaignGetResponse(string Id, string Name, string Description, string? PdfPath, string? VectorCollectionName);