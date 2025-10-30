using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RpWeave.Server.Data.Entities;

public class AppUserRefreshToken
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public required string UserId { get; set; }
    
    public required string RefreshToken { get; set; }
    public required DateTime ExpiresOn { get; set; }
}