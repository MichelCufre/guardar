using WIS.Domain.Automatismo.Dtos;

namespace WIS.Domain.Services.Interfaces
{
    public interface IAutomatismoAutoStoreClientService
    {
        void SendCodigosBarras(CodigosBarrasAutomatismoRequest request);
        void SendProductos(ProductosAutomatismoRequest request);
        void SendSalida(SalidaStockAutomatismoRequest request);
        void SendUbicacionesPicking(UbicacionesPickingAutomatismoRequest request);
        void SendReprocesar(int? cdInterfazExterna, string request);
        bool IsEnabled();
    }
}
