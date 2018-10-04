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
  public static class DoCreateSession
  {
    [FunctionName("doCreateSession")]
    public static async Task<IActionResult> DoCreateSessionAsync(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "sessions/do/create")]HttpRequest req,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<Session> sessions = client.GetCollection<Session>("sessions");

      // deserialise body input into class
      string reqBody;
      Session session;
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

        session = JsonConvert.DeserializeObject<Session>(reqBody);
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
      if (string.IsNullOrEmpty(session.name)) input_valid = false;
      if (string.IsNullOrEmpty(session.scenario)) input_valid = false;
      if (session.people.Count < 1) input_valid = false;
      else if (string.IsNullOrEmpty(session.people.First().user)) input_valid = false;

      // put the pins in if invalid input
      if (!input_valid)
      {
        return new BadRequestObjectResult(new
        {
          message = "Invalid input"
        });
      }

      string user = session.people.First().user;

      // filter non-alphanumeric characters
      Regex alphanum = new Regex("[^a-zA-Z0-9 -]");
      session.name = alphanum.Replace(session.name, "");

      session.when_created = DateTime.UtcNow;
      session.when_started = null;
      session.when_ended = null;

      // ensure the host user is cleaned
      SessionPeople host = session.people.Where(x => x.user == user).First();
      host.host = true;
      host.when_joined = DateTime.UtcNow;
      host.when_left = null;

      // check user isn't already in an active session
      FilterDefinition<Session> filter = Builders<Session>.Filter.ElemMatch(
          x => x.people, j => j.user == user &&
        j.when_left.Value != null) &
        Builders<Session>.Filter.Where(x => x.when_ended == null);

      if (await sessions.CountDocumentsAsync(filter) > 0)
      {
        return new BadRequestObjectResult(new
        {
          message = "User already in an active session"
        });
      }

      // TODO: test if the scenario and user exists

      await sessions.InsertOneAsync(session);

      return new OkObjectResult(new
      {
        message = "Successfully created a session",
        session
      });
    }
  }
}
