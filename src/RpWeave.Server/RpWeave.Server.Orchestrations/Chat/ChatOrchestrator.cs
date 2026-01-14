using RpWeave.Server.Core.Startup;
using RpWeave.Server.Orchestrations.Chat.Modules.Classification;
using RpWeave.Server.Orchestrations.Chat.Modules.VectorSearch;
using RpWeave.Server.Orchestrations.Chat.Modules.Writing;

namespace RpWeave.Server.Orchestrations.Chat;

[ScopedService]
public class ChatOrchestrator(
    ClassificationModule classificationModule,
    VectorSearchModule vectorSearchModule,
    WritingModule writingModule)
{
    public async Task<ChatResponse> ChatAsync(ChatRequest chatRequest)
    {
        var classificationRequest = new ClassificationRequest(chatRequest.Query);
        var classification = await classificationModule.ProcessAsync(classificationRequest);

        var context = "";
        if (classification.ShouldSearch)
        {
            var vectorSearchRequest = new VectorSearchRequest(
                chatRequest.CampaignCollectionName,
                classification.StandaloneQuestion);

            var vectorSearchResponse = await vectorSearchModule.ProcessAsync(vectorSearchRequest);

            context = string.Join("\n\n---\n\n",
                vectorSearchResponse.Elements
                    .Select(x => x.Text));
        }
        
        var writingRequest = new WritingRequest(context, classification.StandaloneQuestion);
        var writingResponse = await writingModule.ProcessAsync(writingRequest);

        return new ChatResponse(writingResponse.Message);
    }
}