using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.EXP
{
    public class EXP043PedidosPendientesEgreso : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP043PedidosPendientesEgreso(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
               "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA"
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
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnDetalles", "General_Sec0_btn_Detalles", "fas fa-list")

            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }
        
        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PedidosPendPorCamionQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new PedidosPendPorCamionQuery(idCamion);
                uow.HandleQuery(dbQuery);

                grid.GetColumn("CD_CAMION").Hidden = true;
                grid.GetColumn("DS_CAMION").Hidden = true;
                string desCamio = string.IsNullOrEmpty(uow.CamionRepository.GetCamion(idCamion).Descripcion) ? string.Empty : uow.CamionRepository.GetCamion(idCamion).Descripcion;
                query.AddParameter("EXP043_CD_CAMION", idCamion.ToString());
                query.AddParameter("EXP043_DS_CAMION", desCamio);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

                string coso = grid.Rows.FirstOrDefault()?.GetCell("DS_CAMION").Value;
            }
            else
            {
                dbQuery = new PedidosPendPorCamionQuery();

                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }

            return grid;
        }
        
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PedidosPendPorCamionQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new PedidosPendPorCamionQuery(idCamion);
                uow.HandleQuery(dbQuery);
            }
            else
            {
                dbQuery = new PedidosPendPorCamionQuery();

                uow.HandleQuery(dbQuery);
            }

            query.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PedidosPendPorCamionQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new PedidosPendPorCamionQuery(idCamion);
                uow.HandleQuery(dbQuery);
            }
            else
            {
                dbQuery = new PedidosPendPorCamionQuery();

                uow.HandleQuery(dbQuery);
            }
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext query)
        {

            if (query.ButtonId == "btnDetalles")
            {
                query.Redirect("/preparacion/PRE150", new List<ComponentParameter> {
                    new ComponentParameter("pedido", query.Row.GetCell("NU_PEDIDO").Value),
                    new ComponentParameter("cliente", query.Row.GetCell("CD_CLIENTE").Value),
                    new ComponentParameter("empresa", query.Row.GetCell("CD_EMPRESA").Value)
                });
            }
            return query;
        }
    }
}
