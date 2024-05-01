using Microsoft.Extensions.DependencyInjection;
using Sql.Sample.Api.Domain;
using Sql.Sample.Api.Services.Interfaces;
using System;

namespace Sql.Sample.Api.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, string connectionString)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddDomain(connectionString);

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICommonService, CommonService>();

            return services;
        }
    }
}