using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General.API.Dtos.Entrada
{
    public class PedidoResponse
    {
        [StringLength(40)]
        public string NroPedido { get; set; }                            //NU_PEDIDO

        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }
        public int Empresa { get; set; }                          //CD_EMPRESA

        [StringLength(6)]
        public string CondicionLiberacion { get; set; }           //CD_CONDICION_LIBERACION
        public int? FuncionarioResponsable { get; set; }          //CD_FUN_RESP

        [StringLength(30)]
        public string Origen { get; set; }                        //CD_ORIGEN

        [StringLength(20)]
        public string PuntoEntrega { get; set; }                  //CD_PUNTO_ENTREGA
        public int? Ruta { get; set; }                            //CD_ROTA
        public short Situacion { get; set; }                       //CD_SITUACAO
        public int? CodigoTransportadora { get; set; }            //CD_TRANSPORTADORA

        [StringLength(5)]
        public string CodigoUF { get; set; }                      //CD_UF

        [StringLength(10)]
        public string Zona { get; set; }                          //CD_ZONA

        [StringLength(200)]
        public string Anexo { get; set; }                         //DS_ANEXO1

        [StringLength(200)]
        public string Anexo2 { get; set; }                        //DS_ANEXO2

        [StringLength(200)]
        public string Anexo3 { get; set; }                        //DS_ANEXO3

        [StringLength(200)]
        public string Anexo4 { get; set; }                        //DS_ANEXO4

        [StringLength(100)]
        public string DireccionEntrega { get; set; }              //DS_ENDERECO

        [StringLength(1000)]
        public string Memo { get; set; }                          //DS_MEMO

        [StringLength(1000)]
        public string Memo1 { get; set; }                         //DS_MEMO_1

        public string FechaAlta { get; set; }                  //DT_ADDROW
        public string FechaEmision { get; set; }               //DT_EMITIDO
        public string FechaEntrega { get; set; }               //DT_ENTREGA
        public string FechaFuncionarioResponsable { get; set; }//DT_FUN_RESP
        public string FechaGenerica { get; set; }            //DT_GENERICO_1
        public string FechaLiberarDesde { get; set; }          //DT_LIBERAR_DESDE
        public string FechaLiberarHasta { get; set; }          //DT_LIBERAR_HASTA
        public string FechaUltimaPreparacion { get; set; }     //DT_FUN_RESP
        public string FechaModificacion { get; set; }          //DT_UPDROW

        [StringLength(1)]
        public string SincronizacionRealizada { get; set; }       //FL_SYNC_REALIZADA

        [StringLength(1)]
        public string Agrupacion { get; set; }                    //ID_AGRUPACION

        [StringLength(1)]
        public string IdManual { get; set; }                        //ID_MANUAL

        [StringLength(20)]
        public string Actividad { get; set; }                     //ND_ACTIVIDAD
        public decimal? NuGenerico { get; set; }                //NU_GENERICO_1
        public long? NroIntzFacturacion { get; set; }             //NU_INTERFAZ_FACTURACION
        public int? OrdenEntrega { get; set; }                    //NU_ORDEN_ENTREGA
        public short? NumeroOrdenLiberacion { get; set; }         //NU_ORDEN_LIBERACION

        [StringLength(10)]
        public string IngresoProduccion { get; set; }             //NU_PRDC_INGRESO

        [StringLength(10)]
        public string Predio { get; set; }                        //NU_PREDIO
        public int? NroPrepManual { get; set; }                   //NU_PREPARACION_MANUAL
        public int? PreparacionProgramada { get; set; }           //NU_PREPARACION_PROGRAMADA
        public long? Transaccion { get; set; }                    //NU_TRANSACCION
        public int? NumeroUltimaPreparacion { get; set; }         //NU_ULT_PREPARACION

        [StringLength(6)]
        public string TipoExpedicion { get; set; }   //TP_EXPEDICION

        [StringLength(6)]
        public string TipoPedido { get; set; }                         //TP_PEDIDO

        [StringLength(200)]
        public string ComparteContenedorEntrega { get; set; }    //VL_COMPARTE_CONTENEDOR_ENTREGA

        [StringLength(200)]
        public string ComparteContenedorPicking { get; set; }    //VL_COMPARTE_CONTENEDOR_PICKING

        [StringLength(400)]
        public string DsGenerico { get; set; }                 //VL_GENERICO_1

        [StringLength(4000)]
        public string Serealizado { get; set; }              //VL_SERIALIZADO_1

        public string Telefono { get; set; }                  //NU_TELEFONE

        public string TelefonoSecundario { get; set; }        //NU_TELEFONE2

        public decimal? Longitud { get; set; }                //VL_LONGITUD

        public decimal? Latitud { get; set; }                 //VL_LATITUD

        public List<PedidoDetalleResponse> Detalles { get; set; }
        public PedidoResponse()
        {
            Detalles = new List<PedidoDetalleResponse>();
        }
    }

    public class PedidoDetalleResponse
    {
        public decimal Faixa { get; set; }                   //CD_FAIXA

        [StringLength(40)]
        public string Producto { get; set; }                 //CD_PRODUTO

        [StringLength(200)]
        public string Memo { get; set; }                     //DS_MEMO
        public string FechaAlta { get; set; }             //DT_ADDROW
        public string FechaGenerica { get; set; }       //DT_GENERICO_1
        public string FechaModificacion { get; set; }     //DT_UPDROW

        [StringLength(1)]
        public string Agrupacion { get; set; }               //ID_AGRUPACION

        [StringLength(1)]
        public string EspecificaIdentificador { get; set; }    //ID_ESPECIFICA_IDENTIFICADOR
        public decimal? NuGenerico { get; set; }           //NU_GENERICO_1

        [StringLength(40)]
        public string Identificador { get; set; }            //NU_IDENTIFICADOR
        public long? Transaccion { get; set; }               //NU_TRANSACCION
        public decimal? CantidadAbastecida { get; set; }     //QT_ABASTECIDO
        public decimal? CantidadAnulada { get; set; }        //QT_ANULADO
        public decimal? CantidadAnuladaFactura { get; set; } //QT_ANULADO_FACTURA
        public decimal? CantidadCargada { get; set; }        //QT_CARGADO
        public decimal? CantidadControlada { get; set; }     //QT_CONTROLADO
        public decimal? CantidadCrossDocking { get; set; }   //QT_CROSS_DOCK
        public decimal? CantidadExpedida { get; set; }       //QT_EXPEDIDO
        public decimal? CantidadFacturada { get; set; }      //QT_FACTURADO
        public decimal? CantidadLiberada { get; set; }       //QT_LIBERADO
        public decimal? Cantidad { get; set; }               //QT_PEDIDO
        public decimal? CantidadOriginal { get; set; }       //QT_PEDIDO_ORIGINAL
        public decimal? CantidadPreparada { get; set; }      //QT_PREPARADO
        public decimal? CantidadTransferida { get; set; }    //QT_TRANSFERIDO
        public decimal? CantUndAsociadoCamion { get; set; }  //QT_UND_ASOCIADO_CAMION

        [StringLength(400)]
        public string DsGenerico { get; set; }             //VL_GENERICO_1
        public decimal? PorcentajeTolerancia { get; set; }   //VL_PORCENTAJE_TOLERANCIA

        [StringLength(4000)]
        public string Serializado { get; set; }        //VL_SERIALIZADO_1

        public List<PedidoDetalleDuplicadoResponse> Duplicados { get; set; }

        public PedidoDetalleResponse()
        {
            Duplicados = new List<PedidoDetalleDuplicadoResponse>();
        }
    }

    public class PedidoDetalleDuplicadoResponse
    {
        public string IdLineaSistemaExterno { get; set; }

        public string TipoLinea { get; set; }

        public decimal CantidadPedida { get; set; }

        public decimal? CantidadAnulada { get; set; }

        public decimal? CantidadExpedida { get; set; }

        public decimal? CantidadFacturada { get; set; }

        public string Serializado { get; set; }
    }
}
