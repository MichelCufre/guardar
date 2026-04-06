using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EXP;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;


namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP110ImpBultoModal")]
    [ApiController]
    public class EXP110ImpresionBultoModalController : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly EXP110ImpresionBultoModal _controller;

        public EXP110ImpresionBultoModalController(IFormControllerInvocation formInvoker,
                                EXP110ImpresionBultoModal controller)
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
