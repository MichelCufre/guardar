
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Application.Controllers.FAC;
using WIS.Application.Controllers.ORT;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;


namespace WIS.BackendService.Controllers.FAC
{
    [Route("api/ORT/ORT070")]
    [ApiController]
    public class ORT070OrdenTareaManipuleoInsumoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly ORT070OrdenTareaManipuleoInsumo _controller;

        public ORT070OrdenTareaManipuleoInsumoController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, ORT070OrdenTareaManipuleoInsumo controller)
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
