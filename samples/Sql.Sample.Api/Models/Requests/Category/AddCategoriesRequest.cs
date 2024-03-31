using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sql.Sample.Api.Models.Requests
{
    public class AddCategoriesRequest
    {
        [Required, Length(1, int.MaxValue)]
        public IEnumerable<string> Categories { get; set; }
    }
}