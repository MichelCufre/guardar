using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Validation
{
    public class EjecucionAccessValidation : ActionFilterAttribute
    {
        public EjecucionAccessValidation()
        {
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool isValid = true;

            var request = context.ActionArguments["request"];
            var empresa = (int)request.GetType().GetProperty("Empresa").GetValue(request);
            var cdInterfazExterna = (int)request.GetType().GetProperty("CodigoInterfazExterna").GetValue(request);

            var loginName = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var uowFactory = context.HttpContext.RequestServices.GetService(typeof(IUnitOfWorkFactory)) as IUnitOfWorkFactory;
            var validationService = context.HttpContext.RequestServices.GetService(typeof(IValidationService)) as IValidationService;

            Error error = null;
            using (var uow = uowFactory.GetUnitOfWork())
            {
                var cdParametroInterfaz = new ParamManager().GetParamInterfazHabilitada(uow, cdInterfazExterna, empresa);
                var interfazHabilitada = (uow.ParametroRepository.GetParametro(cdParametroInterfaz, ParamManager.PARAM_EMPR, empresa.ToString()).Result ?? "N") == "S";

                if (!interfazHabilitada)
                {
                    isValid = false;
                    error = new Error("WMSAPI_msg_Error_EmpresaInterfazInhabilitada", cdInterfazExterna, empresa);
                }
                else if (!uow.EmpresaRepository.EmpresaAsignada(empresa, loginName).Result)
                {
                    isValid = false;
                    error = new Error("WMSAPI_msg_Error_EmpresaNoAsignada", loginName, empresa);
                }
            }

            if (!isValid)
                context.Result = new ObjectResult(validationService.Translate(error)) { StatusCode = 401 };
            else
                base.OnActionExecuting(context);
        }

    }
}
