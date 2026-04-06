using Microsoft.Extensions.Options;
using NLog;
using System;
using WIS.Domain.DataModel;
using WIS.Domain.Documento.Integracion.Preparaciones;
using WIS.Domain.Services.Configuracion;
using WIS.Domain.Services.Interfaces;

namespace WIS.Domain.Services
{
    public class AnulacionDocumentalService : IAnulacionDocumentalService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IOptions<AnulacionDocumentalSettings> _settings;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public AnulacionDocumentalService(IUnitOfWorkFactory uowFactory,
            IOptions<AnulacionDocumentalSettings> settings)
        {
            this._uowFactory = uowFactory;
            this._settings = settings;
        }

        public virtual void Start()
        {
            try
            {
                PreparacionDocumental prep = new PreparacionDocumental(this._uowFactory);
                prep.EjecutarAnulacionPreparacionReserva(0, "JobAnuPrep40", this._settings.Value.CantidadAnulaciones);
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                throw ex;
            }
        }
    }
}
