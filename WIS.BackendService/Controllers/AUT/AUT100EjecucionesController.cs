using WIS.Application.Controllers.AUT;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.AUT
{
    [Route("api/AUT/AUT100Ejecuciones")]
    [ApiController]
    public class AUT100EjecucionesController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly AUT100Ejecuciones _controller;

        public AUT100EjecucionesController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, IFormControllerInvocation formInvoker, AUT100Ejecuciones controller)
        {
            this._pageInvoker = pageInvoker;
            this._gridInvoker = gridInvoker;
            this._formInvoker = formInvoker;
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
