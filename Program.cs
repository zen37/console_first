using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Axpo; // Replace with your correct namespace

class Program
{
    static async Task Main(string[] args)
    {
        // Replace with your actual date
        DateTime dateToRetrieve = DateTime.Today;

        try
        {
            // Create an instance of PowerService
            PowerService powerService = new PowerService();

            // Example: Using asynchronous method GetTradesAsync
            IEnumerable<PowerTrade> trades = await powerService.GetTradesAsync(dateToRetrieve);

            Console.WriteLine($"Power Trades for {dateToRetrieve.ToShortDateString()}:");
            foreach (PowerTrade trade in trades)
            {
                Console.WriteLine($"Trade ID: {trade.TradeId}");
                foreach (PowerPeriod period in trade.Periods)
                {
                    Console.WriteLine($"Period {period.Period}: Volume: {period.Volume}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving trades: {ex.Message}");
        }

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }
}
