
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
static class Config
{

    internal static string ParseEnvironment(string[] args)
    {
        // Default environment if not specified
        string environment = "Development";

        // Parse environment from command-line arguments, if provided
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("--environment="))
            {
                environment = args[i].Substring("--environment=".Length);
                break;
            }
        }

        Console.WriteLine($"using environment: {environment}");

        return environment;
    }

    internal static IConfiguration LoadConfiguration(string environment)
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"config/appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .Build();

        return config;
    }
}