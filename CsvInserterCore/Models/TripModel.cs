namespace CsvInserter.CsvInserterCore.Models;

public class TripModel
{
    public DateTime tpep_pickup_datetime { get; set; }
    public DateTime tpep_dropoff_datetime { get; set; }
    public int passenger_count { get; set; }
    public decimal trip_distance { get; set; }
    public string store_and_fwd_flag { get; set; }
    public int PULocationID { get; set; }
    public int DOLocationID { get; set; }
    public decimal fare_amount { get; set; }
    public decimal tip_amount { get; set; }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(
            tpep_pickup_datetime, 
            tpep_dropoff_datetime, 
            passenger_count);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not TripModel other) return false;
        
        return tpep_pickup_datetime == other.tpep_pickup_datetime &&
               tpep_dropoff_datetime == other.tpep_dropoff_datetime &&
               passenger_count == other.passenger_count;
    }
    
    
}