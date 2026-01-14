using RpWeave.Server.Core.Startup;
using RpWeave.Server.Orchestrations.Chat;

namespace RpWeave.Server.Api.Features.Ai.Prompt;

[ScopedService]
public class PromptHandler(ChatOrchestrator chatOrchestrator)
{
    public async Task<PromptResponse> HandleAsync(PromptRequest request)
    {
        var response = await chatOrchestrator.ChatAsync(new (request.Prompt, request.CollectionName));

        return new PromptResponse(response.Message);
    }
}