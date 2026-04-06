using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using WIS.Domain.Services.Interfaces;
using WIS.Security;
using WIS.Security.Models;

namespace WIS.Domain.Validation
{
    public class RequiredValidation : RequiredAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = base.IsValid(value, validationContext);

            if (result != null)
            {
                var resourceValidationService = validationContext.GetService(typeof(IResourceValidationService)) as IResourceValidationService;
                
                var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                var loginName = httpContextAccessor?.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                var error = new Error("WMSAPI_msg_Error_RequeridoValidation", validationContext.MemberName);
                ErrorMessage = resourceValidationService.Translate(loginName, error);
            }

            return result;
        }
    }
}
