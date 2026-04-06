using NLog;
using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
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

namespace WIS.Application.Controllers.PRE
{
    public class PRE811AsignarUsuario : AppController
    {
        protected readonly Logger _logger;

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE811AsignarUsuario(
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IIdentityService identity)
        {
            this.GridKeys = new List<string>
            {
                "USERID",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("USERID",SortDirection.Descending)
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
            if (grid.Id == "AgregarUsuario_grid_1")
            {
                grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_Agregar"));
            }
            else if (grid.Id == "QuitarUsuario_grid_2")
            {
                grid.MenuItems.Add(new GridButton("btnQuitar", "General_Sec0_btn_Quitar"));
            }

            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            try
            {
                int nuPreferencia = int.Parse(context.GetParameter("keyPreferencia"));

                var codigoPreferencia = context.GetParameter("keyPreferencia");
                if (grid.Id == "AgregarUsuario_grid_1")
                {
                    var dbQuery = new AgregarUsuarioPreferenciaQuery(int.Parse(codigoPreferencia));

                    uow.HandleQuery(dbQuery, false, true);

                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);
                }
                else if (grid.Id == "QuitarUsuario_grid_2")
                {
                    if (!string.IsNullOrEmpty(codigoPreferencia))
                    {
                        Preferencia preferencia = uow.PreferenciaRepository.GetPreferencia(int.Parse(codigoPreferencia));

                        if (preferencia == null)
                            throw new ValidationFailedException("PRE811_Frm1_Error_PreferenciaNoExiste");

                        var dbQuery = new QuitarUsuarioPreferenciaQuery(preferencia.NU_PREFERENCIA);

                        uow.HandleQuery(dbQuery, false, true);

                        grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);
                    }
                }
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
            var count = new GridStats();

            var codigoPreferencia = context.GetParameter("keyPreferencia");
            if (!string.IsNullOrEmpty(codigoPreferencia))
            {
                if (grid.Id == "AgregarUsuario_grid_1")
                {
                    var dbQuery = new AgregarUsuarioPreferenciaQuery(int.Parse(codigoPreferencia));
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    count.Count = dbQuery.GetCount();
                }
                else if (grid.Id == "QuitarUsuario_grid_2")
                {
                    var dbQuery = new QuitarUsuarioPreferenciaQuery(int.Parse(codigoPreferencia));
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    count.Count = dbQuery.GetCount();
                }
            }

            return count;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int nuPreferencia = int.Parse(context.GetParameter("keyPreferencia"));

            if (grid.Id == "AgregarUsuario_grid_1")
            {
                var dbQuery = new AgregarUsuarioPreferenciaQuery(nuPreferencia);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else if (grid.Id == "QuitarUsuario_grid_2")
            {
                var dbQuery = new QuitarUsuarioPreferenciaQuery(nuPreferencia);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }


            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "AgregarUsuario_grid_1" && context.ButtonId == "btnAgregar")
                this.ProcesarAgregar(context);
            else if (context.GridId == "QuitarUsuario_grid_2" && context.ButtonId == "btnQuitar")
                this.ProcesarQuitar(context);

            return context;
        }

        #region Metodos Auxiliares

        public virtual void ProcesarAgregar(GridMenuItemActionContext context)
        {

            var codigoPreferencia = context.GetParameter("keyPreferencia");
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {

                int nuPreferencia = int.Parse(codigoPreferencia);

                Preferencia prefrencia = uow.PreferenciaRepository.GetPreferencia(nuPreferencia);

                if (prefrencia == null)
                    throw new ValidationFailedException("PRE811_Frm1_Error_PreferenciaNoExiste");

                List<PreferenciaAsociarUsuario> usuarios = new List<PreferenciaAsociarUsuario>();

                var dbQuery = new AgregarUsuarioPreferenciaQuery(nuPreferencia);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                if (context.Selection.AllSelected)
                    usuarios = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, nuPreferencia);
                else
                    usuarios = dbQuery.GetSelectedKeys(context.Selection.Keys, nuPreferencia);

                uow.PreferenciaRepository.AddUsuariosPreferencia(usuarios);

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ProcesarAgregar");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                uow.Rollback();
            }
        }

        public virtual void ProcesarQuitar(GridMenuItemActionContext context)
        {
            var codigoPreferencia = context.GetParameter("keyPreferencia");
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                int nuPreferencia = int.Parse(codigoPreferencia);

                Preferencia prefrencia = uow.PreferenciaRepository.GetPreferencia(nuPreferencia);

                if (prefrencia == null)
                    throw new ValidationFailedException("PRE811_Frm1_Error_PreferenciaNoExiste");

                List<PreferenciaAsociarUsuario> usuarios = new List<PreferenciaAsociarUsuario>();

                var dbQuery = new QuitarUsuarioPreferenciaQuery(nuPreferencia);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                if (context.Selection.AllSelected)
                    usuarios = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, nuPreferencia);
                else
                    usuarios = dbQuery.GetSelectedKeys(context.Selection.Keys, nuPreferencia);

                uow.PreferenciaRepository.RemoveUsuariosPreferencia(usuarios);

                uow.SaveChanges();
                uow.Commit();
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ProcesarQuitar");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                uow.Rollback();
            }
        }

        #endregion
    }
}
