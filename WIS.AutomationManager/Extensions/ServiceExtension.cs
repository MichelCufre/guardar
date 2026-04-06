using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
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
using WIS.AutomationManager.Filters;
using WIS.AutomationManager.Interfaces;
using WIS.AutomationManager.Models.Mappers;
using WIS.AutomationManager.Models.Mappers.Interfaces;
using WIS.AutomationManager.Services;
using WIS.Common.API.Attributes;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Security;

namespace WIS.AutomationManager.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ApplicationServices();
            services.AddDtoMappers();
            services.SwaggerServices(configuration);

            return services;
        }

        private static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IParameterService, ParameterService>();
            services.AddScoped<IUserProvider, UserProvider>();
            services.AddScoped<IBarcodeService, BarcodeService>();
            services.AddScoped<IValidationService, ValidationService>();

            services.AddScoped<IAutomatismoNotificationFactory, AutomatismoNotificationFactory>();
            services.AddScoped<IAutomatismoValidationService, AutomatismoValidationService>();
            services.AddScoped<IAutomatismoEjecucionService, AutomatismoEjecucionService>();
            services.AddScoped<IAutomatismoService, AutomatismoService>();
            services.AddScoped<IPtlFactory, PtlFactory>();

            services.AddScoped<IAutomatismoInterpreterClientService, AutomatismoInterpreterClientService>();
            services.AddScoped<IPtlInterpreterClientService, PtlInterpreterClientService>();
            services.AddScoped<IAutomatismoWmsApiClientService, AutomatismoWmsApiClientService>();

            return services;
        }

        private static IServiceCollection AddDtoMappers(this IServiceCollection services)
        {
            services.AddScoped<INotificacionAjusteStockAutomatismoMapper, NotificacionAjusteStockAutomatismoMapper>();
            services.AddScoped<IPtlAutomatismoMapper, PtlAutomatismoMapper>();
            services.AddScoped<IConfirmacionEntradaAutomatismoMapper, ConfirmacionEntradaAutomatismoMapper>();
            services.AddScoped<IConfirmacionSalidaAutomatismoMapper, ConfirmacionSalidaAutomatismoMapper>();
            services.AddScoped<IConfirmacionMovimientoAutomatismoMapper, ConfirmacionMovimientoAutomatismoMapper>();

            return services;
        }

        private static IServiceCollection SwaggerServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<WISSchemaFilter>();

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

                        options.SwaggerDoc($"AutomationManagerDoc-{l.Trim()}", new OpenApiInfo
                        {
                            Version = assembly.GetName().Version?.ToString() ?? "v1.0.0",
                            Title = $"Automation Manager ({l.Trim()})",
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

                    options.SwaggerDoc($"AutomationManagerDoc-{defaultCulture}", new OpenApiInfo
                    {
                        Version = assembly.GetName().Version?.ToString() ?? "v1.0.0",
                        Title = $"Automation Manager ({defaultCulture})",
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

        private static void LocalizeSwaggerText(XmlNode e,
            IStringLocalizer<Resources> localizer)
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
