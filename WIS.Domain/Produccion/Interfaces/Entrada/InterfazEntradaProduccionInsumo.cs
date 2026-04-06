using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Produccion.Interfaces.Entrada
{
    public class InterfazEntradaProduccionInsumo
    {
        public string IdProcesado { get; set; }
        public long NumeroEjecucion { get; set; }
        public string NumeroRegistro { get; set; }
        public string NumeroRegistroPadre { get; set; }
        public string CodigoProducto { get; set; }
        public decimal CodigoFaixa { get; set; }
        public string Identificador { get; set; }
        public string AccionMovimiento { get; set; }
        public decimal CantidadSalida { get; set; }
        public string Semiacabado { get; set; }
        public string Consumible { get; set; }
    }
}
