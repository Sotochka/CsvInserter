using CsvInserter.CsvInserterCore.Models;

namespace CsvInserter.CsvInserterCore.Services;

public interface IDataRepository
{
    Task BulkInsertAsync(
        IEnumerable<TripModel> rides);
    Task<IEnumerable<LocationTipStats>> GetHighestTipsByLocation();
    Task<IEnumerable<TripModel>> GetLongestTrips(int count);
    Task<IEnumerable<TripModel>> GetLongestDurationTrips(int count);
}