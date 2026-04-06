using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.Recepcion;

namespace WIS.Domain.Services.Interfaces
{
    public interface IModificarDetalleReferenciaServiceContext : IReferenciaRecepcionServiceContext
    {
        Dictionary<string, ReferenciaRecepcionDetalle> Detalles { get; set; }
        HashSet<int> ReferenciaIds { get; set; }
        HashSet<int> ReferenciasEnUso { get; set; }


        Task Load();

        ReferenciaRecepcionDetalle GetDetalleReferencia(int referencia, string idLinea, int empresa, string producto, string identificador);
        string GetIdentificador(string identificador, string producto);
        bool ReferenciaEnUso(int referencia);
    }
}