using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Services.Interfaces
{
    public interface IProducirProduccionServiceContext : IServiceContext
    {
        HashSet<Stock> SeriesExistentes { get; set; }
        List<string> KeysAjustes { get; set; }
        bool NotificarProduccion { get; set; }
        ILogicaProduccion LogicaProduccion { get; set; }

        Task Load();

        bool ExisteMotivo(string motivo);
        bool ExisteSerie(string codigoProducto, string identificador);
        EspacioProduccion GetEspacioProduccion();
        List<string> GetIdsBloqueos(IFormatProvider formatProvider);
        IngresoProduccion GetIngreso();
        EspacioProduccion GetProduccionEspacioActiva();
        Producto GetProducto(int empresa, string codigo);
    }
}