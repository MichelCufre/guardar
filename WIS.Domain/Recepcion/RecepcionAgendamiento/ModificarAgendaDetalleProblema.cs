using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Recepcion.Enums;

namespace WIS.Domain.Recepcion
{
    public class ModificarAgendaDetalleProblema
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly IUnitOfWork _uow;
        protected string _aplicacion;
        protected int _usuario;
        protected AgendaDetalle _detalleAgenda;
        protected AgendaDetalle _detalleAgendaAuto;

        public ModificarAgendaDetalleProblema(IUnitOfWork uow, int usuario, string aplicacion, AgendaDetalle detalleAgenda)
        {
            this._uow = uow;
            this._usuario = usuario;
            this._aplicacion = aplicacion;
            this._detalleAgenda = detalleAgenda;

        }
        public ModificarAgendaDetalleProblema(IUnitOfWork uow, int usuario, string aplicacion, AgendaDetalle detalleAgenda, AgendaDetalle detalleAgendaAuto) : this(uow, usuario, aplicacion, detalleAgenda)
        {

            _detalleAgendaAuto = detalleAgendaAuto;
        }

        /// <summary>
        /// Comprueba los problemas en detalles de Agendas y actualiza la situacion del detalle
        /// Nota: No atacha el detalle ni lo setea como modificado
        /// </summary>
        public virtual void ActualizarProblemasEnDetalle()
        {
            try
            {
                logger.Info("ActualizarProblemasEnDetalle: Inicio ActualizarProblemasEnDetalle");

                this.ProblemaRecibidoExcedeAgendado();
                this.ProblemaProductosNoEsperados();

                _uow.SaveChanges();

                new ModificarAgendaDetalle(_uow, _usuario, _aplicacion).ActualizarSituacionAgendaDetalle(_detalleAgenda);

                logger.Info("ActualizarProblemasEnDetalle: Post ActualizarProblemasEnDetalle");

            }
            catch (Exception ex)
            {
                logger.Info("ActualizarProblemasEnDetalle: excepción: " + ex.ToString());
                throw ex;
            }
        }

        public virtual void ActualizarProblemasEnDetalleLocal()
        {
            try
            {
                new ModificarAgendaDetalle(_uow, _usuario, _aplicacion).ActualizarSituacionAgendaDetalleLocal(_detalleAgenda);
            }
            catch (Exception ex)
            {
                logger.Info("ActualizarProblemasEnDetalleLocal: excepción: " + ex.ToString());
                throw;
            }
        }

        public virtual void ProblemaRecibidoExcedeAgendado()
        {
            try
            {
                decimal? diferencia = 0;

                string TipoRecepcionInterna = _uow.AgendaRepository.GetAgendaTipoRecepcionInterno(_detalleAgenda.IdAgenda);

                var detalleAuto = _detalleAgendaAuto;

                if (detalleAuto == null)
                    detalleAuto = _uow.AgendaRepository.GetAgendaDetalle(_detalleAgenda.IdAgenda, _detalleAgenda.IdEmpresa, _detalleAgenda.CodigoProducto, _detalleAgenda.Faixa, ManejoIdentificadorDb.IdentificadorAuto);

                if (detalleAuto != null)
                {
                    detalleAuto.NumeroTransaccion = _uow.GetTransactionNumber();

                    if (detalleAuto.CantidadAgendada > 0)
                    {
                        diferencia = detalleAuto.CantidadAgendada;

                        this.InsertAgendaDetalleProblema(detalleAuto, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado, diferencia);

                    }
                    else if (detalleAuto.CantidadAgendada == 0)
                    {
                        this.RemoveAgendaDetalleProblema(detalleAuto, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado);
                    }

                    new ModificarAgendaDetalle(_uow, _usuario, _aplicacion).ActualizarSituacionAgendaDetalle(detalleAuto);
                    _uow.AgendaRepository.UpdateAgendaDetalle(detalleAuto);
                }

                diferencia = _detalleAgenda.CantidadAgendada - _detalleAgenda.CantidadRecibida;

                var recepcionTipo = _uow.RecepcionTipoRepository.GetRecepcionTipo(TipoRecepcionInterna);

                if (diferencia < 0)
                {
                    this.InsertAgendaDetalleProblema(_detalleAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeAgendado, diferencia);

                    if (recepcionTipo.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.Bolsa
                        || recepcionTipo.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MonoSeleccion
                        || recepcionTipo.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MultiSeleccion)
                    {
                        //if (!TipoRecepcionInterna.Equals(TipoRecepcionInternoDb.DigitacionLibre) && !TipoRecepcionInterna.Equals(TipoRecepcionInternoDb.DigitacionLibreDeposito))
                        ComprobarCantidadRecibidaExcedeSaldoReferenciaRecepcion();
                    }
                }
                else
                {
                    this.RemoveAgendaDetalleProblema(_detalleAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeAgendado);

                    if (recepcionTipo.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.Bolsa
                       || recepcionTipo.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MonoSeleccion
                       || recepcionTipo.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.MultiSeleccion)
                    {
                        //   if (!TipoRecepcionInterna.Equals(TipoRecepcionInternoDb.DigitacionLibre) && !TipoRecepcionInterna.Equals(TipoRecepcionInternoDb.DigitacionLibreDeposito))
                        ComprobarCantidadRecibidaExcedeSaldoReferenciaRecepcion();
                    }
                }

                if (diferencia > 0)
                {
                    this.InsertAgendaDetalleProblema(_detalleAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado, diferencia);
                }
                else
                {
                    this.RemoveAgendaDetalleProblema(_detalleAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado);
                }
            }
            catch (Exception ex)
            {
                logger.Info("ProblemaRecibidoExcedeAgendado: Exception: " + ex.ToString());
                throw ex;
            }
        }

