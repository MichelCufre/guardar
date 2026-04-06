using System.Collections.Generic;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;

namespace WIS.Domain.Services.Interfaces
{
    public interface IRecepcionService
    {
        List<SelectOption> GetAllValues(IUnitOfWork uow, string codigoParametro, string searchValue, int userId);
    }
}
