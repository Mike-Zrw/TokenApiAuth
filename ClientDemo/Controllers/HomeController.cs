using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
                TestApiReference t = new TestApiReference();
                string name = t.GetName();
                ViewData["Name"] = name;
            }
            catch (Exception ex)
            {
                ApiTokenAuth.Helper.ToolFactory.LogHelper.Error("报错：", ex);
                throw;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}