using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using parablevr.game.api.objects;
using parablevr.game.api.utilities;

namespace parablevr.game.api.events
{
  public static class GetEvents
  {
    [FunctionName("getEvents")]
    public static async Task<IActionResult> GetEventsAsync(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "events/get/recent/{eventSession}")]HttpRequest req,
      string eventSession,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<Event> events = client.GetCollection<Event>("events");
      
      FilterDefinition<Event> filter = Builders<Event>
        .Filter.Eq(x => x.id, eventSession) &
        Builders<Event>
          .Filter.Where(x => x.when_deleted == null);

      Event @event = await events.Find(filter).SingleOrDefaultAsync();

      if (@event == null)
      {
        return new NotFoundObjectResult(new {
          message = "Event session not found"
        });
      }

      List<EventObject> eventObjects = @event.events
        .OrderByDescending(x => x.when_occured).Take(5).ToList();

      return new OkObjectResult(new
      {
        message = "Yup to events and such",
        events = eventObjects
      });
    }
  }
}
