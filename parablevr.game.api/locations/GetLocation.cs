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
  public static class GetLocation
  {
    [FunctionName("getLocationByID")]
    public static async Task<IActionResult> GetLocationByIDAsync(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "locations/get/location/id/{lid}")]HttpRequest req,
      string lid,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<Location> locations = client.GetCollection<Location>("locations");

      List<Location> locResults = (await (await locations.FindAsync(
        x => x.id == lid && x.when_deleted == null)).ToListAsync());

      if (locResults.Count == 0)
      {
        return new NotFoundObjectResult(new
        {
          message = "Location not found"
        });
      }

      return new OkObjectResult(new
      {
        message = "Found location",
        location = locResults.First()
      });
    }
  }
}
