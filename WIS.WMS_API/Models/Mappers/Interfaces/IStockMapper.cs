using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.Interfaces;
using WIS.Domain.StockEntities;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IStockMapper
    {
        FiltrosStock Map(StockRequest request);
        List<AjusteStock> Map(AjustesDeStockRequest request, InterfazEjecucion ejecucion);
        List<TransferenciaStock> Map(TransferenciaStockRequest request);
    }
}
