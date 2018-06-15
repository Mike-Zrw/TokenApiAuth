using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTokenAuth.Helper.Client
{
    public class TokenClientConfig
    {

        public static ILogHelper LogHelper = new LogHelper();
        /// <summary>
        /// 默认的token地址
        /// </summary>
        public static readonly string Default_Token_Url = "rest/Token/GetToken";
        /// <summary>
        /// 默认的token地址
        /// </summary>
        public static readonly string Default_PublicKey_Url = "rest/Token/GetPublicKey";


    }
}
