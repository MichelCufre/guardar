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
using WIS.Domain.Services.Interfaces;
using WIS.Http;
using WIS.Persistence;
using WIS.Security;

namespace WIS.ReportProcessor
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
                    var app = serviceProvider.GetService<IReportGeneratorService>();
                    var settings = serviceProvider.GetService<IOptions<PrintingSettings>>()?.Value;
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
                                app.GeneratePendingReports();
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
            }, "JobReportProc", GeneralDb.PredioSinDefinir);
        }

        public static void ConfigureServices(ServiceCollection services, IConfigurationRoot config)
        {
            services.Configure<DatabaseSettings>(config.GetSection(DatabaseSettings.Position));
            services.Configure<PrintingSettings>(config.GetSection(PrintingSettings.Position));
            services.Configure<AuthSettings>(config.GetSection(AuthSettings.Position));

            services.AddScoped<IDatabaseConfigurationService, DatabaseConfigurationService>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IDatabaseFactory, DatabaseFactory>();
            services.AddScoped<IDapper, DapperService>();
            services.AddScoped<IWebApiClient, WebApiClient>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IPrintingService, PrintingService>();
            services.AddScoped<IReportGeneratorService, ReportGeneratorService>();
            services.AddScoped<IFactoryService, FactoryService>();

            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.AddTypeHandler(new BoolFromStringTypeHandler());

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
