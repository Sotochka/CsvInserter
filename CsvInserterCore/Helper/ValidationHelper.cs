using CsvInserter.CsvInserterCore.Models;

namespace CsvInserter.CsvInserterCore.Helper;

public class ValidationHelper
{
    public static bool IsValidPassengerCount(int count)
    {
        // Typical taxi can't have more than 8 passengers
        return count is >= 0 and <= 8;
    }

    public static bool IsValidtrip_distance(decimal distance)
    {
        // Distance should be positive and within reasonable range (e.g., < 1000 miles)
        return distance is >= 0 and < 1000;
    }

    public static bool IsValidAmount(decimal amount)
    {
        // Amount should be non-negative and within reasonable range
        return amount is >= 0 and < 10000;
    }

    public static bool IsValidLocationId(int locationId)
    {
        // LocationIds should be positive
        return locationId > 0;
    }

    public static bool IsValidStoreAndFwdFlag(string flag)
    {
        if (string.IsNullOrWhiteSpace(flag))
            return false;

        flag = flag.Trim().ToUpper();
        return flag is "Y" or "N" or "YES" or "NO";
    }

    public static string NormalizeStoreAndFwdFlag(string flag)
    {
        return flag.Trim().ToUpper() switch
        {
            "Y" => "Yes",
            "N" => "No",
            _ => flag.Trim()
        };
    }

    public static bool ValidateRecord(TripModel ride, out List<string> validationErrors)
    {
        validationErrors = new List<string>();

        if (!IsValidPassengerCount(ride.passenger_count))
            validationErrors.Add($"Invalid passenger count: {ride.passenger_count}");

        if (!IsValidtrip_distance(ride.trip_distance))
            validationErrors.Add($"Invalid trip distance: {ride.trip_distance}");

        if (!IsValidAmount(ride.fare_amount))
            validationErrors.Add($"Invalid fare amount: {ride.fare_amount}");

        if (!IsValidAmount(ride.tip_amount))
            validationErrors.Add($"Invalid tip amount: {ride.tip_amount}");

        if (!IsValidLocationId(ride.PULocationID))
            validationErrors.Add($"Invalid pickup location ID: {ride.PULocationID}");

        if (!IsValidLocationId(ride.DOLocationID))
            validationErrors.Add($"Invalid dropoff location ID: {ride.DOLocationID}");

        if (!IsValidStoreAndFwdFlag(ride.store_and_fwd_flag))
            validationErrors.Add($"Invalid store and forward flag: {ride.store_and_fwd_flag}");

        if (!DateTimeHelper.IsValidDateTime(ride.tpep_pickup_datetime))
            validationErrors.Add($"Invalid pickup datetime: {ride.tpep_pickup_datetime}");

        if (!DateTimeHelper.IsValidDateTime(ride.tpep_dropoff_datetime))
            validationErrors.Add($"Invalid dropoff datetime: {ride.tpep_dropoff_datetime}");

        if (!DateTimeHelper.IsValidTripDuration(ride.tpep_pickup_datetime, ride.tpep_dropoff_datetime))
            validationErrors.Add("Invalid trip duration");

        return validationErrors.Count == 0;
    }
}