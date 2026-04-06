using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Inventario;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.INV
{
    public class INV415EnderecoDetalleError : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<INV415EnderecoDetalleError> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public INV415EnderecoDetalleError(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<INV415EnderecoDetalleError> logger)
        {
            this.GridKeys = new List<string>
            {
                "NU_ERROR",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ERROR", SortDirection.Ascending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            var nuInventario = context.GetParameter("inventario");
            if (!string.IsNullOrEmpty(nuInventario) && !decimal.TryParse(nuInventario, _identity.GetFormatProvider(), out decimal i))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), _identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new InventarioEnderecoDetalleErrorQuery(nuInventario);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), _identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new InventarioEnderecoDetalleErrorQuery(nuInventario);
            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var nuInventario = decimal.TryParse(context.GetParameter("inventario"), _identity.GetFormatProvider(), out decimal n) ? n : default(decimal?);

            var dbQuery = new InventarioEnderecoDetalleErrorQuery(nuInventario);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
