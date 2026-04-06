using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.FUC;
using WIS.Application.Controllers.PRD;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.FUC
{
    [Route("api/FUC/FUC001")]
    [ApiController]
    public class FUC001Controller : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly FUC001 _controller;

        public FUC001Controller(
            IPageControllerInvocation pageInvoker,
            IGridControllerInvocation gridInvoker,
            FUC001 controller)
        {
            this._pageInvoker = pageInvoker;
            this._gridInvoker = gridInvoker;
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
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
    }
}
