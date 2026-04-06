using WIS.Application.Controllers.PRD;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
	[Route("api/PRD/PRD110DetallesTeoricos")]
	[ApiController]
	public class PRD110DetallesTeoricosController : ControllerBase
	{
		private readonly IGridControllerInvocation _gridInvoker;
		private readonly PRD110DetallesTeoricos _controller;

		public PRD110DetallesTeoricosController(IGridControllerInvocation gridInvoker, PRD110DetallesTeoricos controller)
		{
			this._gridInvoker = gridInvoker;
			this._controller = controller;
		}

		[HttpPost]
		[Route("[action]")]
		public IActionResult Grid(GridWrapper data)
		{
			return Ok(this._gridInvoker.Invoke(data, this._controller));
		}
	}
}
