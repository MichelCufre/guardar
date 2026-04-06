using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PAR;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PAR
{
    [Route("api/PAR/PAR401ModificarAtributoTipoLpnDet")]
    [ApiController]
    public class PAR401ModificarLpnTipoAtributoDetController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly PAR401ModificarLpnTipoAtributoDet _controller;

        public PAR401ModificarLpnTipoAtributoDetController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, PAR401ModificarLpnTipoAtributoDet controller)
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
