using WIS.Application.Controllers.PRD;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
	[Route("api/PRD/PRD111ModificarEspacio")]
	[ApiController]
	public class PRD111ModificarEspacioController : ControllerBase
	{
		private readonly IFormControllerInvocation _formInvoker;
		private readonly PRD111ModificarEspacioProduccion _controller;

		public PRD111ModificarEspacioController(IFormControllerInvocation formInvoker, PRD111ModificarEspacioProduccion controller)
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
