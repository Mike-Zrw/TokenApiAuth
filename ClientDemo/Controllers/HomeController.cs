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
            Dictionary<string, int> aaa = new Dictionary<string, int>();
            aaa.Add("123", 111);
            string a = JsonConvert.SerializeObject(aaa);
            TestApiReference t = new TestApiReference();
            string name=t.GetName();
            ViewData["Name"] = name;
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