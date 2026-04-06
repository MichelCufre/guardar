using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC280PanelDeSugerencias : AppController
    {

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<REC280PanelDeSugerencias> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC280PanelDeSugerencias(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REC280PanelDeSugerencias> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_ALM_ESTRATEGIA", "NU_PREDIO", "TP_ALM_OPERATIVA_ASOCIABLE", "CD_ALM_OPERATIVA_ASOCIABLE", "CD_CLASSE", "CD_GRUPO", "CD_EMPRESA_PRODUTO", "CD_PRODUTO", "CD_REFERENCIA", "CD_AGRUPADOR", "CD_ENDERECO_SUGERIDO","NU_ALM_SUGERENCIA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        #region GRID
        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsEditingEnabled = false;
            query.IsRemoveEnabled = false;
            query.IsAddEnabled = false;
            query.IsCommitEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnDetalles", "REC280_grid1_btn_Detalles", "fas fa-list"),
            }));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PanelDeSugerenciasQuery dbQuery = new PanelDeSugerenciasQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            DisableButtons(grid, uow);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            PanelDeSugerenciasQuery dbQuery = new PanelDeSugerenciasQuery();
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

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            PanelDeSugerenciasQuery dbQuery = new PanelDeSugerenciasQuery();
            uow.HandleQuery(dbQuery);

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
        #endregion

        #region AUX
        public virtual void DisableButtons(Grid grid, IUnitOfWork uow)
        {
            foreach (GridRow row in grid.Rows)
            {
                int estrategia = int.Parse(row.GetCell("NU_ALM_ESTRATEGIA").Value);
                var predio = row.GetCell("NU_PREDIO").Value;
                var tipoOperativa = row.GetCell("TP_ALM_OPERATIVA_ASOCIABLE").Value;
                var codigoOperativa = row.GetCell("CD_ALM_OPERATIVA_ASOCIABLE").Value;
                var codigoClase = row.GetCell("CD_CLASSE").Value;
                var codigoGrupo = row.GetCell("CD_GRUPO").Value;
                var empresa = int.Parse(row.GetCell("CD_EMPRESA_PRODUTO").Value);
                var producto = row.GetCell("CD_PRODUTO").Value;
                var codigoReferencia = row.GetCell("CD_REFERENCIA").Value;
                var codigoAgrupador = row.GetCell("CD_AGRUPADOR").Value;
                var enderecoSugerido = row.GetCell("CD_ENDERECO_SUGERIDO").Value;

                if (!uow.EstrategiaRepository.AnyDetalleSugerencia(estrategia, predio, tipoOperativa, codigoOperativa,
                    codigoClase, codigoGrupo, empresa, producto, codigoReferencia, codigoAgrupador, enderecoSugerido))
                    row.DisabledButtons.Add("btnDetalles");
            }
        }
        #endregion
    }
}
