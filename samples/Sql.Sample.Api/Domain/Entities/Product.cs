﻿using Ogu.Dal.Sql.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sql.Sample.Api.Domain.Entities
{
    public sealed class Product : BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Shortcode { get; set; }
        public int Quantity { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
    }
}