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
    [Route("api/INT/INT103Modal")]
    [ApiController]
    public class INT103ConsultaInterfazReferenciaProveedorDetalleController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly INT103ConsultaInterfazReferenciaProveedorDetalle _controller;

        public INT103ConsultaInterfazReferenciaProveedorDetalleController(IGridControllerInvocation gridInvoker, INT103ConsultaInterfazReferenciaProveedorDetalle controller)
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
