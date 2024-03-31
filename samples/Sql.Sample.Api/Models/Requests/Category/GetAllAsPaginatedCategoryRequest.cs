using Microsoft.AspNetCore.Mvc;

namespace Sql.Sample.Api.Models.Requests.Category
{
    public class GetAllAsPaginatedCategoryRequest
    {
        [FromQuery]
        public bool IncludeProducts { get; set; }

        [FromQuery]
        public int PageIndex { get; set; }

        [FromQuery]
        public int ItemsPerPage { get; set; }
    }
}