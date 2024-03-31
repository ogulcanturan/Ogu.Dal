using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sql.Sample.Api.Domain.Repositories;
using Sql.Sample.Api.Domain.Repositories.Interfaces;
using Sql.Sample.Api.Domain.UnitOfWork;
using System;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Sql.Sample.Api.Domain
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, string connectionString)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

            services.AddDbContextPool<Context>(opts =>
            {
                opts.UseSqlite(connectionString);
                opts.ConfigureWarnings(w => w.Ignore(RelationalEventId.AmbientTransactionWarning));
#if DEBUG
                opts.EnableSensitiveDataLogging();
#endif
            });

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}