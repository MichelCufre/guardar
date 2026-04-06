using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Xml;
using System.Xml.XPath;
using WIS.API.Extension.Extensions;
using WIS.Application.Security;
using WIS.Common.API.Attributes;
using WIS.Common.Extensions;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Picking;
using WIS.Domain.Produccion.Factories;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Services;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.Validation;
using WIS.Persistence.Database;
using WIS.Security;
using WIS.WMS_API.Filters;
using WIS.WMS_API.Models.Mappers;
using WIS.WMS_API.Models.Mappers.Interfaces;

namespace WIS.WMS_API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.ApiBehaviorOptions();
            services.ApplicationServices();
            services.AddDtoMappers();
            services.SwaggerServices(configuration);

            return services;
        }

        public static IServiceCollection ApiBehaviorOptions(this IServiceCollection services)
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

                    var errors = new List<ValidationsError>();

                    foreach (var errorKey in problemDetails.Errors.Keys)
                    {
                        var itemId = 1;
                        var startIndex = errorKey.IndexOf("[") + 1;
                        var endIndex = errorKey.IndexOf("]");

                        if (startIndex <= endIndex)
                        {
                            var item = errorKey.Substring(startIndex, endIndex - startIndex);
                            int.TryParse(item, out itemId);
                            itemId++;
                        }

                        var messages = problemDetails.Errors[errorKey].ToList();
                        errors.Add(new ValidationsError(itemId, false, messages));
                    }

                    var nuInterfazEjecucion = GuardarErrores(context);

                    problemDetails.Detail = JsonConvert.SerializeObject(errors);
                    problemDetails.Extensions["NumeroInterfazEjecucion"] = nuInterfazEjecucion;

                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/json" }
                    };
                };
            });

            return services;
        }

        public static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserProvider, UserProvider>();

            services.AddScoped<ILpnService, LpnService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<IAgenteService, AgenteService>();
            services.AddScoped<IAgendaService, AgendaService>();
            services.AddScoped<IEgresoService, EgresoService>();
            services.AddScoped<IPedidoService, PedidoService>();
            services.AddScoped<IEmpresaService, EmpresaService>();
            services.AddScoped<IBarcodeService, BarcodeService>();
            services.AddSingleton<IIdentityService, IdentityService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<IParameterService, ParameterService>();
            services.AddScoped<IEjecucionService, EjecucionService>();
            services.AddScoped<IValidationService, ValidationService>();
            services.AddScoped<IPreparacionService, PreparacionService>();
            services.AddScoped<ICodigoBarrasService, CodigoBarrasService>();
            services.AddScoped<ICrossDockingService, CrossDockingService>();
            services.AddScoped<IReferenciaRecepcionService, ReferenciaRecepcionService>();
            services.AddScoped<IAutomatismoAutoStoreClientService, AutomatismoAutoStoreClientService>();
            services.AddScoped<IProduccionService, ProduccionService>();
            services.AddScoped<IProducirProduccionService, ProducirProduccionService>();
            services.AddScoped<IConsumirProduccionService, ConsumirProduccionService>();
            services.AddScoped<IFacturaService, FacturaService>();
            services.AddScoped<IPickingProductoService, PickingProductoService>();

            services.AddScoped<ILogicaProduccionFactory, LogicaProduccionFactory>();
            services.AddScoped<IIngresoProduccionFactory, IngresoProduccionFactory>();
            services.AddScoped<IEspacioProduccionFactory, EspacioProduccionFactory>();
            services.AddScoped<IWebhookCallerService, WebhookCallerService>();

            services.AddSingleton<IResourceValidationService, ResourceValidationService>();

            return services;
        }

        public static IServiceCollection AddDtoMappers(this IServiceCollection services)
        {
            services.AddScoped<ILpnMapper, LpnMapper>();
            services.AddScoped<IStockMapper, StockMapper>();
            services.AddScoped<IAgenteMapper, AgenteMapper>();
            services.AddScoped<IAgendaMapper, AgendaMapper>();
            services.AddScoped<IEgresoMapper, EgresoMapper>();
            services.AddScoped<IPedidoMapper, PedidoMapper>();
            services.AddScoped<IEmpresaMapper, EmpresaMapper>();
            services.AddScoped<IProductoMapper, ProductoMapper>();
            services.AddScoped<IEjecucionMapper, EjecucionMapper>();
            services.AddScoped<IPreparacionMapper, PreparacionMapper>();
            services.AddScoped<ICodigoBarrasMapper, CodigoBarrasMapper>();
            services.AddScoped<ICrossDockingMapper, CrossDockingMapper>();
            services.AddScoped<IReferenciaRecepcionMapper, ReferenciaRecepcionMapper>();
            services.AddScoped<IProduccionMapper, ProduccionMapper>();
            services.AddScoped<IProducirProduccionMapper, ProducirProduccionMapper>();
            services.AddScoped<IConsumirProduccionMapper, ConsumirProduccionMapper>();
            services.AddScoped<IFacturaMapper, FacturaMapper>();
            services.AddScoped<IPickingProductoMapper, PickingProductoMapper>();

            return services;
        }

        public static IServiceCollection SwaggerServices(this IServiceCollection services, IConfiguration configuration)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddSwaggerGen(options =>
            {
                options.SchemaFilter<WISSchemaFilter>();
                options.DocumentFilter<WISDocumentFilter>();

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

                        options.SwaggerDoc($"WMSAPIDoc-{l.Trim()}", new OpenApiInfo
                        {
                            Version = assembly.GetName().Version?.ToString() ?? "v1.0.0",
                            Title = $"WMS API ({l.Trim()})",
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

                    options.SwaggerDoc($"WMSAPIDoc-{defaultCulture}", new OpenApiInfo
                    {
                        Version = assembly.GetName().Version?.ToString() ?? "v1.0.0",
                        Title = $"WMS API ({defaultCulture})",
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

        public static void LocalizeSwaggerText(XmlNode e,
            IStringLocalizer<Resources> localizer)
        {
            var text = e.InnerText.RemoveNewLineAndTrim();
            var loc = localizer[text];
            if (loc != text)
                e.InnerText = loc;
        }

        public static IStringLocalizer<Resources> GetSwaggerStringLocalizer(IServiceCollection services)
        {
            return services.BuildServiceProvider().GetService<IStringLocalizer<Resources>>();
        }

        public static long GuardarErrores(ActionContext context)
        {
            long nuInterfazEjecucion = 0;
            ActionExecutingContext actionExeContext = (ActionExecutingContext)context;
            object request = null;
            string controllerName = (string)context.RouteData.Values["controller"];
            string method = (string)context.RouteData.Values["action"];

            if (actionExeContext.ActionArguments.ContainsKey("request"))
                request = actionExeContext.ActionArguments["request"];
            else if (actionExeContext.ActionArguments.Keys.Count > 0)
                request = actionExeContext.ActionArguments[actionExeContext.ActionArguments.Keys.First()];

            var loginName = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var uowFactory = context.HttpContext.RequestServices.GetService(typeof(IUnitOfWorkFactory)) as IUnitOfWorkFactory;

            using (var uow = uowFactory.GetUnitOfWork())
            {
                var empresa = (int?)null;
                var idRequest = (string)null;
                string body;

                context.HttpContext.Request.EnableBuffering();
                context.HttpContext.Request.Body.Position = 0;

                using (var reader = new StreamReader(context.HttpContext.Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true))
                {
                    body = reader.ReadToEnd();
                    context.HttpContext.Request.Body.Position = 0;
                }

                var jObject = !string.IsNullOrWhiteSpace(body)? JObject.Parse(body): new JObject();
                empresa = JsonExtension.GetField(jObject, "Empresa")?.Value<int>() ?? 0;
                idRequest = JsonExtension.GetField(jObject, "IdRequest")?.Value<string>();

                var cdInterfazExterna = ParamManager.GetCodigoInterfazExternaByControllerName(uow, controllerName, method, empresa);

                if (cdInterfazExterna.HasValue)
                {
                    var errores = context.ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    nuInterfazEjecucion = Validations.GuardarError(uow, loginName, empresa, idRequest, cdInterfazExterna.Value, controllerName, jObject, errores);
                }
            }

            return nuInterfazEjecucion;
        }
    }
}
