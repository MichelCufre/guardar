using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Parametrizacion;
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

namespace WIS.Application.Controllers.PAR
{
    public class PAR400Atributos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<PAR400Atributos> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PAR400Atributos(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<PAR400Atributos> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "ID_ATRIBUTO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("ID_ATRIBUTO", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;
            query.IsCommitEnabled = true;

            using var uow = this._uowFactory.GetUnitOfWork();

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                    new GridButton("btnAsociar", "PAR400_frm1_btn_Asociar", "fas fa-bezier-curve"),
                    new GridButton("btnEditar",  "PAR400_frm1_btn_MODIFICAR", "fas fa-edit"),
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {

                        if (row.IsDeleted)
                        {
                            var codigoAtributo = int.Parse(row.GetCell("ID_ATRIBUTO").Value);

                            if (uow.AtributoRepository.AnyLpnTipoAsociadaAtributo(codigoAtributo) || uow.AtributoRepository.AnyValidacionAsociadaAtributo(codigoAtributo))
                            {
                                query.AddErrorNotification("PAR400_Sec0_Error_AtributoTieneAsociaciones");
                                return grid;
                            }
                            else
                            {
                                var atributo = uow.AtributoRepository.GetAtributo(codigoAtributo);
                                uow.AtributoRepository.DeleteAtributo(atributo);
                            }
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PAR400GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new AtributosQuery();

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            DisableButtons(grid.Rows, uow);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new AtributosQuery();

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

            var dbQuery = new AtributosQuery();

            uow.HandleQuery(dbQuery);
            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public virtual void DisableButtons(List<GridRow> rows, IUnitOfWork uow)
        {
            foreach (var row in rows)
            {
                int codigoAtributo = int.Parse(row.GetCell("ID_ATRIBUTO").Value);

                if (uow.AtributoRepository.AnyLpnTipoAsociadaAtributo(codigoAtributo))
                    row.DisabledButtons.Add("btnEditar");

                if (uow.AtributoRepository.AnyValidacionAsociadaAtributo(codigoAtributo))
                    row.DisabledButtons.Add("btnEditar");
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            return context;
        }
    }
}
