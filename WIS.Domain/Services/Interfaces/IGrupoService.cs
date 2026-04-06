using System.Collections.Generic;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IGrupoService
    {
        Grupo GetGrupo(Producto producto);
        List<SelectOption> GetOptionsByParam(IUnitOfWork uow, string codigoParametro, string tipoParametro, string searchValue, int userId);
    }
}
