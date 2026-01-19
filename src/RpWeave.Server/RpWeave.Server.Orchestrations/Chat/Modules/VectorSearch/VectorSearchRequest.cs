namespace RpWeave.Server.Orchestrations.Chat.Modules.VectorSearch;

public record VectorSearchRequest(string CollectionName, string Query);