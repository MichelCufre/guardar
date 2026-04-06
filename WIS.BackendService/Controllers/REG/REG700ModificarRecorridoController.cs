using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG700Update")]
    [ApiController]
    public class REG700ModificarRecorridoController : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly REG700ModificarRecorrido _controller;

        public REG700ModificarRecorridoController(IFormControllerInvocation formInvoker, REG700ModificarRecorrido controller)
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
