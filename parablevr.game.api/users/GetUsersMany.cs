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

namespace parablevr.game.api.users
{
  public static class GetUsersMany
  {
    [FunctionName("getUsersManyByID")]
    public static async Task<IActionResult> GetUsersByIDAsync(
      [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/get/users/id")]HttpRequest req,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<User> users = client.GetCollection<User>("users");

      string reqBody;
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
      }
      catch (Exception e)
      {
        return new BadRequestObjectResult(new
        {
          message = e.Message
        });
      }

      List<string> uidsRaw = JsonConvert.DeserializeObject<List<string>>(reqBody);
      List<string> uidsPoison = new List<string>();
      List<string> uids = new List<string>();

      if (uidsRaw.Count == 0)
      {
        return new NotFoundObjectResult(new
        {
          message = "No search terms specified"
        });
      }

      // try creating a list of object ids
      foreach (string id in uidsRaw)
      {
        try
        {
          uids.Add(id);
        }
        catch
        {
          uidsPoison.Add(id);
        }
      }

      FilterDefinition<User> filter = Builders<User>.Filter.Where(x => x.when_deleted == null) &
        Builders<User>.Filter.In(x => x.id, uids);

      List<User> userResults = (await (await users.FindAsync(filter)).ToListAsync());

      if (userResults.Count == 0)
      {
        return new NotFoundObjectResult(new
        {
          message = "No users found"
        });
      }

      uids.RemoveAll(x => userResults.Select(j => j.id).ToList().Contains(x));

      return new OkObjectResult(new
      {
        message = "Found users",
        users_found = userResults,
        users_notfound = uids,
        users_poison = uidsPoison
      });
    }

    [FunctionName("getUsersManyByUsername")]
    public static async Task<IActionResult> GetUsersByUsernameAsync(
      [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/get/users/username")]HttpRequest req,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<User> users = client.GetCollection<User>("users");

      string reqBody;
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
      }
      catch (Exception e)
      {
        return new BadRequestObjectResult(new
        {
          message = e.Message
        });
      }

      List<string> usernamesRaw = JsonConvert.DeserializeObject<List<string>>(reqBody);
      List<string> usernames = new List<string>();

      if (usernamesRaw.Count == 0)
      {
        return new NotFoundObjectResult(new
        {
          message = "No search terms specified"
        });
      }

      // try creating a list of object ids
      Regex alphanum = new Regex("[^a-zA-Z0-9 -]");
      foreach (string username in usernamesRaw)
      {
        usernames.Add(alphanum.Replace(username, ""));
      }

      FilterDefinition<User> filter = Builders<User>.Filter.Where(x => x.when_deleted == null) &
        Builders<User>.Filter.In(x => x.name_user, usernames);

      List<User> userResults = (await (await users.FindAsync(filter)).ToListAsync());

      if (userResults.Count == 0)
      {
        return new NotFoundObjectResult(new
        {
          message = "No users found"
        });
      }

      usernames.RemoveAll(x => userResults.Select(j => j.name_user).ToList().Contains(x));

      return new OkObjectResult(new
      {
        message = "Found users",
        users_found = userResults,
        users_notfound = usernames
      });
    }
  }
}
