using WIS.Application.Controllers.PRD;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRD
{
	[Route("api/PRD/PRD113ProdNoEsp")]
	[ApiController]
	public class PRD113PanelFabricacionProdNoEsperadosController : ControllerBase
	{
		private readonly IPageControllerInvocation _pageInvoker;
		private readonly IFormControllerInvocation _formInvoker;
		private readonly IGridControllerInvocation _gridInvoker;
		private readonly PRD113PanelFabricacionProductosFinalesNoEsperados _controller;

		public PRD113PanelFabricacionProdNoEsperadosController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, IGridControllerInvocation gridInvoker, PRD113PanelFabricacionProductosFinalesNoEsperados controller)
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
