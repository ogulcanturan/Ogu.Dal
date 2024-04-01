using System;
using System.Collections.Generic;

namespace MongoDb.Sample.Api.Models.Dtos
{
    public class CategoryDto
    {
        public string Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string Name { get; set; }

        public IEnumerable<ProductDto> Products { get; set; }
    }
}