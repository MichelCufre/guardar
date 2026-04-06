using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.General.API.Dtos;

namespace WIS.WMS_API.Filters
{
    public class WISDocumentFilter : IDocumentFilter
    {
        public virtual void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var path in swaggerDoc.Paths)
            {
                foreach (var operation in path.Value.Operations)
                {
                    if (operation.Key == OperationType.Post)
                    {
                        var requestType = GetRequestType(path.Key, operation.Key.ToString(), context);

                        if (requestType != null)
                        {
                            operation.Value.RequestBody = new OpenApiRequestBody
                            {
                                Content = new Dictionary<string, OpenApiMediaType>
                                {
                                    ["application/json"] = new OpenApiMediaType
                                    {
                                        Schema = context.SchemaGenerator.GenerateSchema(requestType.Type, context.SchemaRepository)
                                    }
                                }
                            };
                        }
                    }
                }
            }
        }

        public virtual SwaggerRequestType GetRequestType(string pathKey, string operationKey, DocumentFilterContext context)
        {
            var apiDescription = context.ApiDescriptions.FirstOrDefault(d =>
                d.HttpMethod.Equals(operationKey, StringComparison.OrdinalIgnoreCase) &&
                "/" + d.RelativePath.TrimEnd('/') == pathKey);

            if (apiDescription == null)
                return null;

            var actionDescriptor = apiDescription.ActionDescriptor as ControllerActionDescriptor;
            var methodInfo = actionDescriptor?.MethodInfo;

            if (methodInfo == null)
                return null;

            return methodInfo.GetCustomAttributes(typeof(SwaggerRequestType), false)
                .FirstOrDefault() as SwaggerRequestType;
        }
    }
}
