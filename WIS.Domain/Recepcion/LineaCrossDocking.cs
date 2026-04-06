using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Recepcion
{
    public class LineaCrossDocking
    {
        public int Agenda { get; set; }
        public int Preparacion{ get; set; }
        public string Cliente { get; set; }
        public string Pedido { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public string Identificador { get; set; }
        public long Carga { get; set; }
        public int Empresa { get; set; }
        public bool EspecificaIdentificador { get; set; }
        public decimal Cantidad { get; set; }
        public decimal CantidadPreparada { get; set; }
        public int PreparacionPickeada { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public long? NroTransaccion { get; set; }

        #region API
        public string Agrupacion { get; set; }
        #endregion
    }
}
