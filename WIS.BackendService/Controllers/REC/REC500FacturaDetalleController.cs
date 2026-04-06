using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REC;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REC
{
    [Route("api/REC/REC500Lineas")]
    [ApiController]
    public class REC500FacturaDetalleController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly REC500FacturaDetalle _controller;

        public REC500FacturaDetalleController(IGridControllerInvocation gridInvoker, IFormControllerInvocation formInvoker, REC500FacturaDetalle controller)
        {
            this._gridInvoker = gridInvoker;
            this._formInvoker = formInvoker;
            this._controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }
    }
}
