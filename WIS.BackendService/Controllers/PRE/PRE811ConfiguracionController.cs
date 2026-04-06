using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PRE;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRE
{
    [Route("api/PRE/PRE811Configuracion")]
    [ApiController]
    public class PRE811ConfiguracionController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly PRE811Configuracion _controller;

        public PRE811ConfiguracionController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, IFormControllerInvocation formInvoker, PRE811Configuracion controller)
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
