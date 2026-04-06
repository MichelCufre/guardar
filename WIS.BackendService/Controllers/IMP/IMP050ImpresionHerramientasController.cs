using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.IMP;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.IMP
{
    [Route("api/IMP/IMP050Herramientas")]
    [ApiController]
    public class IMP050ImpresionHerramientasController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly IMP050ImpresionHerramientas _controller;

        public IMP050ImpresionHerramientasController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, IMP050ImpresionHerramientas controller)
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
