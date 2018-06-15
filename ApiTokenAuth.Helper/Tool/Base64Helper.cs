using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{
    public class Base64Helper
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string EncodeBase64(string code)
        {
            string encode = "";
            byte[] bytes = Encoding.UTF8.GetBytes(code); //将一组字符编码为一个字节序列. 
            try
            {
                encode = Convert.ToBase64String(bytes); //将8位无符号整数数组的子集转换为其等效的,以64为基的数字编码的字符串形式. 
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DecodeBase64(string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code); //将2进制编码转换为8位无符号整数数组. 
            try
            {
                decode = Encoding.UTF8.GetString(bytes); //将指定字节数组中的一个字节序列解码为一个字符串。 
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
    }
}
