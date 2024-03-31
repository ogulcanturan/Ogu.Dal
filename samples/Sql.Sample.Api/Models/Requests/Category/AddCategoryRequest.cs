using System.ComponentModel.DataAnnotations;

namespace Sql.Sample.Api.Models.Requests.Category
{
    public class AddCategoryRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}