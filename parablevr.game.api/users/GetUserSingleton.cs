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
using MongoDB.Driver;
using Newtonsoft.Json;
using parablevr.game.api.objects;
using parablevr.game.api.utilities;

namespace parablevr.game.api.users
{
  public static class GetUserSingleton
  {
    [FunctionName("getUserByID")]
    public static async Task<IActionResult> GetUserByIDAsync(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/get/user/id/{uid}")]HttpRequest req,
      string uid,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<User> users = client.GetCollection<User>("users");

      List<User> userResults = (await (await users.FindAsync(
        x => x.id == uid && x.when_deleted == null)).ToListAsync());

      if (userResults.Count == 0)
      {
        return new NotFoundObjectResult(new
        {
          message = "User not found"
        });
      }

      return new OkObjectResult(new
      {
        message = "Found user",
        user = userResults.First()
      });
    }

    [FunctionName("getUserByUsername")]
    public static async Task<IActionResult> GetUserByUsernameAsync(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "users/get/user/username/{username}")]HttpRequest req,
      string username,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<User> users = client.GetCollection<User>("users");

      List<User> userResults = (await (await users.FindAsync(
        x => x.name_user == username && x.when_deleted == null)).ToListAsync());

      if (userResults.Count == 0)
      {
        return new NotFoundObjectResult(new
        {
          message = "User not found"
        });
      }

      return new OkObjectResult(new
      {
        message = "Found user",
        user = userResults.First()
      });
    }
  }
}
