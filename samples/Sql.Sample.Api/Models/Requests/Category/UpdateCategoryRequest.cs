﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Sql.Sample.Api.Models.Requests.Category
{
    public class UpdateCategoryRequest
    {
        [FromRoute(Name = "id"), Range(1, int.MaxValue)]
        public int Id { get; set; }

        [FromQuery] 
        public bool FetchFromDb { get; set; }

        [FromBody]
        public UpdateCategoryRequestBody Body { get; set; }
    }

    public class UpdateCategoryRequestBody
    {
        public string Name { get; set; }
    }
}