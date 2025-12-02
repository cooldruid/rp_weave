using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;
using RpWeave.Server.Data.Repositories;
using RpWeave.Server.Integrations.Ollama.Chat;
using RpWeave.Server.Integrations.Ollama.Embed;
using RpWeave.Server.Integrations.Qdrant;
using RpWeave.Server.Integrations.Qdrant.Requests;

namespace RpWeave.Server.Api.Features.Ai.Prompt;

[ScopedService]
public class PromptHandler(
    IChapterEntityRepository chapterEntityRepository,
    VectorDbClient vectorDbClient, 
    OllamaChatClient ollamaChatClient, 
    OllamaEmbedClient ollamaEmbedClient)
{
    public async Task<PromptResponse> HandleAsync(PromptRequest request)
    {
        var vector = await ollamaEmbedClient.GenerateEmbeddingsAsync(request.Prompt);

        var queryResults = await vectorDbClient.SearchAsync(new VectorDbSearchRequest(request.CollectionName, vector));

        var systemPrompt = $"""
                            You are a TTRPG Game Master's assistant. 
                            Follow these rules strictly:
                            1. Use ONLY the information from the "SOURCE TEXT" below when answering factual questions.
                            2. The provided source material excerpts are ordered by relevance and they are separated by '---'. The earliest an excerpt appears, the more relevant it should be.
                            3. If the answer is not in the source text, give a best guess and justify it. Be explicit that it is a guess.
                            4. Do NOT invent facts.
                            5. Refuse to answer anything outside TTRPGs.
                            6. Be friendly and cheerful to the user. If it makes sense, use emojis to drive your point across.
                            """;
        
        var prompt = $"""
                      BEGIN SOURCE TEXT
                      {string.Join("\n\n---\n\n", queryResults.Elements.Select(x => x.Text))}
                      END SOURCE TEXT

                      BEGIN QUESTION
                      {request.Prompt}
                      END QUESTION
                      """;

        var response = await ollamaChatClient.SendAsync(new OllamaChatRequest(systemPrompt, prompt, []));

        return new PromptResponse(response);
    }
}