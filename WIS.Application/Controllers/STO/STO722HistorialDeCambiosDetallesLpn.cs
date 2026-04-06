using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.STO
{
    public class STO722HistorialDeCambiosDetallesLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;

        protected List<string> Keys { get; }
        protected List<SortCommand> Sorts { get; }

        public STO722HistorialDeCambiosDetallesLpn(
            IGridExcelService excelService,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IIdentityService identity,
            IFilterInterpreter filterInterpreter,
            ISecurityService security)
        {
            this.Keys = new List<string>
            {
                "NU_LOG_SECUENCIA"
            };

            this.Sorts = new List<SortCommand>
            {
                new SortCommand("DT_LOG_ADD_ROW", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._identity = identity;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            int? idDetalleLpn = null;

            if (context.Parameters.Count >= 1)
            {
                idDetalleLpn = int.Parse(context.Parameters.FirstOrDefault(s => s.Id == "idDetalle")?.Value);

                context.AddParameter("tipo", context.Parameters.FirstOrDefault(s => s.Id == "tipo")?.Value);
                context.AddParameter("tipoLpn", context.Parameters.FirstOrDefault(s => s.Id == "tipoLpn")?.Value);
                context.AddParameter("numeroLpn", context.Parameters.FirstOrDefault(s => s.Id == "numeroLpn")?.Value);
                context.AddParameter("codigoEmpresa", context.Parameters.FirstOrDefault(s => s.Id == "codigoEmpresa")?.Value);
                context.AddParameter("empresa", context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value);
                context.AddParameter("idDetalle", context.Parameters.FirstOrDefault(s => s.Id == "idDetalle")?.Value);
            }

            if (grid.Id == "STO722_grid_Detalles")
            {
                var dbQuery = new ConsultaLogsDetalleLpnQuery(idDetalleLpn);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.Keys);
            }
            else if (grid.Id == "STO722_grid_DetallesAtributo")
            {
                var dbQuery = new ConsultaLogsAtributosDetalleLpnQuery(idDetalleLpn);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.Sorts, this.Keys);
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            int? idDetalleLpn = null;

            if (context.Parameters.Count >= 1)
                idDetalleLpn = int.Parse(context.Parameters.FirstOrDefault(s => s.Id == "idDetalle")?.Value);

            if (grid.Id == "STO722_grid_Detalles")
            {
                var dbQuery = new ConsultaLogsDetalleLpnQuery(idDetalleLpn);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "STO722_grid_DetallesAtributo")
            {
                var dbQuery = new ConsultaLogsAtributosDetalleLpnQuery(idDetalleLpn);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            using var uow = this._uowFactory.GetUnitOfWork();
            int? idDetalleLpn = null;

            if (context.Parameters.Count >= 1)
                idDetalleLpn = int.Parse(context.Parameters.FirstOrDefault(s => s.Id == "idDetalle")?.Value);

            if (grid.Id == "STO722_grid_Detalles")
            {
                var dbQuery = new ConsultaLogsDetalleLpnQuery(idDetalleLpn);
                uow.HandleQuery(dbQuery);

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
            }
            else if (grid.Id == "STO722_grid_DetallesAtributo")
            {
                var dbQuery = new ConsultaLogsAtributosDetalleLpnQuery(idDetalleLpn);
                uow.HandleQuery(dbQuery);

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.Sorts);
            }

            return null;
        }
    }
}
