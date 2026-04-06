using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.Impresiones;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
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
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD113ProductosExpulsable : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IPrintingService _printingService;
        protected readonly IBarcodeService _barcodeService;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();

        protected List<string> GridKeys { get; set; }
        protected List<SortCommand> DefaultSort { get; }

        public PRD113ProductosExpulsable(
            IIdentityService identity,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogicaProduccionFactory logicaProduccionFactory,
            IGridValidationService gridValidationService,
            IPrintingService printingService,
            IBarcodeService barcodeService)
        {
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logicaProduccionFactory = logicaProduccionFactory;
            this._concurrencyControl = concurrencyControl;
            this._gridValidationService = gridValidationService;
            this._printingService = printingService;
            this._barcodeService = barcodeService;


            this.GridKeys = new List<string> { "CD_ENDERECO", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR" };
            this.DefaultSort = new List<SortCommand> { new SortCommand("CD_PRODUTO", SortDirection.Ascending) };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;
            context.IsRemoveEnabled = false;

            return GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var nuIngresoProduccion = context.Parameters.FirstOrDefault(x => x.Id == "nuIngresoProduccion").Value;

            var logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

            var ingreso = logicaProduccion.GetIngresoProduccion();

            var ubicacionProduccion = uow.ProduccionRepository.GetUbicacionProduccion(ingreso.IdEspacioProducion);

            var dbQuery = new DetallesProductosExpulsableQuery(ingreso.Empresa ?? 0, ubicacionProduccion);

            uow.HandleQuery(dbQuery);

            grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(new List<string> { "QT_EXPULSAR" });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var nuIngresoProduccion = context.Parameters.FirstOrDefault(x => x.Id == "nuIngresoProduccion").Value;

            var logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

            var ingreso = logicaProduccion.GetIngresoProduccion();

            var ubicacionProduccion = uow.ProduccionRepository.GetUbicacionProduccion(ingreso.IdEspacioProducion);

            var dbQuery = new DetallesProductosExpulsableQuery(ingreso.Empresa ?? 0, ubicacionProduccion);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);

        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new PRD113ProductosExpulsarGridValidationModule(uow, this._identity.GetFormatProvider()), grid, row, context);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var nuIngresoProduccion = query.Parameters.FirstOrDefault(x => x.Id == "nuIngresoProduccion").Value;

            var logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

            var ingreso = logicaProduccion.GetIngresoProduccion();

            var ubicacionProduccion = uow.ProduccionRepository.GetUbicacionProduccion(ingreso.IdEspacioProducion);

            var dbQuery = new DetallesProductosExpulsableQuery(ingreso.Empresa ?? 0, ubicacionProduccion);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var selection = context.Selection.GetSelection(this.GridKeys);
            var stock = selection.Select(item => new ProductosExpulsable
            {
                Ubicacion = item["CD_ENDERECO"],
                Empresa = int.Parse(item["CD_EMPRESA"]),
                Producto = item["CD_PRODUTO"],
                Faixa = decimal.Parse(item["CD_FAIXA"], _identity.GetFormatProvider()),
                Identificador = item["NU_IDENTIFICADOR"]
            }).ToList();

            var nuIngresoProduccion = context.Parameters.FirstOrDefault(x => x.Id == "nuIngresoProduccion").Value;

            var logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

            var ingreso = logicaProduccion.GetIngresoProduccion();

            var ubicacionProduccion = uow.ProduccionRepository.GetUbicacionProduccion(ingreso.IdEspacioProducion);

            if (context.Selection.AllSelected)
            {
                var dbQuery = new DetallesProductosExpulsableQuery(ingreso.Empresa ?? 0, ubicacionProduccion);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                var stockSeleccion = dbQuery.GetProductosExpulsable(_identity);

                stock = stockSeleccion.Join(stock,
                    sts => new { sts.Ubicacion, sts.Producto, sts.Empresa, sts.Identificador, sts.Faixa },
                    st => new { st.Ubicacion, st.Producto, st.Empresa, st.Identificador, st.Faixa },
                    (sts, st) => sts).ToList();

                stock = stockSeleccion.Except(stock).ToList();
            }
            else
            {
                var dbQuery = new DetallesProductosExpulsableQuery(ingreso.Empresa ?? 0, ubicacionProduccion);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                var stockSeleccion = dbQuery.GetProductosExpulsable(_identity);

                stock = stockSeleccion.Join(stock,
                    sts => new { sts.Ubicacion, sts.Producto, sts.Empresa, sts.Identificador, sts.Faixa },
                    st => new { st.Ubicacion, st.Producto, st.Empresa, st.Identificador, st.Faixa },
                    (sts, st) => sts).ToList();
            }

            context.AddParameter("isExplusarConTransferencia", context.ButtonId == "btnExpedir" ? "N" : "S");
            context.AddParameter("PRD113_ProductosExpulsable", JsonConvert.SerializeObject(stock));

            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var transactionTO = this._concurrencyControl.CreateTransaccion();

            var nuIngresoProduccion = context.Parameters.FirstOrDefault(x => x.Id == "nuIngresoProduccion").Value;
            var expulsarConTransferencia = context.Parameters.FirstOrDefault(x => x.Id == "isExplusarConTransferencia").Value == "S";
            var codigoImpresora = context.Parameters.FirstOrDefault(x => x.Id == "codigoImpresora").Value;

            if (expulsarConTransferencia && string.IsNullOrEmpty(codigoImpresora))
            {
                context.AddErrorNotification("PRD113_Sec0_Error_02");
                return grid;
            }

            var logicaProduccion = this._logicaProduccionFactory.GetLogicaProduccion(uow, nuIngresoProduccion);

            var ingreso = logicaProduccion.GetIngresoProduccion();
            var espacioProduccion = logicaProduccion.GetEspacioProduccion();

            var enderecoDestino = expulsarConTransferencia ? espacioProduccion.IdUbicacionSalidaTran : espacioProduccion.IdUbicacionSalida;

            var stocksExpulsar = ObtenerFilasAExpulsar(grid, context, enderecoDestino);

            if (stocksExpulsar == null || stocksExpulsar.Count() == 0)
            {
                context.AddErrorNotification("PRD113_Sec0_Error_03");
                return grid;
            }

            AgregarBloqueos(stocksExpulsar, transactionTO);

            uow.BeginTransaction();
            uow.CreateTransactionNumber("Expulsar Producto Producción");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                Impresion impresion = null;
                decimal? nuEtiquetaExterna = null;

                if (expulsarConTransferencia)
                {
                    ImprimirEtiquetaTransferencia(uow, ingreso, codigoImpresora, out impresion, out nuEtiquetaExterna);
                }

                uow.ProduccionRepository.ExpulsarProductos(uow, ingreso.Predio, stocksExpulsar, expulsarConTransferencia, nuTransaccion, nuEtiquetaExterna, _identity.UserId, ingreso.Id);

                uow.SaveChanges();
                uow.Commit();

                if (expulsarConTransferencia)
                    context.AddSuccessNotification("PRD113_msg_Success_StockExpulsadoEtiqueta", new List<string> { nuEtiquetaExterna.ToString() });
                else
                    context.AddSuccessNotification("General_Db_Success_Update");

                if (impresion != null)
                    _printingService.SendToPrint(impresion.Id);

            }
            catch (EntityLockedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
            }
            finally
            {
                this._concurrencyControl.DeleteTransaccion(transactionTO);
            }
            return grid;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var ingreso = uow.ProduccionRepository.GetIngreso(context.Parameters.FirstOrDefault(x => x.Id == "nuIngresoProduccion").Value);

            this.InicializarSelect(uow, form, context.Parameters, ingreso.Predio);

            context.AddParameter("PRD113_HABILITADO_PRODUCCION", (ingreso.Situacion != SituacionDb.PRODUCCION_FINALIZADA ? "S" : "N"));

            return form;
        }

        #region Metodos Auxiliares

        public virtual void ImprimirEtiquetaTransferencia(IUnitOfWork uow, IngresoProduccion ingreso, string codImpresora, out Impresion impresion, out decimal? nuEtiquetaExterna)
        {
            var cantidadGenerar = 1;
            var transferencias = new List<EtiquetaTransferencia> { new EtiquetaTransferencia() };

            var impresora = uow.ImpresoraRepository.GetImpresora(codImpresora, ingreso.Predio);
            var listaEstilos = uow.ImpresionRepository.GetEstiloByTipo(EstiloEtiquetaDb.Transferencia);

            IEstiloTemplate estiloTemplate = new EstiloTemplate(uow, listaEstilos.FirstOrDefault(x => x.CodigoLenguaje == impresora.CodigoLenguajeImpresion).CodigoLabel);
            IImpresionDetalleBuildingStrategy strategy = new TransferenciaImpresionStrategy(estiloTemplate, transferencias, uow, _printingService, _barcodeService, cantidadGenerar);

            ImpresionBuilder builder = new ImpresionBuilder(uow.ImpresoraRepository.GetImpresora(codImpresora, ingreso.Predio), strategy, _printingService);

            impresion = builder.GenerarCabezal(this._identity.UserId, ingreso.Predio)
            .GenerarDetalle()
            .Build();

            impresion.Referencia = string.Format("Impresión de {0} etiquetas de {1}", cantidadGenerar, EstiloEtiquetaDb.Transferencia);
            impresion.CantRegistros = cantidadGenerar;

            int numImpresion = uow.ImpresionRepository.Add(impresion);
            uow.SaveChanges();

            DetalleImpresion detalleImpresionInsercion = new DetalleImpresion()
            {
                NumeroImpresion = numImpresion,
                FechaProcesado = DateTime.Now,
                Estado = _printingService.GetEstadoInicial(),
            };

            detalleImpresionInsercion.Contenido += impresion.Detalles.FirstOrDefault().Contenido + "\n" + "\n";

            uow.ImpresionRepository.AddDetalleImpresion(detalleImpresionInsercion);
            uow.SaveChanges();

            nuEtiquetaExterna = transferencias.FirstOrDefault().NumeroEtiqueta;
        }

        public virtual void AgregarBloqueos(List<ProductosExpulsable> stocksExpulsar, TrafficOfficerTransaction transactionTO)
        {
            var detallesStock = stocksExpulsar
                .GroupBy(g => new { g.Ubicacion, g.Empresa, g.Producto, g.Faixa, g.Identificador })
                .Select(s => new { key = $"{s.Key.Ubicacion}#{s.Key.Empresa}#{s.Key.Producto}#{s.Key.Faixa}#{s.Key.Identificador}" }).Select(x => x.key);

            var listLock = this._concurrencyControl.GetLockList("T_STOCK", detallesStock.ToList(), transactionTO);

            if (listLock.Count > 0)
            {
                var keyBloqueo = listLock.FirstOrDefault().Id_Bloqueo.Split("#");
                throw new EntityLockedException("PRD113_msg_Error_StockBloqueada", new string[] { keyBloqueo[2], keyBloqueo[4] });
            }
            this._concurrencyControl.AddLockList("T_STOCK", detallesStock.ToList(), transactionTO);
        }

        public virtual List<ProductosExpulsable> ObtenerFilasAExpulsar(Grid grid, GridFetchContext context, string enderecoDestino)
        {
            int rowId = 0;
            List<ProductosExpulsable> stocksSeleccionado = JsonConvert.DeserializeObject<List<ProductosExpulsable>>(context.Parameters.FirstOrDefault(x => x.Id == "PRD113_ProductosExpulsable").Value);
            foreach (var row in grid.Rows)
            {
                string ubicacion = row.GetCell("CD_ENDERECO").Value;
                int empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                string producto = row.GetCell("CD_PRODUTO").Value;
                decimal faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider());
                string identificador = row.GetCell("NU_IDENTIFICADOR").Value;
                decimal cantidadExpulsar = decimal.Parse(row.GetCell("QT_EXPULSAR").Value, _identity.GetFormatProvider());
                var stockSeleccionado = stocksSeleccionado.FirstOrDefault(x => x.Ubicacion == ubicacion && x.Empresa == empresa && x.Producto == producto && x.Faixa == faixa && x.Identificador == identificador);
                DateTime? vencimiento = string.IsNullOrEmpty(row.GetCell("DT_FABRICACAO").Value) ? null : DateTime.Parse(row.GetCell("DT_FABRICACAO").Value, _identity.GetFormatProvider());
                if (stockSeleccionado == null)
                {
                    stocksSeleccionado.Add(new ProductosExpulsable
                    {
                        Ubicacion = ubicacion,
                        Empresa = empresa,
                        Producto = producto,
                        Faixa = faixa,
                        Identificador = identificador,
                        Cantidad = cantidadExpulsar,
                        UbicacionDestino = enderecoDestino,
                        Vencimiento = vencimiento
                    });
                }
                else
                {
                    stocksSeleccionado.Remove(stockSeleccionado);
                    stocksSeleccionado.Add(new ProductosExpulsable
                    {
                        Ubicacion = ubicacion,
                        Empresa = empresa,
                        Producto = producto,
                        Faixa = faixa,
                        Identificador = identificador,
                        Cantidad = cantidadExpulsar,
                        UbicacionDestino = enderecoDestino,
                        Vencimiento = vencimiento
                    });
                }
            }
            if (stocksSeleccionado != null && stocksSeleccionado.Count() > 0)
            {
                foreach (var det in stocksSeleccionado)
                {
                    det.IdRow = rowId;
                    det.UbicacionDestino = enderecoDestino;
                    rowId = rowId + 1;
                }
            }
            return stocksSeleccionado;
        }

        public virtual void InicializarSelect(IUnitOfWork uow, Form form, List<ComponentParameter> parameters, string predio)
        {
            FormField selectorImpresora = form.GetField("impresora");
            selectorImpresora.Options = new List<SelectOption>();
            List<Impresora> listaImpresoras;

            listaImpresoras = uow.ImpresoraRepository.GetListaImpresorasPredio(predio);
            List<EtiquetaEstiloLenguaje> listaEstilos = uow.ImpresionRepository.GetEstiloByTipo(EstiloEtiquetaDb.Transferencia);

            listaImpresoras.Join(listaEstilos,
                li => li.CodigoLenguajeImpresion,
                ls => ls.CodigoLenguaje,
                (li, ls) => li);

            foreach (var impresora in listaImpresoras)
            {
                selectorImpresora.Options.Add(new SelectOption(impresora.Id, $"{impresora.Id} - {impresora.Descripcion}"));
            }
        }

        #endregion
    }
}
