using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDb.Sample.Api.Domain.Repositories;
using MongoDb.Sample.Api.Domain.Repositories.Interfaces;
using MongoDb.Sample.Api.Models.Settings;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;

namespace MongoDb.Sample.Api.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, string connectionString, string database)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
            ArgumentException.ThrowIfNullOrWhiteSpace(database);

            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoClientSettings = MongoClientSettings.FromConnectionString(connectionString);

#if DEBUG
                var logger = sp.GetRequiredService<ILogger<MongoClient>>();

                mongoClientSettings.ClusterConfigurator = cc =>
                {
                    cc.Subscribe<CommandStartedEvent>(e =>
                        logger.LogInformation("Command: {commandName}, Json: {commandJson}", e.CommandName,
                            e.Command.ToJson()));
                };
#endif

                return new MongoClient(mongoClientSettings);
            });

            services.AddSingleton(_ => new DbSettings { ConnectionString = connectionString, Database = database });

            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();

            return services;
        }
    }
}