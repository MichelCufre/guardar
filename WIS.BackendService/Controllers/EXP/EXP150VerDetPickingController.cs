using WIS.Application.Controllers.EXP;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;


namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP150VerDetPicking")]
    [ApiController]
    public class EXP150VerDetPickingController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EXP150VerDetPicking _controller;

        public EXP150VerDetPickingController(IGridControllerInvocation gridInvoker,
                                EXP150VerDetPicking controller)
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