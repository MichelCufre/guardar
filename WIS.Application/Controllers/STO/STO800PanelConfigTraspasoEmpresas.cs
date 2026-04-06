using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
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
    public class STO800PanelConfigTraspasoEmpresas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public STO800PanelConfigTraspasoEmpresas(IUnitOfWorkFactory uowFactory, IIdentityService identity, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_TRASPASO_CONFIGURACION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_TRASPASO_CONFIGURACION", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = true;
            context.IsRemoveEnabled = true;
            context.IsAddEnabled = false;
            context.IsCommitEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnEditar", "STO800_grid1_btn_Editar", "fas fa-edit"),
                new GridButton("btnAgregarEmpresasDestino", "STO800_grid1_btn_AgregarEmpresaDestino", "fas fa-list"),
                new GridButton("btnAgregarTiposTraspaso", "STO800_grid1_btn_AgregarTipoTraspaso", "fas fa-list")
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ConfiguracionTraspasoEmpresasQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                var idConfig = long.Parse(row.GetCell("NU_TRASPASO_CONFIGURACION").Value);
                var config = uow.TraspasoEmpresasRepository.GetConfiguracionTraspaso(idConfig);

                if (config.TodaEmpresa)
                    row.DisabledButtons.Add("btnAgregarEmpresasDestino");

                if (config.TodoTipoTraspaso)
                    row.DisabledButtons.Add("btnAgregarTiposTraspaso");
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ConfiguracionTraspasoEmpresasQuery();

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

            var dbQuery = new ConfiguracionTraspasoEmpresasQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = $"{this._identity.Application}_{DateTime.Now:yyyy-MM-dd_HH:mm}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("STO800 Eliminación Configuración Traspaso Empresas");
            uow.BeginTransaction();

            try
            {
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        if (row.IsDeleted)
                        {
                            var idConfig = long.Parse(row.GetCell("NU_TRASPASO_CONFIGURACION").Value);
                            var empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);

                            if (!uow.TraspasoEmpresasRepository.AnyConfiguracionTraspaso(idConfig))
                                throw new ValidationFailedException("STO800_Sec0_Error_ConfigNoExiste");

                            if (uow.TraspasoEmpresasRepository.AnyTraspasoActivoEmpresa(empresa))
                                throw new ValidationFailedException("STO800_Sec0_Error_EmpresaConTraspasoEmpresa", new string[] { empresa.ToString() });

                            DeleteConfiguracion(uow, idConfig);
                        }
                    }
                }

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
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

            return grid;
        }

        public virtual void DeleteConfiguracion(IUnitOfWork uow, long idConfig)
        {
            uow.TraspasoEmpresasRepository.RemoverEmpresasDestino(idConfig);
            uow.TraspasoEmpresasRepository.RemoverTiposTraspaso(idConfig);

            uow.SaveChanges();

            uow.TraspasoEmpresasRepository.DeleteConfiguracionTraspaso(idConfig);
        }
    }
}
