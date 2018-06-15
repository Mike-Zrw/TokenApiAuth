using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper.Tool
{
    public class FileHelper
    {
        public static string GetFileText(string fileName, string path = null)
        {
            if (path == null)
            {
                path = AppDomain.CurrentDomain.BaseDirectory;
            }
            try
            {
                if (File.Exists(path))
                {
                    string text = File.ReadAllText(path);
                    byte[] mybyte = Encoding.UTF8.GetBytes(text);
                    return Encoding.UTF8.GetString(mybyte);
                }
                else
                {
                    throw new Exception("文件" + path + "不存在");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
