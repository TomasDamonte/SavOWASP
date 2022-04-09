using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;

namespace SavOWASP
{

    // https://www.zaproxy.org/docs/api
    // http://localhost:9000/scanner/scan/?target=https://conectate.ub.edu.ar
    // http://localhost:9000/scanner/scantohtml/?target=https://conectate.ub.edu.ar
    // http://localhost:9000/scanner/scantoxml/?target=https://conectate.ub.edu.ar

    public class ScannerController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Scan(string target)
        {
            Scanner scanner = new Scanner(target);
            scanner.Scan();
            return new JsonResult<OWASPZAPReport>(scanner.Result, new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }, Encoding.UTF8, this);
        }

        [HttpGet]
        public HttpResponseMessage ScanToHtml(string target)
        {
            Scanner scanner = new Scanner(target);
            scanner.Scan();
            var response = new HttpResponseMessage
            {
                Content = new StringContent(scanner.HtmlResult)
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
            return response;
        }

        [HttpGet]
        public OWASPZAPReport ScanToXml(string target)
        {
            Scanner scanner = new Scanner(target);
            scanner.Scan();
            return scanner.Result;
        }
    }

}
