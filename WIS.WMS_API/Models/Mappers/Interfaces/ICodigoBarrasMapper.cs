using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface ICodigoBarrasMapper
    {
        List<CodigoBarras> Map(CodigosBarrasRequest request);
        CodigoBarrasResponse MapToResponse(CodigoBarras codigoBarras);
    }
}
