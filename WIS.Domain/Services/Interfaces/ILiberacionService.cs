using System.Threading.Tasks;
using WIS.Domain.Liberacion;

namespace WIS.Domain.Services.Interfaces
{
    public interface ILiberacionService
    {
        void Start(ReglaLiberacion reglaUnica = null, int userId = 0);
    }
}