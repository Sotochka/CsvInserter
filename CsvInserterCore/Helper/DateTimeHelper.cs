namespace CsvInserter.CsvInserterCore.Helper;

public class DateTimeHelper
{
    private static readonly TimeZoneInfo EstTimeZone = 
        TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    public static DateTime ConvertEstToUtc(DateTime estDateTime)
    {
        try
        {
            return TimeZoneInfo.ConvertTimeToUtc(estDateTime, EstTimeZone);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error converting EST to UTC: {ex.Message}", ex);
        }
    }

    public static bool IsValidDateTime(DateTime dateTime)
    {
        // Check if date is within reasonable range (e.g., not future date)
        if (dateTime > DateTime.UtcNow)
            return false;

        // Check if date is not too old (e.g., before 2015)
        if (dateTime.Year < 2015)
            return false;

        return true;
    }

    public static bool IsValidTripDuration(DateTime pickupTime, DateTime dropoffTime)
    {
        // Check if dropoff is after pickup
        if (dropoffTime <= pickupTime)
            return false;

        // Check if trip duration is not unreasonably long (e.g., more than 24 hours)
        var duration = dropoffTime - pickupTime;
        if (duration.TotalHours > 24)
            return false;

        return true;
    }
}