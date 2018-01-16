using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Timers;
using System.Web;

namespace ApiTokenAuth.Helper
{
    /// <summary>
    /// 服务端生成token及验证token的有效性
    /// 必须传入的参数可在webconfig中配置或者通过init函数来配置
    /// webconfig中的默认配置名称为：
    /// Token_AllowAuthList(允许请求api的用户标识列表)
    /// PrimaryKey(私钥)
    /// </summary>
    public class TokenService
    {
        /// <summary>
        /// 请求获取token的超时时间
        /// </summary>
        private static string PrimaryKeyPath = ConfigurationManager.AppSettings["PrimaryKey"];
        /// <summary>
        /// 请求获取token的超时时间 
        /// 客户端发起请求获取token，如果超过了此时间才到达api,则视为请求超时
        /// 默认60s
        /// </summary>
        private static int ReqToken_OverTime = 60;
        /// <summary>
        /// 一个token的超时时间，默认300秒
        /// </summary>
        private static long Token_OverTime = 300;
        private static string Config_PrimaryKey { get { return PrimaryKeyPath; } }

        /// <summary>
        /// 有权限调用api的用户列表
        /// </summary>
        private static string[] Token_AllowAuthLists;
        /// <summary>
        /// Token_OverTime和auth的对应配置
        /// </summary>
        private static Dictionary<string, int> AuthMapOverTime;
        static TokenService()
        {
            if (ConfigurationManager.AppSettings["Token_AllowAuthList"] != null)
            {
                string Token_AllowAuthList = ConfigurationManager.AppSettings["Token_AllowAuthList"].ToString();
                Token_AllowAuthLists = Token_AllowAuthList.Split(',');
            }
            if (ConfigurationManager.AppSettings["Token_OverTime"] != null)
            {
                Token_OverTime = Convert.ToInt32(ConfigurationManager.AppSettings["Token_OverTime"]);
            }
            if (ConfigurationManager.AppSettings["ReqToken_OverTime"] != null)
            {
                ReqToken_OverTime = Convert.ToInt32(ConfigurationManager.AppSettings["ReqToken_OverTime"]);
            }
            if (ConfigurationManager.AppSettings["AuthMapOverTime"] != null) //{"auth1":1000}
            {
                string jsonMap = ConfigurationManager.AppSettings["AuthMapOverTime"];
                AuthMapOverTime = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonMap);
            }
            Timer tm = new Timer();
            tm.Interval = 60000;
            tm.Elapsed += Tm_ClearUnUseTokenCache;
            tm.Start();
        }
        /// <summary>
        /// 清除无用的cache
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Tm_ClearUnUseTokenCache(object sender, ElapsedEventArgs e)
        {
            try
            {
                Dictionary<string, string> caches = ToolFactory.CacheHelper.GetAllCache("ServiceTokenCacheKey_");
                int allcount = caches.Count;
                int removeSum = 0;
                if (allcount != 0)
                {
                    foreach (string key in caches.Keys)
                    {
                        string value = caches[key];
                        TokenClaims tcCache = TokenFactory.ParseTokenClaims(value);
                        if (TokenIsTimeLoss(tcCache.Exp))
                        {
                            ToolFactory.CacheHelper.RemoveCache(key);
                            removeSum++;
                        }
                    }
                }
                ToolFactory.LogHelper.Notice(string.Format("服务器定时清空无用缓存,本次共检索缓存{0}个,清除{1}个", allcount, removeSum));
            }
            catch (Exception ex)
            {
                ToolFactory.LogHelper.Error("时清空无用缓存报错", ex);
            }
        }

        /// <summary>
        /// 服务提供端可调用此init方法，修改默认配置
        /// </summary>
        /// <param name="AllowAuthLists">允许访问api的用户</param>
        /// <param name="cacheHelper">设置缓存的实现</param>
        /// <param name="logHelper">日志的实现类</param>
        /// <param name="token_OverTime">一个token的超时时间，默认300秒</param>
        /// <param name="ReqToken_OverTime">请求获取token的超时时间,客户端发起请求获取token，如果超过了此时间才到达api,则视为请求超时,默认60s</param>
        public static void Init(string[] AllowAuthLists, ICacheHelper cacheHelper, ILogHelper logHelper, int token_OverTime = 0, int reqToken_OverTime = 60)
        {
            if (AllowAuthLists != null)
                Token_AllowAuthLists = AllowAuthLists;
            if (cacheHelper != null)
                ToolFactory.CacheHelper = cacheHelper;
            if (logHelper != null)
                ToolFactory.LogHelper = logHelper;
            if (token_OverTime != 0)
                Token_OverTime = token_OverTime;
            ReqToken_OverTime = reqToken_OverTime;
        }

