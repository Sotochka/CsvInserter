using System.Data;
using CsvInserter.CsvInserterCore.Models;
using CsvInserter.CsvInserterCore.Options;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CsvInserter.CsvInserterCore.Services;

public class DataRepository(IOptions<DatabaseOptions> options, ILogger<DataRepository> logger, IConfiguration configuration) : IDataRepository
{
    private readonly string connectionString = configuration.GetConnectionString("DefaultConnection");
    private const int BatchSize = 10000;
    public async Task BulkInsertAsync(IEnumerable<TripModel> trips)
    {
        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        using var bulkCopy = new SqlBulkCopy(connection)
        {
            DestinationTableName = "TripRides",
            BatchSize = BatchSize
        };

        bulkCopy.ColumnMappings.Add(nameof(TripModel.tpep_pickup_datetime), "tpep_pickup_datetime");
        bulkCopy.ColumnMappings.Add(nameof(TripModel.tpep_dropoff_datetime), "tpep_dropoff_datetime");
        bulkCopy.ColumnMappings.Add(nameof(TripModel.passenger_count), "passenger_count");
        bulkCopy.ColumnMappings.Add(nameof(TripModel.trip_distance), "trip_distance");
        bulkCopy.ColumnMappings.Add(nameof(TripModel.store_and_fwd_flag), "store_and_fwd_flag");
        bulkCopy.ColumnMappings.Add(nameof(TripModel.PULocationID), "PULocationID");
        bulkCopy.ColumnMappings.Add(nameof(TripModel.DOLocationID), "DOLocationID");
        bulkCopy.ColumnMappings.Add(nameof(TripModel.fare_amount), "fare_amount");
        bulkCopy.ColumnMappings.Add(nameof(TripModel.tip_amount), "tip_amount");

        var dataTable = ConvertToDataTable(trips);
        await bulkCopy.WriteToServerAsync(dataTable);
    }

    public async Task<IEnumerable<LocationTipStats>> GetHighestTipsByLocation()
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT 
                PULocationID,
                AVG(tip_amount) as AverageTip,
                COUNT(*) as TripCount
            FROM TripRides
            GROUP BY PULocationID
            ORDER BY AVG(tip_amount) DESC";

        await using var reader = await command.ExecuteReaderAsync();
        var results = new List<LocationTipStats>();

        while (await reader.ReadAsync())
        {
            results.Add(new LocationTipStats
            {
                LocationId = reader.GetInt32(0),
                AverageTip = reader.GetDecimal(1),
                TripCount = reader.GetInt32(2)
            });
        }

        return results;
    }

    public async Task<IEnumerable<TripModel>> GetLongestTrips(int count)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT TOP (@Count) *
            FROM TripRides
            ORDER BY trip_distance DESC";
        command.Parameters.AddWithValue("@Count", count);

        return await ReadTripRides(command);
    }

    public async Task<IEnumerable<TripModel>> GetLongestDurationTrips(int count)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT TOP (@Count) *
            FROM TripRides
            ORDER BY DATEDIFF(MINUTE, tpep_pickup_datetime, tpep_dropoff_datetime) DESC";
        command.Parameters.AddWithValue("@Count", count);

        return await ReadTripRides(command);
    }
    
    private async Task<IEnumerable<TripModel>> ReadTripRides(SqlCommand command)
    {
        await using var reader = await command.ExecuteReaderAsync();
        var results = new List<TripModel>();

        while (await reader.ReadAsync())
        {
            results.Add(new TripModel
            {
                tpep_pickup_datetime = reader.GetDateTime(reader.GetOrdinal("tpep_pickup_datetime")),
                tpep_dropoff_datetime = reader.GetDateTime(reader.GetOrdinal("tpep_dropoff_datetime")),
                passenger_count = reader.GetInt32(reader.GetOrdinal("passenger_count")),
                trip_distance = reader.GetDecimal(reader.GetOrdinal("trip_distance")),
                store_and_fwd_flag = reader.GetString(reader.GetOrdinal("store_and_fwd_flag")),
                PULocationID = reader.GetInt32(reader.GetOrdinal("PULocationID")),
                DOLocationID = reader.GetInt32(reader.GetOrdinal("DOLocationID")),
                fare_amount = reader.GetDecimal(reader.GetOrdinal("fare_amount")),
                tip_amount = reader.GetDecimal(reader.GetOrdinal("tip_amount"))
            });
        }

        return results;
    }
    
    private static DataTable ConvertToDataTable(IEnumerable<TripModel> trips)
    {
        var table = new DataTable();
        
        table.Columns.Add("tpep_pickup_datetime", typeof(DateTime));
        table.Columns.Add("tpep_dropoff_datetime", typeof(DateTime));
        table.Columns.Add("passenger_count", typeof(int));
        table.Columns.Add("trip_distance", typeof(decimal));
        table.Columns.Add("store_and_fwd_flag", typeof(string));
        table.Columns.Add("PULocationID", typeof(int));
        table.Columns.Add("DOLocationID", typeof(int));
        table.Columns.Add("fare_amount", typeof(decimal));
        table.Columns.Add("tip_amount", typeof(decimal));

        foreach (var trip in trips)
        {
            table.Rows.Add(
                trip.tpep_pickup_datetime,
                trip.tpep_dropoff_datetime,
                trip.passenger_count,
                trip.trip_distance,
                trip.store_and_fwd_flag,
                trip.PULocationID,
                trip.DOLocationID,
                trip.fare_amount,
                trip.tip_amount
            );
        }

        return table;
    }
}