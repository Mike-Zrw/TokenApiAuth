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
        public static void Init(ILogHelper logHelper)
        {
            if (logHelper != null)
                ToolFactory.LogHelper = logHelper;
        }
       

    }
}
