using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace parablevr.game.api.objects
{
  public class Event
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string session { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime when_started { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? when_deleted { get; set; }
    public string timeline_file { get; set; }
    public List<EventObject> events { get; set; }
  }

  public class EventObject
  {
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    public string type { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string user { get; set; }
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime when_occured { get; set; }
    public double contact_duration { get; set; }
    public bool result { get; set; }
    public string object_coordinator { get; set; }
    public List<string> objects_involved { get; set; }
  }
}