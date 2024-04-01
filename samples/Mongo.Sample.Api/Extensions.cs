using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDb.Sample.Api.Domain.Entities;
using MongoDb.Sample.Api.Domain.Repositories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDb.Sample.Api
{
    public static class Extensions
    {
        public static string[] GetErrors(this ModelStateDictionary modelState) => modelState.IsValid ? [] : modelState.Values.SelectMany(v => v.Errors).Select(v => v.ErrorMessage).ToArray();

        public static async Task SeedWithSomeDataAsync(this ICategoryRepository categoryRepository)
        {
            if (!await categoryRepository.IsAnyAsync())
            {
                var currentTime = DateTime.UtcNow;

                await categoryRepository.InstantAddRangeAsync(new Category[]
                {
                    new Category { Name = "Some", CreatedOn = currentTime },
                    new Category { Name = "Som", CreatedOn = currentTime  },
                    new Category { Name = "So", CreatedOn = currentTime  },
                    new Category { Name = "S", CreatedOn = currentTime  },
                    new Category { Name = "Any", CreatedOn = currentTime  },
                    new Category { Name = "An", CreatedOn = currentTime  },
                    new Category { Name = "A", CreatedOn = currentTime  },
                });
            }
        }
    }
}