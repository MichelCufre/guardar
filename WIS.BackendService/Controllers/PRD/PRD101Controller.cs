using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PRD;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
    [Route("api/PRD/PRD101")]
    [ApiController]
    public class PRD101Controller : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly PRD101 _controller;

        public PRD101Controller(
            IPageControllerInvocation pageInvoker,
            IFormControllerInvocation formInvoker,
            IGridControllerInvocation gridInvoker,
            PRD101 controller)
        {
            this._pageInvoker = pageInvoker;
            this._formInvoker = formInvoker;
            this._gridInvoker = gridInvoker;
            this._controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
    }
}
