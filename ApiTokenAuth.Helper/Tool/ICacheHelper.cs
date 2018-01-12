using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{
    public interface ICacheHelper
    {
        /// <summary>
        /// 设置当前应用程序指定cacheKey的Cache值
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <param name="objObject"></param>
        /// <param name="time"></param>
        void SetCache(string cacheKey, object objObject, TimeSpan time);
        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="cacheKey"></param>
        /// <returns></returns>
        T GetCache<T>(string cacheKey);
        /// <summary>
        /// 获取所有缓存
        /// </summary>
        /// <param name="likeKey"></param>
        /// <returns></returns>
        Dictionary<string, string> GetAllCache(string likeKey);
        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool RemoveCache(string key);
        bool ClearCache();

    }
}