        public virtual void ProblemaProductosNoEsperados()
        {
            if (_detalleAgenda.CantidadAgendada == 0 && _detalleAgenda.CantidadAgendadaOriginal == 0 && _detalleAgenda.CantidadRecibida > 0)
            {
                this.InsertAgendaDetalleProblema(_detalleAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoProductoNoEsperado, 0);
            }
            else
            {
                this.RemoveAgendaDetalleProblema(_detalleAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoProductoNoEsperado);
            }
        }

        public virtual void InsertAgendaDetalleProblema(AgendaDetalle detalle, TipoProblemaAgendaDetalle tipoProblema, ProblemaAgendaDetalle problema, decimal? diferencia)
        {
            try
            {
                if (detalle.CantidadAgendada != 0 || detalle.CantidadRecibida != 0 || detalle.CantidadAgendadaOriginal != 0)
                {
                    var problemaDetalle = _uow.AgendaRepository.GetAgendaDetalleProblemaSinAceptar(tipoProblema, problema, detalle.IdAgenda, detalle.CodigoProducto, detalle.Identificador, detalle.Faixa);

                    if (problemaDetalle == null)
                    {
                        problemaDetalle = new AgendaDetalleProblema()
                        {
                            NumeroAgenda = detalle.IdAgenda,
                            CodigoProducto = detalle.CodigoProducto,
                            Identificador = detalle.Identificador,
                            Embalaje = detalle.Faixa,
                            TipoProblema = tipoProblema,
                            Problema = problema,
                            Diferencia = diferencia,
                            Funcionario = _usuario,
                            Aceptado = false
                        };

                        _uow.AgendaRepository.AddAgendaDetalleProblema(problemaDetalle);

                    }
                    else
                    {
                        problemaDetalle.Diferencia = diferencia;
                        problemaDetalle.Funcionario = _usuario;

                        _uow.AgendaRepository.UpdateAgendaDetalleProblema(problemaDetalle);
                    }

                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual void RemoveAgendaDetalleProblema(AgendaDetalle detalle, TipoProblemaAgendaDetalle tipoProblema, ProblemaAgendaDetalle problema)
        {
            var problemaSinResolver = _uow.AgendaRepository.GetAgendaDetalleProblemaSinAceptar(tipoProblema, problema, detalle.IdAgenda, detalle.CodigoProducto, detalle.Identificador, detalle.Faixa);

            if (problemaSinResolver != null)
            {
                _uow.AgendaRepository.RemoveAgendaDetalleProblema(problemaSinResolver);
            }
        }

        public virtual void ComprobarCantidadRecibidaExcedeSaldoReferenciaRecepcion()
        {
            try
            {
                decimal aux = 0;
                decimal diferencia = _detalleAgenda.CantidadRecibida - _detalleAgenda.CantidadAgendada;
                decimal saldo = 0;

                var agenda = _uow.AgendaRepository.GetAgendaSinDetalles(_detalleAgenda.IdAgenda);
                var tipoReferenciaAgenda = _uow.ReferenciaRecepcionRepository.GetTipoReferencia(agenda.TipoRecepcionInterno);

                List<ReferenciaRecepcionDetalle> detallesDeReferencias = new List<ReferenciaRecepcionDetalle>();

                if (agenda.TipoRecepcion.TipoSeleccionReferencia == TipoSeleccionReferenciaDb.Bolsa)
                {
                    detallesDeReferencias = _uow.ReferenciaRecepcionRepository.GetReferenciasRecepcionModalidadBolsa(tipoReferenciaAgenda, _detalleAgenda.IdEmpresa, agenda.CodigoInternoCliente, _detalleAgenda.CodigoProducto, _detalleAgenda.Faixa, _detalleAgenda.Identificador);
                }
                else if (agenda.TipoRecepcionInterno == TipoSeleccionReferenciaDb.MonoSeleccion &&
                        agenda.TipoRecepcionInterno == TipoSeleccionReferenciaDb.MultiSeleccion)
                {
                    detallesDeReferencias = _uow.ReferenciaRecepcionRepository.GetReferenciasRecepcionModalidadSeleccion(tipoReferenciaAgenda, _detalleAgenda.IdAgenda, _detalleAgenda.IdEmpresa, agenda.CodigoInternoCliente, _detalleAgenda.CodigoProducto, _detalleAgenda.Faixa, _detalleAgenda.Identificador);
                }

                detallesDeReferencias.ForEach(detalle =>
                {
                    saldo += detalle.GetSaldo();
                });

                aux = saldo - diferencia;

                var nuTransaccion = _uow.GetTransactionNumber();

                if (diferencia > 0)
                {
                    if (aux < diferencia)
                    {
                        foreach (var detalle in detallesDeReferencias.OrderBy(x => x.ReferenciaRecepcion.FechaVencimientoOrden).ToList())
                        {
                            decimal cantidadReferencia = (aux - detalle.GetSaldo());

                            detalle.NumeroTransaccion = nuTransaccion;

                            if (cantidadReferencia > 0)
                            {
                                this.AuxAddReferenciaDetalleProblemaSiNoExiste(detalle, _detalleAgenda.IdAgenda, cantidadReferencia);

                                aux = cantidadReferencia;
                            }
                            else if (aux > 0)
                            {
                                this.AuxAddReferenciaDetalleProblemaSiNoExiste(detalle, _detalleAgenda.IdAgenda, aux);

                                break; // Salgo del foreach
                            }
                        }

                        this.InsertAgendaDetalleProblema(_detalleAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeSaldoReferenciaRecepcion, aux);

                    }
                    else if (aux > diferencia)
                    {
                        foreach (var detalle in detallesDeReferencias.OrderBy(x => x.ReferenciaRecepcion.FechaVencimientoOrden).ToList())
                        {
                            decimal cantidadReferencia = (diferencia - detalle.GetSaldo());

                            detalle.NumeroTransaccion = nuTransaccion;

                            if (cantidadReferencia > 0)
                            {
                                this.AuxAddReferenciaDetalleProblemaSiNoExiste(detalle, _detalleAgenda.IdAgenda, cantidadReferencia);

                                diferencia = cantidadReferencia;
                            }
                            else if (diferencia > 0)
                            {
                                this.AuxAddReferenciaDetalleProblemaSiNoExiste(detalle, _detalleAgenda.IdAgenda, diferencia);

                                break;
                            }
                        }

                        this.RemoveAgendaDetalleProblema(_detalleAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeSaldoReferenciaRecepcion);
                    }
                }
                else
                {
                    foreach (var detalle in detallesDeReferencias.Where(x => x.IdLineaSistemaExterno == $"{ProblemaAgendaDb.DescripcionExcedeSaldoReferenciaRecepcion}{_detalleAgenda.IdAgenda}").ToList())
                    {
                        detalle.NumeroTransaccion = nuTransaccion;
                        detalle.NumeroTransaccionDelete = nuTransaccion;
                        detalle.FechaModificacion = DateTime.Now;

                        _uow.ReferenciaRecepcionRepository.UpdateReferenciaDetalle(detalle);
                        _uow.SaveChanges();

                        var detalleRemove = _uow.ReferenciaRecepcionRepository.GetReferenciaDetalle(detalle.Id);

                        _uow.ReferenciaRecepcionRepository.DeleteReferenciaDetalle(detalle);
                    }

                    this.RemoveAgendaDetalleProblema(_detalleAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeSaldoReferenciaRecepcion);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual void AuxAddReferenciaDetalleProblemaSiNoExiste(ReferenciaRecepcionDetalle detalle, int numeroAgenda, decimal cantidadReferencia)
        {
            if (!_uow.ReferenciaRecepcionRepository.AnyReferenciaDetalle(detalle.IdReferencia, detalle.IdEmpresa, detalle.CodigoProducto, detalle.Identificador, detalle.Faixa))
            {
                var detalleProblema = new ReferenciaRecepcionDetalle()
                {
                    IdReferencia = detalle.IdReferencia,
                    CodigoProducto = detalle.CodigoProducto,
                    IdEmpresa = detalle.IdEmpresa,
                    Faixa = detalle.Faixa,
                    Identificador = detalle.Identificador,
                    IdLineaSistemaExterno = $"{ProblemaAgendaDb.DescripcionExcedeSaldoReferenciaRecepcion}{numeroAgenda}",
                    CantidadReferencia = cantidadReferencia,
                    CantidadAnulada = 0,
                    CantidadAgendada = 0,
                    CantidadRecibida = 0,
                    CantidadConfirmadaInterfaz = 0,
                    ImporteUnitario = 0,
                    NumeroTransaccion = detalle.NumeroTransaccion,
                };

                _uow.ReferenciaRecepcionRepository.AddReferenciaDetalle(detalleProblema);
            }
        }
    }
}
