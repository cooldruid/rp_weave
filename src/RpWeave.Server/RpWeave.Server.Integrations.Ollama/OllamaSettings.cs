namespace RpWeave.Server.Integrations.Ollama;

public class OllamaSettings
{
    public required string Url { get; init; }
    public required string ReasoningModel { get; init; }
    public required string EmbeddingsModel { get; init; }
}