using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using WIS.AutomationManager.Extensions;
using WIS.Domain.General.Filters;
using WIS.Domain.Validation;

namespace WIS.AutomationManager.Filters
{
    public class WISSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (schema.Properties.Count == 0)
                return;

            const BindingFlags bindingFlags = BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Instance;

            var members = context.Type
                .GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(context.Type
                .GetProperties(bindingFlags))
                .ToList();

            var ranged = members
                .Where(m => m.GetCustomAttribute<RangeValidation>() != null)
                .ToList();

            var excluded = members
                .Where(m => m.GetCustomAttribute<SwaggerIgnoreAttribute>() != null)
                .Select(m => (m.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? m.Name.ToCamelCase()));

            foreach (var member in ranged)
            {
                var rangedName = member.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? member.Name.ToCamelCase();
                var range = member.GetCustomAttribute<RangeValidation>();

                if (schema.Properties.ContainsKey(rangedName))
                {
                    var property = schema.Properties[rangedName];

                    if (decimal.TryParse(Convert.ToString(range.Minimum, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, out decimal minimum))
                    {
                        property.Minimum = minimum;
                    }

                    if (decimal.TryParse(Convert.ToString(range.Maximum, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, out decimal maximum))
                    {
                        property.Maximum = maximum;
                    }
                }
            }

            foreach (var excludedName in excluded)
            {
                if (schema.Properties.ContainsKey(excludedName))
                    schema.Properties.Remove(excludedName);
            }
        }
    }
}
