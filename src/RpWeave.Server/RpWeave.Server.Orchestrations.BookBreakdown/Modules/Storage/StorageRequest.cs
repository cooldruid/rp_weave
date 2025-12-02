using RpWeave.Server.Orchestrations.BookBreakdown.Modules.Extraction;

namespace RpWeave.Server.Orchestrations.BookBreakdown.Modules.Storage;

public class StorageRequest
{
    public string Name { get; set; }
    public string CampaignId { get; set; }
    public List<TextChunk> Chunks { get; set; } = [];
}