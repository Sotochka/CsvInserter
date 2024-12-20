This is a console app wroted in C#.

1. This is sql script for creating a Database

```
CREATE TABLE TripRides (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    tpep_pickup_datetime DATETIME2 NOT NULL,
    tpep_dropoff_datetime DATETIME2 NOT NULL,
    passenger_count INT NOT NULL,
    trip_distance DECIMAL(18,2) NOT NULL,
    store_and_fwd_flag VARCHAR(3) NOT NULL,
    PULocationID INT NOT NULL,
    DOLocationID INT NOT NULL,
    fare_amount DECIMAL(18,2) NOT NULL,
    tip_amount DECIMAL(18,2) NOT NULL,
    INDEX IX_PULocationId (PULocationID),
    INDEX IX_PickupDateTime (tpep_pickup_datetime),
    INDEX IX_TripDistance (trip_distance)
)
```

2. Rows affected was 29840
![image](https://github.com/user-attachments/assets/53375a64-b8cf-45c7-86ae-22be3c2dc113)
