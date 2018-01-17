using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{

    public class TokenBuilder
    {
        /// <summary>
        /// 创建一个token字符串,结构为tokenclaims的加密形式
        /// </summary>
        /// <param name="Iss">token发行者</param>
        /// <param name="role">用户权限</param>
        /// <param name="usr">用户名</param>
        /// <param name="overTime">token超时时间段 秒</param>
        /// <returns></returns>
        public static string CreateTokenStr(string Iss, string role, string usr, long overTime)
        {
            long time = TimeHelper.GetTimeSecond();
            string str = Iss + "." + role + "." + usr + "." + time + "." + (time + overTime) + "." + Guid.NewGuid().ToString();
            return str;
        }
        private static int _TokenIndex = 1;
        private static int TokenIndex
        {
            get
            {
                _TokenIndex++; if (_TokenIndex <= 999999)
                {
                    return _TokenIndex;
                }
                else
                {
                    _TokenIndex = 1;
                    return _TokenIndex;
                }
            }
        }
        /// <summary>
        /// 根据权限生成一个用户名称
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static string CreateUserName(string role)
        {
            string uname = role + DateTime.Now.ToString("MMddHHmm")+ TokenIndex;
            return uname;
        }

        /// <summary>
        /// 将token字符串转为token结构
        /// </summary>
        /// <param name="TokenStr"></param>
        /// <returns></returns>
        public static TokenClaims ParseTokenClaims(string TokenStr)
        {
            string[] data = TokenStr.Split('.');
            TokenClaims result = new TokenClaims()
            {
                Iss = data[0],
                Role = data[1],
                Usr = data[2],
                Iat = long.Parse(data[3]),
                Exp = long.Parse(data[4]),
                SingleStr = data[5],
            };
            return result;
        }
    }
}
