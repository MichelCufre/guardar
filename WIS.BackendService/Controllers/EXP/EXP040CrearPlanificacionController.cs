using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Application.Controllers.EXP;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP040CreatePlanificacion")]
    [ApiController]
    public class EXP040CrearPlanificacionController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly EXP040CrearPlanificacion _controller;

        public EXP040CrearPlanificacionController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, EXP040CrearPlanificacion controller)
        {
            this._pageInvoker = pageInvoker;
            this._formInvoker = formInvoker;
            this._controller = controller;
        }

        [Route("[action]")]
        [HttpPost]
        public IActionResult Page(PageWrapper data)
        {
            return Ok(this._pageInvoker.Invoke(data, this._controller));
        }
        [Route("[action]")]
        [HttpPost]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }
    }
}
