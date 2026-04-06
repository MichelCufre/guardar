using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionUnidadMedidaEmpresa
    {
        public string UnidadMedida { get; set; }
        public int Empresa { get; set; }
        public int? Funcionario { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
