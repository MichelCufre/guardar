using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using NLog;
using NLog.Web;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using WIS.API.Extension.Extensions;
using WIS.API.Extension.Middlewares;
using WIS.AutomationManager.Extensions;
using WIS.AutomationManager.Filters;
using WIS.AutomationManager.Models;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Handlers;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.Persistence.InMemory;

namespace WIS.AutomationManager
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
			var assembly = Assembly.GetExecutingAssembly();

            services.AddMvc(options =>
            {

            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = Configuration.GetValue<string>("AuthSettings:Authority");
                    options.TokenValidationParameters.ValidateAudience = false; //No hay una sola audiencia, no hace falta controlarla
                    options.TokenValidationParameters.ValidateIssuer = true;

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            return Task.CompletedTask; //para debug
                        }
                    };

                    if (!string.IsNullOrEmpty(Configuration.GetValue<string>("AuthSettings:TrustedCertificateThumbprint"))) //Permite aceptar certificados autofirmados en desarrollo
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler()
                        {
                            ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                            {
                                return cert.Thumbprint == Configuration.GetValue<string>("AuthSettings:TrustedCertificateThumbprint");
                            }
                        };
                    }
                });

			services.AddSingleton<ProblemDetailsFactory, WISProblemDetailsFactory>();
			services.AddScoped<IDatabaseOptionProvider, DatabaseOptionProvider>();
            services.AddScoped<IDatabaseConfigurationService, DatabaseConfigurationService>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IUnitOfWorkInMemoryFactory, UnitOfWorkInMemoryFactory>();
            services.AddScoped<IDatabaseFactory, DatabaseFactory>();
            services.AddScoped<IDapper, DapperService>();
            services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.Position));
            services.Configure<AutomationSettings>(Configuration.GetSection(AutomationSettings.Position));
            services.AddScoped<IFactoryService, FactoryService>();

            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.AddTypeHandler(new BoolFromStringTypeHandler());

            services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.Position));
            services.Configure<IISServerOptions>(options =>
                options.MaxRequestBodySize = Configuration.GetValue<long>("AppSettings:MaxRequestBodySize")
            );

            services.AddHttpContextAccessor();

            services.AddApplicationServices(Configuration);

            services.AddControllers(options =>
            {
                options.Filters.Add<WISActionFilter>();
            })
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

            IdentityModelEventSource.ShowPII = true;
        }

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
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
                        c.SwaggerEndpoint($"/swagger/AutomationManagerDoc-{l.Trim()}/swagger.json?lang={l.Trim()}", $"Automation Manager ({l.Trim()})");
                    }

                    if (enableUITranslation)
                        c.HeadContent = "<script src='./script/jquery-3.6.3.min.js'></script><script src='./script/jquery.initialize.min.js'></script><script src='./script/translate/translate.js'></script>";
                }
                else
                {
                    c.SwaggerEndpoint($"/swagger/AutomationManagerDoc-{defaultCulture}/swagger.json?lang={defaultCulture}", $"Automation Manager ({defaultCulture})");

                    if (!string.IsNullOrWhiteSpace(defaultCulture) && enableUITranslation)
                        c.HeadContent = "<script src='./script/jquery-3.6.3.min.js'></script><script src='./jscript/query.initialize.min.js'></script><script id='languagefile' src='./script/translate/" + defaultCulture.Trim() + ".js'></script><script src='./script/translate/translate.js'></script>";
                }

                c.HeadContent += "<script src='./script/rapipdf-min.js'></script><script src='./script/custom.js'></script>";
                c.RoutePrefix = string.Empty;
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next => context => {
                context.Request.EnableBuffering();
                return next(context);
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBaseMiddleware();
            app.UseLocalizationMiddleware(Configuration);

            app.ConfigureExceptionHandler(_logger);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}");
            });
        }
    }
}
