using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogHelper : ILogHelper
    {
        public static string TokenLogLevel = "error";
        public void Error(string message, Exception ex = null)
        {
            try
            {
                Task.Run(() =>
                {
                    WriteLog(GetExceptionMessage(message, ex), "error");
                });
            }
            catch (Exception)
            {
            }
        }
        public void Info(string message, Exception ex = null)
        {
            try
            {
                Task.Run(() =>
                {
                    WriteLog(GetExceptionMessage(message, ex), "info");
                });
            }
            catch (Exception)
            {
            }
        }

        public void Notice(string message)
        {
            try
            {
                Task.Run(() =>
                {
                    WriteLog(GetExceptionMessage(message), "notice");
                });
            }
            catch (Exception)
            {
            }
        }
        private static string GetExceptionMessage(string message, Exception ex = null)
        {
            StringBuilder sb = new StringBuilder(DateTime.Now.ToString("【yy-MM-dd HH:mm:ss】\t") + message);
            if (ex != null)
            {
                Exception ex2 = ex;
                sb.Append("\n" + ex2.Message);
                sb.Append("\n" + ex2.StackTrace);
                while (ex2.InnerException != null)
                {
                    ex2 = ex2.InnerException;
                    sb.Append("\n" + ex2.Message);
                }
            }
            return sb.ToString();
        }
        private static void WriteLog(string message, string filename)
        {
            if (filename == "notice" && (TokenLogLevel == null || !TokenLogLevel.ToLower().Contains("notice")))
                return;
            if (filename == "info" && (TokenLogLevel == null || !TokenLogLevel.ToLower().Contains("info")))
                return;
            string path = AppDomain.CurrentDomain.BaseDirectory + "/tokenlog/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filePath = path + filename + DateTime.Now.ToString("yyMMdd") + ".txt";
            if (File.Exists(filePath))
            {
                FileInfo info = new FileInfo(filePath);
                if (info.Length > 0xa00000L)
                {
                    int num = 1;
                    filePath = path + filename + DateTime.Now.ToString("yyMMdd") + "_" + num.ToString() + ".txt";
                    while (File.Exists(filePath))
                    {
                        info = new FileInfo(filePath);
                        if (info.Length <= 0xa00000L)
                        {
                            break;
                        }
                        num++;
                        filePath = path + filename + DateTime.Now.ToString("yyMMdd") + "_" + num.ToString() + ".txt";
                    }
                }
            }
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }

    }
}
