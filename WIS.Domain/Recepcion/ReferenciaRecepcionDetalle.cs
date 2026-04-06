using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Recepcion
{
    public class ReferenciaRecepcionDetalle
    {
        public ReferenciaRecepcionDetalle()
        {
        }

        public int Id { get; set; }                             //NU_RECEPCION_REFERENCIA_DET
        public int IdReferencia { get; set; }                   //NU_RECEPCION_REFERENCIA
        public string IdLineaSistemaExterno { get; set; }       //ID_LINEA_SISTEMA_EXTERNO
        public int IdEmpresa { get; set; }                      //CD_EMPRESA
        public string CodigoProducto { get; set; }              //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public decimal? CantidadReferencia { get; set; }        //QT_REFERENCIA
        public decimal? CantidadAnulada { get; set; }           //QT_ANULADA
        public decimal? CantidadAgendada { get; set; }          //QT_AGENDADA
        public decimal? CantidadRecibida { get; set; }          //QT_RECIBIDA
        public decimal? CantidadConfirmadaInterfaz { get; set; }//QT_CONFIRMADA_INTERFAZ
        public decimal? ImporteUnitario { get; set; }           //IM_UNITARIO
        public DateTime? FechaVencimiento { get; set; }         //DT_VENCIMIENTO
        public string Anexo1 { get; set; }                      //DS_ANEXO1
        public DateTime? FechaInsercion { get; set; }           //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public long? NumeroTransaccion { get; set; }            //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }      //NU_TRANSACCION_DELETE

        public virtual ReferenciaRecepcion ReferenciaRecepcion { get; set; }

        public string TipoOperacionId { get; set; }

        public ReferenciaRecepcionDetalle(string tipoOperacionId)
        {
            TipoOperacionId = tipoOperacionId;
        }

        public virtual string GetCompositeId()
        {
            return $"{Id}";
        }

        public virtual decimal GetSaldo()
        {
            return ((CantidadReferencia ?? 0) - (CantidadAgendada ?? 0) - (CantidadAnulada ?? 0) - (CantidadRecibida ?? 0));
        }
    }
}
