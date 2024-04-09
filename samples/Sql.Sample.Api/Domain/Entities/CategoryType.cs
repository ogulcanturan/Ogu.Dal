using Ogu.Dal.Sql.Entities;
using Sql.Sample.Api.Models.Enums;

namespace Sql.Sample.Api.Domain.Entities
{
    public class CategoryType : EnumDatabase<CategoryTypeEnum>
    {
        public CategoryType() : base() { }

        public CategoryType(CategoryTypeEnum categoryType) : base(categoryType)
        {
        }
    }
}