using System.Collections.Generic;
using WIS.Domain.General;
using WIS.Domain.General.API.Dtos.Entrada;

namespace WIS.WMS_API.Models.Mappers.Interfaces
{
    public interface IEmpresaMapper
    {
        List<Empresa> Map(EmpresasRequest request);
        EmpresaResponse MapToResponse(Empresa empresa);
    }
}
