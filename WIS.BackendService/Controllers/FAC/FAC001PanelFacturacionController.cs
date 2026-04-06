using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.FAC;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.FAC
{
    [Route("api/FAC/FAC001")]
    [ApiController]
    public class FAC001PanelFacturacionController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly FAC001PanelFacturacion _controller;

        public FAC001PanelFacturacionController(IPageControllerInvocation pageInvoker, IGridControllerInvocation gridInvoker, IFormControllerInvocation formInvoker, FAC001PanelFacturacion controller)
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
