using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;
using System.Reflection;
using System.Web;
using WIS.Configuration;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.XmlProcessorEntrada.Services;

namespace WIS.XmlProcessorEntrada
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            ConfigureServices(services, config);

            var logger = LogManager.GetCurrentClassLogger();

            logger.Info("Iniciando aplicación");

            try
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    var app = serviceProvider.GetService<IXmlDataExternalManager>();
                    var settings = serviceProvider.GetService<IOptions<ApplicationSettings>>()?.Value;
                    var mutexId = settings?.MutexId ?? HttpUtility.UrlEncode(typeof(Program).Assembly.Location);
                    var mutexTimeout = settings?.MutexTimeout ?? 3000;
                    var hasHandle = false;

                    using (var mutex = new Mutex(false, $"Global\\{mutexId}"))
                    {
                        try
                        {
                            hasHandle = mutex.WaitOne(mutexTimeout, false);

                            if (hasHandle)
                            {
                                app.Start().Wait();
                            }
                            else
                            {
                                logger.Warn("No es posible obtener un bloqueo exclusivo para la aplicación");
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
                }

                logger.Info("Aplicación finalizada");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Ocurrió un error no controlado");
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static void ConfigureServices(ServiceCollection services, IConfigurationRoot config)
        {
            services.Configure<DatabaseSettings>(config.GetSection(DatabaseSettings.Position));
            services.Configure<ApplicationSettings>(config.GetSection(ApplicationSettings.Position));

            services.AddSingleton<IConfiguration>(config);

            services.AddScoped<IDatabaseConfigurationService, DatabaseConfigurationService>();
            services.AddScoped<IDatabaseFactory, DatabaseFactory>();
            services.AddScoped<IXmlDataExternalManager, XmlDataExternalManager>();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.AddNLog("NLog.config");
            });

            services.AddHttpClient();
            services.AddHttpContextAccessor();
        }
    }
}