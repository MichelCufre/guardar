using System;

namespace WIS.Domain.OrdenTarea
{
    public class Orden
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string DsReferencia { get; set; }
        public string Estado { get; set; }
        public int? Funcionario { get; set; }
        public DateTime? FechaAgregado { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaUltimaOperacion { get; set; }
    }
}
