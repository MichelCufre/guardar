using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG700Create")]
    [ApiController]
    public class REG700CrearRecorridoController : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly REG700CrearRecorrido _controller;

        public REG700CrearRecorridoController(IFormControllerInvocation formInvoker, REG700CrearRecorrido controller)
        {
            this._formInvoker = formInvoker;
            this._controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }
    }
}
