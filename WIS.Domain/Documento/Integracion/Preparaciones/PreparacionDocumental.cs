using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Documento.Execution;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Documento;
using WIS.Domain.Documento.Constants;
using WIS.Domain.Documento.Reserva;
using WIS.Domain.Documento.Serializables.Reserva;
using WIS.Exceptions;

namespace WIS.Domain.Documento.Integracion.Preparaciones
{
    public class PreparacionDocumental
    {
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IUnitOfWorkFactory _uowFactory;

        public PreparacionDocumental(IUnitOfWorkFactory uowFactory)
        {
            this._uowFactory = uowFactory;
        }

        public virtual ReservaDocumentalResponse ReservaDocumentalPicking(ReservaDocumentalRequest request)
        {
            var result = new ReservaDocumentalResponse();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                try
                {
                    uow.CreateTransactionNumber("PreparacionDocumental ReservaDocumentalPicking");
                    uow.BeginTransaction();

                    var nuTransaccion = uow.GetTransactionNumber();
                    var lineasModificadas = new List<DocumentoLineaDesafectada>();

                    foreach (var reserva in request.Reservas)
                    {
                        if (reserva.CantidadAfectada > 0)
                        {
                            this.AfectarReserva(uow, reserva.Producto, reserva.Identificador, reserva.Empresa, reserva.CantidadAfectada, reserva.Preparacion);
                        }
                        else
                        {
                            this.DesafectarReserva(uow, reserva.Producto, reserva.Identificador, reserva.Empresa, reserva.CantidadAfectada, reserva.Preparacion, lineasModificadas);
                        }
                    }

                    foreach (var linea in lineasModificadas)
                    {
                        uow.DocumentoRepository.UpdateDetailWithoutDocument(linea.NroDocumento, linea.TipoDocumento, linea.LineaModificada, nuTransaccion);
                    }

                    uow.SaveChanges();
                    uow.Commit();
                }
                catch (Exception ex)
                {
                    uow.Rollback();
                    _logger.Error(ex, ex.Message);
                    result.Success = false;
                    result.ErrorMsg = ex.Message;
                }
            }

            return result;
        }

        #region - Produccion Desreservar entradas -

        public virtual void DesreservarEntradaAnularPreparacion(IUnitOfWork uow, int empresa, string producto, decimal? faixa, string identificador, decimal cantidadAnular)
        {
            var lista = new List<DesreservarStockDocumentalRequestLinea>();
            lista.Add(new DesreservarStockDocumentalRequestLinea()
            {
                Empresa = empresa,
                Producto = producto,
                NumeroIdentificador = identificador,
                CantidadAnular = cantidadAnular,
                Faixa = faixa
            });

            this.DesreservarEntradasAnularPreparaciones(uow, lista);
        }

        public virtual List<DocumentoAnulacionPreparacionReserva> DesreservarEntradasAnularPreparaciones(IUnitOfWork uow, List<DesreservarStockDocumentalRequestLinea> request, bool anular = true)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var anulacionesReserva = new List<DocumentoAnulacionPreparacionReserva>();

