namespace RpWeave.Server.Integrations.Qdrant.Requests;

public record VectorDbSearchRequest(
    string CollectionName,
    float[] Vector);