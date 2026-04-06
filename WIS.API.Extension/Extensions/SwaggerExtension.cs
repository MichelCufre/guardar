using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using WIS.Common.Attributes;

namespace WIS.API.Extension.Extensions
{
    public class CustomRequestBodyFilter<T> : IRequestBodyFilter
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<T> _localizer;

        public CustomRequestBodyFilter(IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor, 
            IStringLocalizer<T> localizer)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public void Apply(OpenApiRequestBody requestBody, RequestBodyFilterContext context)
        {
            string queryLang = (_httpContextAccessor?.HttpContext?.Request?.Query["lang"])?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(queryLang))
                Thread.CurrentThread.SetCulture(queryLang);
            else
            {
                string culture = _configuration["Globalization:DefaultCulture"];
                Thread.CurrentThread.SetCulture(culture);
            }

            var descr = requestBody.Description.RemoveNewLineAndTrim();
            if (!string.IsNullOrWhiteSpace(descr))
            {
                var locDescr = _localizer[descr];
                if (locDescr != descr)
                    requestBody.Description = locDescr;
            }
        }
    }

    public class CustomParameterFilter<T> : IParameterFilter
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<T> _localizer;

        public CustomParameterFilter(IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor, 
            IStringLocalizer<T> localizer)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;

        }

        public void Apply(OpenApiParameter parameter, ParameterFilterContext context)
        {
            string queryLang = (_httpContextAccessor?.HttpContext?.Request?.Query["lang"])?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(queryLang))
                Thread.CurrentThread.SetCulture(queryLang);
            else
            {
                string culture = _configuration["Globalization:DefaultCulture"];
                Thread.CurrentThread.SetCulture(culture);
            }

            var descr = parameter.Description.RemoveNewLineAndTrim();
            if (!string.IsNullOrWhiteSpace(descr))
            {
                var locDescr = _localizer[descr];
                if (locDescr != descr)
                    parameter.Description = locDescr;
            }
        }
    }

    public class CustomOperationFilter<T> : IOperationFilter
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<T> _localizer;
        const string captureName = "routeParameter";

        public CustomOperationFilter(IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor, 
            IStringLocalizer<T> localizer)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;

        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {

            var list = _configuration.GetSection("Globalization:SupportedCultures").Get<List<string>>().Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            bool.TryParse(_configuration["Globalization:Swagger:EnableMultilanguageDocumentation"], out bool enableMultiLangSwagger);
            bool.TryParse(_configuration["Globalization:Swagger:EnableLocalizedDocumentation"], out bool enableLocalizedDoc);
            bool.TryParse(_configuration["Globalization:Swagger:EnableAcceptLanguageParam"], out bool enablesAccLang);
            string defaultCulture = null;

            if (enableMultiLangSwagger && list != null && list.Count > 1 && enableLocalizedDoc)
            {
                string queryLang = (_httpContextAccessor?.HttpContext?.Request?.Query["lang"])?.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(queryLang))
                {
                    defaultCulture = queryLang;
                    Thread.CurrentThread.SetCulture(queryLang);

                }
                else
                {
                    string culture = _configuration["Globalization:DefaultCulture"];
                    if (!string.IsNullOrWhiteSpace(culture))
                    {
                        defaultCulture = culture;
                        Thread.CurrentThread.SetCulture(culture);
                    }
                }

                var summary = operation.Summary.RemoveNewLineAndTrim();
                if (!string.IsNullOrWhiteSpace(summary))
                {
                    var loc = _localizer[summary];
                    if (loc != summary)
                        operation.Summary = loc;
                }

                var descr = operation.Description.RemoveNewLineAndTrim();
                if (!string.IsNullOrWhiteSpace(descr))
                {
                    var locDescr = _localizer[descr];
                    if (locDescr != descr)
                        operation.Description = locDescr;
                }

                if (operation.Responses != null && operation.Responses.Count > 0)
                {
                    foreach (var r in operation.Responses)
                    {
                        var v = r.Value;
                        if (v != null)
                        {
                            var desc = v.Description.RemoveNewLineAndTrim();
                            if (!string.IsNullOrWhiteSpace(desc))
                            {
                                var loc = _localizer[desc];
                                if (loc != desc)
                                    v.Description = loc;
                            }
                        }
                    }
                }

                if (operation.RequestBody != null)
                {
                    var desc = operation.RequestBody.Description.RemoveNewLineAndTrim();
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        var loc = _localizer[desc];
                        if (loc != desc)
                            operation.RequestBody.Description = loc;
                    }
                }

                if (operation.Parameters != null && operation.Parameters.Count > 0)
                {
                    foreach (var r in operation.Parameters)
                    {

                        var desc = r.Description.RemoveNewLineAndTrim();
                        if (!string.IsNullOrWhiteSpace(desc))
                        {
                            var loc = _localizer[desc];
                            if (loc != desc)
                                r.Description = loc;
                        }
                    }
                }
            }
            else
            {
                defaultCulture = _configuration["Globalization:DefaultCulture"];
                if (!string.IsNullOrWhiteSpace(defaultCulture) && enablesAccLang && list != null && list.Count > 0)
                    Thread.CurrentThread.SetCulture(defaultCulture);
            }

            if (enablesAccLang && list != null && list.Count > 0)
            {

                if (!string.IsNullOrWhiteSpace(defaultCulture)) //put default culture in the first position
                    list.Move(x => x.ToLower().Trim() == defaultCulture.Trim().ToLower(), 0);


                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "accept-language",
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = enableLocalizedDoc ? _localizer["supported_languages"] : SwaggerDescr.SupportedLanguages,

                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                        Enum = list.Select(value => (IOpenApiAny)new OpenApiString(value))
                                            .ToList()
                    }

                });
            }

            var httpMethodAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<Microsoft.AspNetCore.Mvc.RouteAttribute>();

            var httpMethodWithOptional = httpMethodAttributes?.FirstOrDefault(m => m.Template != null && m.Template.Contains("?"));
            if (httpMethodWithOptional == null)
                return;

            string regex = $"{{(?<{captureName}>\\w+)\\?}}";

            var matches = System.Text.RegularExpressions.Regex.Matches(httpMethodWithOptional.Template, regex);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var name = match.Groups[captureName].Value;

                var parameter = operation.Parameters.FirstOrDefault(p => p.In == ParameterLocation.Path && p.Name == name);
                if (parameter != null)
                {
                    parameter.AllowEmptyValue = true;
                    parameter.Required = false;
                    parameter.Schema.Nullable = true;
                }
            }
        }
    }

    public class CustomDocumentFilter<T> : IDocumentFilter
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<T> _localizer;

        public CustomDocumentFilter(IConfiguration configuration, 
            IHttpContextAccessor httpContextAccessor, 
            IStringLocalizer<T> localizer)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            string queryLang = (_httpContextAccessor?.HttpContext?.Request?.Query["lang"])?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(queryLang))
                Thread.CurrentThread.SetCulture(queryLang);
            else
            {
                string culture = _configuration["Globalization:DefaultCulture"];
                Thread.CurrentThread.SetCulture(culture);
            }

            // workaround for document tags
            var tags = swaggerDoc.Paths.Values.SelectMany(p => p.Operations.Values).SelectMany(o => o.Tags);
            foreach (var t in tags)
            {
                t.Description = _localizer["swagger_tag_" + t.Name.ToLower()];
                swaggerDoc.Tags.Add(t);
            }
        }
    }

    public class CustomSchemaFilter<T1, T2> : ISchemaFilter where T2 : TextAttribute
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<T1> _localizer;

        public CustomSchemaFilter(IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            IStringLocalizer<T1> localizer)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            string queryLang = (_httpContextAccessor?.HttpContext?.Request?.Query["lang"])?.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(queryLang))
                Thread.CurrentThread.SetCulture(queryLang);
            else
            {
                string culture = _configuration["Globalization:DefaultCulture"];
                Thread.CurrentThread.SetCulture(culture);
            }

            if (context.MemberInfo != null)
            {
                var declaringType = context.MemberInfo.DeclaringType.Name.ToLower();
                var memberName = context.MemberInfo.Name.ToLower();
                var example = context.MemberInfo.GetCustomAttribute<T2>();

                schema.Description = _localizer[$"swagger_summary_{declaringType}_{memberName}"];

                if (example != null)
                {
                    schema.Example = new OpenApiString(example.Value);
                }
            }
        }
    }
}
