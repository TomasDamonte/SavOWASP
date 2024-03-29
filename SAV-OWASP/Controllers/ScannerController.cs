﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;

namespace SavOWASP
{

    // https://www.zaproxy.org/docs/api
    // http://localhost:9000/scanner/scan/?target=https://conectate.ub.edu.ar&&prof=1
    // http://localhost:9000/scanner/scantohtml/?target=https://conectate.ub.edu.ar&&prof=1

    public class ScannerController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Scan(string target, int prof = 1)
        {
            return new JsonResult<OWASPZAPReport>(new Scanner(target, prof).Result, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }, Encoding.UTF8, this);
        }

        [HttpGet]
        public HttpResponseMessage ScanToHtml(string target, int prof = 1)
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(new Scanner(target, prof).HtmlResult)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

    }
}
