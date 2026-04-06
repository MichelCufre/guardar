using System;
using System.Collections.Generic;
using WIS.Components.Common;
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
    public class PRE220ControlesDePreparacion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }

        public PRE220ControlesDePreparacion(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PREPARACION","CD_EMPRESA","CD_PRODUTO","CD_FAIXA","NU_IDENTIFICADOR","NU_CONTENEDOR","NU_LOG_CONT_PICKEO"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnDetalles", "PRE220_grid1_btn_Detalles", "fas fa-list"),
                new GridButton("btnLog", "PRE220_grid1_btn_Log", "fas fa-plus-square"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_PREPARACION", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlesDePreaparacionQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, this.GridKeys);

            if (grid.Rows.Count > 0)
                CalcularDatosAdicionales(uow, context);

            return grid;
        }

        public virtual void CalcularDatosAdicionales(IUnitOfWork uow, GridFetchContext context)
        {
            var dbQuery = new ControlesDePreaparacionQuery();

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
            dbQuery.ApplyFilter(this._filterInterpreter, context.ExplicitFilter);

            var fechaInicio = dbQuery.GetMinFechaPrimerControl();
            var fechaFin = dbQuery.GetMaxFechaUltimoControl();
            var diferencia = new TimeSpan(0, 0, 0);

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                diferencia = fechaFin.Value - fechaInicio.Value;
            }

            var formatProvider = this._identity.GetFormatProvider();

            context.AddOrUpdateParameter("fechaInicio", fechaInicio?.ToString(formatProvider));
            context.AddOrUpdateParameter("fechaFin", fechaFin?.ToString(formatProvider));
            context.AddOrUpdateParameter("diferenciaDias", diferencia.Days.ToString());
            context.AddOrUpdateParameter("diferenciaHoras", diferencia.Hours.ToString());
            context.AddOrUpdateParameter("diferenciaMinutos", diferencia.Minutes.ToString());
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ControlesDePreaparacionQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            SortCommand defaultSort = new SortCommand("NU_PREPARACION", SortDirection.Descending);

            using var uow = this._uowFactory.GetUnitOfWork();

            ControlesDePreaparacionQuery dbQuery = new ControlesDePreaparacionQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                switch (context.ButtonId)
                {
                    case "btnDetalles":
                        context.Redirect("/preparacion/PRE130", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "preparacion", Value = context.Row.GetCell("NU_PREPARACION").Value },
                            new ComponentParameter(){ Id = "empresa", Value = context.Row.GetCell("CD_EMPRESA").Value },
                            new ComponentParameter() { Id = "FROM_PRE220", Value = "PRE220" },
                        });
                        break;

                    case "btnLog":
                        context.Redirect("/preparacion/PRE221", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "preparacion", Value = context.Row.GetCell("NU_PREPARACION").Value },
                            new ComponentParameter(){ Id = "empresa", Value = context.Row.GetCell("CD_EMPRESA").Value },
                            new ComponentParameter() { Id = "FROM_PRE220", Value = "PRE220" },
                        });
                        break;

                }
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return context;
        }
    }
}
