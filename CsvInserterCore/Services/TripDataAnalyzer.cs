using Microsoft.Extensions.Logging;

namespace CsvInserter.CsvInserterCore.Services;

public class TripDataAnalyzer(ILogger<TripDataAnalyzer> logger, IDataRepository dataRepository)
{
    public async Task RunAnalysisAsync()
    {
        logger.LogInformation("Starting data analysis...");

        var highestTips = await dataRepository.GetHighestTipsByLocation();
        logger.LogInformation("Top locations by average tip:");
        foreach (var location in highestTips.Take(5))
        {
            logger.LogInformation("Location {LocationId}: Average tip ${AverageTip}, Trip count: {TripCount}", 
                location.LocationId, 
                location.AverageTip.ToString("F2"), 
                location.TripCount);
        }

        var longestTrips = await dataRepository.GetLongestTrips(5);
        logger.LogInformation("\nLongest trips by distance:");
        foreach (var trip in longestTrips)
        {
            logger.LogInformation("Trip distance: {Distance} miles, Fare: ${Fare}", 
                trip.trip_distance.ToString("F2"), 
                trip.fare_amount.ToString("F2"));
        }

        var longestDurationTrips = await dataRepository.GetLongestDurationTrips(5);
        logger.LogInformation("\nLongest trips by duration:");
        foreach (var trip in longestDurationTrips)
        {
            var duration = trip.tpep_dropoff_datetime - trip.tpep_pickup_datetime;
            logger.LogInformation("Trip duration: {Duration} hours, Distance: {Distance} miles", 
                duration.TotalHours.ToString("F2"), 
                trip.trip_distance.ToString("F2"));
        }
    }
}