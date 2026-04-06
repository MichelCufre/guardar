using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel;
using WIS.Domain.Impresiones;

namespace WIS.Domain.Services.Interfaces
{
    public interface IPrintingService
    {
        string GetEstadoInicial();
        void SendToPrint(IUnitOfWork uow, int nuImpresion);
        void SendToPrint(int nuImpresion);
        Task SendToPrintAsync(IUnitOfWork uow, int nuImpresion);
        Task SendToPrintAsync(IUnitOfWork uow, Impresion impresion, CancellationToken cancellationToken);
    }
}
