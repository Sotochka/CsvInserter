using System.Globalization;
using CsvHelper;
using CsvInserter.CsvInserterCore.Models;
using Microsoft.Extensions.Logging;

namespace CsvInserter.CsvInserterCore.Services;

public class EtlProcessor(ICsvProcessor csvProcessor, IDataCleaner dataCleaner, IDataRepository dataRepository, ILogger<EtlProcessor> logger) : IEtlProcessor
{
    public async Task ProcessAsync(string filePath)
    {
        try
        {
            logger.LogInformation("Starting ETL process for file: {FilePath}", filePath);
            var records = csvProcessor.ProcessCsvAsync(filePath);

            var (validRecords, duplicates) = await dataCleaner.CleanDataAsync(
                records);

            if (duplicates.Any())
            {
                var duplicatesPath = Path.Combine(
                    Path.GetDirectoryName(filePath) ?? string.Empty,
                    "duplicates.csv");
                    
                await WriteDuplicatesToCsvAsync(duplicates, duplicatesPath);
            }

            await dataRepository.BulkInsertAsync(validRecords);

            logger.LogInformation("ETL process completed successfully");

        }catch (Exception ex)
        {
            logger.LogError(ex, "Error during ETL process");
            throw;
        }
    }
    
    private async Task WriteDuplicatesToCsvAsync(
        IEnumerable<TripModel> duplicates, 
        string filePath)
    {
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(duplicates);
    }

}