using Microsoft.Extensions.Options;
using WIS.Configuration;
using WIS.Domain.DataModel;
using WIS.PageComponent.Execution;
using WIS.Security;

namespace WIS.Application.Controllers.PRO
{
    public class PRO030Indicadores : AppController
    {
        protected readonly IOptions<PowerBiSettings> _powerBiSettings;

        public PRO030Indicadores(IIdentityService identity, IUnitOfWorkFactory uowFactory, IOptions<PowerBiSettings> powerBiSettings)
        {
            this._powerBiSettings = powerBiSettings;
        }

        public override PageContext PageLoad(PageContext data)
        {
            data.AddParameter("Endpoint", _powerBiSettings.Value.Endpoint);

            return data;
        }
    }
}
