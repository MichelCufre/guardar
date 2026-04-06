using NLog;
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
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE811PanelDePreferencias : AppController
    {
        protected readonly Logger _logger;

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE811PanelDePreferencias(
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IIdentityService identity)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREFERENCIA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREFERENCIA",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._identity = identity;
            this._logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton> { new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"), }));

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>()
            {
                new GridButton("btnAsignarUsuarios", "PRE811_grid1_btn_AsignarUsuarios", "fas fa-wrench"),
                new GridButton("btnAsignarEquipos", "PRE811_grid1_btn_AsignarEquipos", "fas fa-wrench"),
                new GridButton("btnConfiguracion", "PRE811_grid1_btn_Configuracion", "fas fa-gear"),
            }));

            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                PanelPreferenciasQuery dbQuery;

                if (context.Parameters.Count > 0)
                {
                    if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "nuPreferencia").Value))
                        throw new ValidationFailedException("General_Sec0_Error_ParametrosURI");

                    string nuPreferencia = context.Parameters.FirstOrDefault(s => s.Id == "nuPreferencia").Value;

                    dbQuery = new PanelPreferenciasQuery(int.Parse(nuPreferencia));
                }
                else
                {
                    dbQuery = new PanelPreferenciasQuery();
                }

                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message);
            }
            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PanelPreferenciasQuery dbQuery;
            if (context.Parameters.Count > 0)
            {
                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "nuPreferencia").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string nuPreferencia = context.Parameters.FirstOrDefault(s => s.Id == "nuPreferencia").Value;

                dbQuery = new PanelPreferenciasQuery(int.Parse(nuPreferencia));
            }
            else
            {
                dbQuery = new PanelPreferenciasQuery();
            }

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PanelPreferenciasQuery dbQuery;
            if (context.Parameters.Count > 0)
            {
                if (string.IsNullOrEmpty(context.Parameters.FirstOrDefault(s => s.Id == "nuPreferencia").Value))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                string nuPreferencia = context.Parameters.FirstOrDefault(s => s.Id == "nuPreferencia").Value;

                dbQuery = new PanelPreferenciasQuery(int.Parse(nuPreferencia));
            }
            else
            {
                dbQuery = new PanelPreferenciasQuery();
            }

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
    }
}
