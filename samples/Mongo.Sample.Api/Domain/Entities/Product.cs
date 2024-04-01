using MongoDB.Bson.Serialization.Attributes;
using Ogu.Dal.MongoDb.Attributes;
using Ogu.Dal.MongoDb.Entities;

namespace MongoDb.Sample.Api.Domain.Entities
{
    [MongoDatabase(table: "Products")]
    public sealed class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Shortcode { get; set; }
        public int Quantity { get; set; }

        public string CategoryId { get; set; }

        [BsonIgnore]
        public Category Category { get; set; }
    }
}