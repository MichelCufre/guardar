using System.ComponentModel.DataAnnotations;
using WIS.Domain.DataModel;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class ExisteEmpresaValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && (int)value >= 0)
            {
                var uowFactory = validationContext.GetService(typeof(IUnitOfWorkFactory)) as IUnitOfWorkFactory;
                var validationService = validationContext.GetService(typeof(IValidationService)) as IValidationService;

                using (var uow = uowFactory.GetUnitOfWork())
                {
                    if (uow.EmpresaRepository.GetEmpresa((int)value) == null)
                    {
                        Error error = new Error("WMSAPI_msg_Error_ExisteEmpresaValidation", value);
                        var msgError = validationService.Translate(error);
                        return new ValidationResult(msgError);
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
