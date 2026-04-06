using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Validation
{
    public class LpnAccessValidation : ActionFilterAttribute
    {
        protected readonly int _interfazExterna;

        public LpnAccessValidation(int interfazExterna)
        {
            _interfazExterna = interfazExterna;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            bool isValid = true;

            var loginName = context.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var controllerName = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ControllerName;
            var method = ((ControllerBase)context.Controller).ControllerContext.ActionDescriptor.ActionName;

            var uowFactory = context.HttpContext.RequestServices.GetService(typeof(IUnitOfWorkFactory)) as IUnitOfWorkFactory;
            var validationService = context.HttpContext.RequestServices.GetService(typeof(IValidationService)) as IValidationService;

            Error error = null;
            using (var uow = uowFactory.GetUnitOfWork())
            {
                Lpn lpn = null;

                context.ActionArguments.TryGetValue("nuLpn", out object nuLpn);
                context.ActionArguments.TryGetValue("idExterno", out object idExterno);
                context.ActionArguments.TryGetValue("tpLpn", out object tpLpn);

                if (nuLpn != null)
                {
                    lpn = uow.ManejoLpnRepository.GetLpnOrNull((long)nuLpn).Result;
                }
                else if (!string.IsNullOrEmpty(idExterno?.ToString()) &&
                         !string.IsNullOrEmpty(tpLpn?.ToString()))
                {
                    lpn = uow.ManejoLpnRepository.GetLpnOrNull(idExterno.ToString(), tpLpn.ToString()).Result;
                }

                if (lpn == null)
                {
                    isValid = false;
                    if (nuLpn != null)
                        error = new Error("WMSAPI_msg_Error_LpnNoEncontrado", nuLpn);
                    else
                        error = new Error("WMSAPI_msg_Error_LpnExternoTipoNoEncontrado", new object[] { idExterno, tpLpn });
                }
                else
                {
                    int empresa = lpn.Empresa;
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
