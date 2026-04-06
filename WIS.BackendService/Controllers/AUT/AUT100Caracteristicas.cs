using WIS.Application.Controllers.AUT;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.AUT
{
    [Route("api/AUT/AUT100Caracteristicas")]
    [ApiController]
    public class AUT100Caracteristicas : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly Application.Controllers.AUT.AUT100Caracteristicas _controller;

        public AUT100Caracteristicas(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, Application.Controllers.AUT.AUT100Caracteristicas controller)
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
