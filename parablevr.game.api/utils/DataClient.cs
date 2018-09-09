using System;
using MongoDB.Driver;

namespace parablevr.game.api.utilities
{
  public class DataClient
  {
    private IMongoClient _client;
    private IMongoDatabase _db;

    public DataClient(string database = "game")
    {
      _client = new MongoClient(Config.GetEnv("MongoDB_vr-game--presence"));
      _db = _client.GetDatabase(database);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
      return _db.GetCollection<T>(collectionName);
    }
  }
}