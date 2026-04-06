using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Picking
{
    public class AnulacionPreparacionDetalle
    {
        public int NroAnulacionPreparacion { get; set; }        //NU_ANULACION_PREPARACION
        public int TipoAnulacion { get; set; }                  //TP_ANULACION
        public string TipoAgrupacion { get; set; }              //TP_AGRUPACION
        public string EstadoAnulacion { get; set; }             //ND_ESTADO_ANULACION
        public int? UserIdAnulacion { get; set; }               //USERID_ANULACION

        public int NumeroPreparacion { get; set; }              //NU_PREPARACION
        public string Producto { get; set; }                    //CD_PRODUTO
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Lote { get; set; }                        //NU_IDENTIFICADOR
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string Ubicacion { get; set; }                   //CD_ENDERECO
        public string Pedido { get; set; }                      //NU_PEDIDO
        public string Cliente { get; set; }                     //CD_CLIENTE
        public long? Carga { get; set; }                        //NU_CARGA
        public int NumeroSecuencia { get; set; }                //NU_SEQ_PREPARACION
        public string EspecificaLote { get; set; }              //ID_ESPECIFICA_IDENTIFICADOR
        public string Agrupacion { get; set; }                  //ID_AGRUPACION
        public decimal Cantidad { get; set; }                   //QT_PRODUTO
        public decimal? CantidadPreparada { get; set; }         //QT_PREPARADO
        public decimal? CantidadPickeo { get; set; }            //QT_PICKEO
        public decimal? CantidadControlada { get; set; }        //QT_CONTROLADO
        public decimal? CantidadControl { get; set; }           //QT_CONTROL
        public int? Usuario { get; set; }                       //CD_FUNCIONARIO
        public int? NumeroContenedorSys { get; set; }           //NU_CONTENEDOR_SYS
        public DateTime? FechaPickeo { get; set; }              //DT_PICKEO
        public int? NumeroContenedorPickeo { get; set; }        //NU_CONTENEDOR_PICKEO
        public int? UsuarioPickeo { get; set; }                 //CD_FUNC_PICKEO
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public DateTime? VencimientoPickeo { get; set; }        //DT_FABRICACAO_PICKEO
        public string AveriaPickeo { get; set; }                //ID_AVERIA_PICKEO
        public int? Proveedor { get; set; }                     //CD_FORNECEDOR
        public string AreaAveria { get; set; }                  //ID_AREA_AVERIA
        public string ErrorControl { get; set; }                //FL_ERROR_CONTROL
        public long? Transaccion { get; set; }                  //NU_TRANSACCION
        public string EstadoDetPicking { get; set; }            //ND_ESTADO_DETALLE_PICKING
        public string ReferenciaEstado { get; set; }            //VL_ESTADO_REFERENCIA
        public DateTime? FechaSeparacion { get; set; }          //DT_SEPARACION
        public string CanceladoId { get; set; }                 //FL_CANCELADO
        public int? NroContenedor { get; set; }                 //NU_CONTENEDOR
        public string TipoArmadoEgreso { get; set; }            //TP_ARMADO_EGRESO
        public string FacturaAutoCompletar { get; set; }        //FL_FACTURA_AUTO_COMPLETAR
        public long? IdDetallePickingLpn { get; set; }          //ID_DET_PICKING_LPN

        public int? NumeroSecuenciaDest { get; set; }   //NU_SEQ_PREPARACION_DEST
        public long? IdDetallePickingLpnDest { get; set; }

    }
}
