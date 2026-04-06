using System.ComponentModel.DataAnnotations;
using WIS.Domain.DataModel;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class IdProduccionExternoValidation : ValidationAttribute
    {

        public IdProduccionExternoValidation()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                var empresaProperty = validationContext.ObjectType.GetProperty("Empresa");
                if (empresaProperty != null)
                {
                    var uowFactory = validationContext.GetService(typeof(IUnitOfWorkFactory)) as IUnitOfWorkFactory;
                    var validationService = validationContext.GetService(typeof(IValidationService)) as IValidationService;

                    using (var uow = uowFactory.GetUnitOfWork())
                    {
                        var empresa = (int)empresaProperty.GetValue(validationContext.ObjectInstance);
                        if (uow.EmpresaRepository.GetEmpresa(empresa) == null)
                        {
                            Error error = new Error("WMSAPI_msg_Error_ExisteEmpresaValidation", empresa);
                            var msgError = validationService.Translate(error);
                            return new ValidationResult(msgError);
                        }

                        if (!uow.IngresoProduccionRepository.ExisteIngresoByIdExternoEmpresa(value.ToString(), empresa))
                        {
                            Error error = new Error("General_Sec0_Error_Error98", value, empresa);
                            var msgError = validationService.Translate(error);
                            return new ValidationResult(msgError);
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
