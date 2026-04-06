using WIS.Application.Controllers.PRD;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
	[Route("api/PRD/PRD111ConsumirStock")]
	[ApiController]
	public class PRD111ConsumirStockController : ControllerBase
	{
		private readonly IPageControllerInvocation _pageInvoker;
		private readonly IFormControllerInvocation _formInvoker;
		private readonly IGridControllerInvocation _gridInvoker;
		private readonly PRD111ConsumirStock _controller;

		public PRD111ConsumirStockController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, IGridControllerInvocation gridInvoker, PRD111ConsumirStock controller)
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
