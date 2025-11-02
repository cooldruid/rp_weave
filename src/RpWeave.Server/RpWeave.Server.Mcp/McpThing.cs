using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using RpWeave.Server.Mcp.Tools;

namespace RpWeave.Server.Mcp;

public class McpThing
{
    private readonly IChatClient _chatClient;
    private readonly ChatOptions _chatOptions;

    public McpThing(string ollamaUrl = "http://ollama:11434", string model = "gpt-oss:20b")
    {
        // 1️⃣ Setup Ollama
        var ollama = new OllamaApiClient(new Uri(ollamaUrl))
        {
            SelectedModel = model
        };

        // 2️⃣ Configure DI and Extensions.AI client
        var services = new ServiceCollection();
        services.AddChatClient((IChatClient)ollama).UseFunctionInvocation();
        var serviceProvider = services.BuildServiceProvider();

        _chatClient = serviceProvider.GetRequiredService<IChatClient>();

        // 3️⃣ Register tools (methods exposed to AI)
        _chatOptions = new ChatOptions
        {
            Tools =
            [
                // Register methods from both tool classes
                AIFunctionFactory.Create(JsonTools.WriteJsonToFileAsync),
                AIFunctionFactory.Create(PdfTools.ExtractTextFromPdf)
            ]
        };
    }

    // 4️⃣ Send message to the model
    public async Task<string> SendAsync(string prompt)
    {
        Console.WriteLine($"\n[User]: {prompt}");
        string response = "";
        await foreach (var token in _chatClient.GetStreamingResponseAsync(prompt, _chatOptions))
        {
            response += token;
        }

        Console.WriteLine();
        
        return response;
    }
}