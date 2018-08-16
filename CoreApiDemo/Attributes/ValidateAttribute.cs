using ApiTokenAuth.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace ApiDemo.Attributes
{
    /// <summary>
    /// 验证授权
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ValidateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            ValidTokenResult result;
            try
            {
                result = TokenService.ValidClientToken(actionContext.HttpContext.Request.Headers["Authorization"].ToString().Replace("auth ", ""));
            }
            catch (Exception e)
            {
                result = new ValidTokenResult() { Success = false, Message = e.Message };
                ToolFactory.LogHelper.Error("验证授权发生异常", e);
            }
            if (result.Success)
                base.OnActionExecuting(actionContext);
            else
                actionContext.Result = new UnauthorizedResult();
        }
    }
}