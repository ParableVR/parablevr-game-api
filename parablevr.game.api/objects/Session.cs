using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace parablevr.game.api.objects
{
  public class Session
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    public string name { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string scenario { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime when_created { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? when_started { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? when_ended { get; set; }
    public List<SessionPeople> people { get; set; }
  }

  public class SessionPeople
  {
    [BsonRepresentation(BsonType.ObjectId)]
    public string user { get; set; }
    public bool spectator { get; set; }
    public bool host { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime when_joined { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? when_left { get; set; }
  }
}