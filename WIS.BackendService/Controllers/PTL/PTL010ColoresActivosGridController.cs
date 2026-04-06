using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PTL;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PTL
{
    [Route("api/PTL/PTL010ColoresActivos")]
    [ApiController]
    public class PTL010ColoresActivosGridController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly PTL010ColoresActivos _controller;

        public PTL010ColoresActivosGridController(IGridControllerInvocation gridInvoker, PTL010ColoresActivos controller)
        {
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
