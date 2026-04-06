using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REC;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REC
{
    [Route("api/REC/REC170SeleccionLpn")]
    [ApiController]
    public class REC170SeleccionLpnController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly REC170SeleccionLpn _controller;

        public REC170SeleccionLpnController(IGridControllerInvocation gridInvoker, IPageControllerInvocation pageInvoker, REC170SeleccionLpn controller)
        {
            _gridInvoker = gridInvoker;
            _pageInvoker = pageInvoker;
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
