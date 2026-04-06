using Microsoft.Extensions.Options;
using NLog;
using System;
using WIS.Domain.DataModel;
using WIS.Domain.ManejoStock.AjusteStockDocumental;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class AjusteStockDocumentalService : IAjusteStockDocumentalService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IOptions<AjusteStockDocumentalSettings> _settings;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AjusteStockDocumentalService(IUnitOfWorkFactory uowFactory,
            IOptions<AjusteStockDocumentalSettings> settings)
        {
            this._uowFactory = uowFactory;
            this._settings = settings;
        }

        public virtual void Start()
        {
            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    var ajuste = new AjusteStockDocumental();
                    ajuste.ImportarAjustesDeStock(uow, 0, "JobImpAjust40", this._settings.Value.CantidadAjustes);
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw ex;
            }
        }
    }
}
