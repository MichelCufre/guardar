using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface IFacturaServiceContext : IServiceContext
    {
        HashSet<string> Predios { get; set; }
        HashSet<string> Monedas { get; set; }
        HashSet<string> TiposFactura { get; set; }

        HashSet<Stock> SeriesExistentes { get; set; }
        Dictionary<string, string> Agentes { get; set; }
        Dictionary<string, Factura> Facturas { get; set; }
        Dictionary<string, Producto> Productos { get; set; }

        Task Load();

        bool ExisteFactura(string factura, string serie, int empresa, string cliente);
        bool ExisteMoneda(string moneda);
        bool ExistePredio(string predio);
        bool ExisteSerie(string codigoProducto, string identificador);
        bool ExisteTipoFactura(string tipo);

        Agente GetAgente(string codigo, int empresa, string tipo);
        Factura GetFactura(string factura, string serie, int empresa, string cliente);
        Producto GetProducto(int empresa, string codigo);
    }
}
