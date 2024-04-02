using Microsoft.Extensions.DependencyInjection;
using System;
using MongoDb.Sample.Api.Domain;
using MongoDb.Sample.Api.Services.Interfaces;
using MongoDb.Sample.Api.Settings;

namespace MongoDb.Sample.Api.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, DbSettings dbSettings)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddDomain(dbSettings);

            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IProductService, ProductService>();

            return services;
        }
    }
}