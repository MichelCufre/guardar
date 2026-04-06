using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.StockEntities
{
    public class AuditoriaLpn
    {
        public long Auditoria { get; set; }
        public int? LpnDet { get; set; }
        public long Lpn { get; set; }
        public string Producto { get; set; }
        public decimal Faixa { get; set; }
        public int Empresa { get; set; }
        public string Identificador { get; set; }
        public long? Transaccion { get; set; }
        public decimal Estoque { get; set; }
        public decimal? Auditada { get; set; }
        public decimal? Diferencia { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? Vencimiento { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public int? FuncionarioAlta { get; set; }
        public int? FuncionarioModificacion { get; set; }
        public int? FuncionarioModificacionEstado { get; set; }
        public string Nivel { get; set; }
        public long? TransaccionDelete { get; set; }
        public long? AuditoriaAgrupador { get; set; }
    }
}
