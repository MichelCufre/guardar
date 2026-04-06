using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.INT;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.INT
{
    [Route("api/INT/INT100Modal")]
    [ApiController]
    public class INT100ConsultaInterfazPedidosSalidaDetalleController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly INT100ConsultaInterfazPedidosSalidaDetalle _controller;

        public INT100ConsultaInterfazPedidosSalidaDetalleController(IGridControllerInvocation gridInvoker, INT100ConsultaInterfazPedidosSalidaDetalle controller)
        {
            this._gridInvoker = gridInvoker;
            this._controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
    }
}
