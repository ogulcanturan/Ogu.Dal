using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MongoDb.Sample.Api.Models.Requests.Category
{
    public class UpdateCategoryRequest
    {
        [FromRoute(Name = "id"), Length(24, 24)]
        public string Id { get; set; }

        [FromBody]
        public UpdateCategoryRequestBody Body { get; set; }
    }

    public class UpdateCategoryRequestBody
    {
        public string Name { get; set; }
    }
}