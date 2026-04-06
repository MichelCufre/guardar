using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface IReferenciaRecepcionServiceContext : IServiceContext
    {
        HashSet<string> TiposAgente { get; set; }
        HashSet<Stock> SeriesExistentes { get; set; }
        HashSet<string> TiposReferencia { get; set; }
        HashSet<string> TiposReferenciaRecepcion { get; set; }
        HashSet<string> TiposReferenciaAgenteRecepcion { get; set; }
        HashSet<string> Predios { get; set; }
        HashSet<string> Monedas { get; set; }
        Dictionary<string, string> Agentes { get; set; }
        Dictionary<string, Producto> Productos { get; set; }
        Dictionary<string, ReferenciaRecepcion> Referencias { get; set; }

        Task Load();

        bool ExisteMoneda(string moneda);
        bool ExistePredio(string predio);
        bool ExisteReferencia(string referencia, int empresa, string tipoReferencia, string cliente);
        bool ExisteSerie(string codigoProducto, string identificador);
        bool ExisteTipoAgente(string tipoAgente);
        bool ExisteTipoReferencia(string tipo);
        bool ExisteTpRefTpAgente(string tipoReferencia, string tipoAgente);
        bool ExisteTpRefTpRecepcion(string tipoReferencia);
        Agente GetAgente(string codigo, int empresa, string tipo);
        Producto GetProducto(int empresa, string codigo);
        ReferenciaRecepcion GetReferencia(string referencia, int empresa, string tipoReferencia, string cliente);
    }
}