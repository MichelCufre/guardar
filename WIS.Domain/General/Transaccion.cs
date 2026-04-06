using System;

namespace WIS.Domain.General
{
    public class Transaccion
    {
        public long NumeroTransaccion { get; set; }           //NU_TRANSACCION
        public string DescripcionTransaccion { get; set; }    //DS_TRANSACCION
        public string CodigoAplicacion { get; set; }          //CD_APLICACION
        public int? CodigoFuncionario { get; set; }           //CD_FUNCIONARIO
        public string Data { get; set; }                      //VL_DATA
        public DateTime FechaAlta { get; set; }               //DT_ADDROW
        public DateTime? FechaFinTransaccion { get; set; }    //DT_ADDROW_FIN_TRAN
    }
}
