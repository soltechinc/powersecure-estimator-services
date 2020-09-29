using System;

namespace PowerSecure.Estimator.Services
{
    public static class AppSettings
    {
        public static string Get(string name) => Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
    }

}
