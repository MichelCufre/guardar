using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WIS.WebApplication.ActionFilters;
using WIS.WebApplication.Models;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WIS.WebApplication.Controllers
{
    [Authorize, CheckAuthorization]
    public class FormController : BaseController
    {
        private readonly IFormService _service;

        public FormController(IFormService service)
        {
            this._service = service;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Initialize([FromBody]ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.Initialize(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");

        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ValidateField([FromBody]ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.ValidateField(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Submit([FromBody]ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.Submit(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ButtonAction([FromBody]ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.ButtonAction(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> SelectSearch([FromBody]ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.SelectSearch(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }
    }
}
