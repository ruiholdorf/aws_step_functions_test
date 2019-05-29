using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TestIfOddOrEven.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IsEvenController : ControllerBase
    {
        [HttpGet]
        public JsonResult Get([FromQuery] int value)
        {
            return new JsonResult(new Dictionary<string, bool> { { "isEven", value % 2 == 0 } });
        }
    }
}
