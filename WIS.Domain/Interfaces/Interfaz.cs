using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Interfaces
{
    public class Interfaz
    {
        public long Id { get; set; }
        public string ObjetoConsulta { get; set; }
        public string TipoObjetoDb { get; set; }
        public string NombreProcedimiento { get; set; }
        public string NombreInterfaz { get; set; }
        public string IdEntradaSalida { get; set; }
        public bool IgnorarErrorCarga { get; set; }
        public bool EsperarAprobacion { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string DescripcionObjeto { get; set; }
        public string DescripcionInterfaz { get; set; }
    }
}
