using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REC;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REC
{
    [Route("api/REC/REC010Create")]
    [ApiController]
    public class REC010CrearReferenciaRecepcionController : ControllerBase
    {
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly REC010CrearReferenciaRecepcion _controller;

        public REC010CrearReferenciaRecepcionController(IGridControllerInvocation gridInvoker, IFormControllerInvocation formInvoker, REC010CrearReferenciaRecepcion controller)
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
