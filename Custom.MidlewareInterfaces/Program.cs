using Dapper;
using Custom.Domain.DataModel;
using Custom.Domain.Services;
using Custom.Domain.Services.Configuration;
using Custom.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Configuracion;
using WIS.Http;
using WIS.Persistence;
using WIS.Security;

namespace Custom.MidlewareInterfaces
{
    public class Program
    {
        static void Main(string[] args)
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
                    var app = serviceProvider.GetService<IMiddlewareService>();
                    var mutexId = HttpUtility.UrlEncode(typeof(Program).Assembly.Location);
                    var mutexTimeout = 3000;
                    var hasHandle = false;

                    using (var mutex = new Mutex(false, $"Global\\{mutexId}"))
                    {
                        try
                        {
                            hasHandle = mutex.WaitOne(mutexTimeout, false);

                            if (hasHandle)
                            {
                                app.Run();
                                
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
            services.Configure<WmsApiSettings>(config.GetSection(WmsApiSettings.Position));
            services.Configure<AuthSettings>(config.GetSection(AuthSettings.Position));
            services.Configure<DatabaseSettings>(config.GetSection(DatabaseSettings.Position));
            services.Configure<ErpClientSettings>(config.GetSection(ErpClientSettings.Position));

            services.AddSingleton<IConfiguration>(config);

            services.AddScoped<IWebApiClient, WebApiClient>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkCustomFactory>();
            services.AddScoped<BatchWmsApiService>();
            services.AddScoped<WsGenQueryClient>();
            services.AddScoped<ErpDataExtractor>();
            services.AddScoped<IMiddlewareService, MiddlewareService>();

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
