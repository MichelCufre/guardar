using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Expedicion;
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

namespace WIS.Application.Controllers.EXP
{
    public class EXP051ProductosProblema : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP051ProductosProblema(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR", "CD_CAMION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO",SortDirection.Ascending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.AddLink("CD_AGENTE", "registro/REG220", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_CLIENTE", "cliente"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            query.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            query.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            return this.GridFetchRows(grid, query.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProductosProblemasQuery dbQuery;

            if (query.Parameters.Count > 2)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "respetaOrden")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string respetaOrden = query.Parameters.FirstOrDefault(s => s.Id == "respetaOrden").Value;

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "pedido")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idPedido = query.Parameters.FirstOrDefault(s => s.Id == "pedido").Value;

                dbQuery = new ProductosProblemasQuery(idPedido, idCamion, respetaOrden);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                grid.GetColumn("CD_CAMION").Hidden = true;
                grid.GetColumn("NU_PEDIDO").Hidden = true;

                Camion cam = uow.CamionRepository.GetCamion(idCamion);

                query.AddParameter("EXP051_NU_PEDIDO", idPedido);
                query.AddParameter("EXP051_CD_CAMION", cam.Id.ToString() + " - " + cam.Descripcion);

            }
            else
            {

                dbQuery = new ProductosProblemasQuery();

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }

            this.SetColorRow(uow, grid, query);

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProductosProblemasQuery dbQuery;

            if (query.Parameters.Count > 2)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "respetaorden")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string respetaOrden = query.Parameters.FirstOrDefault(s => s.Id == "respetaorden").Value;

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "pedido")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idPedido = query.Parameters.FirstOrDefault(s => s.Id == "pedido").Value;

                dbQuery = new ProductosProblemasQuery(idPedido, idCamion, respetaOrden);


            }
            else
            {
                dbQuery = new ProductosProblemasQuery();
            }

            uow.HandleQuery(dbQuery);
            query.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ProductosProblemasQuery dbQuery;

            if (query.Parameters.Count > 2)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "respetaorden")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string respetaOrden = query.Parameters.FirstOrDefault(s => s.Id == "respetaorden").Value;

                if (string.IsNullOrEmpty(query.Parameters.FirstOrDefault(s => s.Id == "pedido")?.Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string idPedido = query.Parameters.FirstOrDefault(s => s.Id == "pedido").Value;

                dbQuery = new ProductosProblemasQuery(idPedido, idCamion, respetaOrden);


            }
            else
            {
                dbQuery = new ProductosProblemasQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public virtual void SetColorRow(IUnitOfWork uow, Grid grid, GridFetchContext query)
        {
            foreach (var row in grid.Rows)
            {
                GridCell pendAsignar = row.GetCell("FL_PEND_ASIGNAR");
                GridCell pendLiberar = row.GetCell("FL_PEND_LIBERAR");

                if (pendLiberar.Value.Equals("S"))
                {
                    row.CssClass = row.CssClass + " pendLiberar";
                }

                if (query.Parameters.Any(z => z.Id == "fromEXP013") && pendAsignar.Value.Equals("S"))
                {
                    row.CssClass = row.CssClass + " pendAsignar";
                }
            }
        }
    }
}
