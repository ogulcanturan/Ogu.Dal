using Microsoft.Extensions.DependencyInjection;
using Sql.Sample.Api.Services.Interfaces;
using System;

namespace Sql.Sample.Api.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICommonService, CommonService>();

            return services;
        }
    }
}