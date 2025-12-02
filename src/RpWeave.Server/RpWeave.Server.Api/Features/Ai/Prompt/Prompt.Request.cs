namespace RpWeave.Server.Api.Features.Ai.Prompt;

public record PromptRequest(
    string CampaignId,
    string CollectionName, 
    string Prompt);