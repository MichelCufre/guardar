using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.POR;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.POR
{
    [Route("api/POR/POR070")]
    [ApiController]
    public class POR070AsociarCamionesVehiculoController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly POR070AsociarCamionesVehiculo _controller;

        public POR070AsociarCamionesVehiculoController(
            IPageControllerInvocation pageInvoker, 
            IGridControllerInvocation gridInvoker, 
            IFormControllerInvocation formInvoker, 
            POR070AsociarCamionesVehiculo controller)
        {
            this._pageInvoker = pageInvoker;
            this._gridInvoker = gridInvoker;
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
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }
    }
}
