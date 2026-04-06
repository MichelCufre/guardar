using Microsoft.AspNetCore.Mvc;
using WIS.Application.Controllers.DEF;
using WIS.Application.Invocation;
using WIS.PageComponent.Execution.Serialization;

namespace WIS.BackendService.Controllers.Def
{
    [Route("api/DEF/Default")]
    [ApiController]
    public class DefaultController : ControllerBase
    {
        private readonly IPageControllerInvocation _pageInvoker;
        private readonly HomePage _controller;
        public DefaultController(IPageControllerInvocation pageInvoker, HomePage controller)
        {
            this._pageInvoker = pageInvoker;
            this._controller = controller;
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Page(PageWrapper data)
        {
            return Ok(this._pageInvoker.Invoke(data, this._controller));
        }
    }
}
