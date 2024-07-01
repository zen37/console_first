using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Axpo; // Adjust to the correct namespace where PowerTrade and PowerPeriod are defined

[TestFixture]
public class CalculateTotalVolumesByPeriodTests
{
    [Test]
    public async Task CalculateTotalVolumesByPeriod_WithSampleData_ReturnsCorrectTotals()
    {
        // Arrange
        IEnumerable<PowerTrade> trades = await RetrieveTradesAsync(new DateTime(2015, 4, 1));

        // Act
        Dictionary<int, double> result = Program.CalculateTotalVolumesByPeriod(trades);

        // Assert
        Assert.That(result[1], Is.EqualTo(175), "Period 1 total volume should be 175");
        Assert.That(result[2], Is.EqualTo(200), "Period 2 total volume should be 200");
        Assert.That(result[3], Is.EqualTo(320), "Period 3 total volume should be 320");

        // Add more assertions for other periods as needed
    }

    private async Task<IEnumerable<PowerTrade>> RetrieveTradesAsync(DateTime dateToRetrieve)
    {
        // Simulate asynchronous retrieval
        await Task.Delay(100); // Replace with actual async logic to retrieve trades

        // Example data
        var trades = new List<PowerTrade>
        {
            new PowerTrade(1, new List<PowerPeriod>
            {
                new PowerPeriod { Period = 1, Volume = 100 },
                new PowerPeriod { Period = 2, Volume = 150 },
                new PowerPeriod { Period = 1, Volume = 75 }
            }),
            new PowerTrade(2, new List<PowerPeriod>
            {
                new PowerPeriod { Period = 3, Volume = 200 },
                new PowerPeriod { Period = 2, Volume = 50 },
                new PowerPeriod { Period = 3, Volume = 120 }
            })
            // Add more trades as needed for different scenarios
        };

        return trades;
    }
}
