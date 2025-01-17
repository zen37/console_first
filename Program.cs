﻿using Microsoft.Extensions.Configuration;

using Axpo;

class Program
{
    static async Task Main(string[] args)
    {
        // Parse environment from command-line arguments or use default
        string environment = Config.ParseEnvironment(args);

        IConfiguration config = Config.LoadConfiguration(environment);

        if (config == null)
            throw new ArgumentNullException(nameof(config));

        string csvFolderPath = GetFolderPath(config);
        string timeZoneId = config["timeZoneId"];
        TimeZoneInfo timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime dateToRetrieve = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timezone);
        Console.WriteLine($"date time for {timeZoneId}: {dateToRetrieve }");

        //DateTime dateToRetrieve = DateTime.Today.AddDays(0);
        string formattedDateTime = dateToRetrieve.ToString("yyyyMMdd_HHmm");

        Console.WriteLine($"formatted {timeZoneId} Time: " + formattedDateTime);

        string outputFileName = GetFileName(config, formattedDateTime);
        Console.WriteLine($"file extract name: {outputFileName}");


        try
        {
            IEnumerable<PowerTrade> trades = await RetrieveTradesAsync(dateToRetrieve);

            Dictionary<int, double> totalVolumesByPeriod = CalculateTotalVolumesByPeriod(trades);

            //Console.WriteLine($"Power Trades for {dateToRetrieve.ToShortDateString()}:");
            //PrintTrades(trades);

            //PrintTotalVolumes(totalVolumesByPeriod);
            SavePowerPositionsToFile(totalVolumesByPeriod, Path.Combine(csvFolderPath, outputFileName));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving or processing trades: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static async Task<IEnumerable<PowerTrade>> RetrieveTradesAsync(DateTime dateToRetrieve)
    {
        PowerService powerService = new PowerService();
        IEnumerable<PowerTrade> trades = await powerService.GetTradesAsync(dateToRetrieve);
        return trades;
    }


    internal static Dictionary<int, double> CalculateTotalVolumesByPeriod(IEnumerable<PowerTrade> trades)
    {
        Dictionary<int, double> totalVolumesByPeriod = new Dictionary<int, double>();

        foreach (var trade in trades)
        {
            foreach (var period in trade.Periods)
            {
                if (totalVolumesByPeriod.ContainsKey(period.Period))
                {
                    totalVolumesByPeriod[period.Period] += period.Volume;
                }
                else
                {
                    totalVolumesByPeriod.Add(period.Period, period.Volume);
                }
            }
        }

        return totalVolumesByPeriod;
    }

    static string GetFolderPath(IConfiguration config)
    {
        string folder = config["csvFolder"];
        string csvFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), folder);

        if (!Directory.Exists(csvFolderPath))
        {
            Console.WriteLine("The folder does not exist.");
        }

        return csvFolderPath;
    }

    //output file name
    static string GetFileName(IConfiguration config, string datetime)
    {
        try
        {
            string fileName = $"{config["filePrefix"]}{config["filePrefixSep"]}{datetime}{config["fileExtension"]}";
            return fileName;
        }
        catch (Exception ex)
        {
            // Handle or log the exception as needed
            throw new ApplicationException("Failed to generate file name", ex);
        }
    }

    static void PrintTrades(IEnumerable<PowerTrade> trades)
    {
        Console.WriteLine($"Power Trades:");

        foreach (PowerTrade trade in trades)
        {
            Console.WriteLine($"Trade ID: {trade.TradeId}");

            foreach (PowerPeriod period in trade.Periods)
            {
                Console.WriteLine($"Period {period.Period}: Volume: {period.Volume}");
            }
        }
    }

    static void PrintTotalVolumes(Dictionary<int, double> totalVolumesByPeriod)
    {
        Console.WriteLine("\nTotal Volumes:");

        foreach (var kvp in totalVolumesByPeriod)
        {
            Console.WriteLine($"Period {kvp.Key}: Total Volume {kvp.Value}");
        }
    }

    static void SavePowerPositionsToFile(Dictionary<int, double> dictionary, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Period,Total Volume");
            foreach (var kvp in dictionary)
            {
                writer.WriteLine($"{kvp.Key},{kvp.Value}");
            }
        }
        Console.WriteLine($"Saved total volumes to {filePath}");
    }


}