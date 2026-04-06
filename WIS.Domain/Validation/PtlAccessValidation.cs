using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WIS.Domain.Validation
{
    public class PtlAccessValidation : TypeFilterAttribute
    {
        public PtlAccessValidation() : base(typeof(PtlActionFilter))
        {

        }

        private class PtlActionFilter : IActionFilter
        {
            public void OnActionExecuting(ActionExecutingContext context)
            {
                var isValid = true;
                var msgError = string.Empty;

                if (!context.HttpContext.User.Identity.IsAuthenticated)
                {
                    isValid = false;
                    msgError = ValidationMessage.Ptl_msg_Error_UsuarioNoAutenticado;
                }

                if (!isValid)
                {
                    context.Result = new ObjectResult(msgError) { StatusCode = 401 };
                }
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {

            }
        }
    }
}
