using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RpWeave.Server.Data.Entities;

public class ChapterEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

    public required ObjectId CampaignId { get; set; }
    public required string Text { get; set; }
}