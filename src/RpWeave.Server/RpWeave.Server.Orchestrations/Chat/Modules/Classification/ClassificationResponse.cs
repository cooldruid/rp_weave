namespace RpWeave.Server.Orchestrations.Chat.Modules.Classification;

public record ClassificationResponse(bool ShouldSearch, string StandaloneQuestion, string ConversationSummary, string Advice);