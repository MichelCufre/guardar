using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Liberacion;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
	public class PRE250LiberacionAutomatica : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILiberacionService _liberacionService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE250LiberacionAutomatica(IUnitOfWorkFactory uowFactory, IIdentityService identity, ISecurityService security, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter, ILiberacionService liberacionService)
        {
            this.GridKeys = new List<string>
            {
                "NU_REGLA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_REGLA", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._liberacionService = liberacionService;
        }

        public override PageContext PageLoad(PageContext data)
        {
            return data;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = false;
            context.IsCommitEnabled = true;
            context.IsRollbackEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem>(){
                    new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
                    new GridButton("btnEjecutar", "PRE250_Sec0_btn_Regla", "fa fa-play", new ConfirmMessage("PRE250_Sec0_msg_Confirm")),
                    new GridButton("btnAsignarAgentes", "PRE250_Sec0_btn_AsignarAgentes", "fas fa-list")
                }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var dbQuery = new LiberacionAutomaticaQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                GridCell horaInicio = row.GetCell("HR_INICIO");
                GridCell horaFin = row.GetCell("HR_FIN");

                if (!string.IsNullOrEmpty(horaInicio.Value))
                {
                    horaInicio.Value = TimeSpan.FromMilliseconds(long.Parse(horaInicio.Value)).ToString(@"hh\:mm", this._identity.GetFormatProvider());
                    horaInicio.ForceSetOldValue(horaInicio.Value);
                }
                if (!string.IsNullOrEmpty(horaFin.Value))
                {
                    horaFin.Value = TimeSpan.FromMilliseconds(long.Parse(horaFin.Value)).ToString(@"hh\:mm", this._identity.GetFormatProvider());
                    horaFin.ForceSetOldValue(horaFin.Value);
                }

                if (row.GetCell("FL_ACTIVE").Value == "N")
                    row.DisabledButtons.Add("btnEjecutar");

                row.SetEditableCells(new List<string>
                {
                    "FL_ACTIVE"
                });
            }
            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int nroRegla = int.Parse(context.Row.GetCell("NU_REGLA").Value);

            if (context.ButtonId == "btnEjecutar")
            {
                var regla = uow.LiberacionRepository.GetReglaLiberacion(nroRegla, true);
                _liberacionService.Start(regla, _identity.UserId);
                context.AddSuccessNotification("PRE250_Sec0_msg_Success");
            }

            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Rows.Any())
            {
                foreach (var row in grid.Rows)
                {
                    this.EditarEstadoRegla(uow, row);
                }
            }

            uow.SaveChanges();
            query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new LiberacionAutomaticaQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new LiberacionAutomaticaQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public virtual void EditarEstadoRegla(IUnitOfWork uow, GridRow row)
        {
            if (int.TryParse(row.GetCell("NU_REGLA").Value, out int nuRegla))
            {
                ReglaLiberacion regla = uow.LiberacionRepository.GetReglaLiberacion(nuRegla, false);
                bool activa = !string.IsNullOrEmpty(row.GetCell("FL_ACTIVE").Value) && row.GetCell("FL_ACTIVE").Value == "S";

                if (regla.FlActiva != activa)
                {
                    if (activa)
                        regla.Enable();
                    else
                        regla.Disable();
                }

                uow.LiberacionRepository.UpdateReglaLiberacion(regla);
            }
        }
    }
}
