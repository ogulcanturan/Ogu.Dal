using Microsoft.Extensions.DependencyInjection;
using System;
using MongoDb.Sample.Api.Services.Interfaces;

namespace MongoDb.Sample.Api.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<IProductService, ProductService>();

            return services;
        }
    }
}