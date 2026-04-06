using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Documento.Integracion.Recepcion;
using WIS.Domain.Logic;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Domain.Recepcion
{
    public class CerrarAgenda
    {
        protected readonly IUnitOfWork _uow;
        protected ITrafficOfficerService _concurrencyControl;
        protected string _aplicacion;
        protected int _usuario;
        protected string _predio;
        protected readonly Logger _logger;
        protected Agenda agenda;
        protected CrossDockingAgenda _crossDockingAgenda;
        protected readonly IFactoryService _factoryService;
        protected readonly IParameterService _parameterService;
        protected readonly IIdentityService _identity;

        public CerrarAgenda(IUnitOfWork uow,
            ITrafficOfficerService concurrencyControl,
            int usuario,
            string aplicacion,
            string predio,
            Agenda agenda,
            Logger logger,
            IFactoryService factoryService,
            IParameterService parameterService,
            IIdentityService identity)
        {
            this._uow = uow;
            this._concurrencyControl = concurrencyControl;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._predio = predio;
            this.agenda = agenda;
            this._logger = logger;
            this._crossDockingAgenda = new CrossDockingAgenda(uow, agenda);
            this._factoryService = factoryService;
            this._parameterService = parameterService;
            this._identity = identity;
        }

        public virtual bool PuedeCerrarAgenda()
        {
            return agenda != null && agenda.EnEstadoConferidaSinDiferencias();
        }

        public virtual List<long> ProcesarCierreAgenda(TrafficOfficerTransaction transaction, out List<String> cambiosUbicacionesComprobarStock)
        {
            var reports = new List<long>();

            if (agenda.Estado != Enums.EstadoAgenda.ConferidaSinDiferencias)
                throw new Exception("REC170_Sec0_Error_AgendaConDiferencias");

            cambiosUbicacionesComprobarStock = new List<string>();

            var cambiosStock = new EntityChanges<Stock>();
            var cambiosEtiquetaLote = new EntityChanges<EtiquetaLote>();
            var cambiosDetalleAgenda = new EntityChanges<AgendaDetalle>();
            var cambiosDetalleReferencia = new EntityChanges<ReferenciaRecepcionDetalle>();
            var cambiosAsociaciones = new EntityChanges<DetalleAgendaReferenciaAsociada>();

            LockearAgendaConDetalles(transaction);

            var tipoRecepcion = _uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);

            agenda.Estado = Enums.EstadoAgenda.Cerrada;
            agenda.FechaCierre = DateTime.Now;
            agenda.MarcarParaGenerarInterfaz();

            _logger.Info($"CIERRE_AGENDA: {agenda.Id} - Actualizo estado agenda");

            EliminarLineasAuxiliares(transaction, cambiosDetalleReferencia);

            // Prorrate cantidad y carga en objeto detalle de agenda las referencias asociadas para trabajarlas luego
            ProrratearCantidades(transaction, cambiosAsociaciones, cambiosDetalleReferencia, false);

            //Anular saldo de referencias
            CancelarSaldoCierre(transaction, tipoRecepcion, cambiosDetalleReferencia);

            // Ajuste de etiquetas de recepción
            var detallesEtiquetaLoteAgenda = _uow.EtiquetaLoteRepository.GetDetallesDeEtiquetaLoteAgenda(agenda.Id);

            LockearDetallesEtiqueta(transaction, detallesEtiquetaLoteAgenda);

            //Elimino posibles reservas de entradas no recibidas
            _logger.Info($"CIERRE_AGENDA: {agenda.Id} - Eliminar Reservas no recibidas");
            EliminarReservasNoRecibidas(cambiosStock, cambiosUbicacionesComprobarStock, detallesEtiquetaLoteAgenda);

            //Seteo situacion de Etiquetas en 29 (sin productos a las etiquetas que no fueron recibidas)
            ActualizarEstadoEtiquetaSinProductos(cambiosEtiquetaLote, detallesEtiquetaLoteAgenda);

            AgendarEnvasesRecibidoFicticio(cambiosDetalleAgenda);

            if (EsEmpresaDocumental(agenda.IdEmpresa))
            {
                var ingreso = new IngresoDocumental(this._factoryService, this._parameterService, this._identity);
                ingreso.CerrarAgendaDocumental(this._uow, agenda.Id, this._usuario, this._predio);
            }

           // _crossDockingAgenda.DesatenderPedidosCrossDocking(false);

            _logger.Info($"CIERRE_AGENDA: {agenda.Id} - Preparar reportes");

            var lReportes = new LReportes(_uow, _usuario, _aplicacion);
            var predio = agenda.Predio ?? _identity.Predio;

            reports.AddRange(lReportes.PrepararReportes(agenda, predio));

            //Seteo situacion de Etiquetas en 29 (sin productos a las etiquetas conferidas con cantidad recibida 0)
            ActualizarEstadoEtiquetaConferidaSinProductos(cambiosEtiquetaLote, detallesEtiquetaLoteAgenda);

            agenda.NumeroTransaccion = _uow.GetTransactionNumber();
            _uow.AgendaRepository.UpdateAgendaSinDependencias(agenda);

            UpdateAgendaDetalles(cambiosDetalleAgenda);
            UpdateReferenciaDetalles(cambiosDetalleReferencia);
            UpdateDetalleAgendaReferenciaAsociada(cambiosAsociaciones);
            UpdateStocks(cambiosStock);
            UpdateEtiquetaLote(cambiosEtiquetaLote);

            return reports;
        }

        #region Metodos Auxiliares

        public virtual void LockearAgendaConDetalles(TrafficOfficerTransaction transaction)
        {
            if (_concurrencyControl.IsLocked("T_AGENDA", agenda.GetCompositeId(), true))
                throw new ValidationFailedException("REC170_msg_Error_AgendaBloqueada", [agenda.GetCompositeId()]);

            _concurrencyControl.AddLock("T_AGENDA", agenda.GetCompositeId(), transaction, true);

            foreach (var detalle in agenda.Detalles)
            {
                _concurrencyControl.AddLock("T_DET_AGENDA", detalle.GetCompositeId(), transaction, true);
            }
        }

        public virtual void EliminarLineasAuxiliares(TrafficOfficerTransaction transaction, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            var detalle = ProblemaAgendaDb.DescripcionExcedeSaldoReferenciaRecepcion + agenda.Id;

            var referenciasAsociadas = _uow.ReferenciaRecepcionRepository.GetAsociacionesAgendaConDetallesAuxiliares(agenda.Id, detalle);

            foreach (var detalleReferencia in referenciasAsociadas)
            {
                _concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", detalleReferencia.DetalleReferencia.GetCompositeId(), transaction, true);

                cambiosDetalleReferencia.DeletedRecords.Add(detalleReferencia.DetalleReferencia);

                _logger.Info($"CIERRE_AGENDA: {agenda.Id} - Marco para eliminar detalle de referencia auxiliar : {detalleReferencia.DetalleReferencia.Id}");
            }
        }

        public virtual void UpdateReferenciaDetalles(EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            cambiosDetalleReferencia.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosDetalleReferencia.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            foreach (var dr in cambiosDetalleReferencia.DeletedRecords)
            {
                dr.NumeroTransaccion = nuTransaccion;
                dr.NumeroTransaccionDelete = nuTransaccion;
                dr.FechaModificacion = DateTime.Now;

                _uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalle(dr);
            }

            _uow.SaveChanges();
            _uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalles(cambiosDetalleReferencia);
        }

        public virtual void UpdateAgendaDetalles(EntityChanges<AgendaDetalle> cambiosDetalleAgenda)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            cambiosDetalleAgenda.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosDetalleAgenda.DeletedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosDetalleAgenda.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            _uow.AgendaRepository.UpdateAgendaDetalles(cambiosDetalleAgenda);
        }

        public virtual void UpdateEtiquetaLote(EntityChanges<EtiquetaLote> cambiosEtiquetaLote)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            cambiosEtiquetaLote.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosEtiquetaLote.DeletedRecords.ForEach(d => d.NumeroTransaccion = d.NumeroTransaccionDelete = nuTransaccion);
            cambiosEtiquetaLote.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            _uow.EtiquetaLoteRepository.UpdateEtiquetaLote(cambiosEtiquetaLote);
        }

        public virtual void UpdateDetalleAgendaReferenciaAsociada(EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            cambiosAsociaciones.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosAsociaciones.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            foreach (var dr in cambiosAsociaciones.DeletedRecords)
            {
                dr.NumeroTransaccion = nuTransaccion;
                dr.NumeroTransaccionDelete = nuTransaccion;
                _uow.ReferenciaRecepcionRepository.UpdateDetalleAgendaReferenciaAsociada(dr);
            }

            _uow.SaveChanges();
            _uow.ReferenciaRecepcionRepository.UpdateDetalleAgendaReferenciaAsociada(cambiosAsociaciones);
        }

        public virtual void UpdateStocks(EntityChanges<Stock> cambiosStock)
        {
            var nuTransaccion = _uow.GetTransactionNumber();

            cambiosStock.AddedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);
            cambiosStock.UpdatedRecords.ForEach(d => d.NumeroTransaccion = nuTransaccion);

            foreach (var dr in cambiosStock.DeletedRecords)
            {
                dr.NumeroTransaccion = nuTransaccion;
                dr.NumeroTransaccionDelete = nuTransaccion;
                _uow.StockRepository.UpdateStock(dr);
            }

            _uow.SaveChanges();
            _uow.StockRepository.UpdateStocks(cambiosStock);
        }

        public virtual bool EsEmpresaDocumental(int empresa)
        {
            return this._parameterService.GetValueByEmpresa(ParamManager.MANEJO_DOCUMENTAL, empresa) == "S";
        }

        public virtual void LockearDetallesEtiqueta(TrafficOfficerTransaction transaction, List<EtiquetaLoteDetalle> detallesEtiquetas)
        {
            foreach (var detalle in detallesEtiquetas)
            {
                _concurrencyControl.AddLock("T_DET_ETIQUETA_LOTE", detalle.GetCompositeId(), transaction, true);
            }
        }

        public virtual void CancelarSaldoCierre(TrafficOfficerTransaction transaction, RecepcionTipo tipoRecepcion, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            if (tipoRecepcion.CancelarSaldosReferenciasAlCierreDeAgenda)
            {
                _logger.Info($"CIERRE_AGENDA: {agenda.Id} - Cancelar saldos detalles de referencia");

                // Anulo los detalles de referencias asociados a los detalles de agenda
                foreach (var detalleAgenda in agenda.Detalles)
                {
                    foreach (var asociacion in detalleAgenda.AsociacionesDetalleReferencia.Where(s => s.DetalleReferencia.GetSaldo() > 0 && s.DetalleReferencia.CantidadReferencia > 0).ToList())
                    {
                        this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", asociacion.DetalleReferencia.GetCompositeId(), transaction, true);

                        asociacion.DetalleReferencia.CantidadAnulada = asociacion.DetalleReferencia.CantidadReferencia - asociacion.DetalleReferencia.CantidadRecibida - asociacion.DetalleReferencia.CantidadAgendada;
                        UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);
                    }
                }
            }
        }

        public virtual void EliminarReservasNoRecibidas(EntityChanges<Stock> cambiosStock, List<String> cambiosUbicacionesComprobarStock, List<EtiquetaLoteDetalle> detallesEtiquetas)
        {
            //Busco los detalles con reserva no recibidas
            var detalles = detallesEtiquetas.Where(s => s.CantidadEtiquetaGenerada > (s.CantidadRecibida ?? 0)).OrderBy(s => s.EtiquetaLote.IdUbicacionSugerida);

            foreach (var detalle in detalles)
            {
                var stock = _uow.StockRepository.GetStock(detalle.IdEmpresa, detalle.CodigoProducto, detalle.Faixa, detalle.EtiquetaLote.IdUbicacionSugerida, detalle.Identificador);

                stock.CantidadTransitoEntrada -= (detalle.CantidadEtiquetaGenerada - (detalle.CantidadRecibida ?? 0));
                stock.NumeroTransaccion = _uow.GetTransactionNumber();

                UpdateRecordStock(cambiosStock, stock);

                if (!string.IsNullOrEmpty(detalle.EtiquetaLote.IdUbicacionSugerida) && (detalle.CantidadRecibida ?? 0) == 0)
                    UpdateRecordUbicacionesAComprobarStock(cambiosUbicacionesComprobarStock, stock.Ubicacion);
            }
        }

        public virtual void ActualizarEstadoEtiquetaSinProductos(EntityChanges<EtiquetaLote> cambiosEtiquetaLote, List<EtiquetaLoteDetalle> detallesEtiquetas)
        {
            var etiquetasSinProductos = detallesEtiquetas
                .GroupBy(d => d.IdEtiquetaLote)
                .Select(d => new
                {
                    cantidadRecibida = d.Sum(e => e.Cantidad ?? 0),
                    IdEtiquetaLote = d.Key //NU_ETIQUETA_LOTE, por problema con generacion de codigo SQL se usa el Primary Key y no el valor de la columna.
                })
                .Where(c => c.cantidadRecibida == 0)
                .ToList();

            foreach (var etiqueta in etiquetasSinProductos)
            {
                var etiquetaLote = detallesEtiquetas.Where(s => s.EtiquetaLote.Numero == etiqueta.IdEtiquetaLote).Select(s => s.EtiquetaLote).FirstOrDefault();

                etiquetaLote.Estado = SituacionDb.PalletSinProductos;
                UpdateRecordEtiquetaLote(cambiosEtiquetaLote, etiquetaLote);

                _logger.Info("CIERRE_AGENDA - Cambio situación ET sin productos a 29 ET: " + etiquetaLote.Numero);
            }
        }

        public virtual void ActualizarEstadoEtiquetaConferidaSinProductos(EntityChanges<EtiquetaLote> cambiosEtiquetaLote, List<EtiquetaLoteDetalle> detallesEtiquetas)
        {
            // Elimino (pasar a 29) etiquetas en situacion 23 (Pallet conferido) que no tengan productos ----
            var etiquetasSinProductos = detallesEtiquetas
                .Where(e => e.EtiquetaLote.Estado == SituacionDb.PalletConferido)
                .ToList()
                .GroupBy(e => e.IdEtiquetaLote)
                .Select(d => new
                {
                    cantidadRecibida = d.Sum(e => e.CantidadRecibida ?? 0),
                    IdEtiquetaLote = d.Key
                })
                .Where(c => c.cantidadRecibida == 0)
                .ToList();

            foreach (var etiqueta in etiquetasSinProductos)
            {
                var etiquetaLote = detallesEtiquetas.Where(s => s.EtiquetaLote.Numero == etiqueta.IdEtiquetaLote).Select(s => s.EtiquetaLote).FirstOrDefault();

                etiquetaLote.Estado = SituacionDb.PalletSinProductos;
                UpdateRecordEtiquetaLote(cambiosEtiquetaLote, etiquetaLote);

                _logger.Info("CIERRE_AGENDA - Cambio situación ET sin productos 23 a 29 ET: " + etiquetaLote.Numero);
            }
        }

        public virtual void AgendarEnvasesRecibidoFicticio(EntityChanges<AgendaDetalle> cambiosDetalleAgenda)
        {
            _logger.Info($"CIERRE_AGENDA: {agenda.Id} - Agendar envasess Recibidos ficticio");

            var loteEnvase = ManejoIdentificadorDb.IdentificadorProducto;

            var dbQuery = new GetEnvasesRecibidosQuery(agenda.Id);
            _uow.HandleQuery(dbQuery);

            var envases = dbQuery.GetEnvasesRecibidos();

            foreach (var envase in envases)
            {
                var detalleAgenda = agenda.Detalles.FirstOrDefault(d => d.IdEmpresa == envase.IdEmpresa
                                                          && d.Faixa == envase.Faixa
                                                          && d.CodigoProducto == envase.Codigo
                                                          && d.Identificador == loteEnvase);
                if (detalleAgenda != null)
                {
                    // Actualizao detalle de agenda con cantidad de envase recibido

                    detalleAgenda.CantidadRecibidaFicticia += envase.Cantidad;
                    detalleAgenda.CantidadRecibida += envase.Cantidad;
                    detalleAgenda.FechaModificacion = DateTime.Now;
                    detalleAgenda.NumeroTransaccion = _uow.GetTransactionNumber();

                    UpdateRecordDetalleAgenda(cambiosDetalleAgenda, detalleAgenda);
                }
                else
                {
                    //Si tenía un envase y no lo pude dar de alta, tengo que insertar en la agenda

                    detalleAgenda = new AgendaDetalle()
                    {
                        IdAgenda = agenda.Id,
                        CodigoProducto = envase.Codigo,
                        Identificador = loteEnvase,
                        Faixa = envase.Faixa,
                        IdEmpresa = envase.IdEmpresa,
                        Estado = Enums.EstadoAgendaDetalle.ConferidaSinDiferencias,
                        CantidadRecibida = envase.Cantidad,
                        CantidadRecibidaFicticia = envase.Cantidad,
                        CantidadAgendada = 0,
                        CantidadCrossDocking = 0,
                        CantidadAgendadaOriginal = 0,
                        FechaAlta = DateTime.Now,
                        FechaModificacion = DateTime.Now,
                        NumeroTransaccion = _uow.GetTransactionNumber(),
                    };

                    agenda.Detalles.Add(detalleAgenda);
                    cambiosDetalleAgenda.AddedRecords.Add(detalleAgenda);
                }
            }
        }


        #region  - PRORRATEO DE CANTIDADES -

        public virtual void ProrratearCantidades(TrafficOfficerTransaction transaction, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia, bool vaciarAgendadoRelaciones = true)
        {
            if (_uow.ReferenciaRecepcionRepository.AnyAsociacionesAgendaConDetallesReferencias(agenda.Id))
            {
                _logger.Info($"CIERRE_AGENDA: {agenda.Id} - Inicio prorrateo de cantidades");

                // Levanto todas las asociaciones de la agenda y las cargo en los detalles
                var asociaciones = _uow.ReferenciaRecepcionRepository.GetAllDetalleAgendaReferenciaAsociadaActivas(agenda);

                // Levanto las referencias disponibles no asociadas en caso de haber un sobrante no agendado
                var detallesReferenciasNoAsociados = _uow.ReferenciaRecepcionRepository.GetDetalleReferenciasNoAsociadasDisponibles(agenda, asociaciones.Where(s => s.DetalleReferencia.Identificador == ManejoIdentificadorDb.IdentificadorAuto).ToList());

                int nroSobrante = 1;

                //Recorrer lineas con identificador especificado y que no sea (AUTO) ya que no se reciben lotes "(AUTO)"
                // Procesos todo lo esperado con lote
                var detallesAgenda = agenda.Detalles.Where(s => s.CantidadAgendadaOriginal != 0 && s.Identificador != ManejoIdentificadorDb.IdentificadorAuto).ToList();
                this.ProrratearDetallesDeAgenda(transaction, asociaciones, detallesReferenciasNoAsociados, detallesAgenda, ref nroSobrante, cambiosAsociaciones, cambiosDetalleReferencia);

                //Recorrer lineas con lotes recibidos no esperados, que consumen de referencias (AUTO)
                var detallesAgendaAuto = agenda.Detalles.Where(s => s.CantidadAgendadaOriginal == 0).ToList();
                this.ProrratearDetallesDeAgenda(transaction, asociaciones, detallesReferenciasNoAsociados, detallesAgendaAuto, ref nroSobrante, cambiosAsociaciones, cambiosDetalleReferencia);
                // this.ProrratearDetallesDeAgendaAuto(transaction, asociaciones, detallesReferenciasNoAsociados, detallesAgendaAuto, ref nroSobrante, cambiosAsociaciones, cambiosDetalleReferencia);

                // Liberar saldo no recibido
                this.LiberarCantidadesAgendadasNoRecibidas(asociaciones, cambiosDetalleReferencia);

                //Pasar cantidad agendada de las asociaciones de detalles de agenda y detalle de referencia a 0
                if (vaciarAgendadoRelaciones)
                {
                    this.VaciarCantidadAgendado(agenda.Detalles, cambiosAsociaciones, cambiosDetalleReferencia);
                }

            }

        }

        public virtual void ProrratearDetallesDeAgenda(TrafficOfficerTransaction transaction, List<DetalleAgendaReferenciaAsociada> asociaciones, List<ReferenciaRecepcionDetalle> detallesReferenciasNoasociados, List<AgendaDetalle> lineasAgenda, ref int nroSobrante, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            foreach (var linea in lineasAgenda.Where(s => s.CantidadRecibida > 0))
            {

                decimal cantRecibida = linea.CantidadRecibida;

                // Ajusto cantidades en las asociaciones del detalle de agenda
                this.AjustarCantidadesRecibidas(transaction, linea.AsociacionesDetalleReferencia, ref cantRecibida, cambiosAsociaciones, cambiosDetalleReferencia);

                if (cantRecibida > 0)
                {
                    // Si no tengo saldo en las asociaciones originales como fueron agendadas busco si las referencias de esas asociaciones contienen saldo para consumir.

                    this.AjustarCantidadesRecibidasAsociadasConSaldoenReferenciaDisponible(transaction, linea.AsociacionesDetalleReferencia, ref cantRecibida, cambiosAsociaciones, cambiosDetalleReferencia);

                }

                if (cantRecibida > 0)
                {
                    // Si no tengo saldo en las referencias con identificador asociadas previamente en el agendamiento 
                    // busco otras referencias de la agenda con lote especificado

                    var noAsociadoConSaldo = detallesReferenciasNoasociados.Where(s => s.IdEmpresa == linea.IdEmpresa
                                                                                   && s.CodigoProducto == linea.CodigoProducto
                                                                                   && s.Faixa == linea.Faixa
                                                                                   && s.Identificador == linea.Identificador)
                                                                      .ToList();

                    this.AjustarCantidadesRecibidasNoAsociadas(transaction, noAsociadoConSaldo, linea, ref cantRecibida, cambiosAsociaciones, cambiosDetalleReferencia);
                }

                if (cantRecibida > 0)
                {
                    // Si no tengo saldo en las referencias con identificador, consumo de las (AUTO) asociadas previamente en el agendamiento

                    var relLineaAuto = asociaciones.Where(d => d.DetalleReferencia.IdEmpresa == linea.IdEmpresa
                                                            && d.DetalleReferencia.CodigoProducto == linea.CodigoProducto
                                                            && d.DetalleReferencia.Faixa == linea.Faixa
                                                            && d.DetalleReferencia.Identificador == ManejoIdentificadorDb.IdentificadorAuto).ToList();

                    this.AjustarCantidadesRecibidas(transaction, relLineaAuto, ref cantRecibida, cambiosAsociaciones, cambiosDetalleReferencia, linea);
                }
                //----
                if (cantRecibida > 0)
                {
                    // Si no tengo saldo en las asociaciones (AUTO) originales como fueron agendadas busco si las referencias de esas asociaciones contienen saldo para consumir.

                    var relLineaAuto = asociaciones.Where(d => d.DetalleReferencia.IdEmpresa == linea.IdEmpresa
                                                          && d.DetalleReferencia.CodigoProducto == linea.CodigoProducto
                                                          && d.DetalleReferencia.Faixa == linea.Faixa
                                                          && d.DetalleReferencia.Identificador == ManejoIdentificadorDb.IdentificadorAuto).ToList();

                    this.AjustarCantidadesRecibidasAsociadasConSaldoenReferenciaDisponible(transaction, relLineaAuto, ref cantRecibida, cambiosAsociaciones, cambiosDetalleReferencia);

                }
                //--

                if (cantRecibida > 0)
                {
                    // Si no tengo saldo en las referencias con identificador (AUTO) asociadas previamente en el agendamiento 
                    //busco otras (AUTO) en las referencias de la agenda

                    var noAsociadoConSaldo = detallesReferenciasNoasociados.Where(s => s.IdEmpresa == linea.IdEmpresa
                                                                                   && s.CodigoProducto == linea.CodigoProducto
                                                                                   && s.Faixa == linea.Faixa
                                                                                   && s.Identificador == ManejoIdentificadorDb.IdentificadorAuto)
                                                                          .ToList();

                    this.AjustarCantidadesRecibidasNoAsociadas(transaction, noAsociadoConSaldo, linea, ref cantRecibida, cambiosAsociaciones, cambiosDetalleReferencia);
                }

                if (cantRecibida > 0)
                {
                    // Por ultimo si no tengo de donde sacar saldo agrego un sobrante
                    ReferenciaRecepcion referencia = null;
                    if (linea.AsociacionesDetalleReferencia.Any())
                    {
                        referencia = linea.AsociacionesDetalleReferencia.First().DetalleReferencia.ReferenciaRecepcion;
                    }
                    else
                    {
                        referencia = _uow.ReferenciaRecepcionRepository.GetCabezalReferenciasAgenda(agenda.Id).First();
                    }

                    this.AgregarSobrante(linea, referencia, ref cantRecibida, ref nroSobrante, cambiosAsociaciones, cambiosDetalleReferencia);
                }
            }
        }

        public virtual void AjustarCantidadesRecibidas(TrafficOfficerTransaction transaction, List<DetalleAgendaReferenciaAsociada> detallesReferenciasAsociadosADetalleAgenda, ref decimal cantidadRecibidaDetalleAgenda, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia, AgendaDetalle detalleAgenda = null)
        {

            foreach (var asociacion in detallesReferenciasAsociadosADetalleAgenda)
            {

                var saldoAsociacion = asociacion.CantidadAgendada - asociacion.CantidadRecibida;

                if (saldoAsociacion == 0)
                    continue; //Si saldoAsociacion es 0, no tengo espacio para agregar recibidos

                if (cantidadRecibidaDetalleAgenda > saldoAsociacion)
                {
                    //Lockeo Detalle Referencia
                    this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", asociacion.DetalleReferencia.GetCompositeId(), transaction);

                    //Actualizo cantidad recibida asociación y detalle de referencia
                    asociacion.CantidadRecibida += saldoAsociacion;
                    this.UpdateRecordAsociacion(cambiosAsociaciones, asociacion);

                    asociacion.DetalleReferencia.CantidadRecibida += saldoAsociacion;
                    asociacion.DetalleReferencia.CantidadAgendada -= saldoAsociacion;
                    this.UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);

                    cantidadRecibidaDetalleAgenda -= saldoAsociacion;

                    // Si detalle de agenda es != null es porque consumo de un lote (AUTO) y es una nueva relacion por lo tanto la seteo en la agenda
                    if (detalleAgenda != null)
                        detalleAgenda.AsociacionesDetalleReferencia.Add(asociacion);
                }
                else
                {
                    //Lockeo Detalle Referencia
                    this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", asociacion.DetalleReferencia.GetCompositeId(), transaction);

                    //Actualizo cantidad recibida asociación y detalle de referencia
                    asociacion.CantidadRecibida += cantidadRecibidaDetalleAgenda;
                    this.UpdateRecordAsociacion(cambiosAsociaciones, asociacion);

                    asociacion.DetalleReferencia.CantidadRecibida += cantidadRecibidaDetalleAgenda;
                    asociacion.DetalleReferencia.CantidadAgendada -= cantidadRecibidaDetalleAgenda;
                    this.UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);

                    cantidadRecibidaDetalleAgenda = 0;

                    // Si detalle de agenda es != null es porque consumo de un lote (AUTO) y es una nueva relacion por lo tanto la seteo en la agenda
                    if (detalleAgenda != null)
                        detalleAgenda.AsociacionesDetalleReferencia.Add(asociacion);

                    break;
                }
            }

        }

        public virtual void AjustarCantidadesRecibidasAsociadasConSaldoenReferenciaDisponible(TrafficOfficerTransaction transaction, List<DetalleAgendaReferenciaAsociada> detallesReferenciasAsociadosADetalleAgenda, ref decimal cantidadRecibidaDetalleAgenda, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            if (detallesReferenciasAsociadosADetalleAgenda.Count > 0)
            {

                foreach (var asociacion in detallesReferenciasAsociadosADetalleAgenda.Where(s => s.DetalleReferencia.GetSaldo() > 0))
                {

                    var saldoReferencia = asociacion.DetalleReferencia.GetSaldo();

                    if (cantidadRecibidaDetalleAgenda > saldoReferencia)
                    {
                        //Lockeo Detalle Referencia
                        this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", asociacion.DetalleReferencia.GetCompositeId(), transaction);

                        //Actualizo cantidad recibida asociación y detalle de referencia
                        asociacion.CantidadRecibida += saldoReferencia;
                        this.UpdateRecordAsociacion(cambiosAsociaciones, asociacion);

                        asociacion.DetalleReferencia.CantidadRecibida += saldoReferencia;
                        this.UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);

                        cantidadRecibidaDetalleAgenda -= saldoReferencia;

                    }
                    else
                    {
                        //Lockeo Detalle Referencia
                        this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", asociacion.DetalleReferencia.GetCompositeId(), transaction);

                        //Actualizo cantidad recibida asociación y detalle de referencia
                        asociacion.CantidadRecibida += cantidadRecibidaDetalleAgenda;
                        this.UpdateRecordAsociacion(cambiosAsociaciones, asociacion);

                        asociacion.DetalleReferencia.CantidadRecibida += cantidadRecibidaDetalleAgenda;
                        this.UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);

                        cantidadRecibidaDetalleAgenda = 0;

                        break;
                    }
                }
            }
        }

        public virtual void AjustarCantidadesRecibidasNoAsociadas(TrafficOfficerTransaction transaction, List<ReferenciaRecepcionDetalle> detallesReferenciasAuto, AgendaDetalle detalleAgenda, ref decimal cantidadRecibidaDetalleAgenda, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {

            foreach (var detalleReferencia in detallesReferenciasAuto)
            {

                var saldo = detalleReferencia.GetSaldo();

                if (saldo == 0)
                    continue; //Si saldo es 0, no tengo espacio para agregar recibidos

                if (cantidadRecibidaDetalleAgenda > saldo)
                {
                    //Lockeo Detalle Referencia
                    this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", detalleReferencia.GetCompositeId(), transaction);

                    //Actualizo cantidad recibida asociación y detalle de referencia

                    var asociacion = new DetalleAgendaReferenciaAsociada()
                    {
                        CantidadAgendada = 0, // Cargo 0 ya que no fue agendado
                        CantidadRecibida = saldo,
                        DetalleReferencia = detalleReferencia,
                        DetalleAgenda = detalleAgenda,
                    };

                    this.UpdateRecordAsociacion(cambiosAsociaciones, asociacion);

                    asociacion.DetalleReferencia.CantidadRecibida += saldo;

                    this.UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);

                    cantidadRecibidaDetalleAgenda -= saldo;

                    detalleAgenda.AsociacionesDetalleReferencia.Add(asociacion);

                }
                else
                {
                    //Lockeo Detalle Referencia
                    this._concurrencyControl.AddLock("T_RECEPCION_REFERENCIA_DET", detalleReferencia.GetCompositeId(), transaction);

                    //Actualizo cantidad recibida asociación y detalle de referencia
                    var asociacion = new DetalleAgendaReferenciaAsociada()
                    {
                        CantidadAgendada = 0, // Cargo 0 ya que no se agendo y no se espera
                        CantidadRecibida = cantidadRecibidaDetalleAgenda,
                        DetalleReferencia = detalleReferencia,
                        DetalleAgenda = detalleAgenda,
                    };

                    this.UpdateRecordAsociacion(cambiosAsociaciones, asociacion);

                    asociacion.DetalleReferencia.CantidadRecibida += cantidadRecibidaDetalleAgenda;

                    this.UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);

                    cantidadRecibidaDetalleAgenda = 0;

                    detalleAgenda.AsociacionesDetalleReferencia.Add(asociacion);

                    break;
                }
            }
        }

        public virtual void AgregarSobrante(AgendaDetalle detalleAgenda, ReferenciaRecepcion referencia, ref decimal cantRecibida, ref int nroSobrante, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            //Agregar sobrante
            ReferenciaRecepcionDetalle nuevaDetReferencia = new ReferenciaRecepcionDetalle()
            {
                IdLineaSistemaExterno = "WIS-SOBRANTE_" + detalleAgenda.IdAgenda + "_" + nroSobrante,
                IdEmpresa = detalleAgenda.IdEmpresa,
                Faixa = detalleAgenda.Faixa,
                CodigoProducto = detalleAgenda.CodigoProducto,
                Identificador = detalleAgenda.Identificador,
                IdReferencia = referencia.Id,
                CantidadRecibida = cantRecibida,
                CantidadAgendada = 0,
                CantidadAnulada = 0,
                CantidadConfirmadaInterfaz = 0,
                CantidadReferencia = 0
            };

            var nuevaRelacion = new DetalleAgendaReferenciaAsociada()
            {
                DetalleAgenda = detalleAgenda,
                DetalleReferencia = nuevaDetReferencia,
                CantidadAgendada = 0,
                CantidadRecibida = cantRecibida,
                FechaInsercion = DateTime.Now,
                NumeroTransaccion = detalleAgenda.NumeroTransaccion,
            };

            //Agregar a la lista de relaciones para considerarlo en siguientes operaciones
            detalleAgenda.AsociacionesDetalleReferencia.Add(nuevaRelacion);

            // Agrego referencia nueva a cambiosPendientes
            cambiosDetalleReferencia.AddedRecords.Add(nuevaDetReferencia);
            // UpdateRecordDetalleReferencia(cambiosDetalleReferencia, nuevaDetReferencia);

            UpdateRecordAsociacion(cambiosAsociaciones, nuevaRelacion);

            nroSobrante++;
        }

        public virtual void LiberarCantidadesAgendadasNoRecibidas(List<DetalleAgendaReferenciaAsociada> asociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            foreach (var asociacion in asociaciones)
            {

                var saldoNoRecibido = asociacion.CantidadAgendada - asociacion.CantidadRecibida;

                if (saldoNoRecibido > 0)
                {

                    asociacion.DetalleReferencia.CantidadAgendada -= saldoNoRecibido;
                    UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);

                }
            }
        }

        public virtual void VaciarCantidadAgendado(List<AgendaDetalle> detallesAgenda, EntityChanges<DetalleAgendaReferenciaAsociada> cambiosAsociaciones, EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia)
        {
            foreach (var detalle in detallesAgenda)
            {
                foreach (var asociacion in detalle.AsociacionesDetalleReferencia)
                {
                    if (asociacion.CantidadAgendada > 0)
                    {
                        // Controlo que sea mayor a cero, ya qeu los sobrantes no tieen que reducir y evito un problema de update de la misma entidad agregada
                        asociacion.DetalleReferencia.CantidadAgendada -= asociacion.CantidadAgendada;
                        UpdateRecordDetalleReferencia(cambiosDetalleReferencia, asociacion.DetalleReferencia);

                        asociacion.CantidadAgendada = 0;
                        UpdateRecordAsociacion(cambiosAsociaciones, asociacion);
                    }
                }

            }
        }

        public virtual void UpdateRecordDetalleReferencia(EntityChanges<ReferenciaRecepcionDetalle> cambiosDetalleReferencia, ReferenciaRecepcionDetalle detalleReferencia)
        {
            var record = cambiosDetalleReferencia.UpdatedRecords.FirstOrDefault(d => d.Id == detalleReferencia.Id);

            if (record != null)
                cambiosDetalleReferencia.UpdatedRecords.Remove(record);

            cambiosDetalleReferencia.UpdatedRecords.Add(detalleReferencia);

        }

        public virtual void UpdateRecordAsociacion(EntityChanges<DetalleAgendaReferenciaAsociada> cambioAsociaciones, DetalleAgendaReferenciaAsociada asociacion)
        {
            // Agregar nueva o modificacion de existente
            // Verifico que no exista en base si existe la elimino y creo la nueva


            if (!cambioAsociaciones.DeletedRecords.Any(s => s.DetalleAgenda.IdAgenda == asociacion.DetalleAgenda.IdAgenda
                                                   && s.DetalleAgenda.IdEmpresa == asociacion.DetalleAgenda.IdEmpresa
                                                   && s.DetalleAgenda.CodigoProducto == asociacion.DetalleAgenda.CodigoProducto
                                                   && s.DetalleAgenda.Faixa == asociacion.DetalleAgenda.Faixa
                                                   && s.DetalleAgenda.Identificador == asociacion.DetalleAgenda.Identificador

                                                   && s.DetalleReferencia.Id == asociacion.DetalleReferencia.Id))
            {
                // En caso que no este ya marcado a borrar lo busco en base y agrego para eliminar

                var asociacionExistente = _uow.ReferenciaRecepcionRepository.GetDetalleAgendaReferenciaAsociada(asociacion.DetalleAgenda, asociacion.DetalleReferencia);

                if (asociacionExistente != null)
                    cambioAsociaciones.DeletedRecords.Add(asociacionExistente);
            }


            //Added records

            var addedRecord = cambioAsociaciones.AddedRecords.FirstOrDefault(s => s.DetalleAgenda.IdAgenda == asociacion.DetalleAgenda.IdAgenda
                                                  && s.DetalleAgenda.IdEmpresa == asociacion.DetalleAgenda.IdEmpresa
                                                  && s.DetalleAgenda.CodigoProducto == asociacion.DetalleAgenda.CodigoProducto
                                                  && s.DetalleAgenda.Faixa == asociacion.DetalleAgenda.Faixa
                                                  && s.DetalleAgenda.Identificador == asociacion.DetalleAgenda.Identificador

                                                  && s.DetalleReferencia.Id == asociacion.DetalleReferencia.Id);

            if (addedRecord != null)
            {
                cambioAsociaciones.AddedRecords.Remove(addedRecord);
            }

            cambioAsociaciones.AddedRecords.Add(asociacion);

        }

        #endregion

        public virtual void UpdateRecordStock(EntityChanges<Stock> cambiosStock, Stock stock)
        {
            var record = cambiosStock.UpdatedRecords.FirstOrDefault(d => d.Ubicacion == stock.Ubicacion
                                                                        && d.Producto == stock.Producto
                                                                        && d.Faixa == stock.Faixa
                                                                        && d.Identificador == stock.Identificador
                                                                        && d.Empresa == stock.Empresa);

            if (record != null)
                cambiosStock.UpdatedRecords.Remove(record);

            cambiosStock.UpdatedRecords.Add(stock);
        }

        public virtual void UpdateRecordUbicacionesAComprobarStock(List<string> cambiosUbicacionesComprobarStock, string idUbicacion)
        {
            if (!cambiosUbicacionesComprobarStock.Any(s => s == idUbicacion))
            {
                cambiosUbicacionesComprobarStock.Add(idUbicacion);
            }

        }

        public virtual void UpdateRecordEtiquetaLote(EntityChanges<EtiquetaLote> cambiosEtiquetaLote, EtiquetaLote etiquetaLote)
        {
            var record = cambiosEtiquetaLote.UpdatedRecords.FirstOrDefault(d => d.Numero == etiquetaLote.Numero);

            if (record != null)
                cambiosEtiquetaLote.UpdatedRecords.Remove(record);

            cambiosEtiquetaLote.UpdatedRecords.Add(etiquetaLote);
        }

        public virtual void UpdateRecordDetalleAgenda(EntityChanges<AgendaDetalle> cambiosDetalleAgenda, AgendaDetalle detalleAgenda)
        {
            var record = cambiosDetalleAgenda.UpdatedRecords.FirstOrDefault(d => d.CodigoProducto == detalleAgenda.CodigoProducto
                                                                        && d.Faixa == detalleAgenda.Faixa
                                                                        && d.Identificador == detalleAgenda.Identificador
                                                                        && d.IdEmpresa == detalleAgenda.IdEmpresa);

            if (record != null)
                cambiosDetalleAgenda.UpdatedRecords.Remove(record);

            cambiosDetalleAgenda.UpdatedRecords.Add(detalleAgenda);

        }

        #endregion
    }
}
