using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{
    /// <summary>
    /// 一个token所包含的数据
    /// </summary>
    public class TokenClaims
    {
        /// <summary>
        /// token的发行者
        /// </summary>
        public string Iss { get; set; }
        /// <summary>
        /// 用户权限
        /// </summary>
        public string Role { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Usr { get; set; }
        /// <summary>
        /// 签发时间 秒,时间点
        /// </summary>
        public long Iat { get; set; }
        /// <summary>
        /// 到期时间 秒,时间点
        /// </summary>
        public long Exp { get; set; }
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string SingleStr { get; set; }
    }

}
