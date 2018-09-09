using System;
using System.Configuration;

namespace parablevr.game.api.utilities
{
  public static class Config
  {
    public static string GetEnv(string var)
    {
      return Environment.GetEnvironmentVariable(var, EnvironmentVariableTarget.Process);
    }
  }
}
