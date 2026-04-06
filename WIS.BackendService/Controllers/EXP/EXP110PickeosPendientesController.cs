using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EXP;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP110PickeosPendientes")]
    [ApiController]
    public class EXP110PickeosPendientesController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EXP110PickeosPendientes _controller;

        public EXP110PickeosPendientesController(IGridControllerInvocation gridInvoker,
                                EXP110PickeosPendientes controller)
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
