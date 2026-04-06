using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Domain.Tracking.Models;
using WIS.TrafficOfficer;

namespace WIS.Domain.Recepcion
{
    public class Agenda
    {
        public int Id { get; set; }                                 //NU_AGENDA
        public int IdEmpresa { get; set; }                          //CD_EMPRESA
        public string CodigoInternoCliente { get; set; }            //CD_CLIENTE
        public string NumeroDocumento { get; set; }                 //NU_DOCUMENTO
        public DateTime? FechaInsercion { get; set; }               //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }            //DT_UPDROW
        public string Predio { get; set; }                          //NU_PREDIO
        public int? FuncionarioResponsable { get; set; }            //CD_FUN_RESP
        public DateTime? FechaFuncionarioResponsable { get; set; }  //DT_FUN_RESP
        public EstadoAgenda Estado { get; set; }                    //CD_SITUACAO
        public string TipoDocumento { get; set; }                   //CD_TIPO_DOCUMENTO
        public short? CodigoOperacion { get; set; }                 //CD_OPERACAO
        public short? CodigoPuerta { get; set; }                    //CD_PORTA
        public DateTime? FechaInicio { get; set; }                  //DT_INICIO
        public DateTime? FechaFin { get; set; }                     //DT_FIN
        public string PlacaVehiculo { get; set; }                   //DS_PLACA
        public string DUA { get; set; }                             //NU_DUA
        public string Anexo1 { get; set; }                          //DS_ANEXO1
        public string Anexo2 { get; set; }                          //DS_ANEXO2
        public string Anexo3 { get; set; }                          //DS_ANEXO3
        public string Anexo4 { get; set; }                          //DS_ANEXO4
        public bool EnviaDocumentacion { get; set; }                //ID_ENVIO_DOCUMENTACION
        public int? IdUsuarioEnvioDocumentacion { get; set; }       //CD_FUNC_ENVIO_DOCU
        public bool Averiado { get; set; }                          //ID_AVERIA
        public DateTime? FechaCierre { get; set; }                  //DT_CIERRE
        public DateTime? FechaEntrega { get; set; }                 //DT_ENTREGA
        public string TipoRecepcionInterno { get; set; }            //TP_RECEPCION
        public int? IdFuncionarioAsignado { get; set; }             //CD_FUNCIONARIO_ASIGNADO
        public long? NumeroInterfazEjecucion { get; set; }          //NU_INTERFAZ_EJECUCION
        public bool CargaDetalleAutomatica { get; set; }            //FL_CARGA_AUTO_DETALLE
        public int? NroOrdenTarea { get; set; }                     //NU_ORT_ORDEN
        public bool SincronizacionRealizada { get; set; }           //FL_SYNC_REALIZADA
        public long? NumeroTransaccion { get; set; }                //NU_TRANSACCION
        public RecepcionTipo TipoRecepcion { get; set; }            //TP_RECEPCION

        public bool EsFacturaValidada { get; set; }
        public List<AgendaDetalle> Detalles { get; set; }
        protected ICrearDetallesAgendaStrategy CreacionDetalleStrategy { get; set; }

        #region Api

        public short EstadoId { get; set; }                       //CD_SITUACAO
        public string CodigoAgente { get; set; }                  //CD_AGENTE
        public string TipoAgente { get; set; }                    //TP_AGENTE
        public bool? LiberarAgenda { get; set; }                  // => Liberacion automatica de la misma
        public string TipoReferenciaId { get; set; }
        public string CargaDetalleAutomaticaId { get; set; }      //FL_CARGA_AUTO_DETALLE
        public string AveriadoId { get; set; }                    //ID_AVERIA
        public string EnviaDocumentacionId { get; set; }          //ID_ENVIO_DOCUMENTACION
        public int? ReferenciaId { get; set; }                    //NU_RECEPCION_REFERENCIA
        public string ManejaInterfazId { get; set; }              //T_RECEPCION_REL_EMPRESA_TIPO.FL_MANEJO_INTERFAZ

        #endregion

        public Agenda()
        {
            this.Detalles = new List<AgendaDetalle>();
        }

