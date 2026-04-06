using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Impresion;
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

namespace WIS.Application.Controllers.IMP
{
    public class IMP010ImpresionDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> Sorts { get; }

        public IMP010ImpresionDetalle(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_IMPRESION",
            };

            this.Sorts = new List<SortCommand> {
                new SortCommand("NU_IMPRESION", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ColaImpresionDetalleQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "impresion").Value, out int numImpresion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ColaImpresionDetalleQuery(numImpresion);

                context.AddParameter("IMP010DetalleImpresion", numImpresion.ToString());

            }
            else
            {
                dbQuery = new ColaImpresionDetalleQuery();
            }


            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ColaImpresionDetalleQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "impresion").Value, out int numImpresion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ColaImpresionDetalleQuery(numImpresion);
            }
            else
            {
                dbQuery = new ColaImpresionDetalleQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ColaImpresionDetalleQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "impresion").Value, out int numImpresion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ColaImpresionDetalleQuery(numImpresion);
            }
            else
            {
                dbQuery = new ColaImpresionDetalleQuery();
            }

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
        }
    }
}
