using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EXP;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP040ArmarEgresoCarga")]
    [ApiController]
    public class EXP040ArmarEgresoCargaController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EXP040ArmarEgresoCarga _controller;

        public EXP040ArmarEgresoCargaController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, EXP040ArmarEgresoCarga controller)
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
