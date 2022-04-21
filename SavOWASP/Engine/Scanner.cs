using Newtonsoft.Json;
using OWASPZAPDotNetAPI;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace SavOWASP
{
    public class Scanner
    {
        private string _target;
        private string _apikey = "lsi776rff5176e6eb3a6ts8n39";
        private ClientApi _api;
        private IApiResponse _apiResponse;

        public Scanner(string target)
        {
            _api = new ClientApi(ConfigurationManager.AppSettings["ZapOwaspURL"], int.Parse(ConfigurationManager.AppSettings["ZapOwaspPort"]), _apikey);
            _target = target;
            _api.alert.deleteAllAlerts();
            ApiResponseList sites = (ApiResponseList)_api.core.sites();
            foreach(ApiResponseElement site in sites.List)
            {
                _api.core.deleteSiteNode(site.Value,"","");
            }           
        }

        public void Go()
        {
            string spiderScanId = StartSpidering();
            PollTheSpiderTillCompletion(spiderScanId);

            //StartAjaxSpidering();
            //PollTheAjaxSpiderTillCompletion();

            //string activeScanId = StartActiveScanning();
            //PollTheActiveScannerTillCompletion(activeScanId);

            string reportFileName = $"Report_{DateTime.Now:yyy-MM-dd-hh-mm-ss}.html";
            //WriteXmlReport(reportFileName);
            WriteHtmlReport(reportFileName);
            Process.Start(reportFileName);
            //PrintAlertsToConsole();

            //ShutdownZAP();
        }

        public void Scan()
        {
            PollTheSpiderTillCompletion(StartSpidering());
            PollTheActiveScannerTillCompletion(StartActiveScanning());            
        }

        public string HtmlResult
        {
            get
            {
                return Encoding.UTF8.GetString(_api.core.htmlreport());
            }
        }

        public string JsonResult
        {
            get
            {
                return Encoding.UTF8.GetString(_api.core.jsonreport());
            }
        }

        public string XmlResult
        {
            get
            {
                return Encoding.UTF8.GetString(_api.core.xmlreport());
            }
        }

        public OWASPZAPReport Result
        {
            get
            {
                return JsonConvert.DeserializeObject<OWASPZAPReport>(JsonResult);
            }
        }

        private void ShutdownZAP()
        {
            _apiResponse = _api.core.shutdown();
            if ("OK" == ((ApiResponseElement)_apiResponse).Value)
                Console.WriteLine("ZAP shutdown success " + _target);
        }

        private void PrintAlertsToConsole()
        {
            //no anda
            //List<Alert> alerts = _api.GetAlerts(_target, 0, 0, "");
            //foreach (Alert alert in alerts)
            //{
            //    Console.WriteLine(alert.AlertMessage
            //        + Environment.NewLine
            //        + alert.CWEId
            //        + Environment.NewLine
            //        + alert.Url
            //        + Environment.NewLine
            //        + alert.WASCId
            //        + Environment.NewLine
            //        + alert.Evidence
            //        + Environment.NewLine
            //        + alert.Parameter
            //        + Environment.NewLine
            //    );
            //}
        }

        private void WriteHtmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName, _api.core.htmlreport());
        }

        private void WriteXmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName + ".xml", _api.core.xmlreport());
        }

        private void PollTheActiveScannerTillCompletion(string activeScanId)
        {
            int activeScannerprogress;
            while (true)
            {
                //Sleep(1000);
                activeScannerprogress = int.Parse(((ApiResponseElement)_api.ascan.status(activeScanId)).Value);
                //Console.WriteLine("Active scanner progress: {0}%", activeScannerprogress);
                if (activeScannerprogress >= 100)
                    break;
            }
            //Console.WriteLine("Active scanner complete");
            while (int.Parse(((ApiResponseElement)_api.alert.numberOfAlerts("", "")).Value) == 0)
            {
            }
            Thread.Sleep(500);
        }

        private string StartActiveScanning()
        {
            //Console.WriteLine("Active Scanner: " + _target);
            _apiResponse = _api.ascan.scan(_target, "", "true", "", "", "", "");
            return ((ApiResponseElement)_apiResponse).Value;
        }

        private void PollTheAjaxSpiderTillCompletion()
        {
            while (true)
            {
                Sleep(1000);
                string ajaxSpiderStatusText = string.Empty;
                ajaxSpiderStatusText = Convert.ToString(((ApiResponseElement)_api.ajaxspider.status()).Value);
                if (ajaxSpiderStatusText.IndexOf("running", StringComparison.InvariantCultureIgnoreCase) > -1)
                    Console.WriteLine("Ajax Spider running");
                else
                    break;
            }
            Console.WriteLine("Ajax Spider complete");
            Sleep(10000);
        }

        private void StartAjaxSpidering()
        {
            Console.WriteLine("Ajax Spider: " + _target);
            _apiResponse = _api.ajaxspider.scan(_target, "", "", "");

            if ("OK" == ((ApiResponseElement)_apiResponse).Value)
                Console.WriteLine("Ajax Spider started for " + _target);
        }

        private void PollTheSpiderTillCompletion(string scanid)
        {
            int spiderProgress;
            while (true)
            {
                //Sleep(1000);
                spiderProgress = int.Parse(((ApiResponseElement)_api.spider.status(scanid)).Value);
                //Console.WriteLine("Spider progress: {0}%", spiderProgress);
                if (spiderProgress >= 100)
                    break;
            }
            //Console.WriteLine("Spider complete");
        }

        private string StartSpidering()
        {
            //Console.WriteLine("Spider: " + _target);
            _apiResponse = _api.spider.scan(_target, "", "", "", "");
            return ((ApiResponseElement)_apiResponse).Value;
        }

        private void LoadTargetUrlToSitesTree()
        {
            _api.AccessUrl(_target);
        }

        private static void Sleep(int milliseconds)
        {
            do
            {
                Thread.Sleep(milliseconds);
                //Console.WriteLine("...zz" + Environment.NewLine);
                milliseconds -= 2000;
            } while (milliseconds > 2000);
        }

    }
}
