using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PRE;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRE
{
    [Route("api/PRE/PRE100DetallePedidoAtributo")]
    [ApiController]
    public class PRE100DetallePedidoAtributoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly PRE100DetallePedidoAtributo _controller;

        public PRE100DetallePedidoAtributoController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, PRE100DetallePedidoAtributo controller)
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
