using WIS.Application.Controllers.PRD;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;


namespace WIS.BackendService.Controllers.PRD
{
	[Route("api/PRD/PRD111CrearEspaacio")]
	[ApiController]
	public class PRD111CrearEspacioController : ControllerBase
	{
		private readonly IFormControllerInvocation _formInvoker;
		private readonly PRD111CrearEspacioProduccion _controller;

		public PRD111CrearEspacioController(IFormControllerInvocation formInvoker, PRD111CrearEspacioProduccion controller)
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
