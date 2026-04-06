using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PRE;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRE
{
    [Route("api/PRE/PRE100DefinicionAtributo")]
    [ApiController]
    public class PRE100DefinicionAtributoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly IFormControllerInvocation _formInvoker;

        private readonly PRE100DefinicionAtributo _controller;

        public PRE100DefinicionAtributoController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, IFormControllerInvocation formInvoker, PRE100DefinicionAtributo controller)
        {
            this._pageInvoker = pageInvoker;
            this._gridInvoker = gridInvoker;
            this._controller = controller;
            this._formInvoker = formInvoker;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Page(PageWrapper data)
        {
            return Ok(this._pageInvoker.Invoke(data, this._controller));
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }
    }
}
