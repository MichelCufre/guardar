using WIS.Domain.General.Auxiliares;
using WIS.Domain.Recepcion;

namespace WIS.Domain.Services.Interfaces
{
    public interface IBarcodeService
    {
        string GenerateBarcode(string nroEtiqueta, string tipoEtiqueta, string predio = null, string anexoEtiqueta = null, string prefijo = "");
        EtiquetaLote GetEtiquetaLote(string cdBarras);
        EtiquetaPosicionEquipo GetEtiquetaPosicionEquipo(string cdBarras);
        bool ValidarEtiquetaContenedor(string codigoBarras, int userId,out AuxContenedor AuxContenedor, out int cantidadEmpresa ,string cdCliente = null, int? cdEmpresa = null);
    }
}
