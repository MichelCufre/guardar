using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WIS.Application.Extension;
using WIS.Application.Localization;
using WIS.BackendService.Configuration;
using WIS.BackendService.Services;
using WIS.Configuration;
using WIS.Domain.DataModel.Handlers;
using WIS.Domain.Services;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering.Expressions;
using WIS.FormComponent.Execution;
using WIS.GridComponent.Build;
using WIS.Persistence;
using WIS.TrafficOfficer.Configuration;
using WIS.Translation;

namespace WIS.BackendService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();

            services.AddApplicationServices();
            
            services.AddScoped<IExpressionLiteralConverter, ExpressionLiteralConverter>();
                        
            services.AddScoped<ITranslationResourceProvider, TranslationResourceProvider>();
            services.AddScoped<ITranslationUpdateService, TranslationUpdateService>();

            services.AddScoped<IDatabaseConfigurationService, DatabaseConfigurationService>();

            services.AddScoped<IFormConfigProvider, FormConfigProvider>();

            services.AddScoped<IGridConfigProvider, GridConfigProvider>();
            services.AddScoped<ITranslator, GridTranslator>();
            services.AddScoped<IDatabaseFactory, DatabaseFactory>();
            services.AddScoped<IDapper, DapperService>();
            services.AddScoped<ILiberacionService, LiberacionService>();
            services.AddScoped<ITrackingService, TrackingService>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();

            services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.Position));
            services.Configure<TrafficOfficerSettings>(Configuration.GetSection(TrafficOfficerSettings.Position));
            services.Configure<SelectSettings>(Configuration.GetSection(SelectSettings.Position));
            services.Configure<TaskQueueSettings>(Configuration.GetSection(TaskQueueSettings.Position));
            services.Configure<WmsApiSettings>(Configuration.GetSection(WmsApiSettings.Position));
            services.Configure<AuthSettings>(Configuration.GetSection(AuthSettings.Position));
            services.Configure<MaxItemsSettings>(Configuration.GetSection(MaxItemsSettings.Position));
            services.Configure<FileSettings>(Configuration.GetSection(FileSettings.Position));
            services.Configure<DocumentoSettings>(Configuration.GetSection(DocumentoSettings.Position));
            services.Configure<PowerBiSettings>(Configuration.GetSection(PowerBiSettings.Position));
            services.Configure<PrintingSettings>(Configuration.GetSection(PrintingSettings.Position));

            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.AddTypeHandler(new BoolFromStringTypeHandler());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
