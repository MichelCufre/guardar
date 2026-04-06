using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.StockEntities;
using WIS.Domain.Validation;

namespace WIS.Domain.Services.Interfaces
{
    public interface IStockService
    {
        Task<StockValidationsResult> GetStock(FiltrosStock filtros, string loginName, int empresa);
        Task<ValidationsResult> ProcesarAjuste(List<AjusteStock> ajustes, int userId);
        Task<ValidationsResult> ProcesarTransferencia(List<TransferenciaStock> transferencias, int userId);
        Task<ValidationsResult> ProcesarTransferenciaAutomatismo(List<TransferenciaStock> transferencias, int userId);
    }
}
