using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
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
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using WIS.API.Extension.Extensions;
using WIS.API.Extension.Middlewares;
using WIS.Common.API.Attributes;
using WIS.WebhookClient.Extensions;

namespace WIS.WebhookClient
{
    public class Startup
    {
        Logger _logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private void LocalizeSwaggerText(XmlNode e,
            IStringLocalizer<Resources> localizer)
        {
            var text = e.InnerText.RemoveNewLineAndTrim();
            var loc = localizer[text];
            if (loc != text)
                e.InnerText = loc;
        }

        private IStringLocalizer<Resources> GetSwaggerStringLocalizer(IServiceCollection services)
        {
            return services.BuildServiceProvider().GetService<IStringLocalizer<Resources>>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
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

            services.AddSwaggerGen(options =>
            {
                var list = Configuration.GetSection("Globalization:SupportedCultures").Get<List<string>>().Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
                var defaultCulture = Configuration["Globalization:DefaultCulture"];
                bool.TryParse(Configuration["Globalization:Swagger:EnableMultilanguageDocumentation"], out bool enableMultiLangSwagger);
                bool.TryParse(Configuration["Globalization:Swagger:EnableLocalizedDocumentation"], out bool enableLocalizedDoc);

                var localizer = GetSwaggerStringLocalizer(services);
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (enableMultiLangSwagger && list != null && list.Count > 1)
                {
                    foreach (var l in list)
                    {
                        Thread.CurrentThread.SetCulture(l);

                        options.SwaggerDoc($"WebhookClientDoc-{l.Trim()}", new OpenApiInfo
                        {
                            Version = assembly.GetName().Version?.ToString() ?? "v1.0.0",
                            Title = $"Webhook Client ({l.Trim()})",
                            Description = localizer["swagger_description"],
                            Contact = new OpenApiContact
                            {
                                Name = Configuration.GetValue<string>("SwaggerSettings:ContactName"),
                                Url = new Uri(Configuration.GetValue<string>("SwaggerSettings:ContactUrl"))
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

                    options.SwaggerDoc($"WebhookClientDoc-{defaultCulture}", new OpenApiInfo
                    {
                        Version = assembly.GetName().Version?.ToString() ?? "v1.0.0",
                        Title = $"Webhook Client ({defaultCulture})",
                        Description = localizer["swagger_description"],
                        Contact = new OpenApiContact
                        {
                            Name = Configuration.GetValue<string>("SwaggerSettings:ContactName"),
                            Url = new Uri(Configuration.GetValue<string>("SwaggerSettings:ContactUrl"))
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
            });

            services.AddHttpContextAccessor();
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
                        c.SwaggerEndpoint($"/swagger/WebhookClientDoc-{l.Trim()}/swagger.json?lang={l.Trim()}", $"Webhook Client ({l.Trim()})");
                    }

                    if (enableUITranslation)
                        c.HeadContent = "<script src='./script/jquery-3.6.3.min.js'></script><script src='./script/jquery.initialize.min.js'></script><script src='./script/translate/translate.js'></script>";
                }
                else
                {
                    c.SwaggerEndpoint($"/swagger/WebhookClientDoc-{defaultCulture}/swagger.json?lang={defaultCulture}", $"Webhook Client ({defaultCulture})");

                    if (!string.IsNullOrWhiteSpace(defaultCulture) && enableUITranslation)
                        c.HeadContent = "<script src='./script/jquery-3.6.3.min.js'></script><script src='./jscript/query.initialize.min.js'></script><script id='languagefile' src='./script/translate/" + defaultCulture.Trim() + ".js'></script><script src='./script/translate/translate.js'></script>";
                }

                c.HeadContent += "<script src='./script/rapipdf-min.js'></script><script src='./script/custom.js'></script>";
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Use(next => context => {
                context.Request.EnableBuffering();
                return next(context);
            });

            app.ConfigureExceptionHandler(_logger);
        }
    }
}
