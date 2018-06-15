using ApiTokenAuth.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace ClientDemo
{
    public class TestApiReference : BaseApiReference
    {

        private static string _ApiUrl;
        public override string ApiUrl { get { if (_ApiUrl == null) { _ApiUrl = ConfigurationManager.AppSettings["ApiUri"]; } return _ApiUrl; } }

        public string GetName()
        {
            using (HttpResponseMessage response = HttpClient.PostAsJsonAsync("rest/Test/GetName",1 ).Result)
            {
               
                if (response.IsSuccessStatusCode)
                {
                    return response.Content.ReadAsAsync<string>().Result;
                }
            }
            return null;
        }
    }
}