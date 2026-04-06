using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.StockEntities.Constants;
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

namespace WIS.Application.Controllers.STO
{
    public class STO820PanelTraspasos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public STO820PanelTraspasos(IUnitOfWorkFactory uowFactory, IIdentityService identity, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter, ISecurityService security)
        {
            this.GridKeys = new List<string>
            {
                "NU_TRASPASO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_TRASPASO", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnEditar", "STO820_grid1_btn_Editar", "fas fa-edit"),
                new GridButton("btnDetalles", "STO820_grid1_btn_Detalles", "fas fa-list"),
                new GridButton("btnCancelarTraspaso", "STO820_grid1_btn_CancelarTraspaso", "fas fa-ban"),
                new GridButton("btnAsignarPreparacion", "STO820_grid1_btn_AsignarPreparacion", "fas fa-plus-circle"),
                new GridButton("btnDetPreparacion", "STO820_grid1_btn_DetallePreparacion", "fas fa-list"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TraspasoEmpresasQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                var idTraspaso = long.Parse(row.GetCell("NU_TRASPASO").Value);
                var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);

                row.DisabledButtons.Add("btnEditar");
                row.DisabledButtons.Add("btnDetalles");
                row.DisabledButtons.Add("btnCancelarTraspaso");
                row.DisabledButtons.Add("btnAsignarPreparacion");
                row.DisabledButtons.Add("btnDetPreparacion");

                if (this._security.IsEmpresaAllowed(traspaso.EmpresaOrigen)
                    && this._security.IsEmpresaAllowed(traspaso.EmpresaDestino))
                {
                    if (traspaso.Estado == TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION)
                    {
                        row.DisabledButtons.Remove("btnEditar");
                        row.DisabledButtons.Remove("btnAsignarPreparacion");
                    }
                    else
                    {
                        row.DisabledButtons.Remove("btnDetalles");
                        row.DisabledButtons.Remove("btnDetPreparacion");
                    }

                    if (traspaso.Estado == TraspasoEmpresasDb.ESTADO_TRASPASO_EN_PROCESO)
                        row.DisabledButtons.Remove("btnCancelarTraspaso");

                }
            }

            return grid;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("STO820 Cancelar Traspaso Empresas");
            uow.BeginTransaction();

            if (context.ButtonId == "btnCancelarTraspaso")
            {
                try
                {
                    if (!long.TryParse(context.Row.GetCell("NU_TRASPASO").Value, out long idTraspaso))
                        throw new ValidationFailedException("STO820_Sec0_Error_TraspasoNoValido");

                    CancelarTraspaso(uow, context, idTraspaso);

                    uow.SaveChanges();
                    uow.Commit();

                    context.AddSuccessNotification("STO820_grid1_Success_TraspasoCancelado");
                }
                catch (ValidationFailedException ex)
                {
                    _logger.Error($"Error {ex.Message} - {ex}");
                    context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                    uow.Rollback();
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error {ex.Message} - {ex}");
                    context.AddErrorNotification(ex.Message);
                    uow.Rollback();
                }
            }

            return context;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TraspasoEmpresasQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new TraspasoEmpresasQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}_{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        #region Metodos Auxiliares

        public virtual void CancelarTraspaso(IUnitOfWork uow, GridButtonActionContext context, long idTraspaso)
        {
            var traspaso = uow.TraspasoEmpresasRepository.GetTraspaso(idTraspaso);

            if (!this._security.IsEmpresaAllowed(traspaso.EmpresaOrigen) || !this._security.IsEmpresaAllowed(traspaso.EmpresaDestino))
                throw new ValidationFailedException("STO820_Sec0_Error_EmpresaNoAsociada");

            traspaso.Estado = TraspasoEmpresasDb.ESTADO_TRASPASO_CANCELADO;
            traspaso.FechaModificacion = DateTime.Now;

            uow.TraspasoEmpresasRepository.UpdateTraspaso(traspaso);
        }

        #endregion
    }
}
