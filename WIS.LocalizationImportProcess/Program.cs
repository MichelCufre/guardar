using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using WIS.HttpLegacy.WebApi;
using WIS.LocalizationImportProcess.Logic;

namespace WIS.LocalizationImportProcess
{
    public class Program
    {
        public static IConfigurationRoot configuration;

        static void Main(string[] args)
        {
            try
            {
                configuration = new ConfigurationBuilder()
                   .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                   .Build();

                ConsoleWriteLine("Ejecutando actualización de localizacion");

                var settings = new LocalizationSettings
                {
                    ServiceUri = configuration.GetSection("app:internalServicesUri").Value,
                    LocalePath = configuration.GetSection("localization:localePath").Value,
                    DefaultLanguage = configuration.GetSection("localization:defaultLanguage").Value,
                    MutexId = configuration.GetSection("mutex:id")?.Value,
                };

                if (int.TryParse(configuration.GetSection("mutex:timeout")?.Value, out int timeout))
                {
                    settings.MutexTimeout = timeout;
                }

                var mutexId = settings.MutexId ?? HttpUtility.UrlEncode(typeof(Program).Assembly.Location);
                var mutexTimeout = settings.MutexTimeout ?? 3000;
                var hasHandle = false;

                using (var mutex = new Mutex(false, $"Global\\{mutexId}"))
                {
                    try
                    {
                        hasHandle = mutex.WaitOne(mutexTimeout, false);

                        if (hasHandle)
                        {
                            var client = new WebApiClientLegacy(HttpClientAccessor.HttpClient);
                            var service = new LocalizationService(client, settings);

                            CancellationTokenSource tokenSource = new CancellationTokenSource();
                            CancellationToken token = tokenSource.Token;

                            service.UpdateDatabaseResources(token).Wait();
                            service.ImportResources(token).Wait();
                        }
                        else
                        {
                            ConsoleWriteLine("Warning: No es posible obtener un bloqueo exclusivo para la aplicación");
                        }
                    }
                    finally
                    {
                        if (hasHandle)
                        {
                            mutex.ReleaseMutex();
                        }
                    }
                }

                ConsoleWriteLine("Listo");
            }
            catch (Exception ex)
            {
                ConsoleWriteLine("Error: " + ex.ToString());
                Console.ReadLine();
            }
        }

        public static void ConsoleWriteLine(string msg)
        {
            Console.WriteLine(System.DateTime.Now.ToString("HH:mm:ss") + " " + msg);
        }
    }
}
