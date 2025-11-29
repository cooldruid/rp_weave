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
        
        // var chapters = new List<ChapterEntity>();
        // foreach (var element in queryResults.Elements)
        // {
        //     if(chapters.Any(x => x.Id.ToString() == element.Id))
        //         continue;
        //     
        //     var chapter = await chapterEntityRepository.GetAsync(element.Id, request.CampaignId);
        //
        //     chapters.Add(chapter);
        // }

        var systemPrompt = $"""
                            You are a TTRPG Game Master's assistant. 
                            Follow these rules strictly:
                            1. Use ONLY the information from the "SOURCE TEXT" below when answering factual questions.
                            2. If the answer is not in the source text, say "The source text does not contain that information."
                            3. Do NOT invent facts.
                            4. Keep answers concise unless asked otherwise.
                            5. Ignore anything outside TTRPGs.
                            """;
        
        var prompt = $"""
                      BEGIN SOURCE TEXT
                      {string.Join("\n\n---\n\n", queryResults.Elements.Select(x => x.Text))}
                      END SOURCE TEXT

                      BEGIN QUESTION
                      {request.Prompt}
                      END QUESTION
                      
                      IMPORTANT: 
                      Answer the question marked between "BEGIN QUESTION" and "END QUESTION".
                      Before answering, check whether the answer exists inside the source text above, marked between "BEGIN SOURCE TEXT" and "END SOURCE TEXT". 
                      The source text may contain typos or improper fragments. If you notice it, try to make sense what the correct form of the text should be.
                      If the answer does not appear, say that and then make a best guess at the answer. Justify your decision.
                      Do NOT use outside knowledge. 
                      Do NOT reference anything not explicitly written in the source text, unless the user explicitly requests an opinion.
                      Be friendly and cheerful to the user. If it makes sense, use emojis to drive your point across.
                      """;

        var response = await ollamaChatClient.SendAsync(new OllamaChatRequest(systemPrompt, prompt, []));

        return new PromptResponse(response);
    }
}