using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PRE;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PRE
{
    [Route("api/PRE/PRE100CreatePedido")]
    [ApiController]
    public class PRE100CrearPedidoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly PRE100CrearPedido _controller;

        public PRE100CrearPedidoController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, PRE100CrearPedido controller)
        {
            this._pageInvoker = pageInvoker;
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
