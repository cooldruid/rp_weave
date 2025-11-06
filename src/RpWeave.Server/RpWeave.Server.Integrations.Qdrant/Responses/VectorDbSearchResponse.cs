namespace RpWeave.Server.Integrations.Qdrant.Responses;

public record VectorDbSearchResponse(List<VectorDbSearchResponseElement> Elements);
    
public record VectorDbSearchResponseElement(
    string TitlePath,
    string Text,
    float Score);