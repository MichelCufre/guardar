using WIS.Application.Controllers.PRD;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
	[Route("api/PRD/PRD110DetallesProducccion")]
	[ApiController]
	public class PRD110DetallesProducccionController : ControllerBase
	{
		private readonly IGridControllerInvocation _gridInvoker;
		private readonly PRD110DetallesProducccion _controller;
        private readonly IFormControllerInvocation _formInvoker;
        public PRD110DetallesProducccionController(IGridControllerInvocation gridInvoker, IFormControllerInvocation formInvoker, PRD110DetallesProducccion controller)
		{
			this._gridInvoker = gridInvoker;
			this._controller = controller;
            this._formInvoker = formInvoker;

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
