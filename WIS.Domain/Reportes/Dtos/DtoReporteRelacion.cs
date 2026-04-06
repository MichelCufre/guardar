using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Reportes.Dtos
{
    public class DtoReporteRelacion
    {
        public long IdReporteRelacion { get; set; }
        public string Clave { get; set; }
        public string Tabla { get; set; }

        public long Id { get; set; }
        public string Tipo { get; set; }
        public byte[] Contenido { get; set; }
        public int? Usuario { get; set; }
        public DateTime? FechaAlta { get; set; }
        public string NombreArchivo { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Predio { get; set; }
        public string Zona { get; set; }
    }
}
