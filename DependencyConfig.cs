﻿using CsvInserter.CsvInserterCore.Options;
using CsvInserter.CsvInserterCore.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CsvInserter;

public static class DependencyConfig
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddOptions();

        // Add configuration
        services.Configure<DatabaseOptions>(
            configuration.GetSection("ConnectionStrings"));

        // Register services
        services.AddScoped<ICsvProcessor, CsvProcessor>()
            .AddScoped<IDataCleaner, DataCleaner>()
            .AddScoped<IDataRepository, DataRepository>()
            .AddScoped<IEtlProcessor, EtlProcessor>()
            .AddScoped<TripDataAnalyzer>();

        return services;
    }
}