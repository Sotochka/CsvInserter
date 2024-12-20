using CsvInserter.CsvInserterCore.Models;

namespace CsvInserter.CsvInserterCore.Services;

public interface IDataCleaner
{
    Task<(IEnumerable<TripModel> validRecords, IEnumerable<TripModel> duplicates)> CleanDataAsync(IAsyncEnumerable<TripModel> records);
}