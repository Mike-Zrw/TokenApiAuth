using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ApiTokenAuth.Helper
{


    /// <summary>
    /// token的cache操作,默认以 System.Web.Caching.Cache实现
    /// 若改变实现方式，需要重写SetCacheImpl和GetCacheImpl委托
    /// </summary>
    public class CacheHelper : ICacheHelper
    {
        /// <summary>
        /// 设置当前应用程序指定cacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="objObject"></param>
        /// <param name="time"></param>
        public void SetCache(string cacheKey, object objObject, TimeSpan time)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(cacheKey, objObject, null, DateTime.Now.Add(time), System.Web.Caching.Cache.NoSlidingExpiration);

        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        public T GetCache<T>(string cacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            object result = objCache[cacheKey];
            if (result == null)
                return default(T);
            return (T)result;
        }
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="likeKey"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetAllCache(string likeKey)
        {
            Dictionary<string, string> reuslt = new Dictionary<string, string>();
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            IDictionaryEnumerator CacheEnum = objCache.GetEnumerator();
            while (CacheEnum.MoveNext())
            {
                string key = CacheEnum.Key.ToString();
                if (key.Contains(likeKey))
                {
                    reuslt.Add(key, CacheEnum.Value.ToString());
                }
            }
            return reuslt;
        }

        public bool RemoveCache(string cacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Remove(cacheKey);
            return true;
        }

        public bool ClearCache()
        {
            IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());//逐一清除缓存。

            }
            return true;
        }
    }
}
