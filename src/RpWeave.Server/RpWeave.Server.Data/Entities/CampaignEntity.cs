using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RpWeave.Server.Data.Entities;

public class CampaignEntity
{
    public CampaignEntity()
    {
        Id = ObjectId.GenerateNewId();
    }
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }  
    
    public string? PdfPath { get; set; }
    public string? VectorCollectionName { get; set; }
}