using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{

    public class TokenBuilder
    {
  
        public static string CreateTokenStr(string Iss, string role, string usr, long overTime)
        {
            long time = TimeHelper.GetTimeSecond();
            string str = Iss + "." + role + "." + usr + "." + time + "." + (time + overTime) + "." + Guid.NewGuid().ToString();
            return str;
        }

        /// <summary>
        /// 创建一个token字符串,结构为tokenclaims的加密形式
        /// </summary>
        /// <param name="usr">用户名</param>
        /// <param name="role">用户权限</param>
        /// <param name="overTime">token超时时间段 秒</param>
        /// <returns></returns>
        public static string MakeToken(string usr, string role, long overTime)
        {
            TokenClaims Claim = GetTokenClaims(usr, role, overTime);
            var token = EncodeToken(Claim);
            return token;
        }
        /// <summary>
        /// 生成一个token结构
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="sysid"></param>
        /// <returns></returns>
        private static TokenClaims GetTokenClaims(string usr, string role, long overTime)
        {
            long time = TimeHelper.GetTimeSecond();
            return new TokenClaims()
            {
                Usr = usr,
                Iat = time,
                Iss = "apicenter",
                Role = role,
                SingleStr = Guid.NewGuid().ToString(),
                Exp = time + overTime
            };
        }

        /// <summary>
        /// 加密token结构为
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private static string EncodeToken(TokenClaims token)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var tokenStr = encoder.Encode(token, TokenConfig.JwtKey);
            return tokenStr;
        }

        /// <summary>
        /// 解密token为token结构
        /// </summary>
        /// <param name="encodetokenStr"></param>
        /// <returns></returns>
        public static TokenClaims DecodeToken(string encodetokenStr)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);
                var json = decoder.Decode(encodetokenStr, TokenConfig.JwtKey, verify: true);//token为之前生成的字符串
                TokenClaims claim = serializer.Deserialize<TokenClaims>(json);
                return claim;
            }
            catch (Exception ex)
            {
                ToolFactory.LogHelper.Error("解密token发生异常", ex);
                throw ex;
            }
          
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
            string uname = role + DateTime.Now.ToString("MMddHHmm") + TokenIndex;
            return uname;
        }
        
    }
}
