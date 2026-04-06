using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REC;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REC
{
	[Route ("api/REC/REC171Imprimir"), ApiController]
	public class REC171ImprimirEtiquetasController : ControllerBase
	{
		private readonly IFormControllerInvocation _formInvoker;
		private readonly REC171ImprimirEtiquetas _controller;

		public REC171ImprimirEtiquetasController (
			IFormControllerInvocation formInvoker,
			REC171ImprimirEtiquetas controller)
		{
			this._formInvoker = formInvoker;
			this._controller  = controller;
		}

		[HttpPost]
		[Route ("[action]")]
		public IActionResult Form (FormWrapper data)
			=> Ok (this._formInvoker.Invoke (data, this._controller));
	}
}
