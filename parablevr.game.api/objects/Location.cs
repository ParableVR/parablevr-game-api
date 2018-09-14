using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace parablevr.game.api.objects
{
  public class Location
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    public string name { get; set; }
    public int max_players { get; set; }
    public DateTime when_created { get; set; }
    public DateTime? when_deleted { get; set; }
    public List<Scenario> scenarios { get; set; }
  }

  public class Scenario
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string goal { get; set; }
    public DateTime when_created { get; set; }
    public DateTime? when_deleted { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string linked_prev_scenario { get; set; }
    [BsonRepresentation(BsonType.ObjectId)]
    public string linked_next_scenario { get; set; }
    public List<Object> objects { get; set; }
  }
}