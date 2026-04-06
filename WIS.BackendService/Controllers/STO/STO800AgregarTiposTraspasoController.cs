using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.STO;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.STO
{
    [Route("api/STO/STO800AgregarTiposTraspaso")]
    [ApiController]
    public class STO800AgregarTiposTraspasoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly STO800AgregarTiposTraspaso _controller;

        public STO800AgregarTiposTraspasoController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, STO800AgregarTiposTraspaso controller)
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
