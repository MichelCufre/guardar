using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG700AsociarAplicacionUsuario")]
    [ApiController]
    public class REG700AsociarAplicacionUsuarioController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly REG700AsociarAplicacionUsuario _controller;

        public REG700AsociarAplicacionUsuarioController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, REG700AsociarAplicacionUsuario controller)
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
