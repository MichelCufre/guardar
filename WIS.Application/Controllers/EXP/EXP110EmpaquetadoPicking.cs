using System;
using System.Collections.Generic;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
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

namespace WIS.Application.Controllers.EXP
{
    public class EXP110EmpaquetadoPicking : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP110EmpaquetadoPicking(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_CONTENEDOR", "NU_PREPARACION", "CD_EMPRESA", "CD_CLIENTE", "NU_PEDIDO", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                new SortCommand("NU_PEDIDO", SortDirection.Ascending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
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
            var queryData = GetEmpaquetadoPickingQueryData(context);

            if (queryData == null)
                return grid;

            using var uow = this._uowFactory.GetUnitOfWork();

            EmpaquetadoPickingQuery dbQuery = new EmpaquetadoPickingQuery(queryData.Contenedor, queryData.Preparacion);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var queryData = GetEmpaquetadoPickingQueryData(context);

            if (queryData == null)
                return Array.Empty<byte>();

            using var uow = this._uowFactory.GetUnitOfWork();

            EmpaquetadoPickingQuery dbQuery = new EmpaquetadoPickingQuery(queryData.Contenedor, queryData.Preparacion);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var queryData = GetEmpaquetadoPickingQueryData(context);

            if (queryData == null)
                return new GridStats
                {
                    Count = 0
                };

            using var uow = this._uowFactory.GetUnitOfWork();

            EmpaquetadoPickingQuery dbQuery = new EmpaquetadoPickingQuery(queryData.Contenedor, queryData.Preparacion);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        protected virtual EmpaquetadoPickingQueryData GetEmpaquetadoPickingQueryData(ComponentContext context)
        {
            string nuContenedor = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR");
            string nuPreparacion = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION");
            string confInicial = context.GetParameter("CONF_INICIAL");

            if (string.IsNullOrEmpty(confInicial) ||
                string.IsNullOrEmpty(nuContenedor) ||
                string.IsNullOrEmpty(nuPreparacion))
                return null;

            return new EmpaquetadoPickingQueryData
            {
                Contenedor = nuContenedor.ToNumber<int>(),
                Preparacion = nuPreparacion.ToNumber<int>()
            };
        }
    }

    public class EmpaquetadoPickingQueryData
    {
        public int Contenedor { get; set; }
        public int Preparacion { get; set; }
    }
}
