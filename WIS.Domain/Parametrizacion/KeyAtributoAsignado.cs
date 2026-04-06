using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Parametrizacion
{
    public class KeyAtributoAsignado
    {
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public string IdEspecificaIdentificador { get; set; }
        public string TipoLpn { get; set; }
        public string IdExternoLpn { get; set; }
        public long IdConfiguracion { get; set; }
        public int IdAtributo { get; set; }
        public string IdCabezal { get; set; }
    }
}
