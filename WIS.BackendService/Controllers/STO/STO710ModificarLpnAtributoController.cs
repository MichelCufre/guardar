using Microsoft.AspNetCore.Mvc;

using WIS.Application.Controllers.STO;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.STO
{
    [Route("api/STO/STO710ModificarAtributoLpn")]
    [ApiController]
    public class STO710ModificarLpnAtributoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly STO710ModificarLpnAtributo _controller;

        public STO710ModificarLpnAtributoController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, STO710ModificarLpnAtributo controller)
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
