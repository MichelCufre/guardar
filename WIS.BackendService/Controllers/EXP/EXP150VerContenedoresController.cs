using WIS.Application.Controllers.EXP;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP150VerContenedores")]
    [ApiController]
    public class EXP150VerContenedoresController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EXP150VerContenedores _controller;

        public EXP150VerContenedoresController(IGridControllerInvocation gridInvoker,
                                EXP150VerContenedores controller)
        {
            _gridInvoker = gridInvoker;
            _controller = controller;
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
    }
}