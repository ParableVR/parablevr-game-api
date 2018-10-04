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
  public static class GetScenario
  {
    [FunctionName("getScenarioByID")]
    public static async Task<IActionResult> GetScenarioByIDAsync(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "locations/get/scenario/id/{sid}")]HttpRequest req,
      string sid,
      ILogger log)
    {
      // set up connection
      DataClient client = new DataClient();
      IMongoCollection<Location> locations = client.GetCollection<Location>("locations");

      var scenarioResults = await locations.Find(x => x.scenarios.Any(j => j.id == sid))
        .Project(x => new { scenario = x.scenarios.Where(j => j.id == sid) })
        .ToListAsync();

      if (scenarioResults.Count == 0)
      {
        return new NotFoundObjectResult(new
        {
          message = "Scenario not found"
        });
      }

      return new OkObjectResult(new
      {
        message = "Found scenario",
        scenario = scenarioResults.First().scenario.First()
      });
    }
  }
}
