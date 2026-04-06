using WIS.Application.Controllers.PRD;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
	[Route("api/PRD/PRD111")]
	[ApiController]
	public class PRD111PanelEspacioController : ControllerBase
	{
		private readonly IPageControllerInvocation _pageInvoker;
		private readonly IFormControllerInvocation _formInvoker;
		private readonly IGridControllerInvocation _gridInvoker;
		private readonly PRD111PanelEspacioProduccion _controller;

		public PRD111PanelEspacioController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, IGridControllerInvocation gridInvoker, PRD111PanelEspacioProduccion controller)
		{
			this._pageInvoker = pageInvoker;
			this._formInvoker = formInvoker;
			this._gridInvoker = gridInvoker;
			this._controller = controller;
		}

		[HttpPost]
		[Route("[action]")]
		public IActionResult Page(PageWrapper data)
		{
			return Ok(this._pageInvoker.Invoke(data, this._controller));
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
