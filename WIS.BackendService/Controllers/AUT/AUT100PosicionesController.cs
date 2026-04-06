using WIS.Application.Controllers.AUT;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.AUT
{
    [Route("api/AUT/AUT100Posiciones")]
    [ApiController]
    public class AUT100PosicionesController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly AUT100Posiciones _controller;

        public AUT100PosicionesController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, AUT100Posiciones controller)
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
