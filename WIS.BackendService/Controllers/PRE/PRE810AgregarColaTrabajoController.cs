using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PRE;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRE
{
    [Route("api/PRE/PRE810CreateCola")]
    [ApiController]
    public class PRE810AgregarColaTrabajoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
		private readonly IFormControllerInvocation _formInvoker;
		private readonly PRE810AgregarColaTrabajo _controller;

        public PRE810AgregarColaTrabajoController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, IFormControllerInvocation formInvoker, PRE810AgregarColaTrabajo controller)
        {
            this._pageInvoker = pageInvoker;
            this._gridInvoker = gridInvoker;
			this._formInvoker = formInvoker;
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
	}
}
