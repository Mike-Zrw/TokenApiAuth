using ApiTokenAuth.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ApiDemo.Attributes
{
    /// <summary>
    /// 验证授权
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ValidateAttribute : ActionFilterAttribute
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            ValidTokenResult result;
            try
            {
                result = TokenService.ValidClientToken(actionContext.Request.Headers);
            }
            catch (Exception e)
            {
                result = new ValidTokenResult() { Success = false, Message = e.Message };
                log.Error("验证授权发生异常", e);
            }
            if (result.Success)
                base.OnActionExecuting(actionContext);
            else
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }
    }
}