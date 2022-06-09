using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace SavOWASP
{
    public class Program
    {
        static void Main(string[] args)
        {
            while (!IsZAPOWASPRunning())
            {
                Console.WriteLine("OWASP ZAP no se esta ejecutando. Por favor ejecute OWASP ZAP para continuar.");
                Console.WriteLine("Presione Enter para continuar...");
                Console.ReadLine();
            }            
            string baseAddress = ConfigurationManager.AppSettings["BaseAdress"];
            WebApp.Start<Startup>(baseAddress);
            Console.WriteLine($"Servicio SAV-OWASP ejecutandose en {baseAddress}{Environment.NewLine}Presione una tecla para salir.");
            Console.ReadKey();             
        }

        private static bool IsZAPOWASPRunning()
        {
            var procesos = new List<Process>(Process.GetProcesses());
            return procesos.Any(p => p.MainWindowTitle.ToLower().Contains("owasp zap")) ;
        }

    }
}
