using ApiDemo.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;

namespace ApiDemo.Controllers
{
    [ValidateAttribute]
    public class TestController : ApiController
    {
        [HttpPost]
        public string GetName()
        {
            return "mike";
        }
    }
}
