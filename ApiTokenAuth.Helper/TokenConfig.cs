using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{
    public class TokenConfig
    {
        /// <summary>
        /// 系统默认的用户标识
        /// </summary>
        public static readonly string Default_WebAuth = "DEFAULT001";
        /// <summary>
        /// 默认的token地址
        /// </summary>
        public static readonly string Default_Token_Url = "rest/Token/GetToken";
        /// <summary>
        ///请求获取token的超时时间
        /// 客户端发起请求获取token，如果超过了此时间才到达api,则视为请求超时
        /// </summary>
        public static readonly int ReqToken_OverTime = 60;
        /// <summary>
        /// 一个token的超时时间
        /// </summary>
        public static readonly long Token_OverTime = 300;
      
    }
}
