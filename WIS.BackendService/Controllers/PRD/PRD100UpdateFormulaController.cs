using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PRD;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
    [Route("api/PRD/PRD100Update")]
    [ApiController]
    public class PRD100UpdateFormulaController : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly PRD100ModificarFormula _controller;

        public PRD100UpdateFormulaController(
            IFormControllerInvocation formInvoker,
            IGridControllerInvocation gridInvoker,
            PRD100ModificarFormula controller)
        {
            this._formInvoker = formInvoker;
            this._gridInvoker = gridInvoker;
            this._controller = controller;
        }


        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
    }
}
