using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REC;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REC
{
    [Route("api/REC/REC170RecepcionLpn")]
    [ApiController]
    public class REC170RecepcionLpnController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly REC170RecepcionLpn _controller;

        public REC170RecepcionLpnController(IGridControllerInvocation gridInvoker, IPageControllerInvocation pageInvoker, REC170RecepcionLpn controller)
        {
            _gridInvoker = gridInvoker;
            _pageInvoker = pageInvoker;
            _controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Page(PageWrapper datas)
        {
            return Ok(this._pageInvoker.Invoke(datas, this._controller));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper datas)
        {
            return Ok(this._gridInvoker.Invoke(datas, this._controller));
        }
    }
}
