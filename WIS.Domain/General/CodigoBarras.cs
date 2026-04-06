using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class CodigoBarras
    {
        public CodigoBarras()
        {
        }
        public CodigoBarras(string tipoOperacionId)
        {
            TipoOperacionId = tipoOperacionId;
        }

        public string Codigo { get; set; }//CD_BARRAS
        public int Empresa { get; set; }//CD_EMPRESA
        public string Producto { get; set; }//CD_PRODUTO
        public int? TipoCodigo { get; set; }//TP_CODIGO_BARRAS
        public short? PrioridadUso { get; set; }//NU_PRIORIDADE_USO
        public decimal? CantidadEmbalaje { get; set; }//QT_EMBALAGEM
        public DateTime? FechaInsercion { get; set; }//DT_ADDROW
        public DateTime? FechaModificacion { get; set; }//DT_UPDROW
        public long? NumeroTransaccion { get; set; }//NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }//NU_TRANSACCION_DELETE

        #region WMS_API
        public string TipoOperacionId { get; set; }
        #endregion


    }
}
