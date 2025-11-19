namespace RpWeave.Server.Integrations.Qdrant.Requests;

public record VectorDbUpsertRequest(
    string CollectionName,
    float[] Vector,
    string TitlePath,
    string Text);