namespace CsvInserter.CsvInserterCore.Services;

public interface IEtlProcessor
{
    Task ProcessAsync(string filePath);
}