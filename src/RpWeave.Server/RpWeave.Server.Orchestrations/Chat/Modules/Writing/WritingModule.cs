using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Chat;

namespace RpWeave.Server.Orchestrations.Chat.Modules.Writing;

[ScopedService]
public class WritingModule(OllamaChatClient chatClient)
{
    public async Task<WritingResponse> ProcessAsync(WritingRequest request)
    {
        var systemPrompt = WritingPrompts.SystemPrompt(request.Context);

        var chatRequest = new OllamaChatRequest(
            SystemPrompt: systemPrompt,
            UserPrompt: request.Query,
            Tools: []);

        var response = await chatClient.SendAsync(chatRequest);

        return new WritingResponse(response);
    }
}