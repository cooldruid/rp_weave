namespace RpWeave.Server.Orchestrations.Chat.Modules.VectorSearch;

public record VectorSearchResponse(List<VectorSearchResponseElement> Elements);
    
public record VectorSearchResponseElement(
    string Id,
    string Text,
    float Score);