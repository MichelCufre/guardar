using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WIS.WebApplication.ActionFilters;
using WIS.WebApplication.Models;

namespace WIS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [Authorize, CheckAuthorization]
    [ApiController]
    public class PageController : BaseController
    {
        private readonly IPageService _service;
        private readonly ITrafficOfficerFrontendService _trafficOfficerService;

        public PageController(IPageService service, ITrafficOfficerFrontendService trafficOfficerService)
        {
            this._service = service;
            this._trafficOfficerService = trafficOfficerService;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Load([FromBody]ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.Load(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Unload([FromBody]ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.Unload(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Abandon([FromBody]PageAbandonRequest request, CancellationToken cancelToken)
        {
            return Ok(await this._trafficOfficerService.ClearToken(request.Token, request.Application, cancelToken));
        }
    }
}