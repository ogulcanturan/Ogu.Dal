using MongoDb.Sample.Api.Domain.Entities;
using MongoDb.Sample.Api.Models.Dtos;
using System.Collections.Generic;
using System.Linq;

namespace MongoDb.Sample.Api.Services
{
    public static class MappingExtensions
    {
        public static CategoryDto ToDto(this Category category) => new CategoryDto
        {
            Id = category.Id,
            CreatedOn = category.CreatedOn,
            UpdatedOn = category.UpdatedOn,
            Name = category.Name,
            Products = category.Products.ToDto() 
        };

        public static ProductDto ToDto(this Product product) => new ProductDto
        {
            Id = product.Id,
            CreatedOn = product.CreatedOn,
            UpdatedOn = product.UpdatedOn,
            Name = product.Name,
            Description = product.Description,
            Shortcode = product.Shortcode,
            Quantity = product.Quantity,
            CategoryId = product.CategoryId,
            Category = product.Category?.ToDto()
        };

        public static CategoryDto[] ToDto(this IEnumerable<Category> categories) => categories.Select(c => c.ToDto()).ToArray();

        public static ProductDto[] ToDto(this IEnumerable<Product> products) => products.Select(e => e.ToDto()).ToArray();
    }
}