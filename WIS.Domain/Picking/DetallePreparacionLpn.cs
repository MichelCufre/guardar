using System;
using WIS.Domain.General;

namespace WIS.Domain.Picking
{
    public class DetallePreparacionLpn
    {
        public int NroPreparacion { get; set; }             //NU_PREPARACION
        public long IdDetallePickingLpn { get; set; }       //ID_DET_PICKING_LPN
        public int? IdDetalleLpn { get; set; }              //ID_LPN_DET
        public long? NroLpn { get; set; }                   //NU_LPN
        public int? Empresa { get; set; }                   //CD_EMPRESA
        public string Producto { get; set; }                //CD_PRODUTO
        public decimal? Faixa { get; set; }                 //CD_FAIXA
        public string Lote { get; set; }                    //NU_IDENTIFICADOR
        public string Atributos { get; set; }               //VL_ATRIBUTOS
        public decimal CantidadReservada { get; set; }      //QT_RESERVA
        public string TipoLpn { get; set; }                 //TP_LPN_TIPO
        public string Ubicacion { get; set; }               //CD_ENDERECO
        public DateTime? FechaAlta { get; set; }            //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }    //DT_UPDROW
        public long? Transaccion { get; set; }              //NU_TRANSACCION
        public long? TransaccionDelete { get; set; }        //NU_TRANSACCION_DELETE
        public string IdExternoLpn { get; set; }            //ID_LPN_EXTERNO
        public long? IdConfiguracion { get; set; }          //NU_DET_PED_SAI_ATRIB
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public decimal Cantidad { get; set; }

        public int? IdDetalleDestino { get; set; }
        public int EmpresaDestino { get; set; }
        public string CodigoProductoDestino { get; set; }
        public string IdentificadorDestino { get; set; }
        public decimal CantidadDestino { get; set; }
        public DateTime? VencimientoDestino { get; set; }
        public DateTime? Vencimiento { get; set; }
        public long? NumeroLPNDestino { get; set; }
        public string ClienteDestino { get; set; }
        public string PedidoDestino { get; set; }
        public int NumeroSecuencia { get;  set; }
        public string EspecificaLote { get;  set; }
        public string Agrupacion { get; set; }
        public long IdDetallePickingLpnDest { get;  set; }

        public virtual DetallePreparacionLpn CopiarLinea()
        {
            return new DetallePreparacionLpn()
            {
                NroPreparacion = this.NroPreparacion,
                IdDetallePickingLpn = this.IdDetallePickingLpn,
                IdDetalleLpn = this.IdDetalleLpn,
                NroLpn = this.NroLpn,
                Empresa = this.Empresa,
                Producto = this.Producto,
                Faixa = this.Faixa,
                Lote = this.Lote,
                Atributos = this.Atributos,
                CantidadReservada = this.CantidadReservada,
                TipoLpn = this.TipoLpn,
                Ubicacion = this.Ubicacion,
                FechaAlta = this.FechaAlta,
                FechaModificacion = this.FechaModificacion,
                Transaccion = this.Transaccion,
                TransaccionDelete = this.TransaccionDelete,
                IdExternoLpn = this.IdExternoLpn,
                IdConfiguracion = this.IdConfiguracion
            };
        }
    }
}
