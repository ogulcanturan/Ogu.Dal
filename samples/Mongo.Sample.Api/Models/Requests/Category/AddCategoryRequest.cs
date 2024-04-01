using System.ComponentModel.DataAnnotations;

namespace MongoDb.Sample.Api.Models.Requests.Category
{
    public class AddCategoryRequest
    {
        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}