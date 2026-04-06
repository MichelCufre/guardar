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
    public class INV030AjustesStock : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public INV030AjustesStock(IIdentityService security,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_AJUSTE_STOCK"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Ascending),
                new SortCommand("CD_PRODUTO", SortDirection.Ascending),
                new SortCommand("CD_FAIXA", SortDirection.Ascending),
                new SortCommand("NU_IDENTIFICADOR", SortDirection.Ascending),
                new SortCommand("DT_REALIZADO", SortDirection.Ascending),
                new SortCommand("HR_REALIZADO", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = security;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            INV030GridQuery dbQuery;

            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "fechaRealizado")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "documento")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idProducto = context.Parameters.Find(x => x.Id == "producto").Value;
                var fechaRealizado = context.Parameters.Find(x => x.Id == "fechaRealizado").Value;
                var documento = context.Parameters.Find(x => x.Id == "documento")?.Value;

                dbQuery = new INV030GridQuery(idEmpresa, idProducto, documento, fechaRealizado, _identity.GetFormatProvider());
            }
            else
                dbQuery = new INV030GridQuery();


            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);

            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            INV030GridQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "fechaRealizado")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "documento")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idProducto = context.Parameters.Find(x => x.Id == "producto").Value;
                var fechaRealizado = context.Parameters.Find(x => x.Id == "fechaRealizado").Value;
                var documento = context.Parameters.Find(x => x.Id == "documento")?.Value;

                dbQuery = new INV030GridQuery(idEmpresa, idProducto, documento, fechaRealizado, _identity.GetFormatProvider());
            }
            else
                dbQuery = new INV030GridQuery();

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

            INV030GridQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "producto")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "fechaRealizado")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(context.Parameters.Find(x => x.Id == "documento")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idProducto = context.Parameters.Find(x => x.Id == "producto").Value;
                var fechaRealizado = context.Parameters.Find(x => x.Id == "fechaRealizado").Value;
                var documento = context.Parameters.Find(x => x.Id == "documento")?.Value;

                dbQuery = new INV030GridQuery(idEmpresa, idProducto, documento, fechaRealizado, _identity.GetFormatProvider());
            }
            else
                dbQuery = new INV030GridQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, DefaultSort);
        }

    }
}
