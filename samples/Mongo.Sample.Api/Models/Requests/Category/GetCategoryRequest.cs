using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace MongoDb.Sample.Api.Models.Requests.Category
{
    public class GetCategoryRequest
    {
        [FromRoute(Name = "id"), Length(24,24)]
        public string Id { get; set; }

        [FromQuery]
        public bool IncludeProducts { get; set; }
    }
}