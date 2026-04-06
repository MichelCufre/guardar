using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WIS.Domain.General;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;

namespace WIS.Domain.Services.Interfaces
{
    public interface IConsumirProduccionServiceContext : IServiceContext
    {
        public List<IngresoProduccionDetalleReal> DetallesInsumos { get; set; }
        public List<string> KeysAjustes { get; set; }
        public bool NotificarProduccion { get; set; }
        public ILogicaProduccion LogicaProduccion { get; set; }

        Task Load();

        bool ExisteMotivo(string motivo);
        int GetCantidadOrdenesActivas();
        EspacioProduccion GetEspacioProduccion();
        List<string> GetIdsBloqueos(IFormatProvider formatProvider);
        IngresoProduccion GetIngreso();
        EspacioProduccion GetProduccionEspacioActiva();
        Producto GetProducto(int empresa, string codigo);
    }
}