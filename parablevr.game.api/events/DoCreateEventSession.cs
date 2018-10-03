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
  public static class DoCreateEventSession
  {
    [FunctionName("doCreateEventSession")]
    public static async Task<IActionResult> DoCreateEventSessionAsync(
      [HttpTrigger(AuthorizationLevel.Function, "post", Route = "events/do/create/session")]HttpRequest req,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<Event> events = client.GetCollection<Event>("events");

      // deserialise body input into class
      string reqBody;
      Event eventSession;
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

        eventSession = JsonConvert.DeserializeObject<Event>(reqBody);
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
      if (string.IsNullOrEmpty(eventSession.session)) input_valid = false;
      //if (eventSession.events.Count > 0) input_valid = false; // do not allow specifying events here

      // put the pins in if invalid input
      if (!input_valid)
      {
        return new BadRequestObjectResult(new
        {
          message = "Invalid input"
        });
      }

      eventSession.when_started = DateTime.UtcNow;
      eventSession.when_deleted = null;

      await events.InsertOneAsync(eventSession);

      return new OkObjectResult(new
      {
        message = "Successfully created an event session",
        event_session = eventSession
      });
    }
  }
}
