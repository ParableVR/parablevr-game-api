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
  public static class DoRegisterUser
  {
    [FunctionName("doRegisterUser")]
    public static async Task<IActionResult> DoRegisterUserAsync(
      [HttpTrigger(AuthorizationLevel.Function, "post", Route = "users/do/register")]HttpRequest req,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<User> users = client.GetCollection<User>("users");

      // deserialise body input into class
      string reqBody;
      try
      {
        reqBody = await new StreamReader(req.Body).ReadToEndAsync();
        if (String.IsNullOrEmpty(reqBody))
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
      
      User registrant = JsonConvert.DeserializeObject<User>(reqBody);

      // valid input
      bool input_valid = true;
      if (String.IsNullOrEmpty(registrant.name_user)) input_valid = false;
      if (String.IsNullOrEmpty(registrant.name_given)) input_valid = false;
      if (String.IsNullOrEmpty(registrant.name_family)) input_valid = false;

      // put the pins in if invalid input
      if (!input_valid)
      {
        return new BadRequestObjectResult(new
        {
          message = "Invalid input"
        });
      }

      // filter non-alphanumeric characters
      Regex alphanum = new Regex("[^a-zA-Z0-9 -]");
      registrant.name_user = alphanum.Replace(registrant.name_user, "");
      registrant.name_given = alphanum.Replace(registrant.name_given, "");
      registrant.name_family = alphanum.Replace(registrant.name_family, "");

      registrant.when_created = DateTime.Now;
      registrant.when_deleted = null;

      // check username doesn't already exist
      if (await users.CountDocumentsAsync(x =>
        x.name_user == registrant.name_user && !x.when_deleted.HasValue) > 0)
      {
        return new BadRequestObjectResult(new
        {
          message = "Username already exists"
        });
      }

      await users.InsertOneAsync(registrant);

      return new OkObjectResult(new
      {
        message = "Successfully registered user",
        user = registrant
      });
    }
  }
}
