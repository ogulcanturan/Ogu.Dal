using Ogu.Dal.Sql.Entities;
using System.Collections.Generic;

namespace Sql.Sample.Api.Domain.Entities
{
    public sealed class Category : BaseEntity<int>
    {
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; } = [];
    }
}