            if (request.Count > 0)
            {
                var saldosEntradas = new DocumentoProduccionEntradaSaldos();
                string idAnulacion = uow.DocumentoEntradaProduccionReservaRepository.GetIdAnulacion();

                foreach (var linea in request)
                {
                    // Busco para empresa, producto, identificador los stocks volcados en ubicación de entrada de linea 
                    var entradasReservas = uow.DocumentoEntradaProduccionReservaRepository.GetEntradasReservas(linea.Empresa, linea.Producto, linea.NumeroIdentificador);

                    if (entradasReservas.Count == 0 && linea.Semiacabado == "S")
                    {
                        var anulacion = new DocumentoAnulacionPreparacionReserva()
                        {
                            Empresa = linea.Empresa,
                            Producto = linea.Producto,
                            Identificador = linea.NumeroIdentificador,
                            EspecificaIdentificador = linea.EspecificaIdentificador,
                            Estado = EstadoAnularReservaPreparacion.PENDIENTE,
                            Faixa = linea.Faixa,
                            FechaAlta = DateTime.Now,
                            CantidadAnular = linea.CantidadAnular,
                            Semiacabado = linea.Semiacabado
                        };

                        anulacionesReserva.Add(anulacion);

                        continue;
                    }

                    decimal saldo = Math.Round(linea.CantidadAnular, 3);
                    int i = 0;

                    while (saldo > 0 && entradasReservas.Count > 0 && i <= entradasReservas.Count)
                    {
                        decimal cantidadAfectada = 0;
                        var entrada = entradasReservas[i];

                        if (saldo <= entrada.CantidadReservada)
                        {
                            cantidadAfectada = saldo;
                            entrada.CantidadReservada = (entrada.CantidadReservada - saldo);
                            saldo = 0;
                        }
                        else if (saldo > entrada.CantidadReservada)
                        {
                            cantidadAfectada = entrada.CantidadReservada ?? 0;
                            decimal diferencia = entrada.CantidadReservada ?? 0;
                            saldo -= diferencia;
                            entrada.CantidadReservada = 0;
                        }

                        i++;

                        string key = entrada.GetKey(idAnulacion);
                        DocumentoAnulacionPreparacionReserva anulacion = anulacionesReserva.FirstOrDefault(s => s.IdentificadorAnulacion == key);

                        if (anulacion == null)
                        {
                            anulacion = new DocumentoAnulacionPreparacionReserva()
                            {
                                IdentificadorAnulacion = key,
                                NumeroPreparacion = entrada.NumeroPreparacion,
                                Empresa = entrada.Empresa,
                                Producto = entrada.Producto,
                                Identificador = entrada.Identificador,
                                EspecificaIdentificador = linea.EspecificaIdentificador,
                                Estado = EstadoAnularReservaPreparacion.PENDIENTE,
                                Faixa = entrada.Faixa,
                                FechaAlta = DateTime.Now,
                                CantidadAnular = cantidadAfectada,
                                Semiacabado = linea.Semiacabado,
                                Consumible = linea.Consumible
                            };
                            anulacionesReserva.Add(anulacion);
                        }
                        else
                        {
                            anulacion.CantidadAnular = (anulacion.CantidadAnular ?? 0) + cantidadAfectada;
                        }

                        if (entrada.CantidadReservada > 0)
                        {
                            saldosEntradas.AddModificados(entrada);
                        }
                        else
                        {
                            saldosEntradas.AddEliminados(entrada);
                        }
                    }

                    if (saldo > 0)
                    {
                        var ex = new ValidationFailedException(string.Format("Error diferencia en saldos entre lo volcado y consumido, Linea: {0} - {1} - {2} " + linea.Empresa, linea.Producto, linea.NumeroIdentificador));
                        _logger.Error(ex, ex.Message);
                        throw ex;
                    }
                }

                uow.DocumentoEntradaProduccionReservaRepository.ProcesarEntradasModificadas(saldosEntradas, nuTransaccion);

                if (anular)
                {
                    uow.DocumentoAnulacionPreparacionReservaRepository.AddAnulaciones(anulacionesReserva);
                }
            }

