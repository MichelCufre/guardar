using WIS.Application.Controllers.PRD;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD113DifProducido
{
	[Route("api/PRD/PRD113RemanenteIns")]
	[ApiController]
	public class PRD113RemanenteInsumosController : ControllerBase
	{
		private readonly IPageControllerInvocation _pageInvoker;
		private readonly IFormControllerInvocation _formInvoker;
		private readonly IGridControllerInvocation _gridInvoker;
		private readonly PRD113RemanenteInsumos _controller;

		public PRD113RemanenteInsumosController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, IGridControllerInvocation gridInvoker, PRD113RemanenteInsumos controller)
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
		public IActionResult Grid(GridWrapper data)
		{
			return Ok(this._gridInvoker.Invoke(data, this._controller));
		}
	}
}
