using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.STO;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.STO
{
    [Route("api/STO/STO800")]
    [ApiController]
    public class STO800PanelConfigTraspasoEmpresasController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly STO800PanelConfigTraspasoEmpresas _controller;

        public STO800PanelConfigTraspasoEmpresasController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, STO800PanelConfigTraspasoEmpresas controller)
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
