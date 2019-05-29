using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RandomGenerator.Controllers
{
    [Route("api/[controller]")]
    public class Numbers : Controller
    {
        private readonly Random _random = new Random();

        // GET: api/values
        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult(new Dictionary<string, int> { { "number", _random.Next() } });
        }
    }
}
