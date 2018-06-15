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

        public static string JwtKey { get { return "abs123"; } }
    }
}
