using System;

namespace WIS.Domain.Recepcion
{
    public class EtiquetaTransferencia
    {
        public decimal NumeroEtiqueta { get; set; }             //NU_ETIQUETA
        public int NumeroSecEtiqueta { get; set; }              //NU_SEC_ETIQUETA
        public string UbicacionReal { get; set; }               //CD_ENDERECO_REAL
        public string UbicacionDestino { get; set; }            //CD_ENDERECO_DESTINO
        public short Estado { get; set; }                       //CD_SITUACAO
        public string Predio { get; set; }                      //NU_PREDIO
        public DateTime? FechaFinalizacion { get; set; }        //DT_FINALIZACION
        public DateTime? FechaInsercion { get; set; }           //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public string AplicacionOrigen { get; set; }            //CD_APLICACAO_ORIGEN
        public string TpModalidadUso { get; set; }              //TP_MODALIDAD_USO
        public long? NumeroTransaccion { get; set; }            //NU_TRANSACCION
        public string IdExternoEtiqueta { get; set; }           //ID_EXTERNO_ETIQUETA
        public string TipoEtiquetaTransferencia { get; set; }   //TP_ETIQUETA_TRANSFERENCIA
        public long? NroLpn { get; set; }                       //NU_LPN

    }
}
