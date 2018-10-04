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

namespace parablevr.game.api.sessions
{
  public static class DoCreateEvent
  {
    [FunctionName("doCreateEvent")]
    public static async Task<IActionResult> DoCreateEventAsync(
      [HttpTrigger(AuthorizationLevel.Function, "post", Route = "events/do/create/{eventSession}")]HttpRequest req,
      string eventSession,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<Event> events = client.GetCollection<Event>("events");

      // deserialise body input into class
      string reqBody;
      EventObject @event;
      try
      {
        reqBody = await new StreamReader(req.Body).ReadToEndAsync();
        if (string.IsNullOrEmpty(reqBody))
        {
          return new BadRequestObjectResult(new
          {
            message = "No request body supplied"
          });
        }

        @event = JsonConvert.DeserializeObject<EventObject>(reqBody);
      }
      catch (Exception e)
      {
        return new BadRequestObjectResult(new
        {
          message = e.Message
        });
      }

      // valid input
      bool input_valid = true;
      if (string.IsNullOrEmpty(eventSession))
      {
        input_valid = false;
      }

      if (!(@event.type == "transform" || @event.type == "perception" || @event.type == "reaction"))
      {
        input_valid = false;
      }

      // put the pins in if invalid input
      if (!input_valid)
      {
        return new BadRequestObjectResult(new
        {
          message = "Invalid input"
        });
      }

      @event.when_occured = DateTime.UtcNow;
      EventObject eventBuilt = new EventObject()
      {
        id = ObjectId.GenerateNewId().ToString(),
        type = @event.type.ToLower(),
        user = null,
        when_occured = DateTime.UtcNow,
        contact_duration = @event.contact_duration,
        result = @event.result,
        object_coordinator = @event.object_coordinator,
        objects_involved = @event.objects_involved
      };

      FilterDefinition<Event> filter = Builders<Event>
        .Filter.Eq(x => x.id, eventSession);
      UpdateDefinition<Event> update = Builders<Event>.Update
        .Push<EventObject>(x => x.events, eventBuilt);

      await events.UpdateOneAsync(filter, update);

      return new OkObjectResult(new
      {
        message = "Successfully created an event",
        @event = eventBuilt
      });
    }
  }
}
