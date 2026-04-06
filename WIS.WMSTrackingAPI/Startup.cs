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
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Handlers;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence;
using WIS.WMSTrackingAPI.Extensions;
using WIS.WMSTrackingAPI.Filters;
using WIS.WMSTrackingAPI.Policies;
using WIS.WMSTrackingAPI.Providers;

namespace WIS.WMSTrackingAPI
{
    public class Startup
    {
        Logger _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            var authority = Configuration.GetValue<string>("AuthSettings:Authority");
            if (!string.IsNullOrEmpty(authority))
            {
                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.Authority = authority;
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

                services.AddAuthorization(options =>
                {
                    options.FallbackPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .AddRequirements(new ApiAccessRequirement()) //Obligatorio
                        .Build();
                });

                services.AddScoped<IAuthorizationHandler, ApiAccessAuthorizationHandler>();
            }
            else
            {
                services.AddAuthentication();
            }

            //Services
            services.AddSingleton<ProblemDetailsFactory, WISProblemDetailsFactory>();
            services.AddScoped<IDatabaseConfigurationService, DatabaseConfigurationService>();
            services.AddScoped<IDatabaseFactory, DatabaseFactory>();
            services.AddScoped<IDapper, DapperService>();
            services.AddScoped<IFactoryService, FactoryService>();
            services.AddScoped<IUnitOfWorkFactory, UnitOfWorkFactory>();
            services.AddScoped<IFormatoProvider, FormatoProvider>();

            SqlMapper.RemoveTypeMap(typeof(bool));
            SqlMapper.AddTypeHandler(new BoolFromStringTypeHandler());

            services.Configure<DatabaseSettings>(Configuration.GetSection(DatabaseSettings.Position));
            services.Configure<IISServerOptions>(options =>
                options.MaxRequestBodySize = Configuration.GetValue<long>("AppSettings:MaxRequestBodySize")
            );

            services.AddApplicationServices(Configuration);

            services.AddControllers(options =>
            {
                options.Filters.Add<WISActionFilter>();
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                options.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            })
            .AddDataAnnotationsLocalization 
            (
                options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(Resources));
                }
            );

            services.AddLocalization(o =>
            {
                o.ResourcesPath = "Resources";
            });

            IdentityModelEventSource.ShowPII = true;
        }
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
                        c.SwaggerEndpoint($"/swagger/WMSTrackingAPIDoc-{l.Trim()}/swagger.json?lang={l.Trim()}", $"WMS Tracking API ({l.Trim()})");
                    }

                    if (enableUITranslation)
                        c.HeadContent = "<script src='./script/jquery-3.6.3.min.js'></script><script src='./script/jquery.initialize.min.js'></script><script src='./script/translate/translate.js'></script>";
                }
                else
                {
                    c.SwaggerEndpoint($"/swagger/WMSTrackingAPIDoc-{defaultCulture}/swagger.json?lang={defaultCulture}", $"WMS Tracking API ({defaultCulture})");

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

            var authority = Configuration.GetValue<string>("AuthSettings:Authority");
            if (!string.IsNullOrEmpty(authority))
            {
                app.UseAuthentication();
                app.UseAuthorization();
            }

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
