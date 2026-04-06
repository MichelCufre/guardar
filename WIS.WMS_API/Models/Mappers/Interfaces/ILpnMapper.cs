using System.Collections.Generic;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.StockEntities;
using WIS.WMS_API.Controllers.Entrada;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface ILpnMapper
    {
        LpnResponse MapToResponse(Lpn lpn);
        List<Lpn> Map(LpnsRequest request);
    }
}