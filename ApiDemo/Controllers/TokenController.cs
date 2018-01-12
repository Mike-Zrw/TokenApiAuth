using ApiTokenAuth.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ApiDemo.Controllers
{
    /// <summary>
    /// token获取
    /// </summary>
    public class TokenController : ApiController
    {
        /// <summary>
        /// 获取下一个token
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public TokenResult GetToken([FromBody]string obj)
        {
            TokenResult result = TokenService.MakeToken(obj);
            return result;
        }
    }
}
