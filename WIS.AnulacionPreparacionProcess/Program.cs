using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using WIS.Application.Security;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Handlers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Tracking;
using WIS.Domain.Tracking.Config;
using WIS.Http;
using WIS.Persistence;
using WIS.Security;

namespace WIS.AnulacionPreparacionProcess
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
                    var app = serviceProvider.GetService<IAnulacionService>();
                    var settings = serviceProvider.GetService<IOptions<AnulacionSettings>>()?.Value;
                    var mutexId = settings?.MutexId ?? HttpUtility.UrlEncode(typeof(Program).Assembly.Location);
                    var mutexTimeout = settings?.MutexTimeout ?? 3000;
                    var hasHandle = false;

                    SetIdentity(serviceProvider);

                    using (var mutex = new Mutex(false, $"Global\\{mutexId}"))
                    {
                        try
                        {
                            hasHandle = mutex.WaitOne(mutexTimeout, false);

                            if (hasHandle)
                            {
                                app.Start();
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

        public static void SetIdentity(ServiceProvider serviceProvider)
        {
            var identity = serviceProvider.GetService<IIdentityService>();
            var manager = (IIdentityServiceManager)identity;
            manager.SetUser(new BasicUserData
            {
                Language = "es",
                UserId = -1
            }, "JobAnulPrepa", GeneralDb.PredioSinDefinir);
        }

        public static void ConfigureServices(ServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location))
                .AddJsonFile("appsettings.json");

            var config = builder.Build();
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            services.Configure<DatabaseSettings>(config.GetSection(DatabaseSettings.Position));
            services.Configure<AnulacionSettings>(config.GetSection(AnulacionSettings.Position));
            services.Configure<TaskQueueSettings>(config.GetSection(TaskQueueSettings.Position));

            services.AddSingleton<IConfiguration>(configuration);

            services.AddScoped<IDatabaseConfigurationService, DatabaseConfigurationService>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IDatabaseFactory, DatabaseFactory>();
            services.AddScoped<IDapper, DapperService>();
            services.AddScoped<IWebApiClient, WebApiClient>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IAnulacionService, AnulacionService>();
            services.AddScoped<IPrintingService, PrintingService>();
            services.AddScoped<IReportGeneratorService, ReportGeneratorService>();
            services.AddScoped<ITaskQueueService, TaskQueueService>();
            services.AddScoped<IFactoryService, FactoryService>();
            services.AddScoped<ITrackingService, TrackingService>();
            services.AddScoped<IAPITrackingService, APITrackingService>();
            services.AddScoped<ITrackingConfigProvider, TrackingConfigProvider>();

            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.AddTypeHandler(new BoolFromStringTypeHandler());

            #region Requeridos por el TaskQueue
            services.AddScoped<IBarcodeService, BarcodeService>();
            services.AddScoped<IParameterService, ParameterService>();
            services.AddScoped<IEjecucionService, EjecucionService>();
            services.AddScoped<IEmpresaService, EmpresaService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IWebhookCallerService, WebhookCallerService>();
            #endregion

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
