using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WIS.API.Extension.Extensions;
using WIS.API.Extension.Middlewares;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Handlers;
using WIS.Domain.Services;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.TaskQueue.Extensions;
using WIS.TaskQueue.Providers;

namespace WIS.TaskQueue
{
    public class Startup
    {
        Logger _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDatabaseConfigurationService, DatabaseConfigurationService>();
            services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
            services.AddSingleton<IDapper, DapperService>();
            services.AddSingleton<IFactoryService, FactoryService>();

            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.AddTypeHandler(new BoolFromStringTypeHandler());

            AddUnitOfWorkFactoryService(services);

            services.AddSingleton<IFormatoProvider, FormatoProvider>();

            services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.Position));
            services.Configure<TaskQueueSettings>(Configuration.GetSection(TaskQueueSettings.Position));
            services.Configure<PrintingSettings>(Configuration.GetSection(PrintingSettings.Position));
            services.Configure<AuthSettings>(Configuration.GetSection(AuthSettings.Position));

            services.AddControllers();
            services.AddApplicationServices(Configuration);

            var assembly = Assembly.GetExecutingAssembly();

            services.AddControllers()
                //.ConfigureApiBehaviorOptions(c => c.SuppressModelStateInvalidFilter = true)
                .AddNewtonsoftJson(
                    options =>
                    {
                        //options.SerializerSettings.ContractResolver = new DefaultContractResolver(); //enable pascal case
                        options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                        options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
                    })
                //.AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null) //enable pascal case in swagger
                .AddDataAnnotationsLocalization //model data annotation/validation using localization resources
                (
                    options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(Resources));
                    }
                );

            services.AddLocalization(o =>
            {
                // localization in separated assembly
                o.ResourcesPath = "Resources";
            });

            services.AddHttpContextAccessor();

            services.AddSingleton<TaskQueue.Services.TaskQueue>();
            services.AddHostedService<TaskQueue.Services.TaskQueue>(provider => provider.GetService<TaskQueue.Services.TaskQueue>());
        }

        public virtual void AddUnitOfWorkFactoryService(IServiceCollection services)
        {
            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseBaseMiddleware();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseLocalizationMiddleware(Configuration);

            app.ConfigureExceptionHandler(_logger);

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                var list = Configuration.GetSection("Globalization:SupportedCultures").Get<List<string>>().Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                var defaultCulture = Configuration["Globalization:DefaultCulture"];
                bool.TryParse(Configuration["Globalization:Swagger:EnableMultilanguageDocumentation"], out bool enableMultiLangSwagger);
                bool.TryParse(Configuration["Globalization:Swagger:EnableUITranslation"], out bool enableUITranslation);

                if (enableMultiLangSwagger && list != null && list.Count > 1)
                {
                    if (!string.IsNullOrWhiteSpace(defaultCulture)) // put in first position default language version
                        list.Move(x => x.ToLower().Trim() == defaultCulture.ToLower().Trim(), 0);

                    foreach (var l in list)
                    {
                        c.SwaggerEndpoint($"/swagger/TaskQueueDoc-{l.Trim()}/swagger.json?lang={l.Trim()}", $"Task Queue ({l.Trim()})");
                    }

                    if (enableUITranslation)
                        c.HeadContent = "<script src='./script/jquery-3.6.3.min.js'></script><script src='./script/jquery.initialize.min.js'></script><script src='./script/translate/translate.js'></script>";
                }
                else
                {
                    c.SwaggerEndpoint($"/swagger/TaskQueueDoc-{defaultCulture}/swagger.json?lang={defaultCulture}", $"Task Queue ({defaultCulture})");

                    if (!string.IsNullOrWhiteSpace(defaultCulture) && enableUITranslation)
                        c.HeadContent = "<script src='./script/jquery-3.6.3.min.js'></script><script src='./jscript/query.initialize.min.js'></script><script id='languagefile' src='./script/translate/" + defaultCulture.Trim() + ".js'></script><script src='./script/translate/translate.js'></script>";
                }

                c.HeadContent += "<script src='./script/rapipdf-min.js'></script><script src='./script/custom.js'></script>";
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}");
            });
        }
    }
}
