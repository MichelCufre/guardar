using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PRD;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
    [Route("api/PRD/PRD102")]
    [ApiController]
    public class PRD102Controller : ControllerBase
    {        
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly PRD102 _controller;

        public PRD102Controller(            
            IGridControllerInvocation gridInvoker,
            PRD102 controller)
        {
            
            this._gridInvoker = gridInvoker;
            this._controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
    }
}
