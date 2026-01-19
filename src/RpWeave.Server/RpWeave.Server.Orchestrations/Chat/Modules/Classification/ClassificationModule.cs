using System.Text.Json;
using RpWeave.Server.Core.Extensions;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Chat;
using Serilog;

namespace RpWeave.Server.Orchestrations.Chat.Modules.Classification;

[ScopedService]
public class ClassificationModule(OllamaChatClient chatClient)
{
    public async Task<ClassificationResponse> ProcessAsync(ClassificationRequest request)
    {
        var chatRequest = new OllamaChatRequest(
            SystemPrompt: ClassificationPrompts.SystemPrompt(request.ChatHistory),
            UserPrompt: request.Query,
            Tools: []);
        
        var chatResponse = await chatClient.SendAsync(chatRequest);

        var classificationResponse = chatResponse.JsonDeserializeSafe<ClassificationResponse>();

        if (classificationResponse == null)
        {
            Log.Warning("Failed to get classification response. Falling back to raw user input.");
            classificationResponse = new ClassificationResponse(true, request.Query, string.Empty, string.Empty);
        }
        
        return classificationResponse;
    }
}