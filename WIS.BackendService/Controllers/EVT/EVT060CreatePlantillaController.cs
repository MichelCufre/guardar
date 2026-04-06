using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EVT;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.EVT
{
    [Route("api/EVT/EVT060CreatePlantilla")]
    [ApiController]
    public class EVT060CreatePlantillaController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly EVT060CreatePlantilla _controller;

        public EVT060CreatePlantillaController(IPageControllerInvocation pageInvoker, IFormControllerInvocation formInvoker, EVT060CreatePlantilla controller)
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
