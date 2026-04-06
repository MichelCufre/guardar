using Dapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using System.Threading.Tasks;
using WIS.API.Extension.Extensions;
using WIS.API.Extension.Middlewares;
using WIS.Application.Extension;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Handlers;
using WIS.Domain.Services;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;
using WIS.Http;
using WIS.Persistence;
using WIS.TrafficOfficer;
using WIS.TrafficOfficer.Configuration;
using WIS.WMS_API.Extensions;
using WIS.WMS_API.Filters;
using WIS.WMS_API.Models.Mappers;
using WIS.WMS_API.Models.Mappers.Interfaces;
using WIS.WMS_API.Policies;
using WIS.WMS_API.Providers;

namespace WIS.WMS_API
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
			services.AddMvc(options =>
			{
				options.MaxModelValidationErrors = Configuration.GetValue<int>("AppSettings:MaxModelValidationErrors");
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

            services.AddHttpClient();

            services.AddSingleton<ProblemDetailsFactory, WISProblemDetailsFactory>();
			services.AddSingleton<IDatabaseConfigurationService, DatabaseConfigurationService>();
            services.AddSingleton<IDatabaseFactory, DatabaseFactory>();
            services.AddScoped<IWebApiClient, WebApiClient>();
            services.AddSingleton<IDapper, DapperService>();
            services.AddScoped<IPrintingService, PrintingService>();
            services.AddScoped<IReportGeneratorService, ReportGeneratorService>();
            services.AddScoped<ITaskQueueService, TaskQueueService>();
            services.AddSingleton<IFactoryService, FactoryService>();
            services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IFormatoProvider, FormatoProvider>();
            services.AddScoped<ITrafficOfficerService, TrafficOfficerService>();
            services.AddScoped<ITrafficOfficerSessionManager, TrafficOfficerSessionManager>();
            services.AddScoped<IControlCalidadService, ControlCalidadService>();
            services.AddScoped<IControlCalidadMapper, ControlCalidadMapper>();

            services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.Position));
            services.Configure<TaskQueueSettings>(Configuration.GetSection(TaskQueueSettings.Position));
            services.Configure<TrafficOfficerSettings>(Configuration.GetSection(TrafficOfficerSettings.Position));

            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.AddTypeHandler(new BoolFromStringTypeHandler());

            services.Configure<IISServerOptions>(options => {
                options.MaxRequestBodySize = Configuration.GetValue<long>("AppSettings:MaxRequestBodySize");
                options.AllowSynchronousIO = true;
            });

            services.AddHttpContextAccessor();

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new ApiAccessRequirement()) //Obligatorio
                    .Build();
            });

            services.AddScoped<IAuthorizationHandler, ApiAccessAuthorizationHandler>();
            services.AddApplicationServices(Configuration);

            services.Configure<MaxItemsSettings>(Configuration.GetSection(MaxItemsSettings.Position));

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
                        c.SwaggerEndpoint($"/swagger/WMSAPIDoc-{l.Trim()}/swagger.json?lang={l.Trim()}", $"WMS API ({l.Trim()})");
                    }

                    if (enableUITranslation)
                        c.HeadContent = "<script src='./script/jquery-3.6.3.min.js'></script><script src='./script/jquery.initialize.min.js'></script><script src='./script/translate/translate.js'></script>";
                }
                else
                {
                    c.SwaggerEndpoint($"/swagger/WMSAPIDoc-{defaultCulture}/swagger.json?lang={defaultCulture}", $"WMS API ({defaultCulture})");

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
