using OWASPZAPDotNetAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SavOWASP
{
    internal class Program
    {
        //private static string _target = "http://localhost:8080/UI";
        private static string _target = "https://conectate.ub.edu.ar/Home.aspx";
        private static string _apikey = "lsi776rff5176e6eb3a6ts8n39";
        private static ClientApi _api = new ClientApi("localhost", 8080, _apikey);
        private static IApiResponse _apiResponse;

        static void Main(string[] args)
        {
            try
            {
                while (true)
                {
                    Console.Write("Ingrese la URL a escanear");
                    Console.WriteLine();
                    _target = Console.ReadLine();
                    Go();
                }                               
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
            Console.ReadLine();            
        }

        public static void Go()
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

        private static void ShutdownZAP()
        {
            _apiResponse = _api.core.shutdown();
            if ("OK" == ((ApiResponseElement)_apiResponse).Value)
                Console.WriteLine("ZAP shutdown success " + _target);
        }

        private static void PrintAlertsToConsole()
        {
            //no anda
            List<Alert> alerts = _api.GetAlerts(_target, 0, 0, "");
            foreach (Alert alert in alerts)
            {
                Console.WriteLine(alert.AlertMessage
                    + Environment.NewLine
                    + alert.CWEId
                    + Environment.NewLine
                    + alert.Url
                    + Environment.NewLine
                    + alert.WASCId
                    + Environment.NewLine
                    + alert.Evidence
                    + Environment.NewLine
                    + alert.Parameter
                    + Environment.NewLine
                );
            }
        }

        private static void WriteHtmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName, _api.core.htmlreport());
        }

        private static void WriteXmlReport(string reportFileName)
        {
            File.WriteAllBytes(reportFileName + ".xml", _api.core.xmlreport());
        }

        private static void PollTheActiveScannerTillCompletion(string activeScanId)
        {
            int activeScannerprogress;
            while (true)
            {
                Sleep(5000);
                activeScannerprogress = int.Parse(((ApiResponseElement)_api.ascan.status(activeScanId)).Value);
                Console.WriteLine("Active scanner progress: {0}%", activeScannerprogress);
                if (activeScannerprogress >= 100)
                    break;
            }
            Console.WriteLine("Active scanner complete");
        }
        private static string StartActiveScanning()
        {
            Console.WriteLine("Active Scanner: " + _target);
            _apiResponse = _api.ascan.scan(_target, "", "", "", "", "", "");
            string activeScanId = ((ApiResponseElement)_apiResponse).Value;
            return activeScanId;
        }

        private static void PollTheAjaxSpiderTillCompletion()
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

        private static void StartAjaxSpidering()
        {
            Console.WriteLine("Ajax Spider: " + _target);
            _apiResponse = _api.ajaxspider.scan(_target, "", "", "");

            if ("OK" == ((ApiResponseElement)_apiResponse).Value)
                Console.WriteLine("Ajax Spider started for " + _target);
        }

        private static void PollTheSpiderTillCompletion(string scanid)
        {
            int spiderProgress;
            while (true)
            {
                Sleep(1000);
                spiderProgress = int.Parse(((ApiResponseElement)_api.spider.status(scanid)).Value);
                Console.WriteLine("Spider progress: {0}%", spiderProgress);
                if (spiderProgress >= 100)
                    break;
            }
            Console.WriteLine("Spider complete");
            //Sleep(10000);
        }

        private static string StartSpidering()
        {
            Console.WriteLine("Spider: " + _target);
            _apiResponse = _api.spider.scan(_target, "", "", "", "");
            string scanid = ((ApiResponseElement)_apiResponse).Value;
            return scanid;
        }

        private static void LoadTargetUrlToSitesTree()
        {
            _api.AccessUrl(_target);
        }

        private static void Sleep(int milliseconds)
        {
            do
            {
                Thread.Sleep(milliseconds);
                Console.WriteLine("...zz" + Environment.NewLine);
                milliseconds -= 2000;
            } while (milliseconds > 2000);
        }


    }
}
