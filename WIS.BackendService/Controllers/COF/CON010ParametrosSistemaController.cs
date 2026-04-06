using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Application.Controllers.COF;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.COF
{
    [Route("api/CON/CON010")]
    [ApiController]
    public class CON010ParametrosSistemaController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly CON010ParametrosSistema _controller;

        public CON010ParametrosSistemaController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, CON010ParametrosSistema controller)
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
