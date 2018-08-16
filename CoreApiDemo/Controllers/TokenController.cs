using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ApiTokenAuth.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreApiDemo.Controllers
{
    [Route("rest/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        /// <summary>
        /// 获取下一个token
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost("GetToken")]
        public TokenResult GetToken([FromForm]string obj)
        {
            TokenResult result = TokenService.MakeToken(obj);
            return result;
        }
        [HttpPost("GetPublicKey")]
        public string GetPublicKey([FromForm]string obj)
        {
            string result = TokenService.GetPublicKey(obj);
            return result;

        }
    }
}