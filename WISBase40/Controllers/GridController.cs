using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
using WIS.GridComponent.Excel;
using WIS.WebApplication.ActionFilters;
using WIS.WebApplication.Models;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Controllers
{
    [Route("api/[controller]")]
    [Authorize, CheckAuthorization]
    [ApiController]
    public class GridController : BaseController
    {
        private readonly IGridService _service;
        private readonly ISessionManager _sessionManager;

        public GridController(IGridService service, ISessionManager sessionManager) : base()
        {
            this._service = service;
            this._sessionManager = sessionManager;
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> Initialize([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            ServerResponse response = await this._service.Initialize(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> FetchRows([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.FetchRows(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> FetchStats([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.FetchStats(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ValidateRow([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.ValidateRow(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }


        [HttpPost("[action]")]
        public async Task<ActionResult> Commit([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.Commit(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ButtonAction([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.ButtonAction(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> MenuItemAction([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.MenuItemAction(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> UpdateConfig([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.UpdateConfig(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ExportExcel([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.ExportExcel(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> ImportExcel([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.ImportExcel(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> GenerateExcelTemplate([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.GenerateExcelTemplate(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> SelectSearch([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.SelectSearch(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> SaveFilter([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.SaveFilter(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> RemoveFilter([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.RemoveFilter(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> GetFilterList([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.GetFilterList(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> NotifySelection([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.NotifySelection(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpPost("[action]")]
        public async Task<ActionResult> NotifyInvertSelection([FromBody] ServerRequest serverRequest, CancellationToken cancelToken)
        {
            ServerResponse response = await this._service.NotifyInvertSelection(serverRequest, cancelToken);

            return Content(response.Serialize(), "application/json");
        }

        [HttpGet("[action]")]
        public ActionResult DownloadExcel()
        {
            var download = _sessionManager.GetValue<GridExcelDownload>(DownloadSessionKey.GridExcelDownloadFile);

            if (download != null)
            {
                _sessionManager.SetValue(DownloadSessionKey.GridExcelDownloadFile, null);

                return File(download.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", download.FileName);
            }

            return NotFound();
        }
    }
}
