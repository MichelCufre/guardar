using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using WIS.API.Extension.Extensions;
using WIS.Application.Security;
using WIS.Common.API.Attributes;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Http;
using WIS.Security;

namespace WIS.TaskQueue.Extensions
{
    public static class ServiceExtension
    {
        static Logger _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ApiBehaviorOptions();
            services.ApplicationServices();
            services.AddDtoMappers();
            services.SwaggerServices(configuration);

            return services;
        }

        private static IServiceCollection ApiBehaviorOptions(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                //options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = context =>
                {
                    var validationService = context.HttpContext.RequestServices.GetService(typeof(IValidationService)) as IValidationService;
                    Error error = new Error("WMSAPI_msg_Error_ModelStateValidation");
                    string msgError = validationService.Translate(error);

                    var problemDetails = new ValidationProblemDetails(context.ModelState)
                    {
                        Instance = context.HttpContext.Request.Path,
                        Status = StatusCodes.Status400BadRequest,
                        Type = $"https://tools.ietf.org/html/rfc7231#section-6.5.1",
                        Title = msgError
                    };

                    GuardarErrores(context);

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/json" }
                    };
                };
            });

            return services;
        }

        private static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddSingleton<IIdentityService, IdentityService>();
            services.AddSingleton<IBarcodeService, BarcodeService>();
            services.AddSingleton<IValidationService, ValidationService>();
            services.AddSingleton<IEmpresaService, EmpresaService>();
            services.AddSingleton<IPrintingService, PrintingService>();
            services.AddSingleton<IReportGeneratorService, ReportGeneratorService>();
            services.AddSingleton<ITaskQueueService, TaskQueueService>();
            services.AddSingleton<IWmsApiService, WmsApiService>();
            services.AddSingleton<IWebhookCallerService, WebhookCallerService>();
            services.AddSingleton<IWebApiClient, WebApiClient>();
            services.AddSingleton<IParameterService, ParameterService>();
            services.AddSingleton<IEjecucionService, EjecucionService>();
            services.AddSingleton<IFacturacionService, FacturacionService>();

            return services;
        }

        private static IServiceCollection AddDtoMappers(this IServiceCollection services)
        {
            return services;
        }

        private static IServiceCollection SwaggerServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddSwaggerGen(options =>
            {
                var list = configuration.GetSection("Globalization:SupportedCultures").Get<List<string>>().Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                var defaultCulture = configuration["Globalization:DefaultCulture"];
                bool.TryParse(configuration["Globalization:Swagger:EnableMultilanguageDocumentation"], out bool enableMultiLangSwagger);
                bool.TryParse(configuration["Globalization:Swagger:EnableLocalizedDocumentation"], out bool enableLocalizedDoc);

                var localizer = GetSwaggerStringLocalizer(services);
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (enableMultiLangSwagger && list != null && list.Count > 1)
                {
                    foreach (var l in list)
                    {
                        Thread.CurrentThread.SetCulture(l);

                        options.SwaggerDoc($"TaskQueueDoc-{l.Trim()}", new OpenApiInfo
                        {
                            Version = assembly.GetName().Version?.ToString() ?? "v1.0.0",
                            Title = $"Task Queue ({l.Trim()})",
                            Description = localizer["swagger_description"],
                            Contact = new OpenApiContact
                            {
                                Name = configuration.GetValue<string>("SwaggerSettings:ContactName"),
                                Url = new Uri(configuration.GetValue<string>("SwaggerSettings:ContactUrl"))
                            }
                        });
                    }

                    if (enableLocalizedDoc)
                    {
                        var xmlDoc = new XPathDocument(xmlPath);

                        options.ParameterFilter<XmlCommentsParameterFilter>(xmlDoc);
                        options.RequestBodyFilter<XmlCommentsRequestBodyFilter>(xmlDoc);
                        options.OperationFilter<XmlCommentsOperationFilter>(xmlDoc);
                        options.DocumentFilter<XmlCommentsDocumentFilter>(xmlDoc);
                        options.SchemaFilter<XmlCommentsSchemaFilter>(xmlDoc);
                    }
                }
                else
                {
                    Thread.CurrentThread.SetCulture(defaultCulture);

                    options.SwaggerDoc($"TaskQueueDoc-{defaultCulture}", new OpenApiInfo
                    {
                        Version = assembly.GetName().Version?.ToString() ?? "v1.0.0",
                        Title = $"Task Queue ({defaultCulture})",
                        Description = localizer["swagger_description"],
                        Contact = new OpenApiContact
                        {
                            Name = configuration.GetValue<string>("SwaggerSettings:ContactName"),
                            Url = new Uri(configuration.GetValue<string>("SwaggerSettings:ContactUrl"))
                        }
                    });

                    if (enableLocalizedDoc)
                    {
                        var doc = new XmlDocument();
                        doc.Load(xmlPath);

                        //localize comments
                        var nodeList = doc.GetElementsByTagName("summary").Cast<XmlNode>()
                            .Concat<XmlNode>(doc.GetElementsByTagName("remarks").Cast<XmlNode>())
                            .Concat<XmlNode>(doc.GetElementsByTagName("param").Cast<XmlNode>())
                            .Concat<XmlNode>(doc.GetElementsByTagName("response").Cast<XmlNode>());

                        foreach (XmlNode e in nodeList)
                        {
                            LocalizeSwaggerText(e, localizer);
                        }

                        Stream stream = new MemoryStream();
                        doc.Save(stream);
                        stream.Position = 0;
                        options.IncludeXmlComments(() => { return new XPathDocument(stream); });
                    }
                }

                if (!enableLocalizedDoc)
                    options.IncludeXmlComments(xmlPath);

                options.ParameterFilter<CustomParameterFilter<Resources>>();
                options.RequestBodyFilter<CustomRequestBodyFilter<Resources>>();
                options.OperationFilter<CustomOperationFilter<Resources>>();
                options.DocumentFilter<CustomDocumentFilter<Resources>>();
                options.SchemaFilter<CustomSchemaFilter<Resources, ApiDtoExampleAttribute>>();

                options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                options.EnableAnnotations();
                options.IgnoreObsoleteActions();

                options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        ClientCredentials = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri(configuration["AuthSettings:TokenUrl"]),
                            Scopes = new Dictionary<string, string>
                            {
                                { "api", "API access" }
                            }
                        },
                    }
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                    {
                        new OpenApiSecurityScheme{
                            Reference = new OpenApiReference{
                                Id = "OAuth2", //The name of the previously defined security scheme.
                                Type = ReferenceType.SecurityScheme
                            }
                        },new List<string>()
                    }
                });
            });

            return services;
        }

        private static void GuardarErrores(ActionContext context)
        {
            var errores = context.ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.Error(errores);
        }

        private static void LocalizeSwaggerText(XmlNode e, IStringLocalizer<Resources> localizer)
        {
            var text = e.InnerText.RemoveNewLineAndTrim();
            var loc = localizer[text];
            if (loc != text)
                e.InnerText = loc;
        }

        private static IStringLocalizer<Resources> GetSwaggerStringLocalizer(IServiceCollection services)
        {
            return services.BuildServiceProvider().GetService<IStringLocalizer<Resources>>();
        }
    }
}
