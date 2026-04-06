using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.Recorridos;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REG
{
    public class REG700DetallesRecorridos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REG700DetallesRecorridos> _logger;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IRecorridoService _recorridoService;

        protected readonly RecorridoMapper _mapper;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG700DetallesRecorridos(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            ILogger<REG700DetallesRecorridos> logger,
            IRecorridoService recorridoService,
            ITrafficOfficerService concurrencyControl)
        {
            GridKeys =
            [
                "NU_RECORRIDO_DET"
            ];

            DefaultSort =
            [
                new SortCommand("VL_ORDEN", SortDirection.Ascending)
            ];

            _uowFactory = uowFactory;
            _identity = identity;
            _gridService = gridService;
            _excelService = excelService;
            _security = security;
            _filterInterpreter = filterInterpreter;
            _logger = logger;
            _recorridoService = recorridoService;
            _concurrencyControl = concurrencyControl;
            _mapper = new RecorridoMapper();
        }

        #region GRID
        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            grid.AddOrUpdateColumn(new GridColumn { Id = "TP_OPERACION", Name = "TP_OPERACION", Insertable = true, Hidden = true });

            var insertableColumns = new List<string>()
            {
                "CD_ENDERECO",
                "NU_ORDEN",
                "TP_OPERACION"
            };

            grid.SetInsertableColumns(insertableColumns);

            grid = this.GridFetchRows(grid, query.FetchContext);

            grid.GetColumn("VL_ORDEN").Hidden = true;

            return grid;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = GetQuery(query);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override Grid GridImportExcel(Grid grid, GridImportExcelContext context)
        {
            ValidateBeforeImport(context);

            using (var uow = _uowFactory.GetUnitOfWork())
            {
                var numeroRecorrido = context.FetchContext.GetParameter("REG700_DETALLES_NU_RECORRIDO").ToNumber<int>();

                grid = GridFetchRows(grid, context.FetchContext);

                using (var excelImporter = new GridExcelImporter(context.Translator, context.FileName, grid.Columns, context.Payload))
                {
                    var descTransaccion = $"GridImportExcel. Recorrido {numeroRecorrido}";
                    uow.CreateTransactionNumber(descTransaccion, _identity.Application, _identity.UserId);

                    uow.BeginTransaction();

                    this._concurrencyControl.CreateToken();

                    var transaction = this._concurrencyControl.CreateTransaccion();

                    try
                    {
                        this._concurrencyControl.AddLock("T_RECORRIDO", $"GridImportExcel_{numeroRecorrido}", transaction, true);

                        excelImporter.CleanErrors();

                        var excelRows = excelImporter.BuildRows();

                        var detallesRecorrido = ProcessExcelRows(excelRows, grid.Columns, numeroRecorrido);

                        if (detallesRecorrido.Count == 0)
                            throw new ValidationFailedException("REG040_msg_Error_ExcelVacio");

                        var importResult = _recorridoService.ImportarDetalles(uow, detallesRecorrido, _identity.UserId, empresa: 0).GetAwaiter().GetResult();

                        HandleImportResult(importResult, context, excelImporter);

                        uow.SaveChanges();
                        uow.Commit();

                        grid = GridFetchRows(grid, context.FetchContext);
                    }
                    catch (ValidationFailedException ex)
                    {
                        uow.Rollback();

                        _logger.LogError(ex, ex.Message);

                        var payload = Convert.ToBase64String(excelImporter.GetAsByteArray());

                        throw new GridExcelImporterException(ex.Message, payload, ex.StrArguments);
                    }
                    catch (Exception ex)
                    {
                        uow.Rollback();
                        _logger.LogError(ex, ex.Message);
                        context.AddErrorNotification(ex.Message);
                        throw;
                    }
                    finally
                    {
                        _concurrencyControl.DeleteTransaccion(transaction);
                    }
                }
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            var dbQuery = GetQuery(query);

            using var uow = _uowFactory.GetUnitOfWork();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{_identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return _excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            var dbQuery = GetQuery(query);

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        #endregion

        #region AUX METHODS
        public virtual DetallesRecorridosQuery GetQuery(ComponentContext query)
        {
            if (!query.HasParameter("REG700_DETALLES_NU_RECORRIDO") || string.IsNullOrEmpty(query.GetParameter("REG700_DETALLES_NU_RECORRIDO")))
                return new DetallesRecorridosQuery();

            var nuRecorrido = query.Parameters.FirstOrDefault(s => s.Id == "REG700_DETALLES_NU_RECORRIDO").Value.ToNumber<int>();

            return new DetallesRecorridosQuery(nuRecorrido);
        }

        protected virtual void ValidateBeforeImport(GridImportExcelContext context)
        {
            if (!context.FetchContext.HasParameter("REG700_DETALLES_NU_RECORRIDO"))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            if (context.Payload == null) throw new MissingParameterException("Datos nulos");
        }

        protected virtual List<DetalleRecorrido> ProcessExcelRows(IEnumerable<GridRow> excelRows, List<IGridColumn> columns, int numeroRecorrido)
        {
            var detallesRecorridos = new List<DetalleRecorrido>();

            foreach (var row in excelRows)
            {
                AddMissingCellsToRow(row, columns);

                detallesRecorridos.Add(GetObjectFromRow(row, numeroRecorrido));
            }

            return detallesRecorridos;
        }

        protected virtual void AddMissingCellsToRow(GridRow row, List<IGridColumn> columns)
        {
            foreach (var column in columns)
            {
                if (!row.Cells.Any(c => c.Column.Id == column.Id))
                {
                    row.AddCell(new GridCell { Column = column });
                }
            }
        }

        protected virtual DetalleRecorrido GetObjectFromRow(GridRow row, int numeroRecorrido)
        {
            #region - VARIABLES - 
            var ubicacion = row.GetCell("CD_ENDERECO").Value;
            var tipoOperacion = row.GetCell("TP_OPERACION").Value;
            var ordenDefecto = row.GetCell("NU_ORDEN").Value ?? string.Empty;
            var valorOrden = string.IsNullOrEmpty(ordenDefecto) ? string.Empty : ordenDefecto.PadLeft(40, '0');
            #endregion

            return new DetalleRecorrido
            {
                NuOrden = _mapper.NullIfEmpty(ordenDefecto),
                TipoOperacion = string.IsNullOrEmpty(tipoOperacion) ? string.Empty : tipoOperacion.ToUpper(),

                IdRecorrido = numeroRecorrido,
                Ubicacion = string.IsNullOrEmpty(ubicacion) ? string.Empty : ubicacion.ToUpper(),
                ValorOrden = valorOrden,
            };
        }

        public virtual void HandleImportResult(ValidationsResult importResult, GridImportExcelContext context, GridExcelImporter excelImporter)
        {
            if (!importResult.HasError())
            {
                context.AddSuccessNotification("REG700_Sec0_Success_Suc006_DetallesRecorridoImportados");
            }
            else
            {
                var dictErrors = new Dictionary<int, List<string>>();

                foreach (var error in importResult.Errors)
                {
                    if (!dictErrors.ContainsKey(error.ItemId))
                        dictErrors[error.ItemId] = new List<string>();

                    dictErrors[error.ItemId].AddRange(error.Messages.Select(m => m.TrimEnd('.')));
                }

                excelImporter.SetErrors(dictErrors);

                throw new ValidationFailedException("REG700_Err0_Error_Err006_DetallesRecorridosImportados");
            }
        }
        #endregion
    }
}