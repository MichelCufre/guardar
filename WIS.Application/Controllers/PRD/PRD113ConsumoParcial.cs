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
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent.Build;
using WIS.GridComponent.Excel;
using WIS.Security;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD113ConsumoParcial : AppController
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
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();


        public PRD113ConsumoParcial(IIdentityService identity, ITaskQueueService taskQueue, ITrafficOfficerService concurrencyControl, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFormValidationService formValidationService, IFilterInterpreter filterInterpreter, ILogicaProduccionFactory logicaProduccionFactory)
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
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            try
            {
                var nuIngresoProduccion = context.GetParameter("idIngresoProduccion");
                var esConsumible = context.GetParameter("esConsumible") == "S";
                var nuInsumo = long.Parse(context.GetParameter("idInsumoProduccion"));

                var ingreso = uow.IngresoProduccionRepository.GetIngresoById(nuIngresoProduccion);

                if (ingreso == null)
                    throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

                var codigoProducto = context.GetParameter("codigoProducto");
                var producto = uow.ProductoRepository.GetProducto(ingreso.Empresa.Value, codigoProducto);

                form.GetField("producto").Value = producto.Codigo;
                form.GetField("descProducto").Value = producto.Descripcion;
                form.GetField("identificador").Value = context.GetParameter("numeroIdentificador");
                form.GetField("cantidadReal").Value = context.GetParameter("cantidadReal");
                form.GetField("consumido").Value = string.Empty;

                var logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

                if (!esConsumible)
                {
                    var insumoReal = logicaProduccion.GetInsumoProduccion(nuInsumo);
                    form.GetField("consumido").Value = insumoReal?.QtDesafectado?.ToString(this._identity.GetFormatProvider());
                }

                List<SelectOption> colMotivos = new List<SelectOption>();

                var motivos = uow.DominioRepository.GetDominios(TipoIngresoProduccion.MOTIVO_CONSUMO)
                    .Where(d => !esConsumible ? d.Id != TipoIngresoProduccion.MOT_CONS_ADS : true);

                foreach (var motivo in motivos)
                {
                    colMotivos.Add(new SelectOption(motivo.Id, motivo.Descripcion));
                }

                var motivoParam = uow.ParametroRepository.GetParameter(ParamManager.PRODUCCION_MOT_CONS_DEFAULT);
                var motivoDefault = uow.DominioRepository.GetDominio(motivoParam);

                form.GetField("motivo").Value = motivoDefault.Id;
                form.GetField("motivo").Options = colMotivos;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw ex;
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PRD113ConsumoParcialFromValidationModule(uow, this._identity.GetFormatProvider()), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var keyAjuste = string.Empty;
            var transactionTO = _concurrencyControl.CreateTransaccion();

            uow.BeginTransaction();

            try
            {
                var nuIngresoProduccion = context.GetParameter("idIngresoProduccion");
                var nuInsumo = long.Parse(context.GetParameter("idInsumoProduccion"));

                if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", nuIngresoProduccion, true))
                    throw new EntityLockedException("General_msg_Error_ProduccionBloqueada");

                _concurrencyControl.AddLock("T_PRDC_INGRESO", nuIngresoProduccion, transactionTO, true);

                var ingreso = uow.IngresoProduccionRepository.GetIngresoById(nuIngresoProduccion);

                if (ingreso == null)
                    throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

                var logicaProduccion = _logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

                if (!logicaProduccion.ProduccionHabilitadaParaFabricar())
                    throw new ValidationFailedException("PRD113_grid1_Error_ProduccionEnEstadoIncorrecto");

                var codigoProducto = context.GetParameter("codigoProducto");
                var nuIdentificador = context.GetParameter("numeroIdentificador");
                var esConsumible = context.GetParameter("esConsumible") == "S";
                var cantidadReal = decimal.Parse(context.GetParameter("cantidadReal") ?? "0", this._identity.GetFormatProvider());

                decimal faixa = 1;
                var empresa = logicaProduccion.GetEmpresa();
                var motivo = form.GetField("motivo").Value;
                var isSobrante = form.GetField("isSobrante").Value == "true";
                var ubicacionProduccion = logicaProduccion.GetEspacioProduccion().IdUbicacionProduccion;
                var cantidadConsumir = decimal.Parse(form.GetField("consumo").Value, this._identity.GetFormatProvider());

                var idBloqueo = $"{ubicacionProduccion}#{empresa}#{codigoProducto}#{faixa.ToString(_identity.GetFormatProvider())}#{nuIdentificador}";

                if (this._concurrencyControl.IsLocked("T_STOCK", idBloqueo, true))
                    throw new EntityLockedException("General_Sec1_Error_StockBloqueado");

                this._concurrencyControl.AddLock("T_STOCK", idBloqueo, transactionTO, true);

                uow.CreateTransactionNumber("Consumo parcial producción");

                ingreso.Situacion = SituacionDb.PRODUCIENDO;
                ingreso.NuTransaccion = uow.GetTransactionNumber();
                uow.IngresoProduccionRepository.UpdateIngresoProduccion(ingreso);
                uow.SaveChanges();

                if (motivo == TipoIngresoProduccion.MOT_CONS_ADS)
                {
                    cantidadConsumir = GetCantidadConsumir(cantidadReal, isSobrante, cantidadConsumir);
                    keyAjuste = ProcesarBajaStock(uow, logicaProduccion, nuIngresoProduccion, ubicacionProduccion, empresa, codigoProducto, faixa, nuIdentificador, cantidadConsumir);
                }
                else
                    ConsumirParcial(uow, logicaProduccion, nuIngresoProduccion, nuInsumo, codigoProducto, nuIdentificador, cantidadReal, cantidadConsumir, esConsumible, motivo, isSobrante);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("PRD113_grid1_Msg_ConsumoRegistrado");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                _logger.Error(ex, ex.Message);
                uow.Rollback();
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                _logger.Error(ex, ex.Message);
                throw;
            }
            finally
            {
                _concurrencyControl.DeleteTransaccion(transactionTO);

                if (_taskQueue.IsEnabled() && !string.IsNullOrEmpty(keyAjuste))
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keyAjuste);
            }

            return form;
        }

        public virtual void ConsumirParcial(IUnitOfWork uow, ILogicaProduccion logicaProduccion, string nuIngresoProduccion, long nuInsumo, string codigoProducto, string nuIdentificador, decimal cantidadReal, decimal cantidadConsumir, bool esConsumible, string motivo, bool isSobrante)
        {
            var detalleInsumo = new IngresoProduccionDetalleReal();

            if (!esConsumible)
            {
                detalleInsumo = logicaProduccion.GetInsumoProduccion(nuInsumo);
                cantidadConsumir = GetCantidadConsumir(detalleInsumo.QtReal ?? 0, isSobrante, cantidadConsumir);
            }
            else
            {
                detalleInsumo = logicaProduccion.ExisteIngresoReal(codigoProducto, nuIdentificador);

                if (detalleInsumo == null)
                {
                    detalleInsumo = new IngresoProduccionDetalleReal()
                    {
                        Empresa = logicaProduccion.GetEmpresa(),
                        Faixa = 1,
                        Producto = codigoProducto,
                        Identificador = nuIdentificador,
                        QtNotificado = 0,
                        NuPrdcIngreso = nuIngresoProduccion,
                        NuOrden = uow.IngresoProduccionRepository.GetNextValueNuOrdenDetalleReal(nuIngresoProduccion),
                    };

                    cantidadConsumir = GetCantidadConsumir(cantidadReal, isSobrante, cantidadConsumir);

                    detalleInsumo.QtReal = cantidadConsumir;

                    logicaProduccion.AddInsumoProduccion(detalleInsumo);

                    uow.SaveChanges();
                }
                else
                {
                    cantidadConsumir = GetCantidadConsumir(cantidadReal, isSobrante, cantidadConsumir);

                    detalleInsumo.QtReal = (detalleInsumo.QtReal ?? 0) + cantidadConsumir;
                }
                nuInsumo = detalleInsumo.NuPrdcIngresoReal;
            }

            logicaProduccion.ConsumirInsumoParcial(nuInsumo, logicaProduccion.GetEspacioProduccion().IdUbicacionProduccion, cantidadConsumir, out DateTime? vencimiento, esConsumible);

            var insumo = new IngresoProduccionDetalle()
            {
                Empresa = logicaProduccion.GetEmpresa(),
                Faixa = 1,
                Producto = detalleInsumo.Producto,
                Identificador = detalleInsumo.Identificador,
                Cantidad = cantidadConsumir,
                NuPrdcIngreso = nuIngresoProduccion,
                FechaAlta = DateTime.Now,
                NuTransaccion = uow.GetTransactionNumber(),
                Ubicacion = logicaProduccion.GetEspacioProduccion().IdUbicacionProduccion,
                Vencimiento = vencimiento,
                Motivo = motivo,
                FlPendienteNotificar = "S",
                NuPrdcIngresoReal = nuInsumo
            };

            uow.IngresoProduccionRepository.AddMovimientoIngreso(insumo);

            uow.SaveChanges();
        }

        public virtual string ProcesarBajaStock(IUnitOfWork uow, ILogicaProduccion logicaProduccion, string nuIngresoProduccion, string ubicacion, int empresa, string producto, decimal faixa, string identificador, decimal cantidadConsumir)
        {
            var stock = uow.StockRepository.GetStock(empresa, producto, faixa, ubicacion, identificador);

            if (stock == null || stock.Cantidad < cantidadConsumir || cantidadConsumir <= 0)
                throw new ValidationFailedException("PRD113_grid1_Error_InsumoSinSaldo");

            if (!uow.IngresoProduccionRepository.StockInsumoConsumible(nuIngresoProduccion, stock.Producto, stock.Empresa, stock.Faixa, stock.Identificador))
                throw new ValidationFailedException("PRD113_grid1_Error_InsumoSinSaldo");

            var nuevoSaldo = (stock.Cantidad - cantidadConsumir);
            stock.Cantidad = nuevoSaldo < 0 ? 0 : nuevoSaldo;
            stock.NumeroTransaccion = uow.GetTransactionNumber();
            stock.FechaModificacion = DateTime.Now;

            uow.StockRepository.UpdateStock(stock);
            uow.SaveChanges();

            var ajuste = new AjusteStock
            {
                NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                Ubicacion = stock.Ubicacion,
                Empresa = stock.Empresa,
                Producto = stock.Producto,
                Faixa = stock.Faixa,
                Identificador = stock.Identificador,
                QtMovimiento = (-1) * cantidadConsumir,
                FechaVencimiento = stock.Vencimiento,
                FechaRealizado = DateTime.Now,
                TipoAjuste = TipoAjusteDb.Stock,
                CdMotivoAjuste = MotivoAjusteDb.Produccion,
                DescMotivo = $"Consumo para el Ingreso Nro: {nuIngresoProduccion}",
                NuTransaccion = uow.GetTransactionNumber(),
                Predio = logicaProduccion.GetPredio(),
                IdAreaAveria = "N",
                FechaMotivo = DateTime.Now,
                Funcionario = _identity.UserId,
                Aplicacion = _identity.Application,
                Metadata = nuIngresoProduccion,
            };

            uow.AjusteRepository.Add(ajuste);
            uow.SaveChanges();

            return ajuste.NuAjusteStock.ToString();
        }

        public virtual decimal GetCantidadConsumir(decimal cantidadReal, bool isSobrante, decimal cantidadConsumir)
        {
            if ((cantidadReal - cantidadConsumir) < 0)
                throw new ValidationFailedException("PRD113_grid1_Error_ConsumoMayorReal");

            if (isSobrante)
                cantidadConsumir = (cantidadReal - cantidadConsumir);
            return cantidadConsumir;
        }
    }
}
