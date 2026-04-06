using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class ReferenciaAccessValidation : ActionFilterAttribute
    {
        protected readonly int _interfazExterna;

        public ReferenciaAccessValidation(int interfazExterna)
        {
            _interfazExterna = interfazExterna;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool isValid = true;

            int idReferencia = (int)context.ActionArguments["idReferencia"];
            var loginName = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var uowFactory = context.HttpContext.RequestServices.GetService(typeof(IUnitOfWorkFactory)) as IUnitOfWorkFactory;
            var validationService = context.HttpContext.RequestServices.GetService(typeof(IValidationService)) as IValidationService;

            Error error = null;
            using (var uow = uowFactory.GetUnitOfWork())
            {
                var referencia = uow.ReferenciaRecepcionRepository.GetReferenciaOrNull(idReferencia).Result;

                if (referencia == null)
                {
                    isValid = false;
                    error = new Error("WMSAPI_msg_Error_ReferenciaIDNoEncontrada", idReferencia);
                }
                else
                {
                    int empresa = referencia.IdEmpresa;
                    var interfazHabilitada = ParamManager.GetParamInterfazHabilitada(uow, _interfazExterna, empresa);

                    if (!interfazHabilitada)
                    {
                        isValid = false;
                        error = new Error("WMSAPI_msg_Error_EmpresaInterfazInhabilitada", _interfazExterna, empresa);
                    }
                    else if (!uow.EmpresaRepository.EmpresaAsignada(empresa, loginName).Result)
                    {
                        isValid = false;
                        error = new Error("WMSAPI_msg_Error_EmpresaNoAsignada", loginName, empresa);
                    }
                }
            }

            if (!isValid)
                context.Result = new ObjectResult(validationService.Translate(error)) { StatusCode = 401 };
            else
                base.OnActionExecuting(context);
        }

    }
}
