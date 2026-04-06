using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EXP;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP110ProdCont")]
    [ApiController]
    public class EXP110PedidoProductoContenedorController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EXP110PedidoProductoContenedor _controller;

        public EXP110PedidoProductoContenedorController(IGridControllerInvocation gridInvoker,
                                EXP110PedidoProductoContenedor controller)
        {
            _gridInvoker = gridInvoker;
            _controller = controller;
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
    }
}
