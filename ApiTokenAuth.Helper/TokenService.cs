using ApiTokenAuth.Helper.Service;
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
        private static string Iss = TokenServiceConfig.ServiceName ?? Guid.NewGuid().ToString();
        private static List<string> MakeTokenParamHistory = new List<string>();
        /// <summary>
        /// 请求获取token的超时时间 
        /// 客户端发起请求获取token，如果超过了此时间才到达api,则视为请求超时
        /// 默认60s
        /// </summary>
        private static int ReqToken_OverTime = TokenServiceConfig.ReqToken_OverTime;
        /// <summary>
        /// 一个token的超时时间，默认300秒
        /// </summary>
        private static long Token_OverTime = TokenServiceConfig.Token_OverTime;
        private static string Config_PrimaryKey { get { return TokenServiceConfig.PrimaryKey; } }

        /// <summary>
        /// 有权限调用api的用户列表
        /// </summary>
        private static string[] Token_AllowAuthLists = TokenServiceConfig.Token_AllowAuthLists;
        /// <summary>
        /// Token_OverTime和auth的对应配置
        /// </summary>
        private static Dictionary<string, int> AuthMapOverTime;

        #region 初始化

        /// <summary>
        /// 服务提供端可调用此init方法，修改默认配置
        /// </summary>
        /// <param name="AllowAuthListStr">允许访问api的用户</param>
        /// <param name="logHelper">日志的实现类</param>
        /// <param name="token_OverTime">一个token的超时时间，默认300秒</param>
        /// <param name="ReqToken_OverTime">请求获取token的超时时间,客户端发起请求获取token，如果超过了此时间才到达api,则视为请求超时,默认60s</param>
        public static void Init(string AllowAuthListStr, ILogHelper logHelper, int token_OverTime = 0, int reqToken_OverTime = 60)
        {
            if (AllowAuthListStr != null)
                Token_AllowAuthLists = AllowAuthListStr.Split(',');
            if (logHelper != null)
                ToolFactory.LogHelper = logHelper;
            if (token_OverTime != 0)
                Token_OverTime = token_OverTime + 30;
            ReqToken_OverTime = reqToken_OverTime;
        }
        public static void SetAllowAuthList(string listStr)
        {
            Token_AllowAuthLists = listStr.Split(',');
        }
        public static void SetAuthMapOverTime(Dictionary<string, int> map)
        {
            AuthMapOverTime = map;
        }

        public static void SetTokenConfigPath(string path)
        {
            TokenServiceConfig.ConfigFilePath = path;
        }
        #endregion

        public static string GetPublicKey(string role)
        {
            if (!ValidTokenAuth(role))
            {
                return null;
            }
            return TokenServiceConfig.PublicKey;
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

                #region 请求历史是否有重复
                if (MakeTokenParamHistory.Contains(DesAuth))
                {
                    ToolFactory.LogHelper.Info("生成token身份验证失败:该请求的字符串与之前重复：" + DesAuth);
                    return new TokenResult() { Success = false, Error_Message = "请求数据非法" };
                }
                MakeTokenParamHistory.Insert(0, DesAuth);
                if (MakeTokenParamHistory.Count > 1000)
                    MakeTokenParamHistory.RemoveRange(1000, MakeTokenParamHistory.Count - 1000);
                #endregion

                string ReqAuthId = DesAuth.Substring(DesAuth.Length - 46, 10);//请求人身份标识
                long reqTimespan = long.Parse(DesAuth.Substring(0, DesAuth.Length - 46));  //客户端请求时间秒数

                if (!ValidTokenAuth(ReqAuthId))
                {
                    ToolFactory.LogHelper.Info("生成token身份验证失败:DesAuth" + DesAuth);
                    return new TokenResult() { Success = false, Error_Message = "身份验证失败" };
                }

                if ((TimeHelper.GetTimeSecond() - reqTimespan) > ReqToken_OverTime)
                {
                    ToolFactory.LogHelper.Info("生成token请求时间超时:DesAuth" + DesAuth);
                    return new TokenResult() { Success = false, Error_Message = "请求时间超时" };
                }
                string uname = TokenBuilder.CreateUserName(ReqAuthId);
                long TokenOverTime = Token_OverTime;
                if (AuthMapOverTime != null && AuthMapOverTime.ContainsKey(ReqAuthId))
                    TokenOverTime = AuthMapOverTime[ReqAuthId];
                string tokenStr = TokenBuilder.MakeToken(Iss, uname, ReqAuthId, TokenOverTime);
                ToolFactory.LogHelper.Notice("生成token:" + tokenStr);
                return new TokenResult() { Success = true, Token = tokenStr }; ;
            }
            catch (Exception ex)
            {
                ToolFactory.LogHelper.Error("生成token出现异常", ex);
                return new TokenResult() { Success = false, Error_Message = "错误的请求：" + ex.Message };
            }
        }

        public static ValidTokenResult ValidClientToken(HttpRequestHeaders header)
        {
            if (header.Authorization == null || header.Authorization.Parameter == null)
            {
                return new ValidTokenResult() { Success = false, Message = "not exit token" };
            }
            string tokenStr = header.Authorization.Parameter;
            return ValidClientToken(tokenStr);
        }
     
        /// <summary>
        /// 验证客户端发来的token是否有效
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        public static ValidTokenResult ValidClientToken(string tokenStr)
        {

            if (string.IsNullOrWhiteSpace(tokenStr))
            {
                return new ValidTokenResult() { Success = false, Message = "请求的token为空" };
            }
            TokenClaims tcParam = TokenBuilder.DecodeToken(tokenStr);
            if (tcParam.Iss != Iss)
            {
                ToolFactory.LogHelper.Info("token验证失败,token发行者与当前系统不匹配:iss" + tcParam.Iss);
                return new ValidTokenResult() { Success = false, Message = "用户权限验证失败,token发行者与当前系统不匹配" };
            }
            if (!ValidTokenAuth(tcParam.Role))
            {
                ToolFactory.LogHelper.Info("token验证失败,用户权限验证失败,角色没有权限调用该接口:role" + tcParam.Role);
                return new ValidTokenResult() { Success = false, Message = "用户权限验证失败,角色没有权限调用该接口" };
            }
            if (TokenIsTimeLoss(tcParam.Exp))
            {
                ToolFactory.LogHelper.Info("token验证失败,token过时,token:" + tokenStr);
                return new ValidTokenResult() { Success = false, Message = "请求的token过时" };
            }
            else
            {
                return new ValidTokenResult() { Success = true };
            }
        }

        /// <summary>
        /// 验证请求用户是否有权限调用api
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        private static bool ValidTokenAuth(string role)
        {
            return TokenConfig.Default_WebAuth == role || (Token_AllowAuthLists != null && Token_AllowAuthLists.Contains(role));
        }
        /// <summary>
        /// 判断token是否失效
        /// </summary>
        /// <returns></returns>
        private static bool TokenIsTimeLoss(long exptime)
        {
            return TimeHelper.GetTimeSecond() > exptime;
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