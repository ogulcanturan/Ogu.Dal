using Ogu.Dal.Sql.Entities;
using Sql.Sample.Api.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sql.Sample.Api.Domain.Entities
{
    public sealed class Category : BaseEntity<int>
    {
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; } = [];

        public CategoryTypeEnum CategoryTypeId { get; set; }

        [ForeignKey(nameof(CategoryTypeId))]
        public CategoryType CategoryType { get; set; }
    }
}