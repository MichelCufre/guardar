using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface ICodigoBarrasService
    {
        Task<CodigoBarras> GetCodigoDeBarras(string codigo, int empresa);
        Task<ValidationsResult> AgregarCodigosDeBarras(List<CodigoBarras> codigos, int userId);
        void NotificarAutomatismo(IUnitOfWork uow, List<CodigoBarras> codigos);
    }
}
