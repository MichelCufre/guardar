using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PRO;
using WIS.Application.Invocation;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRE
{
	[Route("api/PRO/PRO030")]
    [ApiController]
    public class PRO030IndicadoresController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly PRO030Indicadores _controller;

        public PRO030IndicadoresController(
            IPageControllerInvocation pageInvoker,
            IGridControllerInvocation gridInvoker,
            PRO030Indicadores controller)
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

    }
}
