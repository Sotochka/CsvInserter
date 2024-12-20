using CsvInserter.CsvInserterCore.Helper;
using CsvInserter.CsvInserterCore.Models;

namespace CsvInserter.CsvInserterCore.Services;

public class DataCleaner : IDataCleaner
{
    public async Task<(IEnumerable<TripModel> validRecords, IEnumerable<TripModel> duplicates)> CleanDataAsync(IAsyncEnumerable<TripModel> records)
    {
        var uniqueRecords = new HashSet<TripModel>();
        var duplicates = new List<TripModel>();

        await foreach (var record in records)
        {
            CleanRecord(record);

            if (!uniqueRecords.Add(record))
            {
                duplicates.Add(record);
                continue;
            }
        }

        return (uniqueRecords, duplicates);

    }
    private void CleanRecord(TripModel record)
    {
        record.store_and_fwd_flag = ValidationHelper.NormalizeStoreAndFwdFlag(record.store_and_fwd_flag);
        record.store_and_fwd_flag = record.store_and_fwd_flag.Trim().ToUpper() switch
        {
            "Y" => "Yes",
            "N" => "No",
            _ => record.store_and_fwd_flag
        };

        record.store_and_fwd_flag = record.store_and_fwd_flag.Trim();

        if (record.passenger_count < 0)
            record.passenger_count = 0;

        if (record.trip_distance < 0)
            record.trip_distance = 0;

        if (record.fare_amount < 0)
            record.fare_amount = 0;

        if (record.tip_amount < 0)
            record.tip_amount = 0;
    }
}