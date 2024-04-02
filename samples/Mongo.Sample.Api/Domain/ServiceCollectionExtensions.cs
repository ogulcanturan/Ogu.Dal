using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDb.Sample.Api.Domain.Repositories;
using MongoDb.Sample.Api.Domain.Repositories.Interfaces;
using MongoDb.Sample.Api.Settings;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;

namespace MongoDb.Sample.Api.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, DbSettings dbSettings)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(dbSettings);
            ArgumentException.ThrowIfNullOrWhiteSpace(dbSettings.ConnectionString);
            ArgumentException.ThrowIfNullOrWhiteSpace(dbSettings.Database);


            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoClientSettings = MongoClientSettings.FromConnectionString(dbSettings.ConnectionString);

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

            services.AddSingleton(dbSettings);

            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();

            return services;
        }
    }
}