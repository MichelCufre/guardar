using WIS.Application.Controllers.EXP;
using Microsoft.AspNetCore.Mvc;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;


namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP150Form")]
    [ApiController]
    public class EXP150FormController : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly EXP150Form _controller;

        public EXP150FormController(IFormControllerInvocation formInvoker,
                                EXP150Form controller)
        {
            _formInvoker = formInvoker;
            _controller = controller;
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }
    }
}
