using WIS.Application.Controllers.PRD;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
	[Route("api/PRD/PRD110AsociarEspacioProduccion")]
	[ApiController]
	public class PRD110AsociarEspacioProduccionController : ControllerBase
	{
		private readonly IFormControllerInvocation _formInvoker;
		private readonly PRD110AsociarEspacioProduccion _controller;

		public PRD110AsociarEspacioProduccionController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, IGridControllerInvocation gridInvoker, PRD110AsociarEspacioProduccion controller)
		{
			this._formInvoker = formInvoker;
			this._controller = controller;
		}

		[HttpPost]
		[Route("[action]")]
		public IActionResult Form(FormWrapper data)
		{
			return Ok(this._formInvoker.Invoke(data, this._controller));
		}
	}
}
