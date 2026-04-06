using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PAR;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PAR
{
    [Route("api/PAR/PAR401ModificarAtributoTipoLpn")]
    [ApiController]
    public class PAR401ModificarLpnTipoAtributoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly PAR401ModificarLpnTipoAtributo _controller;

        public PAR401ModificarLpnTipoAtributoController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, PAR401ModificarLpnTipoAtributo controller)
        {
            this._pageInvoker = pageInvoker;
            this._formInvoker = formInvoker;
            this._controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Page(PageWrapper data)
        {
            return Ok(this._pageInvoker.Invoke(data, this._controller));
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }
    }
}
