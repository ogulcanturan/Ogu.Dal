using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MongoDb.Sample.Api.Models.Requests.Category
{
    public class RemoveRequest
    {
        [FromRoute(Name = "id"), Length(24, 24)]
        public string Id { get; set; }
    }
}