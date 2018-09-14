using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace parablevr.game.api.objects
{
  public class Object
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string id { get; set; }
    public double x { get; set; }
    public double y { get; set; }
    public double z { get; set; }
    public double yaw { get; set; }
    public double pitch { get; set; }
    public double roll { get; set; }
    public bool significant { get; set; }
  }
}