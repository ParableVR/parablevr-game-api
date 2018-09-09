using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace parablevr.game.api.objects
{
  public class Session
  {
    [BsonId]
    public ObjectId id { get; set; }
    public string name { get; set; }
    public ObjectId scenario { get; set; }
    public DateTime when_created { get; set; }
    public DateTime? when_ended { get; set; }
    public List<SessionPeople> people { get; set; }

    public Session()
    {
      this.id = ObjectId.GenerateNewId();
    }
  }

  public class SessionPeople
  {
    public ObjectId user { get; set; }
    public bool spectator { get; set; }
    public DateTime when_joined { get; set; }
    public DateTime? when_left { get; set; }
  }
}