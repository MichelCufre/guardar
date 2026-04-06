using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using WIS.Configuration;

namespace WIS.DataCleanerProcess
{
    public class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Iniciando aplicación");

            try
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    //TODO:
                }

                logger.Info("Aplicación finalizada");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Ocurrió un error no controlado");
                throw ex;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .AddJsonFile("appsettings.json");

            var config = builder.Build();

            services.Configure<DatabaseSettings>(config.GetSection(DatabaseSettings.Position));

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                builder.AddNLog("NLog.config");
            });
        }
    }
}