        /// <summary>
        /// 为请求用户生成token
        /// </summary>
        /// <param name="RequestParam">action的参数</param>
        /// <returns></returns>
        public static TokenResult MakeToken(string RequestParam, string PrimaryKey = null)
        {
            try
            {
                dynamic p = JsonConvert.DeserializeObject(RequestParam);
                string RequestAuth = p.RequestAuth;//请求人信息
                string DesAuth;//解密后的author
                if (PrimaryKey == null)
                    DesAuth = RSAHelper.Decrypt(RequestAuth, Config_PrimaryKey);
                else
                    DesAuth = RSAHelper.Decrypt(RequestAuth, PrimaryKey);
                string ReqAuthId = DesAuth.Substring(DesAuth.Length - 13, 10);//请求人身份标识
                long reqTimespan = long.Parse(DesAuth.Substring(0, DesAuth.Length - 13));  //客户端请求时间秒数

                if (!ValidTokenAuth(ReqAuthId))
                    return new TokenResult() { Success = false, Error_Message = "身份验证失败" };

                if ((TimeHelper.GetTimeSecond() - reqTimespan) > ReqToken_OverTime)
                    return new TokenResult() { Success = false, Error_Message = "请求时间超时" };
                string uname = TokenFactory.CreateUserName(ReqAuthId);
                long TokenOverTime = Token_OverTime;
                if (AuthMapOverTime != null && AuthMapOverTime.ContainsKey(ReqAuthId))
                    TokenOverTime = AuthMapOverTime[ReqAuthId];
                string tokenStr = TokenFactory.CreateTokenStr("jwt", ReqAuthId, uname, TokenOverTime);
                ToolFactory.LogHelper.Notice("生成token:" + tokenStr);
                ToolFactory.CacheHelper.SetCache("ServiceTokenCacheKey_" + uname, tokenStr, TimeSpan.FromSeconds(TokenOverTime + 600)); //多存600秒
                return new TokenResult() { Success = true, Token = AesHelper.Encrypt(tokenStr) }; ;
            }
            catch (Exception ex)
            {
                ToolFactory.LogHelper.Error("生成token出现异常", ex);
                return new TokenResult() { Success = false, Error_Message = "错误的请求：" + ex.Message };
            }
        }

        /// <summary>
        /// 验证客户端发来的token是否有效
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static ValidTokenResult ValidClientToken(HttpRequestHeaders header)
        {
            if (header.Authorization == null || header.Authorization.Parameter == null)
            {
                return new ValidTokenResult() { Success = false, Message = "not exit token" };
            }
            string tokenStr = header.Authorization.Parameter;
            //ToolFactory.LogHelper.Notice("接收到带token的请求:" + tokenStr);
            TokenClaims tcParam = TokenFactory.ParseTokenClaims(AesHelper.Decrypt(tokenStr));
            TokenClaims tcCache = TokenFactory.ParseTokenClaims(ToolFactory.CacheHelper.GetCache<string>(tcParam.Usr));
            if (tcCache != null)
            {
                if (TokenIsTimeLoss(tcCache.Exp))
                {
                    return new ValidTokenResult() { Success = false, Message = "token过时" };
                }
                else if (tcCache.SingleStr != tcParam.SingleStr)
                {
                    return new ValidTokenResult() { Success = false, Message = "token不正确" };
                }
                else
                {
                    return new ValidTokenResult() { Success = true };
                }
            }
            else
            {
                return new ValidTokenResult() { Success = false, Message = "未授权的用户" };
            }
        }

        /// <summary>
        /// 验证请求用户是否有权限调用api
        /// </summary>
        /// <param name="Auth"></param>
        /// <returns></returns>
        private static bool ValidTokenAuth(string Auth)
        {
            return Token_AllowAuthLists.Contains(Auth);
        }
        /// <summary>
        /// 判断token是否失效,可以延长30秒来防止边界的错误
        /// </summary>
        /// <returns></returns>
        private static bool TokenIsTimeLoss(long exptime)
        {
            return TimeHelper.GetTimeSecond() > exptime + 30;
        }
    }

    /// <summary>
    /// token验证结果
    /// </summary>
    public class ValidTokenResult
    {
        /// <summary>
        /// 是否验证通过
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }
    }

}