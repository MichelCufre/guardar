using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PAR;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PAR
{
    [Route("api/PAR/PAR050")]
    [ApiController]
    public class PAR050TiposDeUbicacionesController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly PAR050TiposDeUbicaciones _controller;

        public PAR050TiposDeUbicacionesController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, PAR050TiposDeUbicaciones controller)
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
