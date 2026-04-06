using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.ORT;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.ORT
{
    [Route("api/ORT/ORT020AsignarEmpresas")]
    [ApiController]
    public class ORT020AsignarEmpresasController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly ORT020AsignarEmpresas _controller;

        public ORT020AsignarEmpresasController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, ORT020AsignarEmpresas controller)
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
