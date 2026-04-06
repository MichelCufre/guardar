using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules.Recepcion;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.General;
using WIS.Domain.ManejoStock.Constants;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
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
    public class REC410DetalleProductoSinClasificar : AppController
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

        public REC410DetalleProductoSinClasificar(
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
                "NU_ETIQUETA_LOTE", "NU_AGENDA", "NU_EXTERNO_ETIQUETA", "TP_ETIQUETA", "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_FAIXA", "CD_EMPRESA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ETIQUETA_LOTE", SortDirection.Descending),
                new SortCommand("CD_PRODUTO", SortDirection.Descending),
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

            var TipoEtiqueta = context.GetParameter("TipoEtiqueta");
            var NumeroExterno = context.GetParameter("NumeroExterno");
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(TipoEtiqueta, NumeroExterno);

            REC410DetalleProductoSinClasificarQuery dbQuery = new REC410DetalleProductoSinClasificarQuery(etiqueta.Numero);

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var TipoEtiqueta = context.GetParameter("TipoEtiqueta");
            var NumeroExterno = context.GetParameter("NumeroExterno");
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(TipoEtiqueta, NumeroExterno);

            REC410DetalleProductoSinClasificarQuery dbQuery = new REC410DetalleProductoSinClasificarQuery(etiqueta.Numero);

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

            var TipoEtiqueta = context.GetParameter("TipoEtiqueta");
            var NumeroExterno = context.GetParameter("NumeroExterno");
            var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(TipoEtiqueta, NumeroExterno);
            REC410DetalleProductoSinClasificarQuery dbQuery = new REC410DetalleProductoSinClasificarQuery(etiqueta.Numero);

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            form.GetField("etiqueta").Value = context.GetParameter("NumeroExterno");
            form.GetField("tipoEtiqueta").Value = context.GetParameter("TipoEtiqueta");

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REC410DetalleProductoSinClasificarFormValidationModule(uow), form, context);
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("ConfirmarClasificacion");
            uow.BeginTransaction();
            var keys = new List<string>();
            try
            {
                var nroEtiqueta = context.GetParameter("NumeroExterno");
                var tipoEtiqueta = context.GetParameter("TipoEtiqueta");
                var cdEstacion = int.Parse(context.GetParameter("estacion"));
                var estacion = uow.MesaDeClasificacionRepository.GetEstacionDeClasificacion(cdEstacion);
                var etiqueta = uow.EtiquetaLoteRepository.GetEtiquetaLoteActiva(tipoEtiqueta, nroEtiqueta);
                var detalles = uow.MesaDeClasificacionRepository.GetEtiquetaConStockSinClasificar(etiqueta.Numero);

                foreach (var det in detalles)
                {
                    var stock = uow.StockRepository.GetStock(det.CodigoEmpresa, det.CodigoProducto, det.Faixa, det.Ubicacion, det.Lote);

                    this.UpdateStockConsumido(uow, stock, det.Cantidad ?? 0);
                    this.AddAjusteStockConsumo(uow, stock, det, estacion.Predio, keys);

                    var etiquetaLoteDetalle = uow.EtiquetaLoteRepository.GetEtiquetaLoteDetalle(det.CodigoProducto, det.Faixa, det.CodigoEmpresa, det.Lote, det.EtiquetaLote);

                    decimal? cantidad = -etiquetaLoteDetalle.Cantidad;
                    etiquetaLoteDetalle.CantidadAjusteRecibido = -etiquetaLoteDetalle.Cantidad;
                    etiquetaLoteDetalle.Cantidad = 0;
                    etiquetaLoteDetalle.Modificacion = DateTime.Now;
                    etiquetaLoteDetalle.NumeroTransaccion = uow.GetTransactionNumber();

                    uow.EtiquetaLoteRepository.UpdateEtiquetaLoteDetalle(etiquetaLoteDetalle);

                    var logEtiqueta = new LogEtiqueta()
                    {
                        Agenda = etiqueta.NumeroAgenda,
                        NumeroEtiqueta = etiqueta.Numero,
                        CodigoProducto = etiquetaLoteDetalle.CodigoProducto,
                        Faixa = etiquetaLoteDetalle.Faixa,
                        Empresa = etiquetaLoteDetalle.IdEmpresa,
                        Identificador = etiquetaLoteDetalle.Identificador,
                        Cantidad = cantidad,
                        Ubicacion = etiqueta.IdUbicacion,
                        FechaOperacion = DateTime.Now,
                        NroTransaccion = uow.GetTransactionNumber(),
                        Vencimiento = etiquetaLoteDetalle.Vencimiento,
                        TipoMovimiento = TiposMovimiento.AjusteEtiqueta,
                        Aplicacion = this._identity.Application,
                        Funcionario = this._identity.UserId,
                    };
                    uow.EtiquetaLoteRepository.AddLogEtiqueta(logEtiqueta);

                }

                etiqueta.NumeroTransaccion = uow.GetTransactionNumber();
                etiqueta.Estado = SituacionDb.PalletSinProductos;
                etiqueta.FechaModificacion = DateTime.Now;

                uow.EtiquetaLoteRepository.UpdateEtiquetaLote(etiqueta);

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, ex.Message);
                context.AddErrorNotification("REC410_Sec0_Error_COL01_ConfirmarOperacion");

                return form;
            }

            if (_taskQueue.IsEnabled() && keys.Any())
                _taskQueue.Enqueue(TaskQueueCategory.API, CInterfazExterna.AjustesDeStock, keys);

            return form;
        }

        public virtual void AddAjusteStockConsumo(IUnitOfWork uow, Stock stock, DetalleEtiquetaSinClasificar detalle, string predio, List<string> keys)
        {
            var ajuste = new AjusteStock
            {
                IdAreaAveria = stock.Averia,
                Aplicacion = this._identity.Application,
                Funcionario = this._identity.UserId,
                NuAjusteStock = uow.AjusteRepository.GetNextNuAjuste(),
                Producto = detalle.CodigoProducto,
                Faixa = detalle.Faixa,
                Identificador = detalle.Lote,
                Empresa = detalle.CodigoEmpresa,
                FechaRealizado = DateTime.Now,
                TipoAjuste = TipoAjusteDb.Stock,
                QtMovimiento = (-1) * (detalle.Cantidad ?? 0),
                DescMotivo = "Mesa de Clasificación",
                CdMotivoAjuste = MotivoAjusteDb.MesaClasificacion,
                Ubicacion = stock.Ubicacion,
                Serializado = "",
                FechaVencimiento = stock.Vencimiento,
                NuTransaccion = uow.GetTransactionNumber(),
                Metadata = detalle.EtiquetaLote.ToString(),
                Predio = predio
            };

            uow.AjusteRepository.Add(ajuste);
            keys.Add(ajuste.NuAjusteStock.ToString());
        }

        public virtual void UpdateStockConsumido(IUnitOfWork uow, Stock stock, decimal cantidad)
        {
            if (stock.ReservaSalida < cantidad || stock.Cantidad < cantidad)
                throw new ValidationFailedException("General_Sec0_Error_NotEnoughProducto");

            stock.NumeroTransaccion = uow.GetTransactionNumber();

            uow.StockRepository.MovilizarStock(stock, -cantidad);
        }
    }
}
