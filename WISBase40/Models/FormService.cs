using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.FormComponent.Execution;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent.Execution.Responses;
using WIS.FormComponent.Execution.Serialization;
using WIS.Serialization;
using WIS.WebApplication.Models.Managers;

namespace WIS.WebApplication.Models
{
    public class FormService : IFormService
    {
        private readonly IFormCallService _caller;
        private readonly ITrafficOfficerFrontendService _trafficOfficerService;

        public FormService(IFormCallService caller, 
            ITrafficOfficerFrontendService trafficOfficerService)
        {
            this._caller = caller;
            this._trafficOfficerService = trafficOfficerService; 
        }

        public async Task<ServerResponse> Initialize(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            //TODO: Comprobar permisos de usuario antes de realizar llamada, para comprobar que puede acceder a la aplicación provista
            IFormWrapper responseData = await this._caller.CallFormServiceAsync(serverRequest, FormAction.Initialize, cancelToken);

            var content = responseData.GetResolvedData<FormInitializePayload>();

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> ValidateField(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            IFormWrapper responseData = await this._caller.CallFormServiceAsync(serverRequest, FormAction.ValidateField, cancelToken);

            var content = responseData.GetResolvedData<FormValidationPayload>();

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> Submit(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            IFormWrapper responseData = await this._caller.CallFormServiceAsync(serverRequest, FormAction.Submit, cancelToken);

            var content = responseData.GetResolvedData<FormSubmitPayload>();

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> ButtonAction(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            IFormWrapper responseData = await this._caller.CallFormServiceAsync(serverRequest, FormAction.ButtonAction, cancelToken);

            var content = responseData.GetResolvedData<FormButtonActionPayload>();

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }

        public async Task<ServerResponse> SelectSearch(ServerRequest serverRequest, CancellationToken cancelToken)
        {
            IFormWrapper responseData = await this._caller.CallFormServiceAsync(serverRequest, FormAction.SelectSearch, cancelToken);

            var content = responseData.GetResolvedData<FormSelectSearchResponse>();

            var response = new ServerResponse(content);

            if (responseData.Status == TransferWrapperStatus.Error)
                response.SetError(responseData.Message, responseData.MessageArguments);

            response.TooManySessions = this._trafficOfficerService.TooManySessions;

            return response;
        }
    }
}
