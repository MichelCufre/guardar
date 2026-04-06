using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG220Rutas")]
    [ApiController]
    public class REG220RutasAgenteController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly REG220RutasAgente _controller;

        public REG220RutasAgenteController(IGridControllerInvocation gridInvoker, REG220RutasAgente controller)
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
