using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.STO;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.STO
{
    [Route("api/STO/STO800CrearConfiguracion")]
    [ApiController]
    public class STO800CrearConfiguracionController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly STO800CrearConfiguracion _controller;

        public STO800CrearConfiguracionController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, STO800CrearConfiguracion controller)
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
