namespace RpWeave.Server.Orchestrations.Chat;

public record ChatRequest(string Query, string CampaignCollectionName, List<ChatHistoryLine> ChatHistory);

public record ChatHistoryLine(string Type, string Message, int Order);