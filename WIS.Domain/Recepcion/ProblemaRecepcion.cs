using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Recepcion
{
    public class ProblemaRecepcion
    {
        public int Id { get; set; }
        public int Agenda { get; set; }
        public string Producto { get; set; }
        public string Identificador { get; set; }
        public string TipoProblema { get; set; }
        public string NdProblema { get; set; }
        public decimal Diferencia { get; set; }
        public decimal Faixa { get; set; }
        public string Aceptado { get; set; }
        public int Funcionario { get; set; }
        public int? FuncionarioAceptaProblema { get; set; }
        public DateTime? FechaInsercion { get; set; }
        public DateTime? FechaModificacion { get; set; }        
        public DateTime? FechaAceptaProblema { get; set; }
    }
}