            return anulacionesReserva;
        }

        public virtual DesreservarStockDocumentalResponse DesreservarStockEntradaAnularPreparacionProduccion(DesreservarStockDocumentalRequest request)
        {
            DesreservarStockDocumentalResponse response = new DesreservarStockDocumentalResponse();

            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.CreateTransactionNumber("PreparacionDocumental DesreservarStockEntradaAnularPreparacionProduccion");
                    uow.BeginTransaction();

                    DesreservarEntradasAnularPreparaciones(uow, request.LineasAnular);

                    uow.SaveChanges();
                    uow.Commit();

                    response.Succes = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                response.MensajeError = ex.Message;
                response.Succes = false;
            }

            return response;
        }

        #endregion

        public virtual AnularReservaPreparacionResponse AnularPreparacionReserva(AnularReservaPreparacionRequest request)
        {
            AnularReservaPreparacionResponse response = new AnularReservaPreparacionResponse();

            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    List<DocumentoAnulacionPreparacionReserva> anulacionesReserva = new List<DocumentoAnulacionPreparacionReserva>();

                    //Crear objetos de anulacion reserva
                    foreach (var linea in request.LineasAnular)
                    {
                        anulacionesReserva.Add(new DocumentoAnulacionPreparacionReserva()
                        {
                            CantidadAnular = linea.CantidadAnular,
                            Empresa = linea.Empresa,
                            EspecificaIdentificador = linea.EspecificaIdentificador,
                            Estado = EstadoAnularReservaPreparacion.PENDIENTE,
                            Faixa = linea.Faixa,
                            Identificador = linea.NumeroIdentificador,
                            IdentificadorAnulacion = linea.IndetificadorAnulacion,
                            NumeroPreparacion = linea.Preparacion,
                            Producto = linea.Producto,
                            FechaAlta = DateTime.Now
                        });
                    }

                    uow.DocumentoAnulacionPreparacionReservaRepository.AddAnulaciones(anulacionesReserva);
                    uow.SaveChanges();

                    response.Succes = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                response.MensajeError = ex.Message;
                response.Succes = false;
            }

            return response;
        }

        public virtual ConsultaAnularReservaPreparacionResponse ConsultarEstadoAnularPreparacionReserva(ConsultaAnularReservaPreparacionRequest request)
        {
            ConsultaAnularReservaPreparacionResponse response = new ConsultaAnularReservaPreparacionResponse();

            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    var anulaciones = uow.DocumentoAnulacionPreparacionReservaRepository.GetAnulaciones(request.IdentificadoresAnulacion);

                    foreach (var anulacion in anulaciones)
                    {
                        response.estadoLineas.Add(new EstadoAnulaciones()
                        {
                            Estado = anulacion.Estado,
                            IndetificadorAnulacion = anulacion.IdentificadorAnulacion
                        });
                    }

                    response.Succes = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                response.MensajeError = ex.Message;
                response.Succes = false;
            }

            return response;
        }

        public virtual void EjecutarAnulacionPreparacionReserva(int usuario, string aplicacion, int cantidadAnular)
        {
            _logger.Debug($"EjecutarAnulacionPreparacionReserva");

            try
            {
                using (var uow = this._uowFactory.GetUnitOfWork())
                {
                    uow.BeginTransaction();

                    var anulacionesPendientes = uow.DocumentoAnulacionPreparacionReservaRepository.GetAnulacionesPendientes(cantidadAnular);
                    var anulacionesAgrupadas = anulacionesPendientes.GroupBy(a => new { a.Producto, a.Identificador, a.Empresa, a.NumeroPreparacion });
                    var lineasModificadas = new List<DocumentoLineaDesafectada>();
                    var anulacionesEjecutadas = new List<DocumentoAnulacionPreparacionReserva>();

                    if (anulacionesAgrupadas.Any())
                        uow.CreateTransactionNumber("PreparacionDocumental EjecutarAnulacionPreparacionReserva");

                    foreach (var grupo in anulacionesAgrupadas)
                    {
                        _logger.Debug($"Procesar Grupo Anulaciones: Prod. {grupo.Key.Producto}, Nro.Id. {grupo.Key.Identificador}, Empr. {grupo.Key.Empresa}, Prep. {grupo.Key.NumeroPreparacion}");

                        //Obtener todas las anulaciones a ejecutar
                        var anulacionesEjecutar = anulacionesPendientes
                            .Where(a => a.Producto == grupo.Key.Producto
                                && a.Identificador == grupo.Key.Identificador
                                && a.Empresa == grupo.Key.Empresa
                                && a.NumeroPreparacion == grupo.Key.NumeroPreparacion)
                            .ToList();

                        try
                        {
                            var cantidadAnularSuma = anulacionesEjecutar.Sum(s => s.CantidadAnular);
                            this.DesafectarReserva(uow, grupo.Key.Producto, grupo.Key.Identificador, (int)grupo.Key.Empresa, -cantidadAnularSuma, (int)grupo.Key.NumeroPreparacion, lineasModificadas);
                            this.MarcarAnulacionesEjecutadas(anulacionesEjecutadas, anulacionesEjecutar);
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, ex.Message);
                            this.MarcarAnulacionesFallidas(anulacionesEjecutadas, anulacionesEjecutar);
                        }
                    }

                    foreach (var linea in lineasModificadas)
                    {
                        uow.DocumentoRepository.UpdateDetailWithoutDocument(linea.NroDocumento, linea.TipoDocumento, linea.LineaModificada, uow.GetTransactionNumber());
                    }

                    foreach (var anulacionEjecutada in anulacionesEjecutadas)
                    {
                        uow.DocumentoAnulacionPreparacionReservaRepository.UpdateAnulacion(anulacionEjecutada);
                    }

                    uow.SaveChanges();
                    uow.Commit();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw ex;
            }
        }

        public virtual bool SaldoDocumentalSuficiete(ReservaDocumentalRequest request)
        {
            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                foreach (var r in request.Reservas)
                {
                    var query = new DocumentosConSaldoByProducto(r.Producto, r.Identificador, 1, r.Empresa);

                    uow.HandleQuery(query);

                    var saldo = query.GetSaldos();

                    if (!(saldo.Count() > 0 && saldo.Sum(d => d.CantidadDisponible) >= r.CantidadAfectada))
                        return false;
                }
            }

            return true;
        }

        public virtual void AfectarReserva(IUnitOfWork uow, string producto, string identificador, int empresa, decimal? cantidadAfectada, int preparacion)
        {
            var query = new DocumentosConSaldoByProducto(producto, identificador, 1, empresa);

            uow.HandleQuery(query);

            List<SaldoDetalleDocumento> saldos = query.GetSaldos();

            decimal auxQtPreparar = cantidadAfectada ?? 0;

            if (saldos.Count() > 0 && saldos.Sum(d => d.CantidadDisponible) >= cantidadAfectada)
            {
                foreach (var saldo in saldos)
                {

                    if (saldo.CantidadDisponible >= auxQtPreparar)
                    {
                        NuevaReservaPicking(uow, saldo, auxQtPreparar, preparacion, true);
                        break;
                    }
                    else
                    {
                        NuevaReservaPicking(uow, saldo, saldo.CantidadDisponible, preparacion, true);
                        auxQtPreparar -= saldo.CantidadDisponible;
                    }
                }
            }
            else
            {
                throw new Exception(string.Format("No existe suficiente stock documental disponible para realizar la reserva del producto {0}, lote {1}", producto, identificador));
            }
        }

        public virtual void DesafectarReserva(IUnitOfWork uow, string producto, string identificador, int empresa, decimal? cantidadAfectada, int preparacion, List<DocumentoLineaDesafectada> lineasModificadas)
        {
            _logger.Debug($"Desafectar Reserva: Prep. {preparacion}, Empr. {empresa}, Prod. {producto}, Nro.Id. {identificador}, Cant. {cantidadAfectada ?? 0}");

            var nuTransaccion = uow.GetTransactionNumber();
            var auxCantidadDesafectar = cantidadAfectada ?? 0;
            var reservas = uow.DocumentoRepository.GetPreparacionReservas(preparacion, empresa, producto, 1, identificador);

            if (reservas.Count() > 0 && reservas.Sum(p => p.CantidadDisponible()) >= -cantidadAfectada)
            {
                var stop = false;

                foreach (var reserva in reservas)
                {
                    decimal auxCantidadDesafectarLinea;

                    _logger.Debug($"Procesar Preparación Reserva: Nro.Doc. {reserva.NroDocumento}, Tp.Doc. {reserva.TipoDocumento}, Nro.Prep. {reserva.Preparacion}, Empr. {reserva.Empresa}, Prod. {reserva.Producto}, Faixa {reserva.Faixa}, Nro.Id. {reserva.NroIdentificadorPicking}, Cant. {reserva.CantidadProducto}");

                    DocumentoLinea linea = uow.DocumentoRepository.GetLineaIngreso(reserva.NroDocumento, reserva.TipoDocumento, reserva.Producto, reserva.NroIdentificadorPicking);

                    if (linea != null)
                    {
                        //Desafectar saldo en linea de Reserva
                        if (reserva.CantidadDisponible() >= -auxCantidadDesafectar)
                        {
                            reserva.AnularReserva(auxCantidadDesafectar);
                            auxCantidadDesafectarLinea = auxCantidadDesafectar;
                            stop = true;
                        }
                        else
                        {
                            auxCantidadDesafectar += reserva.CantidadDisponible();
                            auxCantidadDesafectarLinea = -reserva.CantidadDisponible();
                            reserva.AnularReserva(-reserva.CantidadDisponible());
                        }

                        reserva.NumeroTransaccion = nuTransaccion;

                        if (reserva.CantidadProducto > 0)
                        {
                            reserva.NumeroTransaccionDelete = null;
                            uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                        }
                        else
                        {
                            reserva.NumeroTransaccionDelete = nuTransaccion;
                            uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                            uow.SaveChanges();
                            uow.DocumentoRepository.RemoveDocumentoPreparacionReserva(reserva);
                        }

                        //Desafectar saldo en linea de Ingreso
                        var lineaExistente = lineasModificadas
                            .FirstOrDefault(lm => lm.NroDocumento == reserva.NroDocumento
                                && lm.TipoDocumento == reserva.TipoDocumento
                                && lm.LineaModificada.Producto == linea.Producto
                                && lm.LineaModificada.Identificador == linea.Identificador
                                && lm.LineaModificada.Empresa == linea.Empresa);

                        if (lineaExistente != null)
                        {
                            linea.CantidadReservada = lineaExistente.LineaModificada.CantidadReservada;
                            lineasModificadas.Remove(lineaExistente);
                        }

                        _logger.Debug($"Afectar Línea: Nro.Doc. {reserva.NroDocumento}, Tp.Doc. {reserva.TipoDocumento}, Prod. {reserva.Producto}, Nro.Id. {reserva.NroIdentificadorPicking}, Cant. {auxCantidadDesafectarLinea}");

                        linea.Afectar(auxCantidadDesafectarLinea);

                        lineasModificadas.Add(new DocumentoLineaDesafectada()
                        {
                            LineaModificada = linea,
                            NroDocumento = reserva.NroDocumento,
                            TipoDocumento = reserva.TipoDocumento
                        });

                        if (stop)
                        {
                            break;
                        }
                    }
                    else
                    {
                        throw new ValidationFailedException("General_Sec0_Error_ErrorPrepReservaDoc", new string[] { producto, identificador });
                    }
                }
            }
            else
            {
                throw new ValidationFailedException("General_Sec0_Error_ErrorPrepNoReserva", new string[] { producto, identificador });
            }
        }

        public virtual void NuevaReservaPicking(IUnitOfWork uow, SaldoDetalleDocumento detalle, decimal qtPreparar, int preparacion, bool especificaIdentificador)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var documentoPreparacionReserva = uow.DocumentoRepository.GetPreparacionReserva(detalle.NumeroDocumento, detalle.TipoDocumento, preparacion, detalle.Empresa, detalle.Producto, detalle.Faixa, detalle.Identificador);

            if (documentoPreparacionReserva != null)
            {
                documentoPreparacionReserva.NumeroTransaccion = nuTransaccion;
                documentoPreparacionReserva.NumeroTransaccionDelete = null;
                documentoPreparacionReserva.AfectarCantidadProducto(qtPreparar);
                uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(documentoPreparacionReserva);
            }
            else
            {
                documentoPreparacionReserva = new DocumentoPreparacionReserva
                {
                    Empresa = detalle.Empresa,
                    Producto = detalle.Producto,
                    Faixa = detalle.Faixa,
                    Identificador = detalle.Identificador,
                    CantidadProducto = qtPreparar,
                    CantidadPreparada = qtPreparar,
                    NroDocumento = detalle.NumeroDocumento,
                    TipoDocumento = detalle.TipoDocumento,
                    Preparacion = preparacion,
                    NroIdentificadorPicking = detalle.Identificador,
                    EspecificaIdentificador = especificaIdentificador,
                    NumeroTransaccion = nuTransaccion,
                    NumeroTransaccionDelete = null
                };

                uow.DocumentoRepository.AddPreparacionReserva(documentoPreparacionReserva);
            }

            DocumentoLinea detDocumento = uow.DocumentoRepository.GetLineaIngreso(detalle.NumeroDocumento, detalle.TipoDocumento, detalle.Producto, detalle.Identificador);

            if (detDocumento != null)
            {
                detDocumento.Afectar(qtPreparar);
                uow.DocumentoRepository.UpdateDetailWithoutDocument(detalle.NumeroDocumento, detalle.TipoDocumentoIngresoDUA, detDocumento, nuTransaccion);
            }
            else
            {
                throw new ValidationFailedException("General_Sec0_Error_ErrorSaldoNoReserva", new string[] { detalle.Producto, detalle.Identificador });
            }
        }

        public virtual void MarcarAnulacionesEjecutadas(List<DocumentoAnulacionPreparacionReserva> anulacionesEjecutadas, List<DocumentoAnulacionPreparacionReserva> grupoEjecutado)
        {
            foreach (var anulacion in grupoEjecutado)
            {
                anulacion.AnulacionEjecutada();
                anulacionesEjecutadas.Add(anulacion);
            }
        }

        public virtual void MarcarAnulacionesFallidas(List<DocumentoAnulacionPreparacionReserva> anulacionesEjecutadas, List<DocumentoAnulacionPreparacionReserva> grupoEjecutado)
        {
            foreach (var anulacion in grupoEjecutado)
            {
                anulacion.AnulacionFallida();
                anulacionesEjecutadas.Add(anulacion);
            }
        }

        public virtual ModificarReservaDocumentalResponse ModificarReservaDocumental(IUnitOfWork uow, ModificarReservaDocumentalRequest request)
        {
            var result = new ModificarReservaDocumentalResponse();

            try
            {
                if (!uow.DocumentoRepository.ExisteReservaEquivalente(request, out decimal cantReservadaTotalLote))
                {
                    ProcesarAltaReserva(uow, request, cantReservadaTotalLote);
                    ProcesarBajaReserva(uow, request, cantReservadaTotalLote);
                }

                result.Success = true;
                result.ErrorMsg = "OK";
            }
            catch (ValidationFailedException ex)
            {
                _logger.Error(ex, ex.Message);
                result.Success = false;
                result.ErrorMsg = ex.Message;
                result.StrArguments = ex.StrArguments;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                result.Success = false;
                result.ErrorMsg = ex.Message;
                throw ex;
            }

            return result;
        }

        public virtual void ProcesarAltaReserva(IUnitOfWork uow, ModificarReservaDocumentalRequest request, decimal cantReservadaTotalLote)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var saldoAReservar = request.Cantidad - cantReservadaTotalLote;
            var detalles = uow.DocumentoRepository.ComprobarReservaDisponible(request.Empresa, request.Producto, request.Faixa, request.Identificador, saldoAReservar);
            var reservasAumentar = new Dictionary<string, decimal>();

            //Se da de alta la cantidad reservada en los documentos para el nuevo lote
            foreach (var detalle in detalles)
            {
                decimal cantidad = 0;
                var linea = detalle.LineaModificada;
                if (saldoAReservar == 0)
                    break;

                var saldoLinea = (linea.CantidadIngresada ?? 0) - (linea.CantidadReservada ?? 0) - (linea.CantidadDesafectada ?? 0);

                if (saldoLinea >= saldoAReservar)
                {
                    cantidad = saldoAReservar;
                    linea.CantidadReservada = (linea.CantidadReservada ?? 0) + saldoAReservar;
                    saldoAReservar = 0;
                }
                else
                {
                    cantidad = saldoLinea;
                    linea.CantidadReservada = (linea.CantidadReservada ?? 0) + saldoLinea;
                    saldoAReservar -= saldoLinea;
                }

                uow.DocumentoRepository.UpdateDetailWithoutDocument(detalle.NroDocumento, detalle.TipoDocumento, linea, nuTransaccion);

                var key = $"{detalle.NroDocumento}.{detalle.TipoDocumento}";
                if (!reservasAumentar.ContainsKey(key))
                    reservasAumentar[key] = cantidad;
                else
                    reservasAumentar[key] += cantidad;
            }

            //Se genera o actualiza una nueva linea en la tabla de reservas para el nuevo lote
            foreach (var reserva in reservasAumentar)
            {
                string nroDoc = reserva.Key.Split(".")[0];
                string tpDoc = reserva.Key.Split(".")[1];
                var docPrepReserva = uow.DocumentoRepository.GetPreparacionReserva(nroDoc, tpDoc, request.Preparacion, request.Empresa, request.Producto, request.Faixa, request.Identificador);

                decimal cantidad = reserva.Value;
                if (docPrepReserva != null)
                {
                    docPrepReserva.NumeroTransaccion = nuTransaccion;
                    docPrepReserva.NumeroTransaccionDelete = null;
                    docPrepReserva.CantidadProducto = (docPrepReserva.CantidadProducto ?? 0) + cantidad;
                    uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(docPrepReserva);
                }
                else
                {
                    docPrepReserva = new DocumentoPreparacionReserva
                    {
                        NroDocumento = nroDoc,
                        TipoDocumento = tpDoc,
                        Preparacion = request.Preparacion,
                        Empresa = request.Empresa,
                        Producto = request.Producto,
                        Faixa = request.Faixa,
                        Identificador = ManejoIdentificadorDb.IdentificadorAuto,
                        CantidadProducto = cantidad,
                        CantidadPreparada = 0,
                        NroIdentificadorPicking = request.Identificador,
                        EspecificaIdentificador = false,
                        NumeroTransaccion = nuTransaccion,
                        NumeroTransaccionDelete = null
                    };

                    uow.DocumentoRepository.AddPreparacionReserva(docPrepReserva);
                }
            }
        }

        public virtual void ProcesarBajaReserva(IUnitOfWork uow, ModificarReservaDocumentalRequest request, decimal cantReservadaTotalLote)
        {
            var nuTransaccion = uow.GetTransactionNumber();
            var docsDisminuirReservas = new Dictionary<string, decimal>();
            var saldoADesreservar = request.Cantidad - cantReservadaTotalLote;

            //Se da de baja la reserva para otros lotes 
            var reservas = uow.DocumentoRepository.GetReservasDistintoLote(request.Preparacion, request.Empresa, request.Producto, request.Faixa, request.Identificador);
            foreach (var reserva in reservas)
            {
                decimal cantidad = 0;
                if (saldoADesreservar == 0)
                    break;

                var saldoLinea = (reserva.CantidadProducto ?? 0) - (reserva.CantidadPreparada ?? 0);

                if (saldoLinea >= saldoADesreservar)
                {
                    cantidad = saldoADesreservar;
                    reserva.CantidadProducto = (reserva.CantidadProducto ?? 0) - saldoADesreservar;
                    saldoADesreservar = 0;
                }
                else
                {
                    cantidad = saldoLinea;
                    reserva.CantidadProducto = (reserva.CantidadProducto ?? 0) - saldoLinea;
                    saldoADesreservar -= saldoLinea;
                }

                reserva.NumeroTransaccion = nuTransaccion;

                if (reserva.CantidadProducto > 0)
                {
                    reserva.NumeroTransaccionDelete = null;
                    uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                }
                else
                {
                    reserva.NumeroTransaccionDelete = nuTransaccion;
                    uow.DocumentoRepository.UpdateDocumentoPreparacionReserva(reserva);
                    uow.SaveChanges();
                    uow.DocumentoRepository.RemoveDocumentoPreparacionReserva(reserva);
                }

                var key = $"{reserva.NroDocumento}.{reserva.TipoDocumento}.{reserva.NroIdentificadorPicking}";
                if (!docsDisminuirReservas.ContainsKey(key))
                    docsDisminuirReservas[key] = cantidad;
                else
                    docsDisminuirReservas[key] += cantidad;
            }

            //Se da de baja la reserva para otros lotes en los detalles de los documentos
            foreach (var detalle in docsDisminuirReservas)
            {
                string nroDoc = detalle.Key.Split(".")[0];
                string tpDoc = detalle.Key.Split(".")[1];
                string lote = detalle.Key.Split(".")[2];

                decimal cantidad = detalle.Value;

                var linea = uow.DocumentoRepository.GetDetalleDocumento(request.Producto, request.Faixa, lote, request.Empresa, nroDoc, tpDoc);

                if (linea != null)
                {
                    linea.CantidadReservada = (linea.CantidadReservada ?? 0) - cantidad;
                    uow.DocumentoRepository.UpdateDetailWithoutDocument(nroDoc, tpDoc, linea, nuTransaccion);
                }
            }
        }
    }
}
