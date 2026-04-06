using System;
using System.Collections.Generic;

namespace WIS.Domain.Picking
{
    public class DetallePedido
    {
        public DetallePedido()
        {
            Duplicados = new List<DetallePedidoDuplicado>();
            DetallesLpn = new List<DetallePedidoLpn>();
            Atributos = new List<DetallePedidoAtributos>();
        }

        public DetallePedido(string especificaIdentificadorId) : this()
        {
            EspecificaIdentificadorId = especificaIdentificadorId;
        }

        public string Id { get; set; }                          //NU_PEDIDO
        public int Empresa { get; set; }                        //CD_EMPRESA
        public string Cliente { get; set; }                     //CD_CLIENTE
        public decimal Faixa { get; set; }                      //CD_FAIXA
        public string Producto { get; set; }                    //CD_PRODUTO
        public string Memo { get; set; }                        //DS_MEMO
        public DateTime? FechaAlta { get; set; }                //DT_ADDROW
        public DateTime? FechaGenerica_1 { get; set; }          //DT_GENERICO_1
        public DateTime? FechaModificacion { get; set; }        //DT_UPDROW
        public string Agrupacion { get; set; }                  //ID_AGRUPACION
        public bool EspecificaIdentificador { get; set; }       //ID_ESPECIFICA_IDENTIFICADOR
        public decimal? NuGenerico_1 { get; set; }              //NU_GENERICO_1
        public string Identificador { get; set; }               //NU_IDENTIFICADOR
        public long? Transaccion { get; set; }                  //NU_TRANSACCION
        public long? TransaccionDelete { get; set; }            //NU_TRANSACCION_DELETE
        public decimal? CantidadAbastecida { get; set; }        //QT_ABASTECIDO
        public decimal? CantidadAnulada { get; set; }           //QT_ANULADO
        public decimal? CantidadAnuladaFactura { get; set; }    //QT_ANULADO_FACTURA
        public decimal? CantidadCargada { get; set; }           //QT_CARGADO
        public decimal? CantidadControlada { get; set; }        //QT_CONTROLADO
        public decimal? CantidadCrossDocking { get; set; }      //QT_CROSS_DOCK
        public decimal? CantidadExpedida { get; set; }          //QT_EXPEDIDO
        public decimal? CantidadFacturada { get; set; }         //QT_FACTURADO
        public decimal? CantidadLiberada { get; set; }          //QT_LIBERADO
        public decimal? Cantidad { get; set; }                  //QT_PEDIDO
        public decimal? CantidadOriginal { get; set; }          //QT_PEDIDO_ORIGINAL
        public decimal? CantidadPreparada { get; set; }         //QT_PREPARADO
        public decimal? CantidadTransferida { get; set; }       //QT_TRANSFERIDO
        public decimal? CantUndAsociadoCamion { get; set; }     //QT_UND_ASOCIADO_CAMION
        public string VlGenerico_1 { get; set; }                //VL_GENERICO_1
        public decimal? PorcentajeTolerancia { get; set; }      //VL_PORCENTAJE_TOLERANCIA
        public string DatosSerializados { get; set; }           //VL_SERIALIZADO_1
        public string SemiAcabado { get; set; }                 //FL_SEMIACABADO
        public string Consumible { get; set; }                  //FL_CONSUMIBLE
        public List<DetallePedidoDuplicado> Duplicados { get; set; }
        public List<DetallePedidoLpn> DetallesLpn { get; set; }
        public List<DetallePedidoAtributos> Atributos { get; set; }

        #region WMS_API
        public string EspecificaIdentificadorId { get; set; }    //ID_ESPECIFICA_IDENTIFICADOR
        #endregion

        public virtual decimal GetSaldo()
        {
            return (this.Cantidad ?? 0) - (this.CantidadLiberada ?? 0) - (this.CantidadAnulada ?? 0);
        }
        public virtual bool HasSaldo()
        {
            return this.GetSaldo() > 0;
        }

        public virtual bool TieneCantidadLiberadaOAnulada()
        {
            return this.CantidadLiberada > 0 || this.CantidadAnulada > 0;
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
