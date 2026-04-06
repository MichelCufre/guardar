using WIS.Application.Controllers.EXP;
using WIS.Application.Controllers.PRE;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP150PedidoComplSinEmpaque")]
    [ApiController]
    public class EXP150PedidosCompleteSinEmpaquetarController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EXP150PedidoComplSinEmpaque _controller;

        public EXP150PedidosCompleteSinEmpaquetarController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker,
                                EXP150PedidoComplSinEmpaque controller)
        {
            this._pageInvoker = pageInvoker;
            _gridInvoker = gridInvoker;
            _controller = controller;
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