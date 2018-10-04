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

namespace parablevr.game.api.locations
{
  public static class GetLocationList
  {
    [FunctionName("getLocationList")]
    public static async Task<IActionResult> GetLocationListAsync(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "locations/get/location/list")]HttpRequest req,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<Location> locations = client.GetCollection<Location>("locations");

      List<Location> locResults = await locations.Find(x => x.when_deleted == null)
        .Project<Location>(
          Builders<Location>.Projection
            .Include(x => x.id)
            .Include(x => x.name)
            .Include(x => x.max_players)
            .Include(x => x.when_created))
        .ToListAsync();

      if (locResults.Count == 0)
      {
        return new NotFoundObjectResult(new
        {
          message = "No locations found"
        });
      }

      return new OkObjectResult(new
      {
        message = "Found locations",
        locations = locResults
      });
    }
  }
}
