using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.EXP;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;


namespace WIS.BackendService.Controllers.EXP
{
    [Route("api/EXP/EXP110SelecProd")]
    [ApiController]
    public class EXP110SeleccionProductoController : ControllerBase
    {
        private readonly IFormControllerInvocation _formInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly EXP110SeleccionProducto _controller;

        public EXP110SeleccionProductoController(IFormControllerInvocation formInvoker,
                                IGridControllerInvocation gridInvoker,
                                EXP110SeleccionProducto controller)
        {
            _formInvoker = formInvoker;
            _controller = controller;
            _gridInvoker = gridInvoker;
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Form(FormWrapper data)
        {
            return Ok(this._formInvoker.Invoke(data, this._controller));
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Grid(GridWrapper data)
        {
            return Ok(this._gridInvoker.Invoke(data, this._controller));
        }
    }
}
