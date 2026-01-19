using System.Text;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Orchestrations.Chat.Modules.Classification;
using RpWeave.Server.Orchestrations.Chat.Modules.Editing;
using RpWeave.Server.Orchestrations.Chat.Modules.VectorSearch;
using RpWeave.Server.Orchestrations.Chat.Modules.Writing;

namespace RpWeave.Server.Orchestrations.Chat;

[ScopedService]
public class ChatOrchestrator(
    ClassificationModule classificationModule,
    VectorSearchModule vectorSearchModule,
    WritingModule writingModule,
    EditingModule editingModule)
{
    public async Task<ChatResponse> ChatAsync(ChatRequest chatRequest)
    {
        var formattedChatHistory = FormatChatHistory(chatRequest.ChatHistory);
        
        var classificationRequest = new ClassificationRequest(chatRequest.Query, formattedChatHistory);
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
        
        var writingRequest = new WritingRequest(context, classification.StandaloneQuestion, classification.ConversationSummary);
        var writingResponse = await writingModule.ProcessAsync(writingRequest);

        // var editingRequest = new EditingRequest(writingResponse.Message, classification.Advice);
        // var editingResponse = await editingModule.ProcessAsync(editingRequest);

        return new ChatResponse(writingResponse.Message);
    }

    private static string FormatChatHistory(List<ChatHistoryLine> chatHistory)
    {
        var sb = new StringBuilder();
        foreach (var historyLine in chatHistory.OrderBy(x => x.Order))
        {
            var tag = historyLine.Type.ToLowerInvariant() == "user" ? "User:" : "Assistant:";
            var line = $"{tag} {historyLine.Message}\n";
            
            sb.AppendLine(line);
        }
        
        return sb.ToString();
    }
}