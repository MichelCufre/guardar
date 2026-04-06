using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Facturacion
{
    public class FacturacionEmpresaProceso
    {
        public int CodigoEmpresa { get; set; }              //CD_EMPRESA
        public string CodigoProceso { get; set; }           //CD_PROCESO
        public string TipoUltimoProceso { get; set; }       //TP_ULTIMO_PROCESO
        public short? SituacionError { get; set; }          //CD_SITUACAO_ERROR
        public decimal? Resultado { get; set; }             //QT_RESULTADO
        public DateTime? UltimoProceso { get; set; }        //HR_ULTIMO_PROCESO
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
        public long? NumeroTransaccion { get; set; }        //NU_TRANSACCION
        public long? NumeroTransaccionDelete { get; set; }  //NU_TRANSACCION_DELETE
    }
}
