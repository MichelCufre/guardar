using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.STO
{
    public class STO800AgregarTiposTraspaso : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public STO800AgregarTiposTraspaso(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;

            this.DefaultSort = new List<SortCommand> { new SortCommand("TP_TRASPASO", SortDirection.Ascending) };
            this.GridKeys = new List<string> { "TP_TRASPASO" };
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            switch (grid.Id)
            {
                case "STO800AsignarTiposTraspaso_grid_1":
                    grid.MenuItems = new List<IGridItem> {
                        new GridButton("btnAgregar", "General_Sec0_btn_Agregar")
                    };
                    break;

                case "STO800AsignarTiposTraspaso_grid_2":
                    grid.MenuItems = new List<IGridItem> {
                        new GridButton("btnQuitar", "General_Sec0_btn_Quitar")
                    };
                    break;
            }
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            switch (grid.Id)
            {
                case "STO800AsignarTiposTraspaso_grid_1":
                    this.GridFetchRowsTiposTraspasoNoAsociados(grid, context, uow);
                    break;
                case "STO800AsignarTiposTraspaso_grid_2":
                    this.GridFetchRowsTiposTraspasoAsociados(grid, context, uow);
                    break;
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!long.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idConfig")?.Value, out long idConfig))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (grid.Id == "STO800AsignarTiposTraspaso_grid_1")
            {
                ConfiguracionTraspasoTiposTraspasoQuery dbQuery;
                dbQuery = new ConfiguracionTraspasoTiposTraspasoQuery(idConfig, false);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                ConfiguracionTraspasoTiposTraspasoQuery dbQuery;
                dbQuery = new ConfiguracionTraspasoTiposTraspasoQuery(idConfig, true);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!long.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idConfig")?.Value, out long idConfig))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (grid.Id == "STO800AsignarTiposTraspaso_grid_1")
            {
                ConfiguracionTraspasoTiposTraspasoQuery dbQuery;
                dbQuery = new ConfiguracionTraspasoTiposTraspasoQuery(idConfig, false);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else
            {
                ConfiguracionTraspasoTiposTraspasoQuery dbQuery;
                dbQuery = new ConfiguracionTraspasoTiposTraspasoQuery(idConfig, true);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoAsignacionInfoValidationModule(uow), grid, row, context);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection)
        {
            if (!long.TryParse(selection.Parameters.FirstOrDefault(x => x.Id == "idConfig")?.Value, out long idConfig))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            switch (selection.GridId)
            {
                case "STO800AsignarTiposTraspaso_grid_1":
                    this.AgregarTiposTraspaso(selection, idConfig);
                    break;
                case "STO800AsignarTiposTraspaso_grid_2":
                    this.QuitarTiposTraspaso(selection, idConfig);
                    break;
            }
            return selection;
        }

        #region Metodos Auxiliares

        public virtual Grid GridFetchRowsTiposTraspasoNoAsociados(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            ConfiguracionTraspasoTiposTraspasoQuery dbQuery;

            if (!long.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idConfig")?.Value, out long idConfig))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new ConfiguracionTraspasoTiposTraspasoQuery(idConfig, false);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public virtual Grid GridFetchRowsTiposTraspasoAsociados(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            ConfiguracionTraspasoTiposTraspasoQuery dbQuery;

            if (!long.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "idConfig")?.Value, out long idConfig))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new ConfiguracionTraspasoTiposTraspasoQuery(idConfig, true);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public virtual void AgregarTiposTraspaso(GridMenuItemActionContext context, long idConfig)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("STO800 Agregar tipos de traspaso");
            try
            {
                List<string> tiposSelected = new List<string>();

                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "TP_TRASPASO" });

                selection.ForEach(item =>
                {
                    tiposSelected.Add(item["TP_TRASPASO"]);
                });

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new ConfiguracionTraspasoTiposTraspasoQuery(idConfig, false);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var tiposTraspasoList = dbQuery.GetTiposTraspaso();
                    tiposSelected = tiposTraspasoList.Except(tiposSelected).ToList();
                }


                uow.TraspasoEmpresasRepository.AgregarTiposTraspaso(idConfig, tiposSelected);
                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("STO800_Sucess_msg_TiposTraspasoActualizados");
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

        public virtual void QuitarTiposTraspaso(GridMenuItemActionContext context, long idConfig)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("STO800 Quitar tipos de traspaso");
            try
            {
                List<string> tiposSelected = new List<string>();

                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "TP_TRASPASO" });

                selection.ForEach(item =>
                {
                    tiposSelected.Add(item["TP_TRASPASO"]);
                });

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new ConfiguracionTraspasoTiposTraspasoQuery(idConfig, true);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var tiposTraspasoList = dbQuery.GetTiposTraspaso();
                    tiposSelected = tiposTraspasoList.Except(tiposSelected).ToList();
                }
                foreach (var pre in tiposSelected)
                {
                    uow.TraspasoEmpresasRepository.RemoverTiposTraspaso(idConfig, tiposSelected);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("STO800_Sucess_msg_TiposTraspasoActualizados");
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

        #endregion
    }
}
