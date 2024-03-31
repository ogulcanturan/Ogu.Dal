using Microsoft.AspNetCore.Mvc;
using Sql.Sample.Api.Services.Interfaces;

namespace Sql.Sample.Api.Controllers
{
    [ApiController]
    [Route("api/common")]
    public class CommonController : ControllerBase
    {
        private readonly ICommonService _commonService;
        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpGet]
        public IActionResult Index() => Ok();
    }
}