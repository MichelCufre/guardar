using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.PTL;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.PTL
{
    [Route("api/PTL/PTL010ColoresActivosForm")]
    [ApiController]
    public class PTL010ModalColoresActivosController : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly PTL010ModalColoresActivos _controller;

        public PTL010ModalColoresActivosController(IFormControllerInvocation formInvoker, PTL010ModalColoresActivos controller)
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
