using MongoDB.Bson.Serialization.Attributes;
using Ogu.Dal.MongoDb.Attributes;
using Ogu.Dal.MongoDb.Entities;
using System.Collections.Generic;

namespace MongoDb.Sample.Api.Domain.Entities
{
    [MongoDatabase(table: "Categories")]
    [MongoIndex(new []{ nameof(Name)})]
    public sealed class Category : BaseEntity
    {
        public string Name { get; set; }

        public ICollection<string> ProductIds { get; set; } = new List<string>();

        [BsonIgnore]
        public ICollection<Product> Products { get; set; } = [];
    }
}