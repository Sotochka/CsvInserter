using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvInserter.CsvInserterCore.Helper;
using CsvInserter.CsvInserterCore.Models;
using Microsoft.Extensions.Logging;

namespace CsvInserter.CsvInserterCore.Services;

public class CsvProcessor(ILogger<CsvProcessor> logger) : ICsvProcessor
{
    public async IAsyncEnumerable<TripModel> ProcessCsvAsync(string filePath)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            IgnoreBlankLines = true,
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);

        await csv.ReadAsync();
        csv.ReadHeader();
        ValidateHeaders(csv.HeaderRecord);

        var rowNumber = 0;
        string[] currentRow;

        while (await csv.ReadAsync())
        {
            rowNumber++;
            
                currentRow = csv.Parser.Record;

                if (currentRow.Any(field => string.IsNullOrWhiteSpace(field)))
                {
                    logger.LogWarning("Row {RowNumber} contains empty fields: {Row}", 
                        rowNumber, 
                        string.Join(", ", currentRow));
                    continue;
                }

                var record = csv.GetRecord<TripModel>();

                if (record != null)
                {
                    record.tpep_pickup_datetime = DateTimeHelper.ConvertEstToUtc(record.tpep_pickup_datetime);
                    record.tpep_dropoff_datetime = DateTimeHelper.ConvertEstToUtc(record.tpep_dropoff_datetime);
                    yield return record;
                }
            
        }
    }
    private void ValidateHeaders(string[] headers)
    {
        var requiredHeaders = new[]
        {
            "tpep_pickup_datetime",
            "tpep_dropoff_datetime",
            "passenger_count",
            "trip_distance",
            "store_and_fwd_flag",
            "PULocationID",
            "DOLocationID",
            "fare_amount",
            "tip_amount"
        };

        var missingHeaders = requiredHeaders
            .Where(required => !headers.Contains(required, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (missingHeaders.Any())
        {
            throw new Exception($"Missing required headers: {string.Join(", ", missingHeaders)}");
        }
    }
}