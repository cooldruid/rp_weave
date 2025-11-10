using RpWeave.Server.Core.Startup;
using RpWeave.Server.Integrations.Ollama.Chat;
using RpWeave.Server.Integrations.Ollama.Embed;
using RpWeave.Server.Integrations.Qdrant;
using RpWeave.Server.Integrations.Qdrant.Requests;

namespace RpWeave.Server.Api.Features.Ai.Prompt;

[ScopedService]
public class PromptHandler(VectorDbClient vectorDbClient, OllamaChatClient ollamaChatClient, OllamaEmbedClient ollamaEmbedClient)
{
    public async Task<string> HandleAsync(string query)
    {
        var vector = await ollamaEmbedClient.GenerateEmbeddingsAsync(query);

        var queryResults = await vectorDbClient.SearchAsync(new VectorDbSearchRequest("test1", vector));
        
        var prompt = $"""
                      You are a Dungeon Master’s assistant. Use only the information provided to answer the user’s question.

                      CONTEXT:
                      {string.Join("\n\n", queryResults.Elements.Select(x => x.Text))}""

                      QUESTION:
                      {query}

                      If the answer is not contained in the context, say "I don't have that information."
                      """;

        var response = await ollamaChatClient.SendAsync(new OllamaChatRequest("", prompt, []));

        return response;
    }
}