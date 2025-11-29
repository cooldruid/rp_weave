using MongoDB.Bson;
using MongoDB.Driver;
using RpWeave.Server.Core.Startup;
using RpWeave.Server.Data.Entities;

namespace RpWeave.Server.Data.Repositories;

[ScopedService(typeof(IChapterEntityRepository))]
public class ChapterEntityRepository : IChapterEntityRepository
{
    private readonly IMongoCollection<ChapterEntity> collection;

    public ChapterEntityRepository()
    {
        var mongoClient = new MongoClient("mongodb://mongo:27017");

        var mongoDatabase = mongoClient.GetDatabase("rpweave");

        collection = mongoDatabase.GetCollection<ChapterEntity>("Chapters");
        
        var indexKeys = Builders<ChapterEntity>.IndexKeys
            .Ascending(c => c.CampaignId)
            .Ascending(c => c.Id);

        collection.Indexes.CreateOne(
            new CreateIndexModel<ChapterEntity>(indexKeys)
        );
    }
    
    public async Task AddAsync(ChapterEntity entity)
    {
        await collection.InsertOneAsync(entity);
    }
    
    public async Task<ChapterEntity> GetAsync(string id, string campaignId)
    {
        return await collection.Find(x => 
                x.Id == ObjectId.Parse(id) &&
                x.CampaignId == ObjectId.Parse(campaignId))
            .FirstOrDefaultAsync();
    }
}

public interface IChapterEntityRepository
{
    Task AddAsync(ChapterEntity entity);
    Task<ChapterEntity> GetAsync(string id, string campaignId);
}