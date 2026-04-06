using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Produccion.Interfaces.Entrada
{
    public class InterfazEntradaProduccionProducido
    {
        public string IdProcesado { get; set; }
        public long NumeroEjecucion { get; set; }
        public string NumeroRegistro { get; set; }
        public string NumeroRegistroPadre { get; set; }
        public string CodigoProducto { get; set; }
        public decimal CodigoFaixa { get; set; }
        public string Identificador { get; set; }
        public string FechaVencimiento { get; set; }
        public decimal? ValorMercaderia { get; set; }
        public decimal? ValorTributo { get; set; }
        public decimal? CantidadProducido { get; set; }
        public string AccionMovimiento { get; set; }
        public string Semiacabado { get; set; }
    }
}
