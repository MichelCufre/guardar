using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.Recorridos;

namespace WIS.Domain.Services.Interfaces
{
    public interface IRecorridoServiceContext : IServiceContext
    {
        List<DetalleRecorrido> RegistrosBaja { get; set; }
        List<DetalleRecorrido> RegistrosAlta { get; set; }

        Task Load();

        bool AreaUbicacionEsRecorrible(string cdUbicacion);
        bool ExisteNumeroOrden(string vlOrden);
        bool ExisteUbicacion(string key);
        bool ExisteUbicacionEnRecorrido(string codigoUbicacion, string operacion);
        bool NumeroOrdenSeRepite(long nroOrden);
        bool UbicacionOperacionSeRepite(string codigoUbicacion, string tipoOperacion);
        bool UbicacionTienePredioCompatibleConRecorrido(string codigoUbicacion);
    }
}