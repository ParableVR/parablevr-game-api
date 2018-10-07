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
  public static class GetLatestEventSession
  {
    [FunctionName("getLatestEventSession")]
    public static async Task<IActionResult> GetLatestEventSessionAsync(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "events/get/session/latest")]HttpRequest req,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<Event> events = client.GetCollection<Event>("events");

      FilterDefinition<Event> filter = Builders<Event>
        .Filter.Where(x => x.when_deleted == null);

      Event eventSession = await events.Find(filter)
        .SortByDescending(x => x.when_started)
        .Limit(1).SingleOrDefaultAsync();

      return new OkObjectResult(new
      {
        message = "Found an event session",
        event_session = eventSession
      });
    }
  }
}
