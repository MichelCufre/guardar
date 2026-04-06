using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules.Produccion;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Produccion;
using WIS.Domain.General;
using WIS.Domain.General.Enums;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.Interfaces;
using WIS.Domain.Produccion.Models;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRD
{
    public class PRD110DetallesTeoricos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly ISecurityService _securityService;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly ILogicaProduccionFactory _logicaProduccionFactory;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys1 { get; }
        protected List<SortCommand> DefaultSort1 { get; }

        protected List<string> GridKeys2 { get; }
        protected List<SortCommand> DefaultSort2 { get; }

        public PRD110DetallesTeoricos(
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            ITrafficOfficerService concurrencyControl,
            ILogicaProduccionFactory logicaProduccion,
            IGridService gridService,
            ISecurityService securityService,
            IIdentityService identity,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._securityService = securityService;
            this._logicaProduccionFactory = logicaProduccion;
            this._gridValidationService = gridValidationService;
            this._identity = identity;
            this._concurrencyControl = concurrencyControl;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;

            this.GridKeys1 = new List<string> { "NU_PRDC_DET_TEORICO" };
            this.DefaultSort1 = new List<SortCommand> { new SortCommand("NU_PRDC_DET_TEORICO", SortDirection.Ascending) };

            this.GridKeys2 = new List<string> { "NU_PRDC_DET_TEORICO" };
            this.DefaultSort2 = new List<SortCommand> { new SortCommand("NU_PRDC_DET_TEORICO", SortDirection.Ascending) };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var permissions = _securityService.CheckPermissions(new List<string> { "PRD110_grid1_btn_ControlesDeCalidad", "PRD110_Section_Access_Editar" });

            var idIngreso = context.GetParameter("idIngreso");

            var ingreso = uow.IngresoProduccionRepository.GetIngresoById(idIngreso);

            if (uow.IngresoProduccionRepository.PuedeEditarseIngresoTeorico(ingreso))
            {
                context.IsEditingEnabled = permissions["PRD110_Section_Access_Editar"];
                context.IsAddEnabled = permissions["PRD110_Section_Access_Editar"];
                context.IsCommitEnabled = permissions["PRD110_Section_Access_Editar"];
                context.IsRemoveEnabled = permissions["PRD110_Section_Access_Editar"];

                grid.SetColumnDefaultValues(new Dictionary<string, string>
                {
                    ["NU_PRDC_INGRESO"] = ingreso.Id,
                    ["CD_EMPRESA"] = ingreso.Empresa.ToString(),
                    ["CD_FAIXA"] = "1",
                });
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            var idIngreso = context.GetParameter("idIngreso");

            if (grid.Id == "PRD110Detalles_grid_1")
            {
                var dbQuery = new DetallesIngresoControlAuxQuery(idIngreso, CIngresoProduccionDetalleTeorico.TipoDetalleEntrada);

                uow.HandleQuery(dbQuery);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort1, this.GridKeys1);

                var ingreso = uow.IngresoProduccionRepository.GetIngresoById(idIngreso);

                if (uow.IngresoProduccionRepository.PuedeEditarseIngresoTeorico(ingreso))
                {
                    grid.SetEditableCells(new List<string> { "NU_IDENTIFICADOR", "QT_TEORICO" });
                    grid.SetInsertableColumns(new List<string> { "CD_PRODUTO", "NU_IDENTIFICADOR", "QT_TEORICO" });
                }

            }
            else if (grid.Id == "PRD110Detalles_grid_2")
            {
                var dbQuery = new DetallesIngresoControlAuxQuery(idIngreso, CIngresoProduccionDetalleTeorico.TipoDetalleSalida);

                uow.HandleQuery(dbQuery);

                grid.Rows = this._gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort2, this.GridKeys2);

                var ingreso = uow.IngresoProduccionRepository.GetIngresoById(idIngreso);
                if (uow.IngresoProduccionRepository.PuedeEditarseIngresoTeorico(ingreso))
                {
                    grid.SetEditableCells(new List<string> { "QT_TEORICO" });
                    grid.SetInsertableColumns(new List<string> { "CD_PRODUTO", "QT_TEORICO" });
                }
            }

            var columnProducto = grid.GetColumn("CD_PRODUTO");
            columnProducto.Type = GridColumnType.SelectAsync;

            grid.AddOrUpdateColumn(columnProducto);

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            if (!grid.Rows.Any(a => a.IsModified || a.IsDeleted || a.IsNew)) return grid;

            var idIngreso = context.GetParameter("idIngreso");

            if (_concurrencyControl.IsLocked("T_PRDC_INGRESO", idIngreso, true))
            {
                context.AddErrorNotification("General_msg_Error_ProduccionBloqueada");
                return grid;
            }

            var transaction = this._concurrencyControl.CreateTransaccion();

            try
            {
                var ingreso = uow.IngresoProduccionRepository.GetIngresoById(idIngreso);

                if (!uow.IngresoProduccionRepository.PuedeEditarseIngresoTeorico(ingreso))
                {
                    context.AddErrorNotification("PRD110_Sec0_Error_ProduccionConPedido");
                    return grid;
                }

                _concurrencyControl.AddLock("T_PRDC_INGRESO", idIngreso, transaction, true);

                var logicaProduccion = _logicaProduccionFactory.GetLogicaProduccion(uow, idIngreso);

                uow.BeginTransaction();

                if (grid.Id == "PRD110Detalles_grid_1")
                    ValidateRows(uow, grid.Rows);

                List<IngresoProduccionDetalleTeorico> detallesAgregar = new List<IngresoProduccionDetalleTeorico>();
                grid.Rows.Where(w => w.IsNew).ToList().ForEach(row =>
                {
                    var detalle = new IngresoProduccionDetalleTeorico
                    {
                        Producto = row.GetCell("CD_PRODUTO").Value,
                        Tipo = grid.Id == "PRD110Detalles_grid_1" ? CIngresoProduccionDetalleTeorico.TipoDetalleEntrada : CIngresoProduccionDetalleTeorico.TipoDetalleSalida,
                        Empresa = int.Parse(row.GetCell("CD_EMPRESA").Value),
                        Faixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, _identity.GetFormatProvider()),
                        IdIngresoProduccion = context.GetParameter("idIngreso"),
                        CantidadTeorica = decimal.Parse(row.GetCell("QT_TEORICO").Value, _identity.GetFormatProvider()),
                        Identificador = row.GetCell("NU_IDENTIFICADOR").Value
                    };

                    logicaProduccion.AddDetalleTeorico(detalle);
                });

                grid.Rows.Where(w => w.IsModified && !w.IsNew).ToList().ForEach(row =>
                {
                    var detalle = logicaProduccion.GetDetalleTeorico(int.Parse(row.GetCell("NU_PRDC_DET_TEORICO").Value));

                    detalle.CantidadTeorica = decimal.Parse(row.GetCell("QT_TEORICO").Value, _identity.GetFormatProvider());

                    logicaProduccion.UpdateDetalleTeorico(detalle);
                });

                grid.Rows.Where(w => w.IsDeleted).ToList().ForEach(row =>
                {
                    var detalle = logicaProduccion.GetDetalleTeorico(int.Parse(row.GetCell("NU_PRDC_DET_TEORICO").Value));

                    if (grid.Id == "PRD110Detalles_grid_2")
                    {
                        var existeSalidaEsperado = uow.IngresoProduccionRepository.IngresoDetalleTieneDetalleSalidaReal(detalle.IdIngresoProduccion, detalle.Producto, detalle.Empresa ?? 0, detalle.Identificador);

                        if (existeSalidaEsperado)
                            throw new ValidationFailedException("PRD110_grid_error_ExisteDetalleSalidaReal", new string[] { detalle.Producto, detalle.Identificador });

                    }

                    logicaProduccion.DeleteDetalleTeorico(detalle);

                });

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                this._logger.Error(ex, ex.Message);
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                this._logger.Error(ex, ex.Message);
                uow.Rollback();
                context.AddErrorNotification("Master_Sec0_metaTitle_MasterErrorMessage");
            }
            finally
            {
                _concurrencyControl.DeleteTransaccion(transaction);
            }

            return grid;
        }


        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            if (grid.Id == "PRD110Detalles_grid_1")
                return this._gridValidationService.Validate(new PRD110DetalleTeoricoEntradaGridValidationModule(uow, _identity.GetFormatProvider()), grid, row, context);
            else if (grid.Id == "PRD110Detalles_grid_2")
                return this._gridValidationService.Validate(new PRD110DetalleTeoricoSalidaGridValidationModule(uow, _identity.GetFormatProvider()), grid, row, context);

            return null;
        }

        public override List<SelectOption> GridSelectSearch(GridRow row, Grid grid, GridSelectSearchContext context)
        {
            switch (context.ColumnId)
            {
                case "CD_PRODUTO":
                    return this.SearchProducto(grid, context);

            }

            return new List<SelectOption>();
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var idIngreso = context.GetParameter("idIngreso");

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                if (grid.Id == "PRD110Detalles_grid_1")
                {
                    var dbQuery = new DetallesIngresoControlAuxQuery(idIngreso, CIngresoProduccionDetalleTeorico.TipoDetalleEntrada);

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("NU_PRDC_DET_TEORICO", SortDirection.Ascending);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }

                if (grid.Id == "PRD110Detalles_grid_2")
                {
                    var dbQuery = new DetallesIngresoControlAuxQuery(idIngreso, CIngresoProduccionDetalleTeorico.TipoDetalleSalida);

                    uow.HandleQuery(dbQuery);

                    var defaultSort = new SortCommand("NU_PRDC_DET_TEORICO", SortDirection.Ascending);

                    context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
                }

                return null;
            }
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();
            var idIngreso = context.GetParameter("idIngreso");

            if (grid.Id == "PRD110Detalles_grid_1")
            {
                var dbQuery = new DetallesIngresoControlAuxQuery(idIngreso, CIngresoProduccionDetalleTeorico.TipoDetalleEntrada);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "PRD110Detalles_grid_2")
            {
                var dbQuery = new DetallesIngresoControlAuxQuery(idIngreso, CIngresoProduccionDetalleTeorico.TipoDetalleSalida);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        #region Metodos Auxiliares

        public virtual List<SelectOption> SearchProducto(Grid grid, GridSelectSearchContext context)
        {
            using var uow = _uowFactory.GetUnitOfWork();

            string paramEmpresa = context.GetParameter("empresa");

            List<SelectOption> opciones = new List<SelectOption>();

            if (string.IsNullOrEmpty(paramEmpresa))
                return opciones;

            int empresa = int.Parse(paramEmpresa);

            List<Producto> productos = uow.ProductoRepository.GetByDescriptionOrCodePartial(empresa, context.SearchValue);

            foreach (Producto producto in productos)
            {
                opciones.Add(new SelectOption(producto.Codigo, $"{producto.Codigo} - {producto.Descripcion}"));
            }

            return opciones;
        }


        public virtual void ValidateRows(IUnitOfWork uow, List<GridRow> rows)
        {
            var insumos = new List<string>();

            foreach (var row in rows.Where(w => !w.IsDeleted))
            {
                var cdProducto = row.GetCell("CD_PRODUTO").Value;
                var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

                var producto = uow.ProductoRepository.GetProducto(empresa, cdProducto);

                if (producto.ManejoIdentificador == ManejoIdentificador.Producto)
                    row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorProducto;
                else if (string.IsNullOrEmpty(row.GetCell("NU_IDENTIFICADOR").Value))
                    row.GetCell("NU_IDENTIFICADOR").Value = ManejoIdentificadorDb.IdentificadorAuto;

                var key = $"{cdProducto}.{row.GetCell("NU_IDENTIFICADOR").Value}";
                var keyAuto = $"{cdProducto}.{ManejoIdentificadorDb.IdentificadorAuto}";

                if (insumos.Contains(key))
                    throw new ValidationFailedException("PRD110_grid_error_ProductosRepetidos");
                else if (insumos.Contains(keyAuto))
                    throw new ValidationFailedException("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido");
                else if (row.GetCell("NU_IDENTIFICADOR").Value == ManejoIdentificadorDb.IdentificadorAuto && insumos.Any(i => i.Contains(cdProducto)))
                    throw new ValidationFailedException("WMSAPI_msg_Error_EnvioLoteEspecificoyAutoNoPermitido");
                else
                    insumos.Add(key);
            }
        }
        #endregion
    }
}
