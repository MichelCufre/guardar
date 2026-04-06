using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EXP;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP110InfPedidoEmpaque")]
    [ApiController]
    public class EXP110InformacionPedidoMesaEmpaqueController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EXP110InformacionPedidoMesaEmpaque _controller;

        public EXP110InformacionPedidoMesaEmpaqueController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker,
                                EXP110InformacionPedidoMesaEmpaque controller)
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
