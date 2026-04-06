using System;
using System.Collections.Generic;

namespace WIS.Domain.Picking
{
    public class DetallePedidoLpn
    {
        public string Pedido { get; set; }                      //NU_PEDIDO

        public int Empresa { get; set; }                        //CD_EMPRESA

        public string Cliente { get; set; }                     //CD_CLIENTE

        public decimal Faixa { get; set; }                      //CD_FAIXA

        public string Producto { get; set; }                    //CD_PRODUTO

        public DateTime? FechaAlta { get; set; }                //DT_ADDROW

        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW

        public string Identificador { get; set; }               //NU_IDENTIFICADOR

        public string IdEspecificaIdentificador { get; set; }   //ID_ESPECIFICA_IDENTIFICADOR

        public string IdLpnExterno { get; set; }                //ID_LPN_EXTERNO

        public string Tipo { get; set; }                        //TP_LPN_TIPO

        public decimal? CantidadPedida { get; set; }            //QT_PEDIDO

        public decimal? CantidadLiberada { get; set; }          //QT_LIBERADO

        public decimal? CantidadAnulada { get; set; }           //QT_ANULADO

        public long? Transaccion { get; set; }                  //NU_TRANSACCION

        public long? TransaccionDelete { get; set; }            //NU_TRANSACCION_DELETE

        public long? NumeroLpn { get; set; }                    //NU_LPN

        public List<DetallePedidoAtributosLpn> Atributos { get; set; }

        public DetallePedidoLpn()
        {
            Atributos = new List<DetallePedidoAtributosLpn>();
        }

        public virtual decimal GetSaldo()
        {
            return (this.CantidadPedida ?? 0) - (this.CantidadLiberada ?? 0) - (this.CantidadAnulada ?? 0);
        }

        public virtual void Anular(decimal cantidad)
        {
            this.CantidadAnulada = (this.CantidadAnulada ?? 0) + cantidad;
        }

        public virtual void AnularTotal()
        {
            this.CantidadAnulada = (this.CantidadAnulada ?? 0) + this.GetSaldo();
        }
    }
}
