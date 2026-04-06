using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class InterfazExternaValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && (int)value >= 0)
            {
                var uowFactory = validationContext.GetService(typeof(IUnitOfWorkFactory)) as IUnitOfWorkFactory;
                var validationService = validationContext.GetService(typeof(IValidationService)) as IValidationService;

                using (var uow = uowFactory.GetUnitOfWork())
                {
                    if (!uow.EjecucionRepository.ExisteIntefazExterna((int)value).Result)
					{
                        Error error = new Error("WMSAPI_msg_Error_InterfazExternaNoExiste", value);
                        return new ValidationResult(validationService.Translate(error));
					}
                }
            }

            return ValidationResult.Success;
        }
    }
}
