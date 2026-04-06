using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG300CreateGrupo")]
    [ApiController]
    public class REG300CreateGrupoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly REG300CreateGrupo _controller;

        public REG300CreateGrupoController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, REG300CreateGrupo controller)
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
