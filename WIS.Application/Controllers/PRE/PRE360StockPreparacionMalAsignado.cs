using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
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
    public class PRE360StockPreparacionMalAsignado : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE360StockPreparacionMalAsignado(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
               "CD_ENDERECO", "CD_PRODUTO", "CD_EMPRESA", "PICKING_ASIGNADO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO", SortDirection.Descending),
                new SortCommand("CD_PRODUTO", SortDirection.Descending),
                new SortCommand("CD_EMPRESA", SortDirection.Descending),
                new SortCommand("PICKING_ASIGNADO", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {

            context.AddLink("CD_ENDERECO", "registro/REG040", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_ENDERECO", "ubicacion") });

            context.AddLink("PICKING_ASIGNADO", "registro/REG040", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("PICKING_ASIGNADO", "ubicacion") });

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {

            using var uow = this._uowFactory.GetUnitOfWork();

            StockPickingMalAsignadoQuery dbQuery;
            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string producto = context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

                string ubicacion = context.Parameters.FirstOrDefault(s => s.Id == "ubicacion")?.Value;

                dbQuery = new StockPickingMalAsignadoQuery(idEmpresa, ubicacion, producto);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                string nomEmpresa = uow.EmpresaRepository.GetNombre(idEmpresa);
                string descProd = uow.ProductoRepository.GetDescripcion(idEmpresa, producto);

                context.AddParameter("PRE360_EMPRESA", idEmpresa.ToString() + " - " + nomEmpresa);
                context.AddParameter("PRE360_PRODUCTO", producto + " - " + descProd);
                context.AddParameter("PRE360_UBICACION", ubicacion);

                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;
                grid.GetColumn("CD_PRODUTO").Hidden = true;
                grid.GetColumn("CD_ENDERECO").Hidden = true;
            }
            else
            {
                dbQuery = new StockPickingMalAsignadoQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.GetColumn("CD_EMPRESA").Hidden = false;
                grid.GetColumn("NM_EMPRESA").Hidden = false;
                grid.GetColumn("CD_PRODUTO").Hidden = false;
                grid.GetColumn("CD_ENDERECO").Hidden = false;
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockPickingMalAsignadoQuery dbQuery;
            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string producto = context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

                string ubicacion = context.Parameters.FirstOrDefault(s => s.Id == "ubicacion")?.Value;

                dbQuery = new StockPickingMalAsignadoQuery(idEmpresa, ubicacion, producto);

            }
            else
            {
                dbQuery = new StockPickingMalAsignadoQuery();

            }

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            StockPickingMalAsignadoQuery dbQuery;
            if (context.Parameters.Count > 2)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa")?.Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string producto = context.Parameters.FirstOrDefault(s => s.Id == "producto")?.Value;

                string ubicacion = context.Parameters.FirstOrDefault(s => s.Id == "ubicacion")?.Value;

                dbQuery = new StockPickingMalAsignadoQuery(idEmpresa, ubicacion, producto);

            }
            else
            {
                dbQuery = new StockPickingMalAsignadoQuery();

            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
    }
}
