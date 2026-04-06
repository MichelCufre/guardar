using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;

namespace WIS.Domain.Services.Interfaces
{
    public interface IEmpresaService
    {
        Task<Empresa> GetEmpresa(int codigo);
        Task<ValidationsResult> AgregarEmpresas(List<Empresa> empresas, int empresaCreadora, int userId);
        byte[] GetFirma(int empresa, string contenido);
        Task UpdateLock(int empresa, bool isLocked);
        
    }
}
