using System;

namespace MongoDb.Sample.Api.Models.Dtos
{
    public class ProductDto
    {
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Shortcode { get; set; }
        public int Quantity { get; set; }
        public string CategoryId { get; set; }
        public CategoryDto Category { get; set; }
    }
}