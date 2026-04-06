using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class RequiredListValidation : ValidationAttribute
    {
        private readonly bool _detalles = false;

        public RequiredListValidation(bool detalles)
        {
            _detalles = detalles;
        }

        public RequiredListValidation()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var list = value as IList;

            if (list == null || list.Count == 0)
            {
                Error error = null;
                var resourceValidationService = validationContext.GetService(typeof(IResourceValidationService)) as IResourceValidationService;

                var httpContextAccessor = validationContext.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
                var loginName = httpContextAccessor?.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (_detalles)
				{
                    error = new Error("WMSAPI_msg_Error_DetallesRequeridos");
                    return new ValidationResult(resourceValidationService.Translate(loginName, error));
				}
				else
				{
                    error = new Error("WMSAPI_msg_Error_ListaDeObjetosRequerida");
                    return new ValidationResult(resourceValidationService.Translate(loginName, error));
				}
            }

            return ValidationResult.Success;
        }
    }
}
