using CsvInserter.CsvInserterCore.Models;

namespace CsvInserter.CsvInserterCore.Services;

public interface ICsvProcessor
{
    IAsyncEnumerable<TripModel> ProcessCsvAsync(string filePath);
}