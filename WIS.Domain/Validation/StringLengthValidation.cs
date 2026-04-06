using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class StringLengthValidation : StringLengthAttribute
    {

        public StringLengthValidation(int maximumLength) : base(maximumLength)
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var result = base.IsValid(value, validationContext);
            
            if (result != null)
            {
                var resourceValidationService = validationContext.GetService(typeof(IResourceValidationService)) as IResourceValidationService;

                var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                var loginName = httpContextAccessor?.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                Error error = new Error("WMSAPI_msg_Error_LargoStringValidation", validationContext.MemberName, MinimumLength, MaximumLength);
                ErrorMessage = resourceValidationService.Translate(loginName, error);
            }

            return result;
        }
    }
}
