using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG700AsociarAplicacion")]
    [ApiController]
    public class REG700AsociarAplicacionController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly REG700AsociarAplicacion _controller;

        public REG700AsociarAplicacionController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, REG700AsociarAplicacion controller)
        {
            this._pageInvoker = pageInvoker;
            this._gridInvoker = gridInvoker;
            this._controller = controller;
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Page(PageWrapper data)
        {
            return Ok(this._pageInvoker.Invoke(data, this._controller));
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
    }
}
