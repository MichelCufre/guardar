using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG700UbicacionesSinAsociar")]
    [ApiController]
    public class REG700PanelRecorridosController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly REG700UbicacionesSinAsociar _controller;

        public REG700PanelRecorridosController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, REG700UbicacionesSinAsociar controller)
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
