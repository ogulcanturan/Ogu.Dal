using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Sql.Sample.Api.Models.Requests.Category
{
    public class RemoveRequest
    {
        [FromRoute, Range(1, int.MaxValue)]
        public int Id { get; set; }

        [FromQuery]
        public bool FetchFromDb { get; set; }
    }
}