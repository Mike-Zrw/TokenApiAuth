using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{

    /// <summary>
    /// 获取token时的api返回结果
    /// </summary>
    public class TokenResult
    {
        public bool Success { get; set; }
        public string Error_Message { get; set; }

        public string Token { get; set; }
    }
}
