using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MongoDb.Sample.Api.Models.Requests.Category
{
    public class UpdateCategoriesRequest
    {
        [FromBody]
        public UpdateCategoriesRequestBody Body { get; set; }
    }

    public class UpdateCategoriesRequestBody
    {
        [Required, Length(1, int.MaxValue)]
        public IEnumerable<Guid> Ids { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Name { get; set; }
    }
}