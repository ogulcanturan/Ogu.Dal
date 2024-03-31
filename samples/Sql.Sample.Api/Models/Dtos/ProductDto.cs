using Sql.Sample.Api.Domain.Entities;
using System;

namespace Sql.Sample.Api.Models.Dtos
{
    public class ProductDto
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Shortcode { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public CategoryDto Category { get; set; }
    }
}