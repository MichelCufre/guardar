using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Facturacion;
using WIS.Domain.Interfaces;
using WIS.Domain.Services.Configuracion;

namespace WIS.Domain.Services.Interfaces
{
    public class FacturacionService : IFacturacionService
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IDapper _dapper;
        protected readonly IOptions<PluginSettings> _settings;
        protected readonly ILogger<FacturacionService> _logger;

        public FacturacionService(IUnitOfWorkFactory uowFactory, IDapper dapper, ILogger<FacturacionService> logger, IOptions<PluginSettings> settings)
        {
            _uowFactory = uowFactory;
            _dapper = dapper;
            _logger = logger;
            _settings = settings;
        }

        public virtual byte[] DescargarExcel(int nuEjecucion, int cdEmpresa, string cdFacturacion, string nameFile)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                return uow.FacturacionRepository.DescargarExcel(nuEjecucion, cdEmpresa, cdFacturacion, nameFile);
            }
        }

        public virtual void Start(int? nroFactEjecucion = null)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var logic = new FacturacionLogic(uow, _dapper, _logger, _settings);
                logic.EjecutarProcesoFacturacion(nroFactEjecucion);
            }
        }
    }
}
