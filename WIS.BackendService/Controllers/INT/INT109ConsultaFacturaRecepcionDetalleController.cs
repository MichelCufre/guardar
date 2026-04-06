using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.INT;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.INT
{
    [Route("api/INT/INT109Modal")]
    [ApiController]
    public class INT109ConsultaFacturaRecepcionDetalleController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly INT109ConsultaFacturaRecepcionDetalle _controller;

        public INT109ConsultaFacturaRecepcionDetalleController(IGridControllerInvocation gridInvoker, INT109ConsultaFacturaRecepcionDetalle controller)
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
