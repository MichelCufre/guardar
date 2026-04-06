using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Produccion.Modules.Forms;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Logic;
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
    public class PRD113ProductosNoEsperados : AppController
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

        public PRD113ProductosNoEsperados(IIdentityService identity, ITaskQueueService taskQueue, ITrafficOfficerService concurrencyControl, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFormValidationService formValidationService, IFilterInterpreter filterInterpreter, ILogicaProduccionFactory logicaProduccionFactory)
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
                var idIngreso = context.GetParameter("idIngresoProduccion");
                var empresaIngreso = uow.IngresoProduccionRepository.GetIngresoByIdConDetalles(idIngreso).Empresa;

                form.GetField("empresa").Value = empresaIngreso.ToString();
                form.GetField("empresa").ReadOnly = true;

                var motivoParam = uow.ParametroRepository.GetParameter(ParamManager.PRODUCCION_MOT_PROD_DEFAULT);

                List<SelectOption> opciones = new List<SelectOption>();
                var motivos = uow.DominioRepository.GetDominios(TipoIngresoProduccion.MOTIVO_PRODUCCION);

                foreach (var motivo in motivos)
                {
                    opciones.Add(new SelectOption(motivo.Id, motivo.Descripcion));
                }

                form.GetField("motivo").Options = opciones;
                form.GetField("motivo").Value = !string.IsNullOrEmpty(motivoParam) ? motivoParam : string.Empty;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
                throw;
            }

            return form;
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext query)
        {
            switch (query.FieldId)
            {
                case "producto": return this.SearchProducto(form, query);
                default: return new List<SelectOption>();
            }
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new PRD113ProudctosNoEsperadosFromValidationModule(uow, this._identity.GetFormatProvider()), form, context);
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
                var loteParam = context.GetParameter("lote")?.Trim()?.ToUpper();
                var idModalidadLote = context.GetParameter("modalidadLote");

                decimal faixa = 1;
                var lote = string.Empty;
                var cdProducto = form.GetField("producto").Value;
                var cdEmpresa = int.Parse(form.GetField("empresa").Value);

                if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", nuIngresoProduccion, true))
                    throw new EntityLockedException("General_msg_Error_ProduccionBloqueada");

                _concurrencyControl.AddLock("T_PRDC_INGRESO", nuIngresoProduccion, transactionTO, true);

                var ingreso = uow.IngresoProduccionRepository.GetIngresoById(nuIngresoProduccion);

                if (ingreso == null)
                    throw new ValidationFailedException("General_Sec0_Error_ProduccionNotFound");

                if (ingreso.Situacion == SituacionDb.PRODUCCION_FINALIZADA || ingreso.Situacion == SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL)
                    throw new ValidationFailedException("PRD113_grid1_Error_ProduccionEnEstadoIncorrecto");

                var logicaProduccion = (LogicaProduccionBlackBox)_logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);
                var espacioProdicion = logicaProduccion.GetEspacioProduccion();
                var producto = uow.ProductoRepository.GetProducto(cdEmpresa, cdProducto);

                if (producto == null)
                    throw new ValidationFailedException("PRD113_grid1_Error_ProductoNoExiste");

                if (logicaProduccion.EsProductoEsperado(producto.CodigoEmpresa, producto.Codigo, CIngresoProduccionDetalleTeorico.TipoDetalleSalida))
                    throw new ValidationFailedException("PRD113_grid1_Error_ProductoNoEsperadoEsEsperado");

                if (producto.IsIdentifiedByProducto())
                    lote = "*";
                else
                    lote = loteParam;

                if (uow.StockRepository.ExisteSerie(cdProducto, cdEmpresa, lote))
                    throw new ValidationFailedException("General_Sec0_Error_SerieYaExiste", new string[] { lote, cdProducto, cdEmpresa.ToString() });

                var idBloqueo = $"{espacioProdicion.IdUbicacionProduccion}#{producto.CodigoEmpresa}#{producto.Codigo}#{faixa.ToString(_identity.GetFormatProvider())}#{lote}";

                if (this._concurrencyControl.IsLocked("T_STOCK", idBloqueo, true))
                    throw new EntityLockedException("General_Sec1_Error_StockBloqueado");

                this._concurrencyControl.AddLock("T_STOCK", idBloqueo, transactionTO, true);

                DateTime? vencimiento = null;

                if (producto.IsFifo())
                    vencimiento = DateTime.Now;
                else if (producto.IsFefo())
                {
                    if (string.IsNullOrEmpty(form.GetField("fechaVencimiento").Value))
                        throw new ValidationFailedException("PRD113_grid1_Error_VencimientoRequerido");
                    else if (DateTime.TryParse(form.GetField("fechaVencimiento").Value, this._identity.GetFormatProvider(), DateTimeStyles.None, out DateTime fecha))
                        vencimiento = fecha;
                }

                if (string.IsNullOrEmpty(form.GetField("cantidad").Value))
                    throw new ValidationFailedException("PRD113_grid1_Error_CantidadRequerido");

                uow.CreateTransactionNumber("FormSubmit - Agregar producto no esperado");

                ingreso.IdModalidadLote = idModalidadLote;
                ingreso.NuTransaccion = uow.GetTransactionNumber();
                ingreso.Situacion = SituacionDb.PRODUCIENDO;

                uow.IngresoProduccionRepository.UpdateIngresoProduccion(ingreso);

                decimal.TryParse(form.GetField("cantidad").Value, NumberStyles.Number, _identity.GetFormatProvider(), out decimal cantidadProductoNoEsperado);

                logicaProduccion.GenerarProductoNoEsperado(producto, faixa, cdEmpresa, lote, cantidadProductoNoEsperado, vencimiento, form.GetField("motivo").Value, form.GetField("dsMotivo").Value, out keyAjuste);

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("PRD113_grid1_Msg_ConfirmacionProductoNoEsperado");
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

                if (_taskQueue.IsEnabled() && !string.IsNullOrEmpty(keyAjuste))
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keyAjuste);
            }

            return form;
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchProducto(Form form, FormSelectSearchContext FormQuery)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            List<SelectOption> opciones = new List<SelectOption>();

            if (!string.IsNullOrEmpty(form.GetField("empresa").Value))
            {
                var empresa = int.Parse(form.GetField("empresa").Value);

                List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, FormQuery.SearchValue);

                foreach (Producto producto in productos)
                {
                    opciones.Add(new SelectOption(producto.Codigo, producto.Codigo + " - " + producto.Descripcion));
                }
            }

            return opciones;
        }

        #endregion
    }
}
