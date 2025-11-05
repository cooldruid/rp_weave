namespace RpWeave.Server.Integrations.Ollama.Chat;

public record OllamaChatRequest(
    string SystemPrompt,
    string UserPrompt,
    Delegate[] Tools);