using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Inventario;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD113PanelFabricacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly ITaskQueueService _taskQueue;
        protected readonly IValidationService _validationService;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public PRD113PanelFabricacion(
            IIdentityService identity,
            ITaskQueueService taskQueue,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogicaProduccionFactory logicaProduccionFactory,
            IValidationService validationService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logicaProduccionFactory = logicaProduccionFactory;
            this._concurrencyControl = concurrencyControl;
            this._taskQueue = taskQueue;
            this._validationService = validationService;
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            string nuIngresoProduccion = context.GetParameter("nuIngresoProduccion");
            ILogicaProduccion logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

            var ingreso = logicaProduccion.GetIngresoProduccion();

            form.GetField("idInternoProduccion").Value = ingreso.Id;
            form.GetField("idExternoProduccion").Value = ingreso.IdProduccionExterno;
            form.GetField("descripcionProduccion").Value = ingreso.Anexo1;

            var tipoIngreso = uow.DominioRepository.GetDominio(ingreso.Tipo);
            form.GetField("tipoEstacion").Value = tipoIngreso?.Descripcion;

            var empresa = uow.EmpresaRepository.GetEmpresa(ingreso.Empresa.Value);
            form.GetField("empresa").Value = empresa.Id.ToString();
            form.GetField("nombreEmpresa").Value = empresa.Nombre;

            context.AddParameter("PRD113_HABILITADO_PRODUCCION", (ingreso.Situacion != SituacionDb.PRODUCCION_FINALIZADA ? "S" : "N"));

            uow.ProduccionRepository.AnyDetallesSalidaManejaLoteOVencimiento(nuIngresoProduccion, out bool manejaLote, out bool manejaVencimiento);

            context.Parameters.Add(new WIS.Components.Common.ComponentParameter() { Id = "PRD113_REQUIRED_MODALIDAD", Value = manejaLote ? "S" : "N" });
            context.Parameters.Add(new WIS.Components.Common.ComponentParameter() { Id = "PRD113_REQUIRED_VENCIMIENTO", Value = manejaVencimiento ? "S" : "N" });
            form.GetField("fechaVencimiento").Disabled = !manejaVencimiento;

            InicializarSelect(uow, form, context, ingreso);

            if (logicaProduccion.TieneEspacioProduccion())
            {
                EspacioProduccion espacioProduccion = logicaProduccion.GetEspacioProduccion();

                form.GetField("idEspacioProduccion").Value = espacioProduccion.Id;
                form.GetField("descripcionEspacioProduccion").Value = espacioProduccion.Descripcion;
                form.GetField("ubicacionEntrada").Value = espacioProduccion.IdUbicacionEntrada;
                form.GetField("ubicacionProduccion").Value = espacioProduccion.IdUbicacionProduccion;
                form.GetField("ubicacionSalida").Value = espacioProduccion.IdUbicacionSalida;
                form.GetField("ubicacionSalidaTran").Value = espacioProduccion.IdUbicacionSalidaTran;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PRD113PanelFabricacionFromValidationModule(uow, this._identity.GetFormatProvider()), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            switch (context.ButtonId)
            {
                case "FinalizarProduccion": FinalizarProduccion(form, context); break;
                case "ConfirmarFinalizarProduccion": ConfirmarFinalizarProduccion(form, context); break;
                case "NotificarProduccion": NotificarProduccion(form, context); break;
                case "btnProducir": Producir(form, context); break;
                case "disableModalidadLoteProduccion":
                    {
                        var modalidadLoteSelect = form.GetField("idModalidadLoteProduccion");
                        if (!string.IsNullOrEmpty(modalidadLoteSelect.Value))
                        {
                            modalidadLoteSelect.ReadOnly = true;
                            modalidadLoteSelect.Disabled = true;
                        }
                    }
                    break;
                default:
                    return form;
            }
            return form;
        }

        #region Metodos Auxiliares

        public virtual void ConfirmarFinalizarProduccion(Form form, FormSubmitContext context)
        {
            string nuIngresoProduccion = context.GetParameter("nuIngresoProduccion");

            if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", nuIngresoProduccion, true))
                throw new Exception("General_msg_Error_ProduccionBloqueada");

            var transactionTO = _concurrencyControl.CreateTransaccion();
            using var uow = _uowFactory.GetUnitOfWork();

            uow.BeginTransaction();

            try
            {
                var notificar = false;
                ILogicaProduccion logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

                _concurrencyControl.AddLock("T_PRDC_INGRESO", nuIngresoProduccion, transactionTO, true);

                if (logicaProduccion.ProduccionEnProcesoDeNotificacion())
                    throw new Exception("PRD113_grid1_Error_NotificacionEnProceso");

                if (!logicaProduccion.ProduccionHabilitadaParaNotificar())
                    throw new Exception("PRD113_grid1_Error_ProduccionEnEstadoIncorrecto");

                uow.CreateTransactionNumber("Finalizar producción");

                var ingreso = logicaProduccion.GetIngresoProduccion();
                var ubicacionProduccion = uow.ProduccionRepository.GetUbicacionProduccion(ingreso.IdEspacioProducion);

                logicaProduccion.DesafectarInsumosConSaldo(ubicacionProduccion, _concurrencyControl, transactionTO);

                var msgSuccess = "PRD113_grid1_Msg_ProduccionFinalizada";
                if (logicaProduccion.HayPendientesDeNotificacion())
                {
                    logicaProduccion.UpdateSituacion(SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL);
                    msgSuccess = "PRD113_grid1_Msg_ProduccionPendNotiFinal";
                    notificar = true;
                }
                else
                {
                    logicaProduccion.FinalizarProduccion();

                    var espacioProduccion = logicaProduccion.GetEspacioProduccion();
                    espacioProduccion.NumeroIngreso = null;
                    espacioProduccion.NumeroTransaccion = uow.GetTransactionNumber();

                    uow.EspacioProduccionRepository.UpdateEspacio(espacioProduccion);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddParameter("PRD113_HABILITADO_PRODUCCION", "N");
                context.AddSuccessNotification(msgSuccess);

                if (_taskQueue.IsEnabled() && notificar)
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionProduccion, nuIngresoProduccion);
            }
            catch (Exception ex)
            {
                uow.Rollback();
                _logger.Error(ex, ex.Message);
                throw;
            }

            finally
            {
                _concurrencyControl.DeleteTransaccion(transactionTO);
            }
        }

        public virtual void Producir(Form form, FormSubmitContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var keysAjuste = new List<string>();
            var transactionTO = _concurrencyControl.CreateTransaccion();

            uow.BeginTransaction();

            try
            {
                var rows = JsonConvert.DeserializeObject<List<GridRow>>(context.GetParameter("rowsProducir"))!;

                var rowsModificadas = rows.Where(row => 
                    row.Cells.Any(cell => (cell.Column.Id == "QT_PRODUCIR" && !string.IsNullOrEmpty(cell.Value))) &&
                    row.Cells.Any(cell => (cell.Column.Id == "ND_MOTIVO" && !string.IsNullOrEmpty(cell.Value))))
                    .ToList();

                if (rowsModificadas == null || rowsModificadas.Count() < 1)
                    throw new ValidationFailedException("PRD113_grid1_Error_NoHayFilasModificadas");

                var idEspacioProduccion = form.GetField("idEspacioProduccion").Value;
                var ingresoProduccion = form.GetField("idInternoProduccion").Value;
                var ubicacionProduccion = form.GetField("ubicacionProduccion").Value;
                var loteUtilizar = form.GetField("loteUtilizar").Value.Trim().ToUpper();
                var idModalidadLote = form.GetField("idModalidadLoteProduccion").Value;

                if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", ingresoProduccion, true))
                    throw new EntityLockedException("General_msg_Error_ProduccionBloqueada");

                _concurrencyControl.AddLock("T_PRDC_INGRESO", ingresoProduccion, transactionTO, true);

                if (string.IsNullOrEmpty(loteUtilizar) && context.GetParameter("isRequiredModalidadLote") == "S")
                    throw new ValidationFailedException("PRD113_form1_Error_ModalidadLoteInvalida");

                var ingreso = uow.IngresoProduccionRepository.GetIngresoByIdConDetalles(ingresoProduccion);
                var empresa = ingreso.Empresa.Value;

                if (ingreso.Situacion == SituacionDb.PRODUCCION_FINALIZADA || ingreso.Situacion == SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL)
                    throw new ValidationFailedException("PRD113_grid1_Error_ProduccionEnEstadoIncorrecto");

                uow.CreateTransactionNumber("Producir Producto Esperado");
                var nuTransaccion = uow.GetTransactionNumber();

                var keysStock = new List<Stock>();

                foreach (var row in rowsModificadas)
                {
                    keysStock.Add(new Stock()
                    {
                        Ubicacion = ubicacionProduccion,
                        Empresa = empresa,
                        Producto = row.GetCell("CD_PRODUTO").Value,
                        Faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, this._identity.GetFormatProvider()),
                        Identificador = row.GetCell("NU_IDENTIFICADOR").Value,
                        NumeroTransaccion = nuTransaccion,
                    });

                    if (uow.StockRepository.ExisteSerie(row.GetCell("CD_PRODUTO").Value, empresa, row.GetCell("NU_IDENTIFICADOR").Value))
                        throw new ValidationFailedException("General_Sec0_Error_SerieYaExiste", new string[] { row.GetCell("NU_IDENTIFICADOR").Value, row.GetCell("CD_PRODUTO").Value, empresa.ToString() });
                }

                var idsBloqueos = keysStock.Select(s => s.GetLockId(_identity.GetFormatProvider())).ToList();

                var listLock = this._concurrencyControl.GetLockList("T_STOCK", idsBloqueos, transactionTO);

                if (listLock.Count > 0)
                {
                    var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                    throw new EntityLockedException("PRD111ConsumirStock_grid1_msg_RegistrosBloqueados", [keyBloqueo[0], keyBloqueo[1], keyBloqueo[2], keyBloqueo[4],]);
                }

                this._concurrencyControl.AddLockList("T_STOCK", idsBloqueos, transactionTO, true);

                DateTime? fechaVencimiento = null;

                if (!string.IsNullOrEmpty(context.Parameters.FirstOrDefault(x => x.Id == "fechaVencimiento").Value) && context.Parameters.FirstOrDefault(x => x.Id == "isRequiredVencimiento").Value == "S")
                {
                    if (DateTime.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "fechaVencimiento").Value, _identity.GetFormatProvider(), out DateTime fecha))
                        fechaVencimiento = fecha;
                    else
                        fechaVencimiento = DateTime.Now;
                }

                if (ingreso.Situacion != SituacionDb.PRODUCIENDO)
                {
                    ingreso.Situacion = SituacionDb.PRODUCIENDO;
                    ingreso.IdModalidadLote = idModalidadLote;
                    ingreso.NuTransaccion = uow.GetTransactionNumber();

                    uow.IngresoProduccionRepository.UpdateIngresoProduccion(ingreso);
                }
                else if (string.IsNullOrEmpty(ingreso.IdModalidadLote))
                {
                    ingreso.IdModalidadLote = idModalidadLote;
                    ingreso.NuTransaccion = uow.GetTransactionNumber();

                    uow.IngresoProduccionRepository.UpdateIngresoProduccion(ingreso);
                }

                uow.SaveChanges();

                var stocks = uow.StockRepository.GetStock(keysStock);

                foreach (var row in rowsModificadas)
                {
                    var cdProducto = row.GetCell("CD_PRODUTO").Value;
                    var lote = row.GetCell("NU_IDENTIFICADOR").Value;
                    var faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, this._identity.GetFormatProvider());

                    var cantidadAProducir = decimal.Parse(row.GetCell("QT_PRODUCIR").Value, this._identity.GetFormatProvider());
                    var motivo = row.GetCell("ND_MOTIVO").Value;

                    var producto = uow.ProductoRepository.GetProducto(empresa, cdProducto);

                    var stock = stocks
                        .FirstOrDefault(s => s.Ubicacion == ubicacionProduccion
                            && s.Empresa == empresa
                            && s.Producto == cdProducto
                            && s.Faixa == faixa
                            && s.Identificador == lote);

                    if (stock == null)
                    {
                        var espacioProduccion = uow.EspacioProduccionRepository.GetEspacioProduccion(idEspacioProduccion);

                        stock = new Stock
                        {
                            Ubicacion = ubicacionProduccion,
                            Empresa = empresa,
                            Producto = cdProducto,
                            Faixa = faixa,
                            Identificador = lote,
                            Cantidad = cantidadAProducir,
                            Vencimiento = producto.IsFefo() ? fechaVencimiento : (producto.IsFifo() ? DateTime.Now : null),
                            ReservaSalida = 0,
                            CantidadTransitoEntrada = 0,
                            FechaModificacion = DateTime.Now,
                            Averia = "N",
                            Inventario = "R",
                            ControlCalidad = EstadoControlCalidad.Controlado,
                            NumeroTransaccion = nuTransaccion,
                            Predio = espacioProduccion.Predio,
                        };

                        uow.StockRepository.AddStock(stock);
                    }
                    else
                    {
                        stock.Cantidad = (stock.Cantidad ?? 0) + cantidadAProducir;
                        stock.FechaModificacion = DateTime.Now;
                        stock.Vencimiento = producto.IsFefo() ? InventarioLogic.ResolverVencimiento(stock.Vencimiento, fechaVencimiento) : stock.Vencimiento;

                        uow.StockRepository.UpdateStock(stock);
                    }

                    uow.SaveChanges();

                    if (motivo == TipoIngresoProduccion.MOT_PROD_ADS)
                    {
                        var ajuste = new AjusteStock
                        {
                            NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                            Ubicacion = stock.Ubicacion,
                            Empresa = stock.Empresa,
                            Producto = stock.Producto,
                            Faixa = stock.Faixa,
                            Identificador = stock.Identificador,
                            QtMovimiento = cantidadAProducir,
                            FechaVencimiento = stock.Vencimiento,
                            FechaRealizado = DateTime.Now,
                            TipoAjuste = TipoAjusteDb.Stock,
                            CdMotivoAjuste = MotivoAjusteDb.Produccion,
                            DescMotivo = $"Producción para el Ingreso Nro: {ingreso.Id}",
                            NuTransaccion = nuTransaccion,
                            Predio = stock.Predio,
                            IdAreaAveria = "N",
                            FechaMotivo = DateTime.Now,
                            Funcionario = _identity.UserId,
                            Aplicacion = _identity.Application,
                            Metadata = ingreso.Id
                        };

                        uow.AjusteRepository.Add(ajuste);

                        keysAjuste.Add(ajuste.NuAjusteStock.ToString());
                    }
                    else
                    {
                        var cantidadTeorica = decimal.Parse(row.GetCell("QT_TEORICO").Value, this._identity.GetFormatProvider());
                        var detalleTeorico = long.Parse(row.GetCell("NU_PRDC_DET_TEORICO").Value);
                        var detalleReal = uow.IngresoProduccionRepository.GetDetalleSalidaReal(ingreso.Id, cdProducto, empresa, lote);

                        if (detalleReal == null)
                        {
                            detalleReal = new IngresoProduccionDetalleSalida()
                            {
                                NuTransaccion = nuTransaccion,
                                DtAddrow = DateTime.Now,
                                QtProducido = cantidadAProducir,
                                NuPrdcIngreso = ingreso.Id,
                                QtNotificado = 0,
                                Identificador = lote,
                                Faixa = stock.Faixa,
                                Producto = cdProducto,
                                Empresa = empresa,
                                NuPrdcIngresoTeorico = detalleTeorico,
                                DtVencimiento = stock.Vencimiento,
                                NdMotivo = motivo
                            };

                            uow.IngresoProduccionRepository.AddDetalleSalidaReal(detalleReal);
                        }
                        else
                        {
                            detalleReal.QtProducido = (detalleReal.QtProducido ?? 0) + cantidadAProducir;
                            detalleReal.NuTransaccion = nuTransaccion;
                            detalleReal.DtVencimiento = stock.Vencimiento;
                            detalleReal.NdMotivo = motivo;

                            uow.IngresoProduccionRepository.UpdateDetalleSalidaProduccion(detalleReal);
                        }

                        var detalleProducido = new SalidaProduccionDetalle()
                        {
                            NuTransaccion = nuTransaccion,
                            FechaAlta = DateTime.Now,
                            Cantidad = cantidadAProducir,
                            NuPrdcIngreso = ingreso.Id,
                            Identificador = lote,
                            Faixa = stock.Faixa,
                            Producto = cdProducto,
                            Empresa = empresa,
                            Ubicacion = ubicacionProduccion,
                            Vencimiento = stock.Vencimiento,
                            Motivo = motivo,
                            FlPendienteNotificar = "S",
                        };

                        uow.IngresoProduccionRepository.AddDetalleSalidaProducido(detalleProducido);
                    }
                    uow.SaveChanges();

                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("General_Db_Success_Update");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.Error(ex, ex.Message);
                uow.Rollback();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                uow.Rollback();
                throw;
            }
            finally
            {
                _concurrencyControl.DeleteTransaccion(transactionTO);

                if (_taskQueue.IsEnabled() && keysAjuste.Any())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keysAjuste);
            }
        }

        public virtual void FinalizarProduccion(Form form, FormSubmitContext context)
        {
            string nuIngresoProduccion = context.GetParameter("nuIngresoProduccion");

            using var uow = _uowFactory.GetUnitOfWork();

            try
            {
                ILogicaProduccion logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

                var diferencia = false;
                var diferenciaProducido = false;
                var diferenciaConsumo = false;
                var remanenteProduccion = false;
                var remanenteInsumos = false;

                var ingreso = logicaProduccion.GetIngresoProduccion();
                var ubicacionProduccion = uow.ProduccionRepository.GetUbicacionProduccion(ingreso.IdEspacioProducion);

                if (logicaProduccion.HayDiferenciasEnProduccion())
                {
                    diferencia = true;
                    diferenciaProducido = true;

                }
                if (uow.IngresoProduccionRepository.HayDiferenciasEnInsumosConsumidos(nuIngresoProduccion))
                {
                    diferencia = true;
                    diferenciaConsumo = true;
                }
                if (uow.IngresoProduccionRepository.HayStockProducidoEnEspacio(ubicacionProduccion))
                {
                    diferencia = true;
                    remanenteProduccion = true;
                }
                if (uow.IngresoProduccionRepository.HayStockInsumosEnEspacio(nuIngresoProduccion))
                {
                    diferencia = true;
                    remanenteInsumos = true;
                }

                if (diferencia)
                {
                    context.Parameters.Add(new WIS.Components.Common.ComponentParameter() { Id = "CONFIRMAR_FINALIZAR_PRODUCCION", Value = "S" });
                    context.AddParameter("DIFERENCIA_PRODUCIDO", diferenciaProducido.ToString());
                    context.AddParameter("DIFERENCIA_CONSUMO", diferenciaConsumo.ToString());
                    context.AddParameter("REEMANENTE_PRODUCCCION", remanenteProduccion.ToString());
                    context.AddParameter("REMANENTE_INSUMOS", remanenteInsumos.ToString());

                    return;
                }
                else
                {
                    context.Parameters.Add(new WIS.Components.Common.ComponentParameter() { Id = "CONFIRMAR_FINALIZAR_PRODUCCION", Value = "N" });
                    ConfirmarFinalizarProduccion(form, context);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw;
            }
        }

        public virtual void NotificarProduccion(Form form, FormSubmitContext context)
        {
            string nuIngresoProduccion = context.GetParameter("nuIngresoProduccion");

            if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", nuIngresoProduccion, true))
                throw new Exception("General_msg_Error_ProduccionBloqueada");

            var transaction = _concurrencyControl.CreateTransaccion();
            using var uow = _uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                ILogicaProduccion logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

                _concurrencyControl.AddLock("T_PRDC_INGRESO", nuIngresoProduccion, transaction, true);

                if (logicaProduccion.ProduccionEnProcesoDeNotificacion())
                    throw new Exception("PRD113_grid1_Error_NotificacionEnProceso");

                if (!logicaProduccion.ProduccionHabilitadaParaNotificar())
                    throw new Exception("PRD113_grid1_Error_ProduccionEnEstadoIncorrecto");

                if (!logicaProduccion.HayPendientesDeNotificacion())
                    throw new Exception("PRD113_grid1_Error_NoHayPendientesDeNotificar");

                uow.CreateTransactionNumber("Notificar producción");

                logicaProduccion.UpdateSituacion(SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_PARCIAL);

                uow.SaveChanges();
                uow.Commit();

                if (_taskQueue.IsEnabled())
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.ConfirmacionProduccion, nuIngresoProduccion);

                context.AddSuccessNotification("PRD113_grid1_Msg_ProduccionNotificada");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                _logger.Error(ex, ex.Message);
                throw ex;
            }
            finally
            {
                _concurrencyControl.DeleteTransaccion(transaction);
            }
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form, FormInitializeContext context, IngresoProduccion ingreso)
        {
            var modalidadLoteSelect = form.GetField("idModalidadLoteProduccion");
            modalidadLoteSelect.Options = new List<SelectOption>();

            var dominios = uow.DominioRepository.GetDominios(CodigoDominioDb.ProduccionModalidadIngresoLote);

            foreach (var dominio in dominios.Where(x => x.Valor != CTipoIngresoModalidadLote.PRODUCTO_ANCLA))
            {
                modalidadLoteSelect.Options.Add(new SelectOption(dominio.Valor, $"{dominio.Valor} - {dominio.Descripcion}"));
            }

            var lote = form.GetField("loteUtilizar");
            lote.ReadOnly = true;

            if (!string.IsNullOrEmpty(ingreso.IdModalidadLote))
            {
                modalidadLoteSelect.Value = ingreso.IdModalidadLote;
                modalidadLoteSelect.ReadOnly = true;
                modalidadLoteSelect.Disabled = true;

                switch (ingreso.IdModalidadLote)
                {
                    case CTipoIngresoModalidadLote.ID_INTERNO:
                        lote.Value = ingreso.Id;
                        break;

                    case CTipoIngresoModalidadLote.ID_EXTERNO:
                        lote.Value = ingreso.IdProduccionExterno;
                        break;

                    case CTipoIngresoModalidadLote.FECHA_DIA:
                        lote.Value = DateTime.Now.ToString("d", this._identity.GetFormatProvider());
                        break;

                    case CTipoIngresoModalidadLote.MES_PROD:
                        var mesProduccion = ingreso.FechaAlta.Value.Month;
                        lote.Value = mesProduccion.ToString();
                        break;
                    default:
                        lote.Value = string.Empty;
                        lote.ReadOnly = false;
                        break;
                }

                context.Parameters.Add(new WIS.Components.Common.ComponentParameter() { Id = "loteUtilizar", Value = lote.Value });
                context.Parameters.Add(new WIS.Components.Common.ComponentParameter() { Id = "idModalidadLoteProduccion", Value = ingreso.IdModalidadLote });
            }
        }

        #endregion
    }
}