        public Agenda(string codigoAgente, string tipoAgente, string tipoReferenciaId = null)
        {
            CodigoAgente = codigoAgente;
            TipoAgente = tipoAgente;
            TipoReferenciaId = tipoReferenciaId;
            this.Detalles = new List<AgendaDetalle>();
        }

        #region Auxs

        #region Consulta de Estados

        /// <summary>
        /// Verifica si la situación de la agenda es abierta
        /// </summary>
        /// <returns>True si la agenda esta abierta, false de lo contrario</returns>
        public virtual bool EnEstadoAbierta()
        {
            return this.Estado == EstadoAgenda.Abierta;
        }

        /// <summary>
        /// Verifica si la situación de la agenda es cerrada
        /// </summary>
        /// <returns>True si la agenda esta cerrada, false de lo contrario</returns>
        public virtual bool EnEstadoCerrada()
        {
            return this.Estado == EstadoAgenda.Cerrada;
        }

        /// <summary>
        /// Verifica si la situación de la agenda esta cancelada
        /// </summary>
        /// <returns>True si la agenda esta cancelada, false de lo contrario</returns>
        public virtual bool EnEstadoCancelada()
        {
            return this.Estado == EstadoAgenda.Cancelada;
        }

        /// <summary>
        /// Verifica si la situación de la agenda es ingresando facturas
        /// </summary>
        /// <returns>True si la agenda esta abierta, false de lo contrario</returns>
        public virtual bool EnEstadoIngresandoFacturas()
        {
            return this.Estado == EstadoAgenda.IngresandoFactura;
        }

        /// <summary>
        /// Verifica si la situación de la agenda es conferida sin diferencias
        /// </summary>
        /// <returns>True si la agenda es conferida sin diferencias, false de lo contrario</returns>
        public virtual bool EnEstadoConferidaSinDiferencias()
        {
            return this.Estado == EstadoAgenda.ConferidaSinDiferencias;
        }

        /// <summary>
        /// Verifica si la situación de la agenda es conferida con diferencias
        /// </summary>
        /// <returns>True si la agenda es conferida con diferencias, false de lo contrario</returns>
        public virtual bool EnEstadoConferidaConDiferencias()
        {
            return this.Estado == EstadoAgenda.ConferidaConDiferencias;
        }

        /// <summary>
        /// Verifica si la situación de la agenda es Documento Asociado
        /// </summary>
        /// <returns>True si la agenda esta en estado Documento Asociado, false de lo contrario</returns>
        public virtual bool EnEstadoDocumentoAsociado()
        {
            return this.Estado == EstadoAgenda.DocumentoAsociado;
        }

        #endregion

        public virtual string GetCompositeId()
        {
            return $"{Id}";
        }

