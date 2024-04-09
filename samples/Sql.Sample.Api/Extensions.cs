using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Sql.Sample.Api.Domain;
using Sql.Sample.Api.Domain.Entities;
using Sql.Sample.Api.Models.Enums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sql.Sample.Api
{
    public static class Extensions
    {
        public static string[] GetErrors(this ModelStateDictionary modelState) => modelState.IsValid ? [] : modelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToArray();

        public static async Task SeedWithSomeDataAsync(this Context context)
        {
            if (!await context.Categories.AnyAsync())
            {
                var currentTime = DateTime.UtcNow;

                context.Categories.AddRange(new Category[]
                {
                    new Category { Name = "Some", CreatedOn = currentTime, CategoryTypeId = CategoryTypeEnum.Ordinary },
                    new Category { Name = "Som", CreatedOn = currentTime, CategoryTypeId = CategoryTypeEnum.Special },
                    new Category { Name = "So", CreatedOn = currentTime, CategoryTypeId = CategoryTypeEnum.Ordinary },
                    new Category { Name = "S", CreatedOn = currentTime, CategoryTypeId = CategoryTypeEnum.Special },
                    new Category { Name = "Any", CreatedOn = currentTime, CategoryTypeId = CategoryTypeEnum.Ordinary },
                    new Category { Name = "An", CreatedOn = currentTime, CategoryTypeId = CategoryTypeEnum.Ordinary },
                    new Category { Name = "A", CreatedOn = currentTime, CategoryTypeId = CategoryTypeEnum.Special },
                });
            }

            await context.SaveChangesAsync();
        }
    }
}