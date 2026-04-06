using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG100AsociarCodigoMultidato")]
    [ApiController]
    public class REG100AsociarCodigoMultidatoController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly REG100AsociarCodigoMultidato _controller;
        private readonly IFormControllerInvocation _formInvoker;

        public REG100AsociarCodigoMultidatoController(
            IGridControllerInvocation gridInvoker,
            IFormControllerInvocation formInvoker,
            REG100AsociarCodigoMultidato controller)
        {
            this._gridInvoker = gridInvoker;
            this._formInvoker = formInvoker;
            this._controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }
    }
}
