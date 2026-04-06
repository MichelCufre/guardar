using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.DOC;
using WIS.Application.Invocation;
using WIS.FormComponent.Execution.Serialization;
using WIS.GridComponent.Execution.Serialization;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.DOC
{
    [Route("api/DOC/DOC290")]
    [ApiController]
    public class DOC290Controller : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly IFormControllerInvocation _formInvoker;
        private readonly IGridControllerInvocation _gridInvoker;
        private readonly DOC290 _controller;

        public DOC290Controller(
            IPageControllerInvocation pageInvoker,
            IFormControllerInvocation formInvoker,
            IGridControllerInvocation gridInvoker,
            DOC290 controller)
        {
            this._pageInvoker = pageInvoker;
            this._formInvoker = formInvoker;
            this._gridInvoker = gridInvoker;
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
