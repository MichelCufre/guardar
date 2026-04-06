using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class ReferenciaRecepcionResponse
    {
        public int IdReferencia { get; set; }//NU_RECEPCION_REFERENCIA

        [StringLength(20)]
        public string Referencia { get; set; }//NU_REFERENCIA

        [StringLength(6)]
        public string TipoReferencia { get; set; }//TP_REFERENCIA

        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public int Empresa { get; set; }//CD_EMPRESA

        [StringLength(6)]
        public string Moneda { get; set; }//CD_MONEDA
        public int Situacion { get; set; }//CD_SITUACAO

        [StringLength(200)]
        public string Anexo1 { get; set; }//DS_ANEXO1

        [StringLength(200)]
        public string Anexo2 { get; set; }//DS_ANEXO2

        [StringLength(200)]
        public string Anexo3 { get; set; }//DS_ANEXO3

        [StringLength(200)]
        public string Memo { get; set; }//DS_MEMO

        [StringLength(10)]
        public string Predio { get; set; }//NU_PREDIO

        [StringLength(200)]
        public string Serializado { get; set; }//VL_SERIALIZADO

        [StringLength(20)]
        public string EstadoReferencia { get; set; }//ND_ESTADO_REFERENCIA

        public long? NumeroInterfazEjecucion { get; set; }//NU_INTERFAZ_EJECUCION
        public string FechaEmitida { get; set; }//DT_EMITIDA
        public string FechaEntrega { get; set; }//DT_ENTREGA
        public string FechaInsercion { get; set; }//DT_ADDROW
        public string FechaModificacion { get; set; }//DT_UPDROW
        public string FechaVencimientoOrden { get; set; }//DT_VENCIMIENTO_ORDEN

        public List<ReferenciaRecepcionDetalleResponse> Detalles { get; set; }
        public ReferenciaRecepcionResponse()
        {
            Detalles = new List<ReferenciaRecepcionDetalleResponse>();
        }
    }

    public class ReferenciaRecepcionDetalleResponse
    {
        public int IdReferenciaDetalle { get; set; }//NU_RECEPCION_REFERENCIA_DET
        public int IdReferencia { get; set; }//NU_RECEPCION_REFERENCIA

        [StringLength(40)]
        public string IdLineaSistemaExterno { get; set; }//ID_LINEA_SISTEMA_EXTERNO
        public int Empresa { get; set; }//CD_EMPRESA

        [StringLength(40)]
        public string CodigoProducto { get; set; }//CD_PRODUTO
        public decimal Faixa { get; set; }//CD_FAIXA

        [StringLength(40)]
        public string Identificador { get; set; }//NU_IDENTIFICADOR
        public decimal? CantidadReferencia { get; set; }//QT_REFERENCIA
        public decimal? CantidadAnulada { get; set; }//QT_ANULADA
        public decimal? CantidadAgendada { get; set; }//QT_AGENDADA
        public decimal? CantidadRecibida { get; set; }//QT_RECIBIDA
        public decimal? CantidadConfirmadaInterfaz { get; set; }//QT_CONFIRMADA_INTERFAZ
        public decimal? ImporteUnitario { get; set; }//IM_UNITARIO

        [StringLength(200)]
        public string Anexo1 { get; set; }//DS_ANEXO1
        public string FechaInsercion { get; set; }//DT_ADDROW
        public string FechaVencimiento { get; set; }//DT_VENCIMIENTO
        public string FechaModificacion { get; set; }//DT_UPDROW
    }
}
