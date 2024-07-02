using Microsoft.Extensions.Configuration;

using Axpo;
using NUnit.Framework.Internal.Execution;
using System.Data;

class Program
{
    static async Task Main(string[] args)
    {
        IConfiguration config = LoadConfiguration();

        if (config == null)
            throw new ArgumentNullException(nameof(config));

        string csvFolderPath = GetFolderPath(config);
        string timeZoneId = config["timeZoneId"];
        TimeZoneInfo londonTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        DateTime londonTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, londonTimeZone);
        Console.WriteLine($"Date time for Europe/London: {londonTime}");

        DateTime dateToRetrieve = DateTime.Today.AddDays(7);
        //DateTime dateToRetrieve = new DateTime(2015, 4, 1); // Example date
        // Format the date and time as YYYYMMDD_HHMM
        string formattedDateTime = londonTime.ToString("yyyyMMdd_HHmm");

        Console.WriteLine("Formatted London Time: " + formattedDateTime);

        string outputFileName = GetFileName(config, formattedDateTime);
        Console.WriteLine(outputFileName);


        try
        {
            IEnumerable<PowerTrade> trades = await RetrieveTradesAsync(dateToRetrieve);

            Dictionary<int, double> totalVolumesByPeriod = CalculateTotalVolumesByPeriod(trades);

            //Console.WriteLine($"Power Trades for {dateToRetrieve.ToShortDateString()}:");
            //PrintTrades(trades);

            PrintTotalVolumes(totalVolumesByPeriod);
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

    static IConfiguration LoadConfiguration()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        return config;
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