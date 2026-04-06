using System.ComponentModel.DataAnnotations;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class InterfazEjecucionValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int? cdInterfazExterna = (int?)validationContext.ObjectInstance.GetType().GetProperty("CodigoInterfazExterna").GetValue(validationContext.ObjectInstance, null);

            if (value != null && (long)value >= 0)
            {
                var uowFactory = validationContext.GetService(typeof(IUnitOfWorkFactory)) as IUnitOfWorkFactory;
                var validationService = validationContext.GetService(typeof(IValidationService)) as IValidationService;

                using (var uow = uowFactory.GetUnitOfWork())
                {
                    var interfaz = uow.EjecucionRepository.GetEjecucion((long)value).Result;
                    Error error = null;
                    string errorMessage;

                    if (interfaz == null)
                    {
                        error = new Error("WMSAPI_msg_Error_InterfazNoExiste", value);
                        errorMessage = validationService.Translate(error);

                        return new ValidationResult(errorMessage);
                    }
                    else if (interfaz.CdInterfazExterna != cdInterfazExterna)
                    {
                        error = new Error("WMSAPI_msg_Error_InterfazExternaNoCoincide", cdInterfazExterna, interfaz.Id);
                        errorMessage = validationService.Translate(error);

                        return new ValidationResult(errorMessage);
                    }
                    else if (cdInterfazExterna != null && interfaz.Situacion != SituacionDb.ProcesadoPendiente)
					{
                        error = new Error("WMSAPI_msg_Error_EstadoInterfazIncorrecto", SituacionDb.ProcesadoPendiente);
                        errorMessage = validationService.Translate(error);

                        return new ValidationResult(errorMessage);
					}
                }
            }

            return ValidationResult.Success;
        }
    }
}
