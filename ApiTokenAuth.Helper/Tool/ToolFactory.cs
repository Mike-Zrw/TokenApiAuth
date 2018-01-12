using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{
    public class ToolFactory
    {
        public static ICacheHelper CacheHelper = new CacheHelper();

        public static ILogHelper LogHelper = new LogHelper();
    }
}
