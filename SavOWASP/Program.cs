using Microsoft.Owin.Hosting;
using System;
using System.Configuration;

namespace SavOWASP
{
    public class Program
    {
        static void Main(string[] args)
        {
            string baseAddress = ConfigurationManager.AppSettings["BaseAdress"];
            // Start OWIN host
            WebApp.Start<Startup>(baseAddress);
            Console.WriteLine($"Servicio SAV-OWASP ejecutandose en {baseAddress}{Environment.NewLine}Presione una tecla para salir.");
            Console.ReadKey();             
        }

    }
}
