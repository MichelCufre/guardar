using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Responses;
using WIS.GridComponent.Execution;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Execution.Responses;
using WIS.GridComponent.Execution.Serialization;
using WIS.Serialization;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Models
{
    //TODO: Ver de redisenar, considerar clases separadas para cada comando
    public class GridService : IGridService
    {
        private readonly IGridCallService _caller;
        private readonly ISessionManager _sessionManager;
        private readonly ITrafficOfficerFrontendService _trafficOfficerService;

        public GridService(ISessionManager sessionManager, 
            IGridCallService caller,
            ITrafficOfficerFrontendService trafficOfficerService)
        {
            this._caller = caller;
            this._sessionManager = sessionManager;
            this._trafficOfficerService = trafficOfficerService;
        }

        public async Task<ServerResponse> Initialize(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            IGridWrapper responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.Initialize, cancelToken);

            var content = responseData.GetResolvedData<GridInitializeResponse>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> FetchRows(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.FetchRows, cancelToken);

            var content = responseData.GetResolvedData<GridFetchResponse>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> FetchStats(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.FetchStats, cancelToken);

            var content = responseData.GetResolvedData<GridFetchStatsResponse>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> ValidateRow(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.ValidateRow, cancelToken);

            var content = responseData.GetResolvedData<GridValidationResponse>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> Commit(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.Commit, cancelToken);

            var content = responseData.GetResolvedData<GridFetchResponse>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> ButtonAction(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.ButtonAction, cancelToken);

            var content = responseData.GetResolvedData<GridButtonActionContext>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> MenuItemAction(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.MenuItemAction, cancelToken);

            var content = responseData.GetResolvedData<GridMenuItemActionContext>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> UpdateConfig(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, "Grid/UpdateConfig", GridAction.UpdateConfig, cancelToken);

            var response = new ServerResponse();

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> ExportExcel(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.ExportExcel, cancelToken);

            var response = new ServerResponse();

            if (responseData.Status == TransferWrapperStatus.Error)
            {
                response.SetError(responseData.Message, responseData.MessageArguments);

                return response;
            }

            var data = responseData.GetData<GridExportExcelResponse>(false, new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            //Esto se maneja así por un tema de performance, mantener múltiples descargas en sesión podría generar un memory leak.
            //De esta manera solo se mantiene una descarga por sesión
            var download = new GridExcelDownload()
            {
                FileName = data.FileName,
                Content = Convert.FromBase64String(data.ExcelContent)
            };

            _sessionManager.SetValue(GridDownloadSession.GridExcelDownloadFile, download);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> ImportExcel(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.ImportExcel, cancelToken);

            var response = new ServerResponse();

            if (responseData.Status == TransferWrapperStatus.Error)
            {
                response.SetError(responseData.Message, responseData.MessageArguments);

                var data = responseData.GetData<GridImportExcelResponse>(false, new List<JsonConverter> {
                    new GridColumnConverter(),
                    new GridColumnItemConverter()
                });

                if (data != null && data.ExcelContent != null)
                {
                    //Esto se maneja así por un tema de performance, mantener múltiples descargas en sesión podría generar un memory leak.
                    //De esta manera solo se mantiene una descarga por sesión
                    var download = new GridExcelDownload()
                    {
                        FileName = data.FileName,
                        Content = Convert.FromBase64String(data.ExcelContent)
                    };

                    this._sessionManager.SetValue(GridDownloadSession.GridExcelDownloadFile, download);
                }
            }
            else
            {
                response.Data = responseData.GetResolvedData<GridFetchResponse>();
                response.SetOk();

            }

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> GenerateExcelTemplate(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.GenerateExcelTemplate, cancelToken);

            var response = new ServerResponse();

            if (responseData.Status == TransferWrapperStatus.Error)
            {
                response.SetError(responseData.Message, responseData.MessageArguments);

                return response;
            }

            var data = responseData.GetData<GridImportExcelResponse>(false, new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            if (data != null && data.ExcelContent != null)
            {
                //Esto se maneja así por un tema de performance, mantener múltiples descargas en sesión podría generar un memory leak.
                //De esta manera solo se mantiene una descarga por sesión
                var download = new GridExcelDownload()
                {
                    FileName = data.FileName,
                    Content = Convert.FromBase64String(data.ExcelContent)
                };

                this._sessionManager.SetValue(GridDownloadSession.GridExcelDownloadFile, download);
            }

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> SelectSearch(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.SelectSearch, cancelToken);

            var content = responseData.GetResolvedData<GridSelectSearchResponse>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> SaveFilter(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, "Grid/SaveFilter", GridAction.SaveFilter, cancelToken);

            var response = new ServerResponse();

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> RemoveFilter(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, "Grid/RemoveFilter", GridAction.RemoveFilter, cancelToken);

            var response = new ServerResponse();

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> GetFilterList(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, "Grid/GetFilterList", GridAction.GetFilterList, cancelToken);

            var content = responseData.GetResolvedData<List<GridFilterData>>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> NotifySelection(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.NotifySelection, cancelToken);

            var content = responseData.GetResolvedData<GridNotifySelectionContext>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> NotifyInvertSelection(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            var responseData = await this._caller.CallGridServiceAsync(serverRequest, GridAction.NotifyInvertSelection, cancelToken);

            var content = responseData.GetResolvedData<GridNotifyInvertSelectionContext>(new List<JsonConverter> {
                new GridColumnConverter(),
                new GridColumnItemConverter()
            });

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }    
    }
}
