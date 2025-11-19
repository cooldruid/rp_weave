using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using OllamaSharp;
using OllamaSharp.Models;
using RpWeave.Server.Mcp.Tools;
using Serilog;

namespace RpWeave.Server.Mcp;

public class OllamaClientWithFunctions
{
    private readonly IChatClient _chatClient;
    private readonly ChatOptions _chatOptions;

    public OllamaClientWithFunctions(string ollamaUrl = "http://ollama:11434", string model = "gpt-oss:20b")
    {
        var ollama = new OllamaApiClient(new Uri(ollamaUrl))
        {
            SelectedModel = model
        };

        var services = new ServiceCollection();
        services.AddChatClient(ollama).UseFunctionInvocation();
        var serviceProvider = services.BuildServiceProvider();
        
        _chatClient = serviceProvider.GetRequiredService<IChatClient>();
        
        _chatOptions = new ChatOptions
        {
            Tools =
            [
                // Register methods from both tool classes
                AIFunctionFactory.Create(JsonTools.WriteJsonToFileAsync),
                AIFunctionFactory.Create(JsonTools.ReadJsonFromFileAsync),
                AIFunctionFactory.Create(JsonTools.ListAllJsonFilesAsync)
            ],
            
        };
        _chatOptions.AddOllamaOption(OllamaOption.Think, true);
    }

    public async Task<string> SendAsync(string systemPrompt, string userPrompt)
    {
        try
        {
            Console.WriteLine($"\n[System]: {systemPrompt}\n");
            Console.WriteLine($"\n[User]: {userPrompt}\n");
            var response = "";
            Console.Write("Thinking: ");
            await foreach (var token in _chatClient.GetStreamingResponseAsync([
                               new ChatMessage(ChatRole.System, systemPrompt),
                               new ChatMessage(ChatRole.User, userPrompt)
                           ], _chatOptions))
            {
                foreach (var content in token.Contents)
                {
                    if (content is TextReasoningContent textReasoningContent)
                    {
                        Console.Write(textReasoningContent.Text);
                    }
                    else
                    {
                        response += token;
                    }
                }
            }

            Console.WriteLine($"Response: {response}");

            return response;
        }
        catch (HttpRequestException ex)
        {
            Log.Error("Ollama returned an HTTP error: {Message}", ex.Message);

            if (ex.Data["ResponseBody"] is string body)
                Log.Error("Ollama error response body: {Body}", body);

            return "";
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unhandled exception while querying Ollama");

            return "";
        }
    }
}