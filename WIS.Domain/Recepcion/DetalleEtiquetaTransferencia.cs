using System;

namespace WIS.Domain.Recepcion
{
    public class DetalleEtiquetaTransferencia
    {
        public decimal NumeroEtiqueta { get; set; }             //NU_ETIQUETA
        public int NumeroSecEtiqueta { get; set; }              //NU_SEC_ETIQUETA
        public string UbicacionOrigen { get; set; }             //CD_ENDERECO_ORIGEN
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public string Producto { get; set; }                    //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public int Empresa { get; set; }                        //CD_EMPRESA
        public int NumeroSecDetalle { get; set; }               //NU_SEC_DETALLE
        public short? Estado { get; set; }                      //CD_SITUACAO
        public decimal? Cantidad { get; set; }                  //QT_PRODUTO
        public DateTime? Vencimiento { get; set; }              //DT_FABRICACAO
        public string Averia { get; set; }                      //ID_AVERIA
        public string InventarioCiclico { get; set; }           //ID_INVENTARIO_CICLICO
        public DateTime? FechaUltimoInventario { get; set; }    //DT_ULT_INVENTARIO
        public string ControlCalidadPendiente { get; set; }     //ID_CTRL_CALI_PEND
        public string UbicacionDestino { get; set; }            //CD_ENDERECO_DESTINO
        public int? Funcionario { get; set; }                   //CD_FUNCIONARIO
        public DateTime? FechaRegistro { get; set; }            //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public string AreaAveria { get; set; }                  //ID_AREA_AVERIA
        public long? Transaccion { get; set; }                  //NU_TRANSACCION
        public string Metadata { get; set; }                    //VL_METADATA
        public long? NroLpn { get; set; }                       //NU_LPN
    }
}
