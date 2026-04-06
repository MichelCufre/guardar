using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EVT;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EVT
{
    [Route("api/EVT/EVT030")]
    [ApiController]
    public class EVT030GruposContactoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EVT030GruposContacto _controller;

        public EVT030GruposContactoController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, EVT030GruposContacto controller)
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
