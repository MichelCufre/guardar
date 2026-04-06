using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class DetalleDisponibleCrossDocking
    {
        public int Agenda { get; set; }
        public int? Etiqueta { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public string Identificador { get; set; }
        public decimal Faixa { get; set; }
        public decimal? CantidadDisponible { get; set; }
        public string ManejoIdentificador { get; set; }
        public string Predio { get; set; }
    }
}
