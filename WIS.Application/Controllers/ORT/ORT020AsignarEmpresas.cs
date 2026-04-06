using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.OrdenTarea;
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

namespace WIS.Application.Controllers.ORT
{
    public class ORT020AsignarEmpresas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public ORT020AsignarEmpresas(
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
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            switch (grid.Id)
            {
                case "ORT020AsignarEmpresas_grid_1":
                    grid.MenuItems = new List<IGridItem> {
                        new GridButton("btnAgregar", "General_Sec0_btn_Agregar")
                    };
                    break;

                case "ORT020AsignarEmpresas_grid_2":
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
                case "ORT020AsignarEmpresas_grid_1":
                    this.GridFetchRowsEmpresasNoAsociadas(grid, context, uow);
                    break;
                case "ORT020AsignarEmpresas_grid_2":
                    this.GridFetchRowsEmpresasAsociadas(grid, context, uow);
                    break;
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var idUsuario = this._identity.UserId;
            var cdInsumoManipuleo = context.Parameters.FirstOrDefault(x => x.Id == "cdInsumoManipuleo")?.Value;

            if (string.IsNullOrEmpty(cdInsumoManipuleo))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (grid.Id == "ORT020AsignarEmpresas_grid_1")
            {
                InsumoManipuleoEmpresaQuery dbQuery;
                dbQuery = new InsumoManipuleoEmpresaQuery(idUsuario, cdInsumoManipuleo, false);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                InsumoManipuleoEmpresaQuery dbQuery;
                dbQuery = new InsumoManipuleoEmpresaQuery(idUsuario, cdInsumoManipuleo, true);
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

            var idUsuario = this._identity.UserId;
            var cdInsumoManipuleo = context.Parameters.FirstOrDefault(x => x.Id == "cdInsumoManipuleo")?.Value;

            if (string.IsNullOrEmpty(cdInsumoManipuleo))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            if (grid.Id == "ORT020AsignarEmpresas_grid_1")
            {
                List<SortCommand> defaultSort = new List<SortCommand> { new SortCommand("CD_EMPRESA", SortDirection.Ascending) };

                InsumoManipuleoEmpresaQuery dbQuery;
                dbQuery = new InsumoManipuleoEmpresaQuery(idUsuario, cdInsumoManipuleo, false);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
            else
            {
                List<SortCommand> defaultSort = new List<SortCommand> { new SortCommand("CD_EMPRESA", SortDirection.Ascending) };

                InsumoManipuleoEmpresaQuery dbQuery;
                dbQuery = new InsumoManipuleoEmpresaQuery(idUsuario, cdInsumoManipuleo, true);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoAsignacionInfoValidationModule(uow), grid, row, context);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            return data;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection)
        {
            int idUsuario = this._identity.UserId;
            var cdInsumoManipuleo = selection.Parameters.FirstOrDefault(x => x.Id == "cdInsumoManipuleo")?.Value;

            if (string.IsNullOrEmpty(cdInsumoManipuleo))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            switch (selection.GridId)
            {
                case "ORT020AsignarEmpresas_grid_1":
                    this.AgregarEmpresas(selection, idUsuario, cdInsumoManipuleo);
                    break;
                case "ORT020AsignarEmpresas_grid_2":
                    this.QuitarEmpresas(selection, idUsuario, cdInsumoManipuleo);
                    break;
            }
            return selection;
        }

        #region Metodos Auxiliares

        public virtual Grid GridFetchRowsEmpresasNoAsociadas(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            List<SortCommand> defaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA",SortDirection.Ascending)
            };

            List<string> gridKeys = new List<string> { "CD_EMPRESA" };

            InsumoManipuleoEmpresaQuery dbQuery;

            var idUsuario = this._identity.UserId;
            var cdInsumoManipuleo = context.Parameters.FirstOrDefault(x => x.Id == "cdInsumoManipuleo")?.Value;

            if (string.IsNullOrEmpty(cdInsumoManipuleo))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new InsumoManipuleoEmpresaQuery(idUsuario, cdInsumoManipuleo, false);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, gridKeys);

            return grid;
        }

        public virtual Grid GridFetchRowsEmpresasAsociadas(Grid grid, GridFetchContext context, IUnitOfWork uow)
        {
            List<SortCommand> defaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA",SortDirection.Ascending)
            };

            List<string> gridKeys = new List<string> { "CD_EMPRESA" };

            InsumoManipuleoEmpresaQuery dbQuery;

            var idUsuario = this._identity.UserId;
            var cdInsumoManipuleo = context.Parameters.FirstOrDefault(x => x.Id == "cdInsumoManipuleo")?.Value;

            if (string.IsNullOrEmpty(cdInsumoManipuleo))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            dbQuery = new InsumoManipuleoEmpresaQuery(idUsuario, cdInsumoManipuleo, true);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, gridKeys);

            return grid;
        }

        public virtual void AgregarEmpresas(GridMenuItemActionContext context, int idUsuario, string cdInsumoManipuleo)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("ORT020 Asignar empresas a insumo / manipuleo");
            try
            {
                List<int> empresasSelected = new List<int>();

                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "CD_EMPRESA" });

                selection.ForEach(item =>
                {
                    empresasSelected.Add(int.Parse(item["CD_EMPRESA"]));
                });

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new InsumoManipuleoEmpresaQuery(idUsuario, cdInsumoManipuleo, false);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var empresasList = dbQuery.GetEmpresas();
                    empresasSelected = empresasList.Except(empresasSelected).ToList();
                }


                uow.InsumoManipuleoRepository.AsignarEmpresas(cdInsumoManipuleo, empresasSelected);
                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("ORT020_Sucess_msg_EmpresasActualizadas");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        public virtual void QuitarEmpresas(GridMenuItemActionContext context, int idUsuario, string cdInsumoManipuleo)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("ORT020 Quitar empresas del insumo / manipuleo");
            try
            {
                List<int> empresasSelected = new List<int>();

                List<Dictionary<string, string>> selection = context.Selection.GetSelection(new List<string> { "CD_EMPRESA" });

                selection.ForEach(item =>
                {
                    empresasSelected.Add(int.Parse(item["CD_EMPRESA"]));
                });

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new InsumoManipuleoEmpresaQuery(idUsuario, cdInsumoManipuleo, true);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                    var empresasList = dbQuery.GetEmpresas();
                    empresasSelected = empresasList.Except(empresasSelected).ToList();
                }
                foreach (var pre in empresasSelected)
                {
                    uow.InsumoManipuleoRepository.RemoverEmpresas(cdInsumoManipuleo, empresasSelected);
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification("ORT020_Sucess_msg_EmpresasActualizadas");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        #endregion
    }
}
