using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
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

namespace WIS.Application.Controllers.PRE
{
    public class PRE170AnalisisRechazo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridRechazoKeys { get; }
        protected List<SortCommand> RechazoDefaultSort { get; }

        protected List<string> GridRechazoLpnKeys { get; }
        protected List<SortCommand> RechazoLpnDefaultSort { get; }

        public PRE170AnalisisRechazo(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridRechazoKeys = new List<string>
            {
                "NU_PREPARACION", "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR"
            };

            this.RechazoDefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREPARACION", SortDirection.Descending),
            };

            this.GridRechazoLpnKeys = new List<string>
            {
                "NU_ANALISIS_RECHAZO","NU_PREPARACION", "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR"
            };

            this.RechazoLpnDefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ANALISIS_RECHAZO", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa"), new GridColumnLinkMapping("CD_PRODUTO", "producto") });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            AnalisisRechazoQuery dbQuery;
            AnalisisRechazoLpnQuery dbQueryLpn;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "preparacion")?.Value, out int idPreparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (grid.Id == "PRE170_grid_1")
                {
                    dbQuery = new AnalisisRechazoQuery(idPreparacion);

                    uow.HandleQuery(dbQuery);

                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.RechazoDefaultSort, this.GridRechazoKeys);
                }
                else
                {
                    dbQueryLpn = new AnalisisRechazoLpnQuery(idPreparacion);

                    uow.HandleQuery(dbQueryLpn);

                    grid.Rows = _gridService.GetRows(dbQueryLpn, grid.Columns, context, this.RechazoLpnDefaultSort, this.GridRechazoLpnKeys);
                }

                context.AddParameter("PRE170_NU_PREPARACION", idPreparacion.ToString());

                grid.GetColumn("NU_PREPARACION").Hidden = true;
            }
            else
            {
                if (grid.Id == "PRE170_grid_1")
                {
                    dbQuery = new AnalisisRechazoQuery();

                    uow.HandleQuery(dbQuery);

                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.RechazoDefaultSort, this.GridRechazoKeys);
                }
                else
                {
                    dbQueryLpn = new AnalisisRechazoLpnQuery();

                    uow.HandleQuery(dbQueryLpn);

                    grid.Rows = _gridService.GetRows(dbQueryLpn, grid.Columns, context, this.RechazoLpnDefaultSort, this.GridRechazoLpnKeys);
                }
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            AnalisisRechazoQuery dbQuery;
            AnalisisRechazoLpnQuery dbQueryLpn;

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "preparacion")?.Value, out int idPreparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (grid.Id == "PRE170_grid_1")
                {
                    dbQuery = new AnalisisRechazoQuery(idPreparacion);

                    uow.HandleQuery(dbQuery);

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.RechazoDefaultSort);
                }
                else
                {
                    dbQueryLpn = new AnalisisRechazoLpnQuery(idPreparacion);

                    uow.HandleQuery(dbQueryLpn);

                    return this._excelService.GetExcel(context.FileName, dbQueryLpn, grid.Columns, context, this.RechazoLpnDefaultSort);
                }
            }
            else
            {
                if (grid.Id == "PRE170_grid_1")
                {
                    dbQuery = new AnalisisRechazoQuery();

                    uow.HandleQuery(dbQuery);

                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.RechazoDefaultSort);

                }
                else
                {
                    dbQueryLpn = new AnalisisRechazoLpnQuery();

                    uow.HandleQuery(dbQueryLpn);

                    return this._excelService.GetExcel(context.FileName, dbQueryLpn, grid.Columns, context, this.RechazoLpnDefaultSort);
                }
            }
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            AnalisisRechazoQuery dbQuery;
            AnalisisRechazoLpnQuery dbQueryLpn;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "preparacion")?.Value, out int idPreparacion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (grid.Id == "PRE170_grid_1")
                {
                    dbQuery = new AnalisisRechazoQuery(idPreparacion);

                    uow.HandleQuery(dbQuery);

                    dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);
                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
                else
                {
                    dbQueryLpn = new AnalisisRechazoLpnQuery(idPreparacion);

                    uow.HandleQuery(dbQueryLpn);

                    dbQueryLpn.ApplyFilter(this._filterInterpreter, query.Filters);
                    return new GridStats
                    {
                        Count = dbQueryLpn.GetCount()
                    };
                }
            }
            else
            {
                if (grid.Id == "PRE170_grid_1")
                {
                    dbQuery = new AnalisisRechazoQuery();

                    uow.HandleQuery(dbQuery);

                    dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);
                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
                else
                {
                    dbQueryLpn = new AnalisisRechazoLpnQuery();

                    uow.HandleQuery(dbQueryLpn);

                    dbQueryLpn.ApplyFilter(this._filterInterpreter, query.Filters);
                    return new GridStats
                    {
                        Count = dbQueryLpn.GetCount()
                    };
                }
            }
        }
    }
}
