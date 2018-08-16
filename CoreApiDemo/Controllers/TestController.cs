using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiDemo.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApiDemo.Controllers
{
    [Route("rest/[controller]")]
    [ApiController]
    [ValidateAttribute]
    public class TestController : ControllerBase
    {
        [HttpPost("GetName")]
        public string GetName()
        {
            return "mikecore";
        }
    }
}
