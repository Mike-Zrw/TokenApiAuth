using ApiTokenAuth.Helper.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;


namespace ApiTokenAuth.Helper
{
    /// <summary>
    /// 带token的api基类
    /// 用于客户端调用服务的过程中自动获取token
    /// </summary>
    public abstract class BaseApiReference
    {
        private HttpClient HttpClientSingle;

        private static string _webAuth;
        /// <summary>
        /// 本站用户标识,默认取webconfig 中的Token_WebAuth节点
        /// </summary>
        public virtual string Token_WebAuth
        {
            get
            {
                if (_webAuth == null)
                {
                    _webAuth = TokenConfig.Default_WebAuth;
                }
                return _webAuth;
            }
        }
        /// <summary>
        /// 获取token的地址
        /// </summary>
        public virtual string TokenUrl { get { return TokenClientConfig.Default_Token_Url; } }
        /// <summary>
        /// 获取tpubliKey的地址
        /// </summary>
        public virtual string PublicKeyUrl { get { return TokenClientConfig.Default_PublicKey_Url; } }
        /// <summary>
        /// 接口地址
        /// </summary>
        public abstract string ApiUrl { get; }
        /// <summary>
        /// 缓存的token key名字，每个连接不可重复
        /// </summary>
        private string CacheName { get { return "token_" + ApiUrl; } }

        private static object _syncToken = new object();
        private static object _syncRoot = new object();
        /// <summary>
        /// 对于同一个连接保存一个httpclient示例，过多会有问题
        /// </summary>
        private static Dictionary<string, HttpClient> HttpClients = new Dictionary<string, HttpClient>();
        private static Dictionary<string, string> PublicKeys = new Dictionary<string, string>();
        /// <summary>
        /// 每个地址保持一个静态的HttpClient,每次实例化会有并发问题
        /// </summary>
        public HttpClient HttpClient
        {
            get
            {
                if (!HttpClients.ContainsKey(ApiUrl))
                {
                    lock (_syncRoot)
                    {
                        if (!HttpClients.ContainsKey(ApiUrl))
                        {
                            HttpClientSingle = CreateHttpClient(ApiUrl);
                            HttpClients.Add(ApiUrl, HttpClientSingle);
                            FillTokenToReqHeader();
                        }
                    }
                }
                HttpClientSingle = HttpClients[ApiUrl];
                string token = CheckCacheToken();
                if (token == null)
                    FillTokenToReqHeader(); ;
                return HttpClientSingle;
            }
        }
        /// <summary>
        /// 生成一个httpclient
        /// </summary>
        /// <param name="apiurl"></param>
        /// <returns></returns>
        private static HttpClient CreateHttpClient(string apiurl)
        {
            HttpClientHandler _httpClientHandler = new HttpClientHandler();
            _httpClientHandler.UseCookies = true;
            HttpClient _httpClient = new HttpClient(_httpClientHandler);
            _httpClient.BaseAddress = new Uri(apiurl);
            _httpClient.Timeout = TimeSpan.FromMinutes(30);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return _httpClient;
        }

        /// <summary>
        /// 核对缓存中是否存在token以及token的有效性
        /// </summary>
        /// <param name="cachename">缓存名称</param>
        private string CheckCacheToken()
        {
            TokenCacheModel token = TokenCache.GetCacheToken(CacheName);
            if (token != null)
            {
                if (TimeHelper.GetTimeSecond() >= (token.TokenClaim.Exp - 60))
                    return null;
                else
                    return token.TokenStr;
            }
            return null;
        }


        /// <summary>
        /// 获取token并填充到httpheader中
        /// </summary>
        /// <returns></returns>
        private void FillTokenToReqHeader()
        {
            lock (_syncToken)
            {
                string tokencache = CheckCacheToken();
                if (tokencache == null)
                {
                    string tokenStr = MakeToken();
                    HttpClientSingle.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("auth", tokenStr);
                    TokenClaims claim = TokenCache.SetTokenCache(CacheName, tokenStr);
                }
            }
        }


        #region 获取token
        /// <summary>
        /// 调用api获取新的token
        /// </summary>
        /// <returns></returns>
        private string MakeToken()
        {
            try
            {
                if (!PublicKeys.ContainsKey(ApiUrl))
                {
                    try
                    {
                        string publickey = Posts(PublicKeyUrl, Token_WebAuth);
                        PublicKeys.Add(ApiUrl, publickey);
                    }
                    catch (Exception ex)
                    {
                        TokenClientConfig.LogHelper.Error(ApiUrl + TokenUrl + ":获取publickey调用失败", ex);
                    }
                }
                string tokenResult = Posts(TokenUrl, TokenClient.GetRequestParam(Token_WebAuth, PublicKeys[ApiUrl]));
                TokenClientConfig.LogHelper.Notice("重新获取token:" + tokenResult + " 当前httpclient数量：" + HttpClients.Count());
                TokenResult getTokenResult = JsonConvert.DeserializeObject<TokenResult>(tokenResult);
                if (getTokenResult.Success)
                {
                    string tokenStr = getTokenResult.Token;
                    return tokenStr;
                }
                else
                {
                    throw new Exception(TokenUrl + "获取token，身份验证失败，未能获取到token,可能由于密钥不匹配或者服务端与客户端的时间不匹配");
                }
            }
            catch (Exception ex)
            {
                TokenClientConfig.LogHelper.Error(ApiUrl + TokenUrl + ":获取tokenApi调用失败", ex);
                throw ex;
            }
        }

        private string Posts(string url, string param)
        {
            string strURL = ApiUrl + url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.Timeout = 30 * 1000;
            request.ContentType = "application/x-www-form-urlencoded";
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes("=" + System.Web.HttpUtility.UrlEncode(param));
            request.ContentLength = bytes.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);
            newStream.Close();
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string phpend = System.Web.HttpUtility.UrlDecode(php.ReadToEnd());
            return phpend;
        }
        #endregion
    }
}
