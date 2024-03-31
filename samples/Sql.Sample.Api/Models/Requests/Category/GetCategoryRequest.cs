using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Sql.Sample.Api.Models.Requests.Category
{
    public class GetCategoryRequest
    {
        [FromRoute(Name = "id")]
        [Range(1, int.MaxValue)]
        public int Id { get; set; }

        [FromQuery]
        public bool IncludeProducts { get; set; }
    }
}