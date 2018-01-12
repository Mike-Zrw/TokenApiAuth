using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{
    public class TimeHelper
    {
        /// <summary>
        /// 获取指定时间时间距离1970年的秒数
        /// </summary>
        /// <param name="time">默认为当前时间</param>
        /// <returns></returns>
        public static long GetTimeSecond(Nullable<DateTime> time = null)
        {
            if (time == null)
                return (DateTime.Now.Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000000;
            return (((DateTime)time).Ticks - DateTime.Parse("1970-01-01 00:00:00").Ticks) / 10000000;
        }
    }
}
