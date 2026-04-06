using System;

namespace WIS.Domain.Produccion
{
    public class PasadaHistorica
    {
        public long NumeroHistorico { get; set; }
        public string Ingreso { get; set; }
        public int CantidadPasadas { get; set; }
        public int AccionIntancia { get; set; }
        public string ValorAccionInstancia { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public int? Transaccion { get; set; }
        public int? Orden { get; set; }
        public int? NumeroFormulaEnsamblada { get; set; }
        public string Linea { get; set; }
        public DateTime? FechaHistorica { get; set; }
    }
}
