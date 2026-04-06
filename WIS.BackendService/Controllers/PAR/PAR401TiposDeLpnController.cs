using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PAR;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PAR
{
    [Route("api/PAR/PAR401")]
    [ApiController]
    public class PAR401TiposDeLpnController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly PAR401TiposDeLpn _controller;

        public PAR401TiposDeLpnController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, PAR401TiposDeLpn controller)
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
