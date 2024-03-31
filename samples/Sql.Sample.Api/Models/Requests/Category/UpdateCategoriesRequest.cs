using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sql.Sample.Api.Models.Requests.Category
{
    public class UpdateCategoriesRequest
    {
        [FromQuery]
        public bool FetchFromDb { get; set; }

        [FromBody]
        public UpdateCategoriesRequestBody Body { get; set; }
    }

    public class UpdateCategoriesRequestBody
    {
        [Required, Length(1, int.MaxValue)]
        public IEnumerable<int> Ids { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}