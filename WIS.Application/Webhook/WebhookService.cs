using System.Text.Json;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.General.API;
using WIS.Domain.Security;
using WIS.Domain.Services.Interfaces;
using WIS.Webhook.Execution;

namespace WIS.Application.Webhook
{
    public class WebhookService : IWebhookService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IWebhookCallerService _webhookCallerService;
        protected int _timeout = 30;

        public WebhookService(IUnitOfWorkFactory uowFactory, IWebhookCallerService webhookCallerService)
        {
            this._webhookCallerService = webhookCallerService;
            this._uowFactory = uowFactory;
        }

        public virtual async Task<WebhookContent> Test(int user, WebhookRequest request)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var testContent = JsonSerializer.Serialize(new TestResponse());
                var content = JsonSerializer.Serialize(WebhookEvent.GetNewInstance(testContent, false));
                var hash = new byte[0];

                if (!string.IsNullOrEmpty(request.Empresa) && request.Secret == null)
                {
                    hash = uow.EmpresaRepository.GetFirma(int.Parse(request.Empresa), content);
                }
                else
                {
                    hash = Signer.ComputeHash(request.Secret, content);
                }

                int.TryParse(await uow.ParametroRepository.GetParametro(ParamManager.WEBHOOK_TIMEOUT), out _timeout);

                return new WebhookContent()
                {
                    Response = await _webhookCallerService.Invoke(request.PayloadUrl, hash, content, _timeout)
                };
            }
        }
    }
}
