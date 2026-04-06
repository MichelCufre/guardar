using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.PageComponent.Execution;
using WIS.PageComponent.Execution.Serialization;
using WIS.Serialization;

namespace WIS.WebApplication.Models
{
    public class PageService : IPageService
    {
        private readonly IPageCallService _caller;
        private readonly ITrafficOfficerFrontendService _trafficOfficerService;

        public PageService(IPageCallService caller, 
            ITrafficOfficerFrontendService trafficOfficerService)
        {
            this._caller = caller;
            this._trafficOfficerService = trafficOfficerService;
        }

        public async Task<ServerResponse> Load(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            IPageWrapper responseData = await this._caller.CallPageServiceAsync(serverRequest, PageAction.Load, cancelToken);

            var content = responseData.GetResolvedData<PageContext>();

            var response = new ServerResponse(content);

            response.PageToken = responseData.PageToken;

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> Unload(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            IPageWrapper responseData = await this._caller.CallPageServiceAsync(serverRequest, PageAction.Unload, cancelToken);

            var content = responseData.GetResolvedData<PageContext>();

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }
    }
}
