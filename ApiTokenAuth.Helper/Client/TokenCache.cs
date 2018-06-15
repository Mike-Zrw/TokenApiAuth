using System;
using System.Collections.Generic;
using System.Text;

namespace ApiTokenAuth.Helper.Client
{
    public class TokenCache
    {
        private static Dictionary<string, TokenCacheModel> DicCache = new Dictionary<string, TokenCacheModel>();
        private static object lockobj = new object();
        public static TokenClaims SetTokenCache(string key, string tokenStr)
        {
            lock (lockobj)
            {
                TokenClaims claim = TokenBuilder.DecodeToken(tokenStr);
                if (DicCache.ContainsKey(key))
                {
                    DicCache.Remove(key);
                    DicCache.Add(key, new TokenCacheModel() { TokenClaim = claim, TokenStr = tokenStr });
                }
                else
                {
                    DicCache.Add(key, new TokenCacheModel() { TokenClaim = claim, TokenStr = tokenStr });
                }
                return claim;
            }
        }

        public static TokenCacheModel GetCacheToken(string key)
        {
            if (DicCache.ContainsKey(key))
                return DicCache[key];
            return null;
        }
    }
    public class TokenCacheModel
    {
        public string TokenStr { get; set; }
        public TokenClaims TokenClaim { get; set; }
    }
}
