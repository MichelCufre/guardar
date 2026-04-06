using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Recepcion;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC410SelecPos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC410ClasificacionDeRecepciones> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IAlmacenamientoService _almacenamientoService;
        protected readonly IBarcodeService _barcodeService;
        protected readonly ITaskQueueService _taskQueue;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC410SelecPos(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC410ClasificacionDeRecepciones> logger,
            IGridValidationService gridValidationService,
            IFormValidationService formValidationService,
            IAlmacenamientoService almacenamientoService,
            IBarcodeService barcodeeService,
            ITaskQueueService taskQueue)
        {
            this.GridKeys = new List<string>
            {
                "CD_EQUIPO",
                "NU_POSICION",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_POSICION", SortDirection.Descending),
                new SortCommand("CD_EQUIPO", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._formValidationService = formValidationService;
            this._almacenamientoService = almacenamientoService;
            this._barcodeService = barcodeeService;
            this._taskQueue = taskQueue;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cdEstacion = int.Parse(context.GetParameter("estacion"));
            var estacion = uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(cdEstacion);

            GetDestinoZona(context, uow, out string destino, out string zona);

            SugerenciasDeClasificacionQuery dbQuery = new SugerenciasDeClasificacionQuery(estacion, destino, zona);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            var totalSugerencias = uow.MesaDeClasificacionRepository.GetTotalSugerenciasDeClasificacionExcluyendoEquiposVacios(estacion, destino, zona);

            context.Parameters.RemoveAll(p => p.Id == "equipoNuevo");
            context.AddParameter("equipoNuevo", (totalSugerencias == 0).ToString().ToLower());

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cdEstacion = int.Parse(context.GetParameter("estacion"));
            var estacion = uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(cdEstacion);

            GetDestinoZona(context, uow, out string destino, out string zona);

            SugerenciasDeClasificacionQuery dbQuery = new SugerenciasDeClasificacionQuery(estacion, destino, zona);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cdEstacion = int.Parse(context.GetParameter("estacion"));
            var estacion = uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(cdEstacion);

            GetDestinoZona(context, uow, out string destino, out string zona);

            SugerenciasDeClasificacionQuery dbQuery = new SugerenciasDeClasificacionQuery(estacion, destino, zona);

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            foreach (var field in form.Fields)
            {
                field.ReadOnly = true;
            }

            try
            {
                form.GetField("estacion").Value = context.GetParameter("estacion");
                form.GetField("codigoProducto").Value = context.GetParameter("codigoProducto");
                form.GetField("descripcionProducto").Value = context.GetParameter("descripcionProducto");
                form.GetField("cantidadOriginal").Value = context.GetParameter("cantidadOriginal");
                form.GetField("cantidadSeparar").Value = context.GetParameter("cantidadSeparar");
                form.GetField("lote").Value = context.GetParameter("lote");
                form.GetField("vencimiento").Value = context.GetParameter("vencimiento");

                if (string.IsNullOrEmpty(form.GetField("cantidadSeparar").Value))
                {
                    form.GetField("posicionEquipo").Value = "";
                }

                var reabastecer = bool.Parse(context.GetParameter("reabastecer"));
                var sugerenciaJson = context.GetParameter("sugerencia");

                if (!string.IsNullOrEmpty(sugerenciaJson))
                {
                    RecomponerSugerencia(form, context, uow, reabastecer, sugerenciaJson);
                }
                else
                {
                    form.Fields.Find(f => f.Id == "cantidadSeparar").Value = context.GetParameter("cantidadSeparar");
                    form.Fields.Find(f => f.Id == "cantidadClasificada").Value = context.GetParameter("cantidadClasificada");

                    context.Parameters.Add(new ComponentParameter() { Id = "focusField", Value = "cantidadSeparar" });
                    context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "SIGUIENTE" });
                    context.Parameters.Add(new ComponentParameter() { Id = "isReabastecer", Value = reabastecer.ToString().ToLower() });
                }
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, ex.Message);
                context.AddErrorNotification("REC410_Sec0_Error_COL01_ConfirmarOperacion");
            }
            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var cantidadSeparar = form.GetField("cantidadSeparar").Value;
            var nuExterno = context.GetParameter("etiqueta");
            var tpEtiqueta = context.GetParameter("tipoEtiqueta");
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tpEtiqueta, nuExterno);
            var producto = form.GetField("codigoProducto").Value;
            var lote = form.GetField("lote").Value;
            var posicionEquipo = form.GetField("posicionEquipo").Value;
            var sugerenciaJson = context.GetParameter("sugerencia");
            var sugerencia = (SugerenciaAlmacenamiento)null;
            var focus = context.Parameters.Find(x => x.Id == "Focus").Value;
            var cdEmpresa = int.Parse(context.GetParameter("cdEmpresa"));
            var faixa = decimal.Parse(context.GetParameter("faixa"), this._identity.GetFormatProvider());

            if (!string.IsNullOrEmpty(sugerenciaJson))
            {
                sugerencia = JsonSerializer.Deserialize<SugerenciaAlmacenamiento>(sugerenciaJson);
            }

            if (context.ButtonId == "btnCerrar")
            {
                if (sugerencia != null)
                {
                    bool isReabastecer = bool.Parse(context.GetParameter("isReabastecer"));
                    CancelarSugerencia(uow, cantidadSeparar, etiqueta, producto, cdEmpresa, faixa, lote, sugerencia, isReabastecer);
                }
            }
            else if ((context.ButtonId == "btnConfirmar" && (focus == "posicionEquipo" && !string.IsNullOrEmpty(posicionEquipo)))
                || (focus == "posicionEquipo" && !string.IsNullOrEmpty(posicionEquipo)))
            {
                var cantidad = decimal.Parse(cantidadSeparar, this._identity.GetFormatProvider());

                ConfirmarClasificacion(form, context, uow, etiqueta, producto, lote, cantidad, sugerencia, posicionEquipo, out string key);

                if (_taskQueue.IsEnabled() && !string.IsNullOrEmpty(key))
                    _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, key);
            }
            else
            {
                ConfirmarCampo(form, context, uow, etiqueta, producto, lote, sugerencia);
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.ButtonId == "btnCerrar")
                return form;

            var culture = this._identity.GetFormatProvider();
            var cantidadOriginal = decimal.Parse(form.GetField("cantidadOriginal").Value, culture);
            var cantidadClasificada = decimal.Parse(form.GetField("cantidadClasificada").Value, culture);
            var cantidadPendiente = cantidadOriginal - cantidadClasificada;

            return this._formValidationService.Validate(new REC410SelecPosFormValidationModule(uow, this._barcodeService, this._identity.GetFormatProvider(), cantidadPendiente), form, context);
        }

        #region Metodos Auxiliares

        public virtual void ConfirmarCampo(Form form, FormSubmitContext context, IUnitOfWork uow, EtiquetaLote etiqueta, string producto, string lote, SugerenciaAlmacenamiento sugerencia)
        {
            var focus = context.Parameters.Find(x => x.Id == "Focus").Value;
            var valor = form.GetField(focus).Value.ToUpper();

            try
            {
                switch (focus)
                {
                    case "cantidadSeparar":
                        ConfirmarCantidadSeparar(form, context, uow, valor, etiqueta, producto, lote, sugerencia);
                        break;
                    case "posicionEquipo":
                        ConfirmarPosicionEquipo(form, context, uow, valor, etiqueta, producto, lote, sugerencia);
                        break;
                    default:
                        // code block
                        break;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, ex.Message);
                context.AddErrorNotification("REC410_Sec0_Error_COL01_ConfirmarOperacion");
            }
        }

        public virtual void ConfirmarCantidadSeparar(Form form, FormSubmitContext context, IUnitOfWork uow, string valor, EtiquetaLote etiqueta, string producto, string lote, SugerenciaAlmacenamiento sugerencia)
        {
            if (string.IsNullOrEmpty(valor))
            {
                form.GetField("destino").Value = "";
                form.GetField("descripcionZona").Value = "";
                form.GetField("codigoZona").Value = "";
                form.GetField("posicionEquipo").Value = "";
            }
            else
            {
                SugerirUbicacion(form, context, uow, valor, etiqueta, producto, lote, sugerencia);
                context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "SIGUIENTE" });
            }
        }

        public virtual void ConfirmarPosicionEquipo(Form form, FormSubmitContext context, IUnitOfWork uow, string valor, EtiquetaLote etiqueta, string producto, string lote, SugerenciaAlmacenamiento sugerencia)
        {
            if (string.IsNullOrEmpty(valor))
            {
                var strCantidadSeparar = form.GetField("cantidadSeparar").Value;

                var cdEmpresa = int.Parse(context.GetParameter("cdEmpresa"));
                var faixa = decimal.Parse(context.GetParameter("faixa"), this._identity.GetFormatProvider());

                if (sugerencia != null)
                {
                    bool isReabastecer = bool.Parse(context.GetParameter("isReabastecer"));
                    CancelarSugerencia(uow, strCantidadSeparar, etiqueta, producto, cdEmpresa, faixa, lote, sugerencia, isReabastecer);
                }

                var culture = this._identity.GetFormatProvider();
                var cantidadOriginal = decimal.Parse(form.GetField("cantidadOriginal").Value, culture);
                var cantidadClasificada = decimal.Parse(form.GetField("cantidadClasificada").Value, culture);

                form.GetField("cantidadSeparar").Value = (cantidadOriginal - cantidadClasificada).ToString(culture);
                form.GetField("destino").Value = "";
                form.GetField("descripcionZona").Value = "";
                form.GetField("codigoZona").Value = "";
                form.GetField("posicionEquipo").Value = "";

                context.Parameters.RemoveAll(p => p.Id == "sugerencia");
                context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "ANTERIOR" });
            }
        }

        protected virtual void ConfirmarClasificacion(Form form, FormSubmitContext context, IUnitOfWork uow, EtiquetaLote etiquetaLote, string producto, string lote, decimal cantidad, SugerenciaAlmacenamiento sugerencia, string posicionEquipo, out string keyAjuste)
        {
            keyAjuste = string.Empty;

            var zona = form.GetField("codigoZona").Value;
            var cdEstacion = int.Parse(form.GetField("estacion").Value);
            var estacion = uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(cdEstacion);
            var etiquetaPosicionEquipo = this._barcodeService.GetEtiquetaPosicionEquipo(posicionEquipo);
            var agenda = uow.AgendaRepository.GetAgenda(etiquetaLote.NumeroAgenda);
            var empresa = agenda.IdEmpresa;
            var detallesEtiquetaLote = uow.EtiquetaLoteRepository.GetDetalles(etiquetaLote.Numero);
            var faixa = detallesEtiquetaLote
                .FirstOrDefault(d => d.CodigoProducto == producto
                    && d.Identificador == lote)?.Faixa ?? 1;

            var strVencimiento = form.GetField("vencimiento").Value;
            var vencimiento = (DateTime?)null;

            if (!string.IsNullOrEmpty(strVencimiento))
            {
                vencimiento = DateTime.Parse(strVencimiento, _identity.GetFormatProvider());
            }

            vencimiento = vencimiento ?? detallesEtiquetaLote
                .FirstOrDefault(d => d.CodigoProducto == producto
                    && d.Identificador == lote)?.Vencimiento;

            uow.CreateTransactionNumber("ConfirmarClasificacion");
            uow.BeginTransaction();

            try
            {
                var equipo = ActualizarEquipo(uow, estacion, etiquetaPosicionEquipo, zona);

                ActualizarEtiquetaLote(uow, etiquetaLote, detallesEtiquetaLote, empresa, producto, faixa, lote, cantidad, vencimiento, equipo, out int nuLogEtiqueta);

                ActualizarEtiquetaPosicionEquipo(uow, etiquetaLote.Numero, empresa, producto, faixa, lote, cantidad, vencimiento, sugerencia, etiquetaPosicionEquipo, equipo, nuLogEtiqueta);

                GenerarAjustesStock(uow, estacion, etiquetaLote.Numero, detallesEtiquetaLote, empresa, producto, faixa, lote, cantidad, vencimiento, equipo, out keyAjuste);

                MovilizarStock(uow, estacion, empresa, producto, faixa, lote, cantidad, vencimiento, equipo);

                bool isReabastecer = bool.Parse(context.GetParameter("isReabastecer"));

                if (sugerencia != null)
                    AprobarSugerenciaAlmacenamiento(uow, etiquetaLote, producto, empresa, faixa, lote, cantidad, vencimiento, sugerencia, isReabastecer);

                BlanquearCampos(form, context);

                uow.SaveChanges();

                var cantidadOriginal = decimal.Parse(form.GetField("cantidadOriginal").Value, this._identity.GetFormatProvider());
                var cantidadClasificada = decimal.Parse(form.GetField("cantidadClasificada").Value, this._identity.GetFormatProvider());
                var cantidadPendiente = cantidadOriginal - cantidadClasificada;

                context.Parameters.RemoveAll(p => p.Id == "cantidadSeparar");
                context.Parameters.RemoveAll(p => p.Id == "cantidadClasificada");
                context.Parameters.RemoveAll(p => p.Id == "sugerencia");


                context.Parameters.Add(new ComponentParameter() { Id = "cantidadSeparar", Value = cantidadPendiente.ToString(this._identity.GetFormatProvider()) });
                context.Parameters.Add(new ComponentParameter() { Id = "cantidadClasificada", Value = cantidadClasificada.ToString(this._identity.GetFormatProvider()) });
                context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "RESET" });

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, ex.Message);
                context.AddErrorNotification("REC410_Sec0_Error_COL01_ConfirmarOperacion");
            }
        }

        protected virtual Equipo ActualizarEquipo(IUnitOfWork uow, EstacionDeClasificacion estacion, EtiquetaPosicionEquipo etiqueta, string zona)
        {
            var equipo = uow.EquipoRepository.GetEquipo(etiqueta.Equipo);

            equipo.CodigoUbicacionReal = estacion.Ubicacion;
            equipo.TipoOperacion = TipoOperacionDb.Clasificacion;
            equipo.CodigoZona = zona;
            equipo.FechaModificacion = DateTime.Now;

            uow.EquipoRepository.UpdateEquipo(equipo);

            return equipo;
        }

        protected virtual void ActualizarEtiquetaPosicionEquipo(IUnitOfWork uow, int nuEtiquetaLote, int empresa, string producto, decimal faixa, string lote, decimal cantidad, DateTime? vencimiento, SugerenciaAlmacenamiento sugerencia, EtiquetaPosicionEquipo etiqueta, Equipo equipo, int nuLogEtiqueta)
        {
            var etiquetaTransferencia = uow.EtiquetaTransferenciaRepository.GetEtiquetaTransferencia(etiqueta.Tipo, etiqueta.NumeroExterno);

            if (etiquetaTransferencia == null || etiquetaTransferencia.Estado == SituacionDb.TransferenciaRealizada)
            {
                etiquetaTransferencia = GetNewEtiquetaTransferencia(uow, sugerencia, etiqueta, equipo);
                uow.EtiquetaTransferenciaRepository.AddEtiqueta(etiquetaTransferencia);
                uow.SaveChanges();
            }

            AgregarDetalleEtiquetaPosicionEquipo(uow, nuEtiquetaLote, etiquetaTransferencia, empresa, producto, faixa, lote, cantidad, vencimiento, equipo, nuLogEtiqueta);
        }

        protected virtual EtiquetaTransferencia GetNewEtiquetaTransferencia(IUnitOfWork uow, SugerenciaAlmacenamiento sugerencia, EtiquetaPosicionEquipo etiqueta, Equipo equipo)
        {
            return new EtiquetaTransferencia
            {
                AplicacionOrigen = this._identity.Application,
                Estado = SituacionDb.EnTransferencia,
                IdExternoEtiqueta = etiqueta.NumeroExterno,
                NumeroEtiqueta = uow.EtiquetaTransferenciaRepository.GetProximoNumeroEtiqueta(),
                NumeroSecEtiqueta = 0,
                NumeroTransaccion = uow.GetTransactionNumber(),
                Predio = equipo.Ubicacion.NumeroPredio,
                TipoEtiquetaTransferencia = etiqueta.Tipo,
                TpModalidadUso = TipoEtiquetaModalidadUso.Clasificacion,
                UbicacionDestino = sugerencia?.UbicacionSugerida,
                UbicacionReal = equipo.CodigoUbicacion,
                FechaInsercion = DateTime.Now,
                FechaModificacion = DateTime.Now,
            };
        }

        protected virtual void AgregarDetalleEtiquetaPosicionEquipo(IUnitOfWork uow, int nuEtiquetaLote, EtiquetaTransferencia etiqueta, int empresa, string producto, decimal faixa, string lote, decimal cantidad, DateTime? vencimiento, Equipo equipo, int nuLogEtiqueta)
        {
            uow.EtiquetaTransferenciaRepository.AddDetalleEtiqueta(new DetalleEtiquetaTransferencia
            {
                Averia = "N",
                AreaAveria = "N",
                ControlCalidadPendiente = "C",
                Cantidad = cantidad,
                Empresa = empresa,
                Estado = SituacionDb.EnTransferencia,
                NumeroEtiqueta = etiqueta.NumeroEtiqueta,
                Faixa = faixa,
                FechaModificacion = DateTime.Now,
                FechaRegistro = DateTime.Now,
                Funcionario = _identity.UserId,
                Identificador = lote,
                InventarioCiclico = "R",
                Metadata = nuLogEtiqueta.ToString() == "-1" ? "" : nuLogEtiqueta.ToString(),
                Producto = producto,
                NumeroSecEtiqueta = etiqueta.NumeroSecEtiqueta,
                NumeroSecDetalle = uow.EtiquetaTransferenciaRepository.GetProximoNumeroSecDetalle(etiqueta),
                Transaccion = uow.GetTransactionNumber(),
                UbicacionDestino = etiqueta.UbicacionDestino,
                UbicacionOrigen = equipo.CodigoUbicacionReal,
                Vencimiento = vencimiento,
            });
        }

        protected virtual void GenerarAjustesStock(IUnitOfWork uow, EstacionDeClasificacion estacion, int nuEtiquetaLote, List<EtiquetaLoteDetalle> detallesEtiquetaLote, int empresa, string producto, decimal faixa, string lote, decimal cantidad, DateTime? vencimiento, Equipo equipo, out string key)
        {
            key = string.Empty;
            var cantidadTeorica = detallesEtiquetaLote
                .Where(d => d.CodigoProducto == producto
                    && d.Faixa == faixa
                    && d.Identificador == lote)
                .Select(d => d.Cantidad ?? 0)
                .DefaultIfEmpty(0)
                .Sum();

            if (cantidadTeorica < cantidad)
            {
                var ajuste = new AjusteStock
                {
                    IdAreaAveria = "N",
                    Aplicacion = this._identity.Application,
                    Empresa = empresa,
                    Ubicacion = estacion.Ubicacion,
                    Faixa = faixa,
                    Funcionario = this._identity.UserId,
                    CdMotivoAjuste = MotivoAjusteDb.MesaClasificacion,
                    Producto = producto,
                    DescMotivo = "Mesa de Clasificación",
                    FechaRealizado = DateTime.Now,
                    FechaVencimiento = vencimiento,
                    Metadata = nuEtiquetaLote.ToString(),
                    NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                    Identificador = lote,
                    Predio = equipo.Ubicacion.NumeroPredio,
                    NuTransaccion = uow.GetTransactionNumber(),
                    QtMovimiento = cantidad - cantidadTeorica,
                    TipoAjuste = TipoAjusteDb.Stock,
                };
                uow.AjusteRepository.Add(ajuste);
                key = ajuste.NuAjusteStock.ToString();
            }
        }

        protected virtual void ActualizarEtiquetaLote(IUnitOfWork uow, EtiquetaLote etiqueta, List<EtiquetaLoteDetalle> detalles, int empresa, string producto, decimal faixa, string lote, decimal cantidad, DateTime? vencimiento, Equipo equipo, out int nuLogEtiqueta)
        {
            nuLogEtiqueta = -1;
            var detallesProductoLote = detalles
                .Where(d => d.IdEmpresa == empresa
                    && d.CodigoProducto == producto
                    && d.Faixa == faixa
                    && d.Identificador == lote);

            var saldoClasificacion = cantidad;

            foreach (var detalle in detallesProductoLote)
            {
                var cantidadAjuste = 0M;

                if (saldoClasificacion <= 0)
                    return;

                if ((detalle.Cantidad ?? 0) >= saldoClasificacion)
                {
                    cantidadAjuste = saldoClasificacion;
                    saldoClasificacion = 0;
                }
                else
                {
                    cantidadAjuste = detalle.Cantidad ?? 0;
                    saldoClasificacion = saldoClasificacion - (detalle.Cantidad ?? 0);
                }

                detalle.NumeroTransaccion = uow.GetTransactionNumber();

                uow.EtiquetaLoteRepository.MovilizarProductoLote(detalle, cantidadAjuste);

            }
            nuLogEtiqueta = GenerarLogEtiqueta(uow, etiqueta, empresa, producto, faixa, lote, cantidad, vencimiento, equipo);
        }

        protected virtual void MovilizarStock(IUnitOfWork uow, EstacionDeClasificacion estacion, int empresa, string producto, decimal faixa, string lote, decimal cantidad, DateTime? vencimiento, Equipo equipo)
        {
            var stockEstacion = uow.StockRepository.GetStock(empresa, producto, faixa, estacion.Ubicacion, lote);
            var stockEquipo = uow.StockRepository.GetStock(empresa, producto, faixa, equipo.CodigoUbicacion, lote);

            if (stockEstacion != null)
            {
                var cantidadAjuste = 0M;

                if ((stockEstacion.Cantidad ?? 0) >= cantidad)
                    cantidadAjuste = -cantidad;

                stockEstacion.NumeroTransaccion = uow.GetTransactionNumber();
                stockEstacion.FechaModificacion = DateTime.Now;

                uow.StockRepository.MovilizarStock(stockEstacion, cantidadAjuste);
            }

            if (stockEquipo == null)
            {
                uow.StockRepository.AddStock(new Stock
                {
                    Averia = "N",
                    Cantidad = cantidad,
                    CantidadTransitoEntrada = 0,
                    ControlCalidad = EstadoControlCalidad.Controlado,
                    Empresa = empresa,
                    Faixa = faixa,
                    FechaModificacion = DateTime.Now,
                    Identificador = lote,
                    Inventario = "R",
                    NumeroTransaccion = uow.GetTransactionNumber(),
                    Producto = producto,
                    ReservaSalida = cantidad,
                    Ubicacion = equipo.CodigoUbicacion,
                    Vencimiento = vencimiento,
                });
            }
            else
            {
                stockEquipo.NumeroTransaccion = uow.GetTransactionNumber();
                stockEquipo.Vencimiento = vencimiento ?? stockEstacion?.Vencimiento ?? stockEquipo.Vencimiento;

                uow.StockRepository.MovilizarStock(stockEquipo, cantidad);
            }
        }

        protected virtual int GenerarLogEtiqueta(IUnitOfWork uow, EtiquetaLote etiqueta, int empresa, string producto, decimal faixa, string lote, decimal cantidad, DateTime? vencimiento, Equipo equipo)
        {
            return uow.EtiquetaLoteRepository.AddLogEtiqueta(new LogEtiqueta
            {
                Aplicacion = this._identity.Application,
                Cantidad = -cantidad,
                CodigoProducto = producto,
                Empresa = empresa,
                Faixa = faixa,
                FechaOperacion = DateTime.Now,
                Funcionario = this._identity.UserId,
                Identificador = lote,
                NroTransaccion = uow.GetTransactionNumber(),
                NumeroEtiqueta = etiqueta.Numero,
                Agenda = etiqueta.NumeroAgenda,
                TipoMovimiento = TiposMovimiento.Clasificacion,
                Ubicacion = equipo.CodigoUbicacion,
                Vencimiento = vencimiento,
            });
        }

        protected virtual void AprobarSugerenciaAlmacenamiento(IUnitOfWork uow, EtiquetaLote etiqueta, string producto, int cdEmpresa, decimal? faixa, string lote, decimal cantidad, DateTime? vencimiento, SugerenciaAlmacenamiento sugerencia, bool isReabastecer)
        {
            if (!isReabastecer)
            {
                _almacenamientoService.AprobarSugerenciaParaProducto(uow, sugerencia, etiqueta.Numero, producto, null, lote, cantidad, vencimiento);
            }
            else
            {
                _almacenamientoService.AprobarSugerenciaParaReabastecimiento(uow, sugerencia, etiqueta.Numero, producto, cdEmpresa, faixa, vencimiento, lote, cantidad);
            }
        }

        protected virtual void BlanquearCampos(Form form, FormSubmitContext context)
        {
            var culture = this._identity.GetFormatProvider();
            var cantidadClasificada = decimal.Parse(form.GetField("cantidadClasificada").Value, culture);
            var cantidadSeparar = decimal.Parse(form.GetField("cantidadSeparar").Value, culture);

            context.Parameters.RemoveAll(p => p.Id == "sugerencia");

            form.GetField("cantidadClasificada").Value = (cantidadClasificada + cantidadSeparar).ToString(culture);
            form.GetField("cantidadSeparar").Value = "";
            form.GetField("destino").Value = "";
            form.GetField("descripcionZona").Value = "";
            form.GetField("codigoZona").Value = "";
            form.GetField("posicionEquipo").Value = "";
        }

        protected virtual SugerenciaAlmacenamiento SugerirUbicacion(Form form, FormSubmitContext context, IUnitOfWork uow, string cantidadSeparar, EtiquetaLote etiqueta, string producto, string lote, SugerenciaAlmacenamiento sugerencia)
        {
            uow.CreateTransactionNumber("SugerirUbicacion");
            uow.BeginTransaction();

            var reabastecer = bool.Parse(context.GetParameter("reabastecer"));
            var vencimiento = (DateTime?)null;
            var culture = this._identity.GetFormatProvider();
            var cantOriginal = decimal.Parse(context.GetParameter("cantidad"), culture);
            var cantSeparar = decimal.Parse(cantidadSeparar, culture);
            var cantClasificada = decimal.Parse(form.GetField("cantidadClasificada").Value, culture);

            if (DateTime.TryParse(form.GetField("vencimiento").Value, culture, DateTimeStyles.None, out DateTime dtVencimiento))
                vencimiento = dtVencimiento;

            if (reabastecer)
            {
                var ignorarStock = bool.Parse(context.GetParameter("ignorarStock"));
                var etiquetaLote = int.Parse(context.GetParameter("etiquetaLote"));
                var cdEmpresa = int.Parse(context.GetParameter("cdEmpresa"));
                var cdFaixa = decimal.Parse(context.GetParameter("faixa"), culture);

                sugerencia = SugerirUbicacionReabastecimiento(form, context.Parameters, uow, etiquetaLote, producto, cdEmpresa, cdFaixa, lote, vencimiento, ignorarStock, cantSeparar, cantClasificada, cantOriginal);
            }

            if (sugerencia == null)
            {
                var cdEstacion = int.Parse(form.GetField("estacion").Value);
                var estacion = uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(cdEstacion);
                var cdFaixa = decimal.Parse(context.GetParameter("faixa"), culture);
                sugerencia = _almacenamientoService.SugerirUbicacionParaProducto(uow, estacion.Predio, etiqueta.Numero, sugerencia?.Agrupador, producto, cdFaixa, lote, cantSeparar, cantClasificada, cantOriginal, vencimiento);
            }

            context.Parameters.RemoveAll(p => p.Id == "sugerencia");

            if (sugerencia != null)
            {
                var ubicacion = uow.UbicacionRepository.GetUbicacion(sugerencia.UbicacionSugerida);
                var zona = uow.ZonaUbicacionRepository.GetZona(ubicacion.IdUbicacionZona);

                form.GetField("destino").Value = ubicacion.Id;
                form.GetField("descripcionZona").Value = $"{zona.Id} - {zona.Descripcion}";
                form.GetField("codigoZona").Value = zona.Id;
                form.GetField("posicionEquipo").Value = "";

                context.AddParameter("sugerencia", JsonSerializer.Serialize(sugerencia));
            }
            else
            {
                form.GetField("destino").Value = "";
                form.GetField("descripcionZona").Value = "";
                form.GetField("codigoZona").Value = "";
                form.GetField("posicionEquipo").Value = "";
            }

            uow.SaveChanges();
            uow.Commit();

            return sugerencia;
        }

        protected virtual SugerenciaAlmacenamiento SugerirUbicacionReabastecimiento(Form form, List<ComponentParameter> Parameters, IUnitOfWork uow, int etiquetaLote, string producto, int cdEmpresa, decimal? cdFaixa, string lote, DateTime? vencimiento, bool ignorarStock, decimal cantidadSeparar, decimal cantidadClasificada, decimal cantidadOriginal)
        {
            SugerenciaAlmacenamiento sugerencia = null;

            var culture = this._identity.GetFormatProvider();
            var cdEstacion = int.Parse(form.GetField("estacion").Value);
            var estacion = uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(cdEstacion);

            sugerencia = _almacenamientoService.SugerirUbicacionParaReabastecer(uow, estacion.Predio, etiquetaLote, producto, cdEmpresa, cdFaixa, lote, vencimiento, ignorarStock, cantidadSeparar, cantidadClasificada, cantidadOriginal);

            Parameters.RemoveAll(p => p.Id == "sugerencia");
            Parameters.RemoveAll(p => p.Id == "isReabastecer");

            if (sugerencia != null)
            {
                var ubicacion = uow.UbicacionRepository.GetUbicacion(sugerencia.UbicacionSugerida);
                var zona = uow.ZonaUbicacionRepository.GetZona(ubicacion.IdUbicacionZona);

                form.GetField("destino").Value = ubicacion.Id;
                form.GetField("descripcionZona").Value = $"{zona.Id} - {zona.Descripcion}";
                form.GetField("codigoZona").Value = zona.Id;
                form.GetField("cantidadSeparar").Value = sugerencia.CantidadSeparar.ToString(culture);
                form.GetField("posicionEquipo").Value = "";

                Parameters.Add(new ComponentParameter() { Id = "sugerencia", Value = JsonSerializer.Serialize(sugerencia) });
                Parameters.Add(new ComponentParameter() { Id = "isReabastecer", Value = "true" });

                Parameters.Add(new ComponentParameter() { Id = "focusField", Value = "posicionEquipo" });
                Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "SIGUIENTE" });
            }
            else
            {
                Parameters.Add(new ComponentParameter() { Id = "focusField", Value = "posicionEquipo" });
                Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "SIGUIENTE" });
                Parameters.Add(new ComponentParameter() { Id = "isReabastecer", Value = "false" });

                form.GetField("destino").Value = "";
                form.GetField("descripcionZona").Value = "";
                form.GetField("codigoZona").Value = "";
                form.GetField("posicionEquipo").Value = "";
            }

            return sugerencia;
        }

        protected virtual void CancelarSugerencia(IUnitOfWork uow, string cantidadSeparar, EtiquetaLote etiqueta, string producto, int cdEmpresa, decimal? faixa, string lote, SugerenciaAlmacenamiento sugerencia, bool isReabastecimiento = false)
        {
            uow.CreateTransactionNumber("CancelarSugerencia");
            uow.BeginTransaction();

            var culture = this._identity.GetFormatProvider();
            var cantidad = decimal.Parse(cantidadSeparar, culture);

            if (!isReabastecimiento)
            {
                _almacenamientoService.CancelarSugerenciaParaProducto(uow, sugerencia, etiqueta.Numero, producto, null, lote, cantidad);
            }
            else
            {
                _almacenamientoService.CancelarSugerenciaParaReabastecimiento(uow, sugerencia, etiqueta.Numero, producto, cdEmpresa, faixa, lote, cantidad);
            }

            uow.SaveChanges();
            uow.Commit();
        }

        protected virtual void GetDestinoZona(ComponentContext context, IUnitOfWork uow, out string destino, out string zona)
        {
            destino = context.GetParameter("destino");
            zona = context.GetParameter("zona");

            if (string.IsNullOrEmpty(destino))
            {
                var sugerenciaJson = context.GetParameter("sugerencia");
                if (!string.IsNullOrEmpty(sugerenciaJson))
                {
                    var sugerencia = JsonSerializer.Deserialize<SugerenciaAlmacenamiento>(sugerenciaJson);
                    var ubicacion = uow.UbicacionRepository.GetUbicacion(sugerencia.UbicacionSugerida);

                    zona = uow.ZonaUbicacionRepository.GetZona(ubicacion.IdUbicacionZona).Id;
                    destino = sugerencia.UbicacionSugerida;

                }
            }
        }

        protected virtual void RecomponerSugerencia(Form form, FormInitializeContext context, IUnitOfWork uow, bool reabastecer, string sugerenciaJson)
        {
            var sugerencia = JsonSerializer.Deserialize<SugerenciaAlmacenamiento>(sugerenciaJson);
            var ubicacion = uow.UbicacionRepository.GetUbicacion(sugerencia.UbicacionSugerida);
            var zona = uow.ZonaUbicacionRepository.GetZona(ubicacion.IdUbicacionZona);
            var cantidadSeparar = sugerencia.CantidadSeparar;
            var cantidadClasificada = sugerencia.CantidadClasificada;

            if (!reabastecer && (sugerencia.Detalles?.Count ?? 0) > 0)
            {
                var fstDet = sugerencia.Detalles.FirstOrDefault();
                cantidadSeparar = fstDet.CantidadSeparar;
                cantidadClasificada = fstDet.CantidadClasificada;
            }

            form.GetField("cantidadSeparar").Value = cantidadSeparar.ToString(_identity.GetFormatProvider());
            form.GetField("cantidadClasificada").Value = cantidadClasificada.ToString(_identity.GetFormatProvider());
            form.GetField("destino").Value = ubicacion.Id;
            form.GetField("descripcionZona").Value = $"{zona.Id} - {zona.Descripcion}";
            form.GetField("codigoZona").Value = zona.Id;
            form.GetField("posicionEquipo").Value = "";

            context.Parameters.RemoveAll(p => p.Id == "sugerencia");

            context.Parameters.Add(new ComponentParameter() { Id = "destino", Value = ubicacion.Id });
            context.Parameters.Add(new ComponentParameter() { Id = "zona", Value = zona.Id });
            context.Parameters.Add(new ComponentParameter() { Id = "sugerencia", Value = sugerenciaJson });
            context.Parameters.Add(new ComponentParameter() { Id = "focusField", Value = "posicionEquipo" });
            context.Parameters.Add(new ComponentParameter() { Id = "Operacion", Value = "SIGUIENTE" });
            context.Parameters.Add(new ComponentParameter() { Id = "isReabastecer", Value = reabastecer.ToString().ToLower() });
        }
        #endregion
    }
}
