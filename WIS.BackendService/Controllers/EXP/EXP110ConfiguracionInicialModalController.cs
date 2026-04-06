using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EXP;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;


namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP110ConfInicialModal")]
    [ApiController]
    public class EXP110ConfiguracionInicialModalController : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly EXP110ConfiguracionInicialModal _controller;

        public EXP110ConfiguracionInicialModalController(IFormControllerInvocation formInvoker,
                                EXP110ConfiguracionInicialModal controller)
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
