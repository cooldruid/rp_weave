using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Chat;

namespace RpWeave.Server.Orchestrations.Chat.Modules.Editing;

[ScopedService]
public class EditingModule(OllamaChatClient chatClient)
{
    public async Task<EditingResponse> ProcessAsync(EditingRequest request)
    {
        var systemPrompt = EditingPrompts.SystemPrompt(request.Advice);

        var chatRequest = new OllamaChatRequest(
            SystemPrompt: systemPrompt,
            UserPrompt: request.Message,
            Tools: []);

        var response = await chatClient.SendAsync(chatRequest);

        return new EditingResponse(response);
    }
}