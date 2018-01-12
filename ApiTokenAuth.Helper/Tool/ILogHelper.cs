using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiTokenAuth.Helper
{
    public interface ILogHelper
    {
        void Error(string message, Exception ex = null);
        void Info(string message, Exception ex = null);
        void Notice(string message);
    }
}
