using Microsoft.AspNetCore.Mvc;

namespace MongoDb.Sample.Api.Models.Requests.Category
{
    public class GetAllCategoriesRequest
    {
        [FromQuery]
        public string CategoryName { get; set; }

        [FromQuery]
        public bool IncludeProducts { get; set; }
    }
}