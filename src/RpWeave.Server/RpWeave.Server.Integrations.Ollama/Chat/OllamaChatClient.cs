using Microsoft.Extensions.AI;
using OllamaSharp;
using OllamaSharp.Models;
using RpWeave.Server.Core.Startup;
using Serilog;

namespace RpWeave.Server.Integrations.Ollama.Chat;

[ScopedService]
public class OllamaChatClient(IChatClient chatClient)
{
    public async Task<string> SendAsync(OllamaChatRequest request)
    {
        try
        {
            Log.Information("Sending request to Ollama Chat.\n\nSystem Prompt: {SystemPrompt}\n\nUser Prompt: {UserPrompt}",
                request.SystemPrompt, request.UserPrompt);

            var chatOptions = new ChatOptions
            {
                Tools =
                [
                    ..request.Tools.Select(tool =>
                        AIFunctionFactory.Create(tool))
                ],
            };
            chatOptions.AddOllamaOption(OllamaOption.Think, true);

            var response = "";
            
            Console.WriteLine("\n");
            Log.Information("Thinking:");
            await foreach (var token in chatClient.GetStreamingResponseAsync([
                               new ChatMessage(ChatRole.System, request.SystemPrompt),
                               new ChatMessage(ChatRole.User, request.UserPrompt)
                           ], chatOptions))
            {
                foreach (var content in token.Contents)
                {
                    if (content is TextReasoningContent textReasoningContent)
                        // Using Console.Write to preserve same line as one token is short
                        Console.Write(textReasoningContent.Text);
                    else
                        response += token;
                }
            }
            
            Log.Information("Response: {Response}", response);

            return response;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error occurred while sending message to Ollama");

            return string.Empty;
        }
    }
}