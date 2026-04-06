using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class GrupoParametroValidate
    {
        public string Codigo { get; set; }
        public string ValorDesde { get; set; }
        public string ValorHasta { get; set; }
        public int Largo { get; set; }
        public int? Precision { get; set; }
        public string Tipo { get; set; }
        public string Dependencia { get; set; }
        public bool ValidacionDesdeHasta { get; set; }
    }
}
