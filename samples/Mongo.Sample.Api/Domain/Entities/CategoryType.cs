using MongoDb.Sample.Api.Models.Enums;
using Ogu.Dal.MongoDb.Attributes;
using Ogu.Dal.MongoDb.Entities;

namespace MongoDb.Sample.Api.Domain.Entities
{
    [MongoDatabase(table: "CategoryTypes")]
    public class CategoryType : EnumDatabase<CategoryTypeEnum>
    {
        public CategoryType() : base() { }

        public CategoryType(CategoryTypeEnum categoryType) : base(categoryType)
        {
        }
    }
}