using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EXP;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP110ContenedoresPedido")]
    [ApiController]
    public class EXP110ContenedoresPedidoController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EXP110ContenedoresPedido _controller;

        public EXP110ContenedoresPedidoController(IGridControllerInvocation gridInvoker,
                                EXP110ContenedoresPedido controller)
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
