using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Chat;
using RpWeave.Server.Orchestrations.BookBreakdown.Prompts;

namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.Classification;

[ScopedService]
public class ClassificationModule(OllamaChatClient chatClient)
{
    public async Task<string> ClassifyAsync(string input)
    {
        var chatRequest =
            new OllamaChatRequest(BookBreakdownPrompts.SystemPrompt, BookBreakdownPrompts.UserPrompt(input), []);
        
        return await chatClient.SendAsync(chatRequest);
    }
}