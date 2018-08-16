using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ApiTokenAuth.Helper.Service
{
    internal class TokenServiceConfig
    {
        /// <summary>
        ///请求获取token的超时时间
        /// 客户端发起请求获取token，如果超过了此时间才到达api,则视为请求超时
        /// </summary>
        public static int ReqToken_OverTime { get; private set; }
        /// <summary>
        /// 一个token的超时时间
        /// </summary>
        public static long Token_OverTime { get; private set; }

        public static string PrimaryKey { get; private set; }
        public static string PublicKey { get; private set; }
        /// <summary>
        /// config文件目录
        /// </summary>
        public static string ConfigFilePath { get; set; }
        public static string[] Token_AllowAuthLists { get; private set; }
        /// <summary>
        /// 本服务的名称
        /// </summary>
        public static string ServiceName { get; private set; }

        static TokenServiceConfig()
        {
            if (ConfigFilePath == null)
            {
                ConfigFilePath = AppDomain.CurrentDomain.BaseDirectory + "TokenAuth.json";
            }
            if (File.Exists(ConfigFilePath))
            {
                string text = File.ReadAllText(ConfigFilePath);
                byte[] mybyte = Encoding.UTF8.GetBytes(text);
                string configStr = Encoding.UTF8.GetString(mybyte);
                try
                {
                    ConfigModel config = JsonConvert.DeserializeObject<ConfigModel>(configStr);
                    if (config.ReqToken_OverTime != 0)
                        ReqToken_OverTime = config.ReqToken_OverTime;
                    else
                        ReqToken_OverTime = 60;
                    if (config.Token_OverTime != 0)
                        Token_OverTime = config.Token_OverTime;
                    else
                        Token_OverTime = 300;
                    PrimaryKey = config.PrimaryKey;
                    PublicKey = config.PublicKey;
                    Token_AllowAuthLists = config.Token_AllowAuthList;
                }
                catch (Exception ex)
                {
                    ToolFactory.LogHelper.Error("Token解析配置文件出错", ex);
                    throw ex;
                }
            }
            else
            {
                ToolFactory.LogHelper.Error("Token配置文件目录不存在");
                throw new Exception("Token配置文件目录不存在");
            }
        }
        private class ConfigModel
        {
            public string[] Token_AllowAuthList { get; set; }
            public int ReqToken_OverTime { get; set; }
            public long Token_OverTime { get; set; }
            public string PrimaryKey { get; set; }
            public string PublicKey { get; set; }
        }
    }
}
