using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.INV;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.INV
{
    [Route("api/INV/INV418")]
    [ApiController]
    public class INV418InventarioAtributosController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly INV418InventarioAtributos _controller;

        public INV418InventarioAtributosController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, INV418InventarioAtributos controller)
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
