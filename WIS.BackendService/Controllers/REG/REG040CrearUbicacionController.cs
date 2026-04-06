using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.REG;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.REG
{
    [Route("api/REG/REG040Create")]
    [ApiController]
    public class REG040CrearUbicacionController : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly REG040CrearUbicacion _controller;

        public REG040CrearUbicacionController(IFormControllerInvocation formInvoker, REG040CrearUbicacion controller)
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
