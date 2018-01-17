using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{
    /// <summary>
    /// 客户端请求
    /// </summary>
    public class TokenClient
    {
        /// <summary>
        /// 客户端可调用此init方法，修改默认配置
        /// </summary>
        /// <param name="cacheHelper">设置缓存的实现</param>
        /// <param name="logHelper">日志的实现类</param>
        public static void Init(ICacheHelper cacheHelper, ILogHelper logHelper)
        {
            if (cacheHelper != null)
                ToolFactory.CacheHelper = cacheHelper;
            if (logHelper != null)
                ToolFactory.LogHelper = logHelper;
        }
        /// <summary>
        /// 获取请求token需要传递的参数(时间戳+请求身份标识10位+guid)
        /// </summary>
        /// <param name="auth">用户身份标识</param>
        /// <param name="PublicKey">密钥，若不传入，需要在WebConfig中配置</param>
        /// <returns></returns>
        public static string GetRequestParam(string auth, string PublicKey)
        {
            string rdStr = Guid.NewGuid().ToString();//new Random().Next(100, 999).ToString();//
            if (PublicKey == null)
            {
                throw new Exception("没有配置publickey");
            }
            else
            {
                string encData = RSAHelper.Encrypt(TimeHelper.GetTimeSecond() + auth + rdStr, PublicKey);
                return JsonConvert.SerializeObject(new { RequestAuth = encData });
            }
        }

    }
}
