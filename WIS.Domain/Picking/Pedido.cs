using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Expedicion;
using WIS.Domain.StockEntities;

namespace WIS.Domain.Picking
{
    public class Pedido
    {
        public Pedido()
        {
            this.Lineas = new List<DetallePedido>();
            this.Lpns = new List<Lpn>();
        }

        public Pedido(string manualId, string tipoExpedicionId, string sincronizacionRealizadaId, string codigoAgente, string tipoAgente)
            : this()
        {
            ManualId = manualId;
            TipoExpedicionId = tipoExpedicionId;
            SincronizacionRealizadaId = sincronizacionRealizadaId;
            TipoAgente = tipoAgente;
            CodigoAgente = codigoAgente;
        }

        public string Id { get; set; }                            //NU_PEDIDO
        public string Cliente { get; set; }                       //CD_CLIENTE
        public int Empresa { get; set; }                          //CD_EMPRESA
        public string CondicionLiberacion { get; set; }           //CD_CONDICION_LIBERACION
        public int? FuncionarioResponsable { get; set; }          //CD_FUN_RESP
        public string Origen { get; set; }                        //CD_ORIGEN
        public string PuntoEntrega { get; set; }                  //CD_PUNTO_ENTREGA
        public int? Ruta { get; set; }                            //CD_ROTA
        public short Estado { get; set; }                         //CD_SITUACAO
        public int? CodigoTransportadora { get; set; }            //CD_TRANSPORTADORA
        public string CodigoUF { get; set; }                      //CD_UF
        public string Zona { get; set; }                          //CD_ZONA
        public string Anexo { get; set; }                         //DS_ANEXO1
        public string Anexo2 { get; set; }                        //DS_ANEXO2
        public string Anexo3 { get; set; }                        //DS_ANEXO3
        public string Anexo4 { get; set; }                        //DS_ANEXO4
        public string DireccionEntrega { get; set; }              //DS_ENDERECO
        public string Memo { get; set; }                          //DS_MEMO
        public string Memo1 { get; set; }                         //DS_MEMO_1
        public DateTime? FechaAlta { get; set; }                  //DT_ADDROW
        public DateTime? FechaEmision { get; set; }               //DT_EMITIDO
        public DateTime? FechaEntrega { get; set; }               //DT_ENTREGA
        public DateTime? FechaFuncionarioResponsable { get; set; }//DT_FUN_RESP
        public DateTime? FechaGenerica_1 { get; set; }            //DT_GENERICO_1
        public DateTime? FechaLiberarDesde { get; set; }          //DT_LIBERAR_DESDE
        public DateTime? FechaLiberarHasta { get; set; }          //DT_LIBERAR_HASTA
        public DateTime? FechaUltimaPreparacion { get; set; }     //DT_FUN_RESP
        public DateTime? FechaModificacion { get; set; }          //DT_UPDROW
        public bool IsSincronizacionRealizada { get; set; }       //FL_SYNC_REALIZADA
        public string Agrupacion { get; set; }                    //ID_AGRUPACION
        public bool IsManual { get; set; }                        //ID_MANUAL
        public string Actividad { get; set; }                     //ND_ACTIVIDAD
        public decimal? NuGenerico_1 { get; set; }                //NU_GENERICO_1
        public long? NroIntzFacturacion { get; set; }             //NU_INTERFAZ_FACTURACION
        public int? OrdenEntrega { get; set; }                    //NU_ORDEN_ENTREGA
        public short? NumeroOrdenLiberacion { get; set; }         //NU_ORDEN_LIBERACION
        public string IngresoProduccion { get; set; }             //NU_PRDC_INGRESO
        public string Predio { get; set; }                        //NU_PREDIO
        public int? NroPrepManual { get; set; }                   //NU_PREPARACION_MANUAL
        public int? PreparacionProgramada { get; set; }           //NU_PREPARACION_PROGRAMADA
        public long? Transaccion { get; set; }                    //NU_TRANSACCION
        public long? TransaccionDelete { get; set; }              //NU_TRANSACCION_DELETE
        public int? NumeroUltimaPreparacion { get; set; }         //NU_ULT_PREPARACION
        public ConfiguracionExpedicionPedido ConfiguracionExpedicion { get; set; }   //TP_EXPEDICION
        public string Tipo { get; set; }                          //TP_PEDIDO
        public string ComparteContenedorEntrega { get; set; }     //VL_COMPARTE_CONTENEDOR_ENTREGA
        public string ComparteContenedorPicking { get; set; }     //VL_COMPARTE_CONTENEDOR_PICKING
        public string VlGenerico_1 { get; set; }                  //VL_GENERICO_1
        public string VlSerealizado_1 { get; set; }               //VL_SERIALIZADO_1
        public long? NuCarga { get; set; }                         //NU_CARGA 

        public string Telefono { get; set; }                      //NU_TELEFONE
        public string TelefonoSecundario { get; set; }            //NU_TELEFONE2
        public decimal? Latitud { get; set; }                     //VL_LATITUD
        public decimal? Longitud { get; set; }                    //VL_LONGITUD

        public List<DetallePedido> Lineas { get; set; }

        #region WMS_API
        public string ManualId { get; set; }                    //ID_MANUAL
        public string TipoExpedicionId { get; set; }            //TP_EXPEDICION
        public string SincronizacionRealizadaId { get; set; }   //FL_SYNC_REALIZADA
        public string CodigoAgente { get; set; }
        public string TipoAgente { get; set; }

        public List<Lpn> Lpns{ get; set; }
        #endregion

        public virtual bool PuedeModificarse()
        {
            return this.Estado == SituacionDb.PedidoAbierto;
        }

        public virtual bool PuedeAnularse()
        {
            return this.Tipo != TipoPedidoDb.Produccion;
        }

        public virtual string GetLockId()
        {
            return $"{this.Empresa}#{this.Cliente}#{this.Id}";
        }

        public virtual bool FueLiberadoCompletamente(out decimal cantidadLiberada)
        {
            cantidadLiberada = this.Lineas.Sum(d => d.CantidadLiberada ?? 0);
            var cantidadPedida = this.Lineas.Sum(d => d.Cantidad ?? 0);
            var cantidadAnulada = this.Lineas.Sum(d => d.CantidadAnulada ?? 0);

            if (cantidadLiberada < cantidadPedida - cantidadAnulada)
                return false;

            return true;
        }

        public virtual void BloquearLiberacion(short orden, int preparacion)
        {
            this.NumeroOrdenLiberacion = orden;
            this.PreparacionProgramada = preparacion;
        }
        public virtual void DesbloquearLiberacion()
        {
            this.NumeroOrdenLiberacion = null;
            this.PreparacionProgramada = null;
        }

        public virtual bool TienePendientes()
        {
            var cantidadLiberada = this.Lineas.Sum(d => d.CantidadLiberada ?? 0);
            var cantidadPedida = this.Lineas.Sum(d => d.Cantidad ?? 0);
            var cantidadAnulada = this.Lineas.Sum(d => d.CantidadAnulada ?? 0);

            if (cantidadPedida > (cantidadLiberada + cantidadAnulada))
                return true;

            return false;
        }
    }
}
