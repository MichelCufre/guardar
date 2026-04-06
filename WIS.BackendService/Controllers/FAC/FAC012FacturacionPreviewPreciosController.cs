using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.FAC;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.FAC
{
    [Route("api/FAC/FAC012")]
    [ApiController]
    public class FAC012FacturacionPreviewPreciosController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly FAC012FacturacionPreviewPrecios _controller;

        public FAC012FacturacionPreviewPreciosController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, FAC012FacturacionPreviewPrecios controller)
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