        public virtual bool PuedeEditar()
        {
            if (SincronizacionRealizada)
                return false;

            return true;
        }
        public virtual bool PuedeDeshacerse()
        {
            if (Estado == EstadoAgenda.AguardandoDesembarque /*|| Estado == EstadoAgenda.ConferidaConDiferencias || Estado == EstadoAgenda.ConferidaSinDiferencias*/)
                return true;

            return false;
        }
        public virtual bool PuedeSerEditada()
        {
            if (Estado == EstadoAgenda.Abierta || Estado == EstadoAgenda.IngresandoFactura)
                return true;

            return false;
        }
        public virtual bool PuedeCerrarAgenda()
        {
            return EnEstadoConferidaSinDiferencias();
        }
        public virtual bool PuedeCancelarAgenda(IUnitOfWork uow)
        {
            switch (this.Estado)
            {
                case EstadoAgenda.Abierta:
                case EstadoAgenda.IngresandoFactura:
                case EstadoAgenda.AguardandoDesembarque:
                case EstadoAgenda.DocumentoAsociado:
                    return true;
                case EstadoAgenda.ConferidaSinDiferencias:
                    if (uow.EtiquetaLoteRepository.AnyDetalleEtiquetaConStockRecibido(this.Id))
                        return false;
                    else
                        return true;
                default:
                    return false;
            }
        }
        public virtual void MarcarParaGenerarInterfaz()
        {
            this.NumeroInterfazEjecucion = -1;
        }
        public virtual bool PuedeLiberarse()
        {
            if (this.TipoRecepcion != null)
            {
                // Si agenda acepta productos no esperados, es de tipo digitación libre y se encuentra en estado abierto o con documento asociado, permite liberar
                if (this.TipoRecepcion.AceptaProductosNoEsperados
                    && (this.TipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.Libre)
                    && (this.EnEstadoAbierta() || this.EnEstadoDocumentoAsociado()))
                    return true;

                //Consulto si agenda tiene al menos 1 detalle, en caso contrario no serviria liberar esa agenda.
                if (this.Detalles.Count() > 0)
                    return (this.EnEstadoAbierta() || this.EnEstadoDocumentoAsociado() || this.EnEstadoIngresandoFacturas()) ? true : false;
            }

            return false;
        }
        public virtual bool ComprobarYCambiarEstadoAgendaSinDiferencias()
        {
            if (!this.Detalles.Any(d => d.TieneProblemasSinResolver()))
            {
                this.Estado = EstadoAgenda.ConferidaSinDiferencias;
                return true;
            }
            return false;
        }
        public virtual AgendaDetalle GetDetalleAgendaAfectadoProblema(int idProblema)
        {
            return Detalles.FirstOrDefault(s => s.ProblemasRecepcion.Any(d => d.Id == idProblema));
        }
        public virtual bool PuedeEnviarTracking(List<PuntoDeEntregaCliente> entregas, RecepcionTipo tpRec)
        {
            if (entregas != null && tpRec != null)
            {
                if (this.Detalles != null && this.Detalles.Count > 0)
                {
                    if (tpRec.TipoAgente == TipoAgenteDb.Cliente && !this.SincronizacionRealizada)
                    {
                        var entregasCliente = entregas.Where(e => e.CodigoCliente == this.CodigoInternoCliente && e.Empresa == this.IdEmpresa).ToList();
                        if (entregasCliente != null && entregasCliente.Count > 0)
                            return this.EnEstadoAbierta();
                    }
                }
            }
            return false;
        }

        // Crear detalle de Agenda
        public virtual void CrearDetallesAgenda()
        {
            this.Detalles = CreacionDetalleStrategy.CrearDetallesAgenda();
        }

        public virtual void SetCrearDetalleStrategy(ICrearDetallesAgendaStrategy strategy)
        {
            CreacionDetalleStrategy = strategy;
        }

        public virtual void SetCrearDetalleStrategy(IUnitOfWork uow, ITrafficOfficerService concurrencyControl, List<int> keysReferencias, List<int> keysFacturas)
        {
            switch (this.TipoRecepcion.TipoSeleccionReferencia)
            {
                case TipoSeleccionReferenciaDb.Libre:
                    this.SetCrearDetalleStrategy(new CrearDetalleAgendaLibreStrategy());
                    break;
                case TipoSeleccionReferenciaDb.Lpn:
                    this.SetCrearDetalleStrategy(new CrearDetalleAgendaLpnStrategy());
                    break;
                case TipoSeleccionReferenciaDb.MonoSeleccion:
                    this.SetCrearDetalleStrategy(new CrearDetalleAgendaMonoSeleccionStrategy(uow, concurrencyControl, this, keysReferencias.FirstOrDefault()));
                    break;
                case TipoSeleccionReferenciaDb.MultiSeleccion:
                    this.SetCrearDetalleStrategy(new CrearDetalleAgendaMultiSeleccionStrategy(uow, concurrencyControl, this, keysReferencias));
                    break;

                case TipoSeleccionReferenciaDb.Factura:
                    this.SetCrearDetalleStrategy(new CrearDetalleAgendaMultiSeleccionConFacturaStrategy(uow, this, keysReferencias));
                    break;
                case TipoSeleccionReferenciaDb.Bolsa:
                    this.SetCrearDetalleStrategy(new CrearDetalleAgendaBolsaConFacturaStrategy(uow, keysReferencias, keysFacturas));
                    break;

            }
        }

        #endregion

    }
}