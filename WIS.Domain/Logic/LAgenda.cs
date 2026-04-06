using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Recepcion;
using WIS.Domain.Recepcion.Enums;
using WIS.Domain.Recepcion.RecepcionAgendamiento;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Domain.Logic
{
    public class LAgenda
    {
        protected readonly IUnitOfWork _uow;
        protected readonly int _userId;
        protected readonly Logger _logger;
        protected readonly string _aplicacion;
        protected readonly ProductoMapper _mapperProducto;

        public LAgenda(IUnitOfWork uow, int userId, string aplicacion, Logger logger)
        {
            this._uow = uow;
            this._userId = userId;
            this._aplicacion = aplicacion;
            this._logger = logger;
            this._mapperProducto = new ProductoMapper();
        }

        public virtual void LiberarRecepcion(Agenda agenda, ITrafficOfficerService concurrencyControl)
        {
            var transaction = concurrencyControl.CreateTransaccion();

            try
            {
                if (concurrencyControl.IsLocked("T_AGENDA", agenda.GetCompositeId(), true))
                    throw new ValidationFailedException("REC170_msg_Error_AgendaBloqueada", [agenda.GetCompositeId()]);

                concurrencyControl.AddLock("T_AGENDA", agenda.GetCompositeId(), transaction, true);

                var crossDock = _uow.CrossDockingRepository.GetCrossDockingByAgenda(agenda.Id);

                if (crossDock != null && crossDock.Estado == EstadoCrossDockingDb.EnEdicion)
                {
                    crossDock.Iniciar(_uow, agenda, false);
                }

                foreach (var detalle in agenda.Detalles)
                {
                    ModificarAgendaDetalleProblema detalleProblema = new ModificarAgendaDetalleProblema(_uow, _userId, _aplicacion, detalle);
                    detalleProblema.InsertAgendaDetalleProblema(detalle, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado, detalle.CantidadAgendada);
                    detalle.Estado = EstadoAgendaDetalle.ConferidaConDiferencias;
                    detalle.NumeroTransaccion = _uow.GetTransactionNumber();
                    _uow.AgendaRepository.UpdateAgendaDetalle(detalle);
                }

                agenda.Estado = EstadoAgenda.AguardandoDesembarque;
                agenda.CodigoOperacion = OperacionAgendaDb.RecepcionAgrupada;
                agenda.NumeroTransaccion = _uow.GetTransactionNumber();

                _uow.AgendaRepository.UpdateAgendaSinDependencias(agenda);
            }
            finally
            {
                concurrencyControl.DeleteTransaccion(transaction);
            }
        }

        public virtual void CancelarAgenda(Agenda agenda, ITrafficOfficerService concurrencyControl)
        {
            var transaction = concurrencyControl.CreateTransaccion();

            try
            {
                var cancelar = new CancelarAgenda(_uow, concurrencyControl, _userId, _aplicacion, agenda);

                if (agenda.PuedeCancelarAgenda(_uow))
                {
                    cancelar.ProcesarCancelarAgenda(transaction);
                    _uow.SaveChanges();
                }
            }
            finally
            {
                concurrencyControl.DeleteTransaccion(transaction);
            }
        }

        public virtual void DeshacerEmbarque(int nroAgenda, IDeshacerEmbarqueServiceLegacy deshacerEmbarqueService, ITrafficOfficerService concurrencyControl)
        {
            var transaction = concurrencyControl.CreateTransaccion();

            try
            {
                if (concurrencyControl.IsLocked("T_AGENDA", nroAgenda.ToString(), true))
                    throw new ValidationFailedException("REC170_msg_Error_AgendaBloqueada", [nroAgenda.ToString()]);

                concurrencyControl.AddLock("T_AGENDA", nroAgenda.ToString(), transaction, true);

                deshacerEmbarqueService.DeshacerEmbarque(nroAgenda, _uow.GetTransactionNumber());
            }
            finally
            {
                concurrencyControl.DeleteTransaccion(transaction);
            }
        }

        public virtual List<long> CerrarAgenda(Agenda agenda, ITrafficOfficerService concurrencyControl, IFactoryService factoryService, IParameterService parameterService, IIdentityService identity)
        {
            var transaction = concurrencyControl.CreateTransaccion();
            var aplicacion = identity.Application;
            var userId = identity.UserId;
            var predio = identity.Predio;
            var reports = new List<long>();

            try
            {
                var agendaCerrada = false;

                _uow.CreateTransactionNumber($"{aplicacion} - CerrarAgenda");
                _uow.BeginTransaction();

                var cerrarAgenda = new CerrarAgenda(_uow, concurrencyControl, userId, aplicacion, predio, agenda, _logger, factoryService, parameterService, identity);

                if (cerrarAgenda.PuedeCerrarAgenda())
                {
                    reports.AddRange(cerrarAgenda.ProcesarCierreAgenda(transaction, out List<string> ubicacionesComprobarStock));

                    _uow.SaveChanges();

                    agendaCerrada = true;
                }

                if (agendaCerrada)
                {
                    var referencias = _uow.ReferenciaRecepcionRepository.GetReferenciasAgenda(agenda.Id);

                    foreach (var referencia in referencias)
                    {
                        if (!referencia.Detalles.Any(s => s.GetSaldo() > 0) && !referencia.Detalles.Any(s => s.CantidadAgendada > 0))
                        {
                            referencia.Estado = EstadoReferenciaRecepcionDb.Finalizada;
                            referencia.NumeroTransaccion = _uow.GetTransactionNumber();
                            _uow.ReferenciaRecepcionRepository.UpdateReferencia(referencia);
                        }
                    }

                    _uow.SaveChanges();

                    var trasferirEtiquetasAgenda = new TransferirEtiquetasAgenda(_logger, identity);
                    trasferirEtiquetasAgenda.ProcesarTransferenciaEtiquetas(_uow, agenda);

                    _uow.AgendaRepository.AddFotoAgendaLpn(agenda.Id);
                    _uow.AgendaRepository.DesvincularLpnsNoRecibidos(agenda.Id, _uow.GetTransactionNumber());
                    _uow.SaveChanges();
                }

                if (agenda.TipoRecepcion.IngresaFactura)
                {
                    ProcesarFacturas(agenda);
                }

                _uow.SaveChanges();
                _uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                agenda.NumeroInterfazEjecucion = null;
                _uow.Rollback();
                throw new ValidationFailedException(ex.Message, ex.StrArguments);
            }
            catch (Exception ex)
            {
                agenda.NumeroInterfazEjecucion = null;
                _uow.Rollback();
                throw;
            }
            finally
            {
                concurrencyControl.DeleteTransaccion(transaction);
            }

            return reports;
        }

        public virtual void ProcesarFacturas(Agenda agenda)
        {
            var facturas = _uow.FacturaRepository.GetFacturasByAgenda(agenda.Id);

            if (facturas.Any())
            {
                var detallesRecibidos = agenda.Detalles
                    .Where(w => w.CantidadRecibida > 0)
                    .OrderBy(o => o.CodigoProducto)
                    .ThenBy(o => o.Identificador)
                    .ToList();

                foreach (var detAgenda in detallesRecibidos)
                {
                    decimal recibidoAgenda = detAgenda.CantidadRecibida;

                    #region Proceso Lote Especifico

                    var facturasCandidatas = facturas
                        .Where(w => w.Detalles.Any(a => a.Producto == detAgenda.CodigoProducto && a.Identificador == detAgenda.Identificador))
                        .OrderBy(o => o.NumeroFactura)
                        .ToList();

                    foreach (var factura in facturasCandidatas)
                    {
                        if (recibidoAgenda == 0)
                            break;

                        var detallesFacturaCandidatos = factura.Detalles
                            .Where(f => f.Producto == detAgenda.CodigoProducto && f.Identificador == detAgenda.Identificador)
                            .ToList();

                        foreach (var detFactura in detallesFacturaCandidatos)
                        {
                            if (detFactura.CantidadValidada == detFactura.CantidadRecibida)
                                continue;

                            var cantMaximaRecibible = (detFactura.CantidadValidada ?? 0) - (detFactura.CantidadRecibida ?? 0);

                            detFactura.CantidadRecibida = (detFactura.CantidadRecibida ?? 0) + Math.Min(cantMaximaRecibible, recibidoAgenda);
                            detFactura.FechaModificacion = DateTime.Now;
                            detFactura.NumeroTransaccion = _uow.GetTransactionNumber();

                            recibidoAgenda -= Math.Min(cantMaximaRecibible, recibidoAgenda);

                            _uow.FacturaRepository.UpdateFacturaDetalle(detFactura);

                            if (recibidoAgenda == 0)
                                break;
                        }
                    }
                    #endregion

                    #region Proceso Lote AUTO

                    if (recibidoAgenda > 0)
                    {
                        facturasCandidatas = facturas
                            .Where(w => w.Detalles.Any(a => a.Producto == detAgenda.CodigoProducto && a.Identificador == ManejoIdentificadorDb.IdentificadorAuto))
                            .OrderBy(o => o.NumeroFactura)
                            .ToList();

                        foreach (var factura in facturasCandidatas)
                        {
                            if (recibidoAgenda == 0)
                                break;

                            var detallesFacturaCandidatos = factura.Detalles
                                .Where(f => f.Producto == detAgenda.CodigoProducto && f.Identificador == ManejoIdentificadorDb.IdentificadorAuto)
                                .ToList();

                            foreach (var detFactura in detallesFacturaCandidatos)
                            {
                                if (detFactura.CantidadValidada == detFactura.CantidadRecibida)
                                    continue;

                                var cantMaximaRecibible = (detFactura.CantidadValidada ?? 0) - (detFactura.CantidadRecibida ?? 0);
                                detFactura.CantidadRecibida = (detFactura.CantidadRecibida ?? 0) + Math.Min(cantMaximaRecibible, recibidoAgenda);
                                detFactura.FechaModificacion = DateTime.Now;
                                detFactura.NumeroTransaccion = _uow.GetTransactionNumber();

                                recibidoAgenda -= Math.Min(cantMaximaRecibible, recibidoAgenda);

                                _uow.FacturaRepository.UpdateFacturaDetalle(detFactura);

                                if (recibidoAgenda == 0)
                                    break;
                            }

                        }
                    }

                    #endregion
                }
            }

            _uow.SaveChanges();

            var liberarFacturas = (_uow.ParametroRepository.GetParameter(ParamManager.REC170_LIBERA_FACTURAS) ?? "N") == "S";

            if (liberarFacturas)
            {
                var facturasNoUsadas = _uow.FacturaRepository.GetFacturasNoUsadasEnAgenda(agenda.Id);

                foreach (var nuRecepcionFactura in facturasNoUsadas)
                    _uow.FacturaRepository.LiberarFacturas(nuRecepcionFactura, _uow.GetTransactionNumber());

                _uow.SaveChanges();
            }
        }

        public virtual void ProcesarEtiqueta(Agenda agenda, string producto, int empresa, decimal faixa, EtiquetaLote etiquetaActiva, string identificador, DateTime? vencimiento, string dsMotivo, decimal Diferencia, string tipoMovimiento)
        {
            EtiquetaLoteDetalle detEtiqueta = _uow.EtiquetaLoteRepository.GetEtiquetaLoteDetalle(etiquetaActiva.Numero, producto, empresa, faixa, identificador);
            string motivo = dsMotivo;
            if (dsMotivo.Count() > 30)
            {
                motivo = motivo.Substring(0, 30);
            }
            if (detEtiqueta == null)
            {
                detEtiqueta = new EtiquetaLoteDetalle
                {
                    IdEtiquetaLote = etiquetaActiva.Numero,
                    CodigoProducto = producto,
                    Faixa = faixa,
                    IdEmpresa = empresa,
                    Identificador = identificador.Trim(),
                    CantidadRecibida = Diferencia,
                    Cantidad = Diferencia,
                    Vencimiento = vencimiento,
                    Insercion = DateTime.Now,
                    DescripcionMotivo = motivo,
                    NumeroTransaccion = _uow.GetTransactionNumber(),
                };
                _uow.EtiquetaLoteRepository.AddDetalleEtiqueta(detEtiqueta);
            }
            else
            {
                detEtiqueta.CantidadRecibida = (detEtiqueta.CantidadRecibida ?? 0) + Diferencia;
                detEtiqueta.Cantidad = (detEtiqueta.Cantidad ?? 0) + Diferencia;
                detEtiqueta.Modificacion = DateTime.Now;
                detEtiqueta.NumeroTransaccion = _uow.GetTransactionNumber();

                if (detEtiqueta.Vencimiento == null)
                    detEtiqueta.Vencimiento = vencimiento;

                _uow.EtiquetaLoteRepository.UpdateEtiquetaLoteDetalle(detEtiqueta);
            }


            var logEtiqueta = new LogEtiqueta()
            {
                Agenda = agenda.Id,
                NumeroEtiqueta = etiquetaActiva.Numero,
                CodigoProducto = producto,
                Faixa = faixa,
                Empresa = empresa,
                Identificador = identificador,
                Cantidad = Diferencia,
                Ubicacion = etiquetaActiva.IdUbicacion,
                FechaOperacion = DateTime.Now,
                NroTransaccion = _uow.GetTransactionNumber(),
                Vencimiento = vencimiento,
                TipoMovimiento = tipoMovimiento,
                Aplicacion = _aplicacion,
                Funcionario = _userId,
            };

            this._uow.EtiquetaLoteRepository.AddLogEtiqueta(logEtiqueta);

        }

        public virtual void ProcesarAltaStock(IUnitOfWork uow, Agenda agenda, string producto, int empresa, string identificador, EtiquetaLote etiquetaActiva, decimal faixa, DateTime? vencimiento, decimal qtProducto)
        {
            EtiquetaLoteDetalle detEtiqueta = _uow.EtiquetaLoteRepository.GetEtiquetaLoteDetalle(etiquetaActiva.Numero, producto, empresa, faixa, identificador);
            if (!string.IsNullOrEmpty(etiquetaActiva.IdUbicacionSugerida))
            {
                decimal qtRecibido = (detEtiqueta.CantidadRecibida ?? 0);
                decimal qtEtiquetaGenerada = (detEtiqueta.CantidadEtiquetaGenerada ?? 0);
                decimal qtRecibidoAntes = qtRecibido - qtProducto;

                if (qtRecibido > qtEtiquetaGenerada)
                {
                    decimal qtTransito = 0;

                    if (qtRecibidoAntes > qtEtiquetaGenerada)
                        qtTransito = qtProducto - qtRecibidoAntes;
                    else
                        qtTransito = qtProducto - qtEtiquetaGenerada;

                    Stock stockActualizar = uow.StockRepository.GetStock(empresa, producto, faixa, etiquetaActiva.IdUbicacionSugerida, identificador);

                    if (stockActualizar != null)
                    {
                        stockActualizar.CantidadTransitoEntrada += qtTransito;
                        stockActualizar.NumeroTransaccion = uow.GetTransactionNumber();
                        uow.StockRepository.UpdateStock(stockActualizar);
                    }
                }
            }

            Stock stock = uow.StockRepository.GetStock(empresa, producto, faixa, etiquetaActiva.IdUbicacion, identificador);
            if (stock == null)
            {
                stock = new Stock
                {
                    Ubicacion = etiquetaActiva.IdUbicacion,
                    Producto = producto,
                    Empresa = empresa,
                    Faixa = faixa,
                    Identificador = identificador,
                    Vencimiento = vencimiento,
                    Cantidad = qtProducto,
                    ReservaSalida = qtProducto,
                    CantidadTransitoEntrada = 0,
                    FechaModificacion = DateTime.Now,
                    Averia = "N",
                    Inventario = null,
                    FechaInventario = null,
                    ControlCalidad = EstadoControlCalidad.Controlado,
                    NumeroTransaccion = uow.GetTransactionNumber()
                };

                uow.StockRepository.AddStock(stock);
            }
            else
            {
                if (vencimiento < stock.Vencimiento)
                {
                    stock.Vencimiento = vencimiento;
                }
                stock.FechaModificacion = DateTime.Now;
                stock.Cantidad += qtProducto;
                stock.ReservaSalida += qtProducto;
                stock.NumeroTransaccion = uow.GetTransactionNumber();

                uow.StockRepository.UpdateStock(stock);
            }
        }

        public virtual void ProcesarControlCalidad(IUnitOfWork uow, Agenda agenda, string producto, int empresa, decimal faixa, string identificador, EtiquetaLote etiquetaActiva, int userId, string application)
        {
            var controlesNecesarios = uow.ControlDeCalidadRepository.GetControlDeCalidadProducto(producto, empresa);

            foreach (var control in controlesNecesarios)
            {
                if (!uow.ControlDeCalidadRepository.AnyControlDeCalidadPendienteSinAceptar(control.Codigo, etiquetaActiva.Numero, empresa, producto, faixa, identificador))
                {
                    var nuevoControlPendiente = new ControlDeCalidadPendiente()
                    {
                        Codigo = control.Codigo,
                        Etiqueta = etiquetaActiva.Numero,
                        Producto = producto,
                        Identificador = identificador,
                        Empresa = empresa,
                        Ubicacion = etiquetaActiva.IdUbicacion,
                        Faixa = faixa,
                        Aceptado = false,
                        FechaModificacion = DateTime.Now,
                        Predio = agenda.Predio
                    };

                    uow.ControlDeCalidadRepository.AddControlDeCalidadPendiente(nuevoControlPendiente);
                }
            }
        }

        public virtual void ProcesarMotivos(IUnitOfWork uow, Agenda agenda, string cdProducto, decimal faixa, int empresa, string identificador, decimal qtProducto, int userId, string application, long nuTransaccionDB)
        {
            var idManejoIdentificadores = new List<string>
            {
                ManejoIdentificadorDb.Lote,
                ManejoIdentificadorDb.Serie
            };
            decimal qtDispLote = 0;
            decimal qtMovimiento = 0;

            Producto producto = uow.ProductoRepository.GetProducto(empresa, cdProducto);
            if (idManejoIdentificadores.Contains(_mapperProducto.MapManejoIdentificador(producto.ManejoIdentificador)) && identificador != ManejoIdentificadorDb.IdentificadorAuto
               && uow.AgenteRepository.AnyLoteAuto(agenda.Id, cdProducto, faixa))
            {
                AgendaDetalle detAgenda = uow.AgendaRepository.GetAgendaDetalle(agenda.Id, empresa, cdProducto, faixa, identificador);

                if (detAgenda != null)
                    qtDispLote = (detAgenda.GetCantDisponibleIngresada() ?? 0);

                if (qtDispLote < 0)
                    qtDispLote = 0;

                if (qtDispLote < qtProducto)
                {
                    AgendaDetalle detAgendaLoteAuto = uow.AgendaRepository.GetAgendaDetalle(agenda.Id, empresa, cdProducto, faixa, ManejoIdentificadorDb.IdentificadorAuto);

                    decimal descontarLoteAuto = qtProducto - qtDispLote;
                    decimal saldoSinLote = detAgendaLoteAuto.GetSaldo();
                    decimal qtAgendadaLoteAuto = detAgendaLoteAuto.CantidadAgendada;

                    if (saldoSinLote > 0)
                    {
                        qtMovimiento = (saldoSinLote > descontarLoteAuto ? descontarLoteAuto : saldoSinLote);

                        if (qtMovimiento > 0)
                        {
                            if ((qtAgendadaLoteAuto - qtMovimiento) == 0)
                            {
                                detAgendaLoteAuto.CantidadAgendada = 0;
                                detAgendaLoteAuto.Estado = EstadoAgendaDetalle.EnProgreso;
                                detAgendaLoteAuto.FechaAlta = DateTime.Now;
                            }
                            else
                            {
                                detAgendaLoteAuto.CantidadAgendada = detAgendaLoteAuto.CantidadAgendada - qtMovimiento;
                            }
                            detAgendaLoteAuto.NumeroTransaccion = nuTransaccionDB;
                            uow.AgendaRepository.UpdateAgendaDetalle(detAgendaLoteAuto);


                            if (detAgenda != null)
                            {
                                detAgenda.CantidadAgendada = detAgenda.CantidadAgendada + qtMovimiento;
                                detAgenda.Estado = EstadoAgendaDetalle.EnProgreso;
                                detAgenda.FechaModificacion = DateTime.Now;
                                detAgenda.NumeroTransaccion = nuTransaccionDB;
                                uow.AgendaRepository.UpdateAgendaDetalle(detAgenda);
                            }
                            else
                            {
                                detAgenda = new AgendaDetalle
                                {
                                    IdAgenda = agenda.Id,
                                    IdEmpresa = empresa,
                                    CodigoProducto = cdProducto,
                                    Faixa = 1,
                                    Identificador = identificador,
                                    Estado = EstadoAgendaDetalle.EnProgreso,
                                    CantidadAgendada = qtMovimiento,
                                    Vencimiento = null,
                                    FechaAlta = DateTime.Now,
                                    CantidadAgendadaOriginal = 0,
                                    NumeroTransaccion = nuTransaccionDB,
                                };
                                uow.AgendaRepository.AddAgendaDetalle(detAgenda);
                            }
                        }
                    }
                }
            }

            decimal cantidadDisponible = uow.AgendaRepository.GetCantidadDisponible(agenda.Id, empresa, cdProducto, faixa, identificador);
            if (cantidadDisponible == 0)
            {
                AgendaDetalle detAgendaUpdate = uow.AgendaRepository.GetAgendaDetalle(agenda.Id, empresa, cdProducto, faixa, identificador);

                if (detAgendaUpdate == null)
                {
                    var newDetAgenda = new AgendaDetalle
                    {
                        IdAgenda = agenda.Id,
                        IdEmpresa = empresa,
                        CodigoProducto = cdProducto,
                        Faixa = 1,
                        Identificador = identificador,
                        Estado = EstadoAgendaDetalle.EnProgreso,
                        CantidadAgendada = qtMovimiento,
                        Vencimiento = null,
                        FechaAlta = DateTime.Now,
                        NumeroTransaccion = nuTransaccionDB,
                    };

                    uow.AgendaRepository.AddAgendaDetalle(newDetAgenda);
                }
                else
                {
                    detAgendaUpdate.NumeroTransaccion = nuTransaccionDB;
                    uow.AgendaRepository.UpdateAgendaDetalle(detAgendaUpdate);
                }

            }
            uow.SaveChanges();
        }

        public virtual void ProcesarQtAgendada(IUnitOfWork uow, Agenda agenda, string producto, int empresa, decimal cdFaixa, string identificador, EtiquetaLote etiquetaActiva, decimal qtRecibido, DateTime? vencimiento, decimal saldoLoteAuto, int userId, string application, long nuTransaccionDB)
        {

            AgendaDetalle detAgenda = uow.AgendaRepository.GetAgendaDetalle(agenda.Id, empresa, producto, cdFaixa, identificador);

            AgendaDetalle detAgendaLoteAuto = uow.AgendaRepository.GetAgendaDetalle(agenda.Id, empresa, producto, cdFaixa, ManejoIdentificadorDb.IdentificadorAuto);
            if (detAgendaLoteAuto != null && (detAgenda.CantidadRecibida == 0) && (detAgenda.CantidadAgendada == 0))
            {
                if (saldoLoteAuto > qtRecibido)
                {
                    detAgenda.CantidadAgendada = qtRecibido;
                }
                else
                {
                    detAgenda.CantidadAgendada = saldoLoteAuto;
                }
            }

            detAgenda.CantidadRecibida = (detAgenda.CantidadRecibida) + qtRecibido;
            detAgenda.Vencimiento = vencimiento;
            detAgenda.FechaModificacion = DateTime.Now;
            detAgenda.NumeroTransaccion = nuTransaccionDB;

            this.RecibidoExcedeAgendado(uow, agenda, detAgenda, userId, application, nuTransaccionDB);
            this.ProductosNoEsperados(uow, detAgenda, userId, application);

            detAgenda.FechaModificacion = DateTime.Now;
            detAgenda.NumeroTransaccion = nuTransaccionDB;

            if (uow.AgenteRepository.AnyProblemaDetalle(detAgenda))
                detAgenda.Estado = EstadoAgendaDetalle.ConferidaConDiferencias;
            else
                detAgenda.Estado = EstadoAgendaDetalle.ConferidaSinDiferencias;

            uow.AgendaRepository.UpdateAgendaDetalle(detAgenda);
            uow.SaveChanges();

            if (uow.AgenteRepository.AnyProblema(agenda))
                agenda.Estado = EstadoAgenda.ConferidaConDiferencias;
            else
                agenda.Estado = EstadoAgenda.ConferidaSinDiferencias;

            agenda.NumeroTransaccion = nuTransaccionDB;

            uow.AgendaRepository.UpdateAgenda(agenda);

        }

        public virtual void RecibidoExcedeAgendado(IUnitOfWork uow, Agenda agenda, AgendaDetalle detAgenda, int userId, string application, long nuTransaccionDB)
        {
            decimal diferencia = 0;
            AgendaDetalle detAgendaAuto = uow.AgendaRepository.GetAgendaDetalle(detAgenda.IdAgenda, detAgenda.IdEmpresa, detAgenda.CodigoProducto, detAgenda.Faixa, ManejoIdentificadorDb.IdentificadorAuto);

            if (detAgendaAuto != null)
            {
                if (detAgendaAuto.CantidadAgendada > 0)
                {
                    diferencia = (detAgendaAuto.CantidadAgendada);

                    AddProblema(uow, detAgendaAuto, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado, diferencia, userId);
                }
                else if (detAgendaAuto.CantidadAgendada == 0)
                {
                    RemoveProblema(uow, detAgendaAuto, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado);
                }
            }

            uow.SaveChanges();

            diferencia = (detAgenda.CantidadAgendada) - (detAgenda.CantidadRecibida);

            if (diferencia < 0)
            {
                AddProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeAgendado, diferencia, userId);

                if (agenda.TipoRecepcion == null)
                    agenda.TipoRecepcion = uow.RecepcionTipoRepository.GetRecepcionTipo(agenda.TipoRecepcionInterno);

                if (agenda.TipoRecepcion.TipoSeleccionReferencia != TipoSeleccionReferenciaDb.Libre
                    && agenda.TipoRecepcion.TipoSeleccionReferencia != TipoSeleccionReferenciaDb.Lpn)
                {
                    decimal? aux;
                    decimal? diferenciaRef = detAgenda.CantidadRecibida - detAgenda.CantidadAgendada;
                    decimal? saldoRef = 0;
                    bool fin = false;

                    var listDetRefRec = uow.ReferenciaRecepcionRepository.GetExcesoSaldoReferencia(agenda, detAgenda);

                    if (listDetRefRec.Count > 0)
                    {
                        saldoRef = listDetRefRec.Sum(d => d.GetSaldo());

                        aux = saldoRef - diferenciaRef;

                        if (diferenciaRef > 0)
                        {
                            if (aux < diferenciaRef)
                            {
                                fin = false;
                                listDetRefRec.OrderBy(x => x.FechaVencimiento).ToList().ForEach(x =>
                                {
                                    if (!fin)
                                    {
                                        var detalle = new ReferenciaRecepcionDetalle()
                                        {
                                            IdReferencia = x.IdReferencia,
                                            CodigoProducto = x.CodigoProducto,
                                            IdEmpresa = x.IdEmpresa,
                                            Faixa = x.Faixa,
                                            Identificador = x.Identificador,
                                            IdLineaSistemaExterno = $"{ProblemaAgendaDb.DescripcionExcedeSaldoReferenciaRecepcion}{detAgenda.IdAgenda}",
                                            CantidadReferencia = 0,
                                            CantidadAnulada = 0,
                                            CantidadAgendada = 0,
                                            CantidadRecibida = 0,
                                            CantidadConfirmadaInterfaz = 0,
                                            ImporteUnitario = 0,
                                            Anexo1 = null,
                                            FechaVencimiento = null,
                                            NumeroTransaccion = nuTransaccionDB,
                                        };

                                        decimal? qtRef = ((aux ?? 0) - x.GetSaldo());
                                        if (qtRef > 0)
                                        {
                                            detalle.CantidadReferencia = qtRef;
                                            uow.ReferenciaRecepcionRepository.AddReferenciaDetalle(detalle);
                                            aux = qtRef;
                                        }
                                        else if (aux > 0)
                                        {
                                            detalle.CantidadReferencia = aux;
                                            uow.ReferenciaRecepcionRepository.AddReferenciaDetalle(detalle);
                                            fin = true;
                                        }
                                    }
                                });

                                AddProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeSaldoReferenciaRecepcion, aux, null);
                            }
                            else if (aux > diferenciaRef)
                            {
                                fin = false;
                                listDetRefRec.OrderBy(x => x.ReferenciaRecepcion.FechaVencimientoOrden).ToList().ForEach(x =>
                                {
                                    if (!fin)
                                    {
                                        var detalle = new ReferenciaRecepcionDetalle()
                                        {
                                            IdReferencia = x.IdReferencia,
                                            CodigoProducto = x.CodigoProducto,
                                            IdEmpresa = x.IdEmpresa,
                                            Faixa = x.Faixa,
                                            Identificador = x.Identificador,
                                            IdLineaSistemaExterno = $"{ProblemaAgendaDb.DescripcionExcedeSaldoReferenciaRecepcion}{detAgenda.IdAgenda}",
                                            CantidadReferencia = 0,
                                            CantidadAnulada = 0,
                                            CantidadAgendada = 0,
                                            CantidadRecibida = 0,
                                            CantidadConfirmadaInterfaz = 0,
                                            ImporteUnitario = 0,
                                            Anexo1 = null,
                                            FechaVencimiento = null,
                                            NumeroTransaccion = nuTransaccionDB,
                                        };

                                        decimal? qtRef = ((diferenciaRef ?? 0) - x.GetSaldo());
                                        if (qtRef > 0)
                                        {
                                            detalle.CantidadReferencia = qtRef;
                                            uow.ReferenciaRecepcionRepository.AddReferenciaDetalle(detalle);
                                            diferenciaRef = qtRef;
                                        }
                                        else if (diferenciaRef > 0)
                                        {
                                            detalle.CantidadReferencia = diferenciaRef;
                                            uow.ReferenciaRecepcionRepository.AddReferenciaDetalle(detalle);
                                            fin = true;
                                        }
                                    }
                                });

                                RemoveProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeSaldoReferenciaRecepcion);
                            }
                        }
                        else
                        {
                            uow.ReferenciaRecepcionRepository.RemoveReferenciaDetetalle(agenda.Id, listDetRefRec, _aplicacion, userId, nuTransaccionDB);
                            RemoveProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeSaldoReferenciaRecepcion);
                        }
                    }
                }

                RemoveProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado);
            }
            else if (diferencia > 0)
            {
                AddProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado, diferencia, userId);
            }
            else
            {
                RemoveProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoExcedeSaldoReferenciaRecepcion);
                RemoveProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoMenorAgendado);
            }

            _uow.SaveChanges();
        }

        public virtual void ProductosNoEsperados(IUnitOfWork uow, AgendaDetalle detAgenda, int cdFunc, string aplicacion)
        {
            if (detAgenda.CantidadAgendada == 0 && detAgenda.CantidadAgendadaOriginal == 0 && detAgenda.CantidadRecibida > 0)
                AddProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoProductoNoEsperado, 0, cdFunc);
            else
                RemoveProblema(uow, detAgenda, TipoProblemaAgendaDetalle.Problema, ProblemaAgendaDetalle.RecibidoProductoNoEsperado);

            uow.SaveChanges();
        }

        public virtual void RemoveProblema(IUnitOfWork uow, AgendaDetalle detAge, TipoProblemaAgendaDetalle tpProb, ProblemaAgendaDetalle problema)
        {
            AgendaDetalleProblema recAgeProb = uow.AgendaRepository.GetProblemaSinAceptar(problema, tpProb, detAge.IdAgenda, detAge.CodigoProducto, detAge.Identificador, detAge.Faixa);

            if (recAgeProb != null)
            {
                uow.AgendaRepository.RemoveAgendaDetalleProblema(recAgeProb);
            }
        }

        public virtual void AddProblema(IUnitOfWork uow, AgendaDetalle detAge, TipoProblemaAgendaDetalle tpProb, ProblemaAgendaDetalle problema, decimal? dif, int? cdFunc)
        {
            if (detAge != null)
                if (detAge.CantidadAgendada != 0 || detAge.CantidadRecibida != 0 || detAge.CantidadAgendadaOriginal != 0)
                {
                    AgendaDetalleProblema recAgeProb = uow.AgendaRepository.GetProblemaSinAceptar(problema, tpProb, detAge.IdAgenda, detAge.CodigoProducto, detAge.Identificador, detAge.Faixa);

                    if (recAgeProb == null)
                    {
                        recAgeProb = new AgendaDetalleProblema();
                        recAgeProb.NumeroAgenda = detAge.IdAgenda;
                        recAgeProb.CodigoProducto = detAge.CodigoProducto;
                        recAgeProb.Identificador = detAge.Identificador;
                        recAgeProb.Embalaje = detAge.Faixa;
                        recAgeProb.TipoProblema = tpProb;
                        recAgeProb.Problema = problema;
                        recAgeProb.Diferencia = dif;
                        recAgeProb.Funcionario = cdFunc;
                        recAgeProb.FechaAceptacionProblema = null;
                        recAgeProb.FechaAlta = DateTime.Now;
                        recAgeProb.FechaModificacion = DateTime.Now;
                        recAgeProb.Aceptado = false;
                        uow.AgendaRepository.AddAgendaDetalleProblema(recAgeProb);
                    }
                    else
                    {
                        recAgeProb.FechaModificacion = DateTime.Now;
                        recAgeProb.Diferencia = dif;
                        recAgeProb.Funcionario = cdFunc;
                    }
                }
        }
    }
}
