using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PTL;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PTL
{
    [Route("api/PTL/PTL010NotiPTLSubClasse")]
    [ApiController]
    public class PTL010NotificarPTLAgrupacionSubClaseProductoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly PTL010NotificarPTLAgrupacionSubClaseProducto _controller;

        public PTL010NotificarPTLAgrupacionSubClaseProductoController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, PTL010NotificarPTLAgrupacionSubClaseProducto controller)
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
