using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace parablevr.game.api.objects
{
  public class User
  {
    [BsonId]
    public ObjectId id { get; set; }
    public string name_user { get; set; }
    public string name_given { get; set; }
    public string name_family { get; set; }
    public DateTime when_created { get; set; }
    public DateTime? when_deleted { get; set; }

    public User()
    {
      this.id = ObjectId.GenerateNewId();
    }
  }
}