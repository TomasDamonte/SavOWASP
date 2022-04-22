using Newtonsoft.Json;
using OWASPZAPDotNetAPI;
using System;
using System.Configuration;
using System.Text;
using System.Threading;

namespace SavOWASP
{
    public class Scanner
    {
        private string _target;
        private ClientApi _api;
        private IApiResponse _apiResponse;

        public string HtmlResult
        {
            get
            {
                return Encoding.UTF8.GetString(_api.core.htmlreport());
            }
        }

        public OWASPZAPReport Result
        {
            get
            {
                return JsonConvert.DeserializeObject<OWASPZAPReport>(Encoding.UTF8.GetString(_api.core.jsonreport()));
            }
        }

        public Scanner(string target, int profundidad)
        {
            _target = target;
            _api = new ClientApi(ConfigurationManager.AppSettings["ZapOwaspURL"], int.Parse(ConfigurationManager.AppSettings["ZapOwaspPort"]), ConfigurationManager.AppSettings["ApiKey"]);
            _api.spider.setOptionMaxDepth(profundidad);            
            _api.alert.deleteAllAlerts();
            ApiResponseList sites = (ApiResponseList)_api.core.sites();
            foreach(ApiResponseElement site in sites.List)
            {
                _api.core.deleteSiteNode(site.Value,"","");
            }
            Scan();
        }

        private void Scan()
        {
            Console.WriteLine($"[{DateTime.Now:dd/MM/yyyy hh:mm:ss}] Escaneando {_target} ...");
            PollTheSpiderTillCompletion(StartSpidering());
            PollTheActiveScannerTillCompletion(StartActiveScanning());
            Console.WriteLine($"[{DateTime.Now:dd/MM/yyyy hh:mm:ss}] Escaneo terminado.");
        }

        private void PollTheActiveScannerTillCompletion(string activeScanId)
        {
            while (int.Parse(((ApiResponseElement)_api.ascan.status(activeScanId)).Value) < 100)
            {
            }
            //Esperar a que se generen las alertas
            Thread.Sleep(5000);
        }

        private string StartActiveScanning()
        {
            _apiResponse = _api.ascan.scan(_target, "", "true", "", "", "", "");
            return ((ApiResponseElement)_apiResponse).Value;
        }

        private void PollTheSpiderTillCompletion(string scanid)
        {
            while (int.Parse(((ApiResponseElement)_api.spider.status(scanid)).Value) < 100)
            {
            }
        }

        private string StartSpidering()
        {
            _apiResponse = _api.spider.scan(_target, "", "", "", "");
            return ((ApiResponseElement)_apiResponse).Value;
        }

    }
}
