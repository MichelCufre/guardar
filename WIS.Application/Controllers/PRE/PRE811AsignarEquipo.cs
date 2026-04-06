using NLog;
using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.Picking;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.PRE
{
    public class PRE811AsignarEquipo : AppController
    {
        protected readonly Logger _logger;

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE811AsignarEquipo(
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IIdentityService identity)
        {
            this.GridKeys = new List<string>
            {
                "CD_EQUIPO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EQUIPO",SortDirection.Descending)
            };

            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._identity = identity;
            this._logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "AgregarEquipo_grid_1")
            {
                grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_Agregar"));
            }
            else if (grid.Id == "QuitarEquipo_grid_2")
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
                if (grid.Id == "AgregarEquipo_grid_1")
                {
                    var dbQuery = new AgregarEquipoPreferenciaQuery(int.Parse(codigoPreferencia));

                    uow.HandleQuery(dbQuery, false, true);

                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);
                }
                else if (grid.Id == "QuitarEquipo_grid_2")
                {
                    if (!string.IsNullOrEmpty(codigoPreferencia))
                    {
                        Preferencia preferencia = uow.PreferenciaRepository.GetPreferencia(int.Parse(codigoPreferencia));

                        if (preferencia == null)
                            throw new ValidationFailedException("PRE811_Frm1_Error_PreferenciaNoExiste");

                        var dbQuery = new QuitarEquipoPreferenciaQuery(preferencia.NU_PREFERENCIA);

                        uow.HandleQuery(dbQuery, false, true);


                        grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);
                    }
                }
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GridFetchRows");
                throw ex;
            }
            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var count = new GridStats();
            try
            {
                var codigoPreferencia = context.GetParameter("keyPreferencia");
                if (!string.IsNullOrEmpty(codigoPreferencia))
                {
                    if (grid.Id == "AgregarEquipo_grid_1")
                    {
                        var dbQuery = new AgregarEquipoPreferenciaQuery(int.Parse(codigoPreferencia));
                        uow.HandleQuery(dbQuery);
                        dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                        count.Count = dbQuery.GetCount();
                    }
                    else if (grid.Id == "QuitarEquipo_grid_2")
                    {
                        var dbQuery = new QuitarEquipoPreferenciaQuery(int.Parse(codigoPreferencia));
                        uow.HandleQuery(dbQuery);
                        dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                        count.Count = dbQuery.GetCount();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GridFetchStats");
                throw ex;
            }
            return count;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            try
            {
                int nuPreferencia = int.Parse(context.GetParameter("keyPreferencia"));

                if (grid.Id == "AgregarEquipo_grid_1")
                {
                    var dbQuery = new AgregarEquipoPreferenciaQuery(nuPreferencia);
                    uow.HandleQuery(dbQuery);

                    context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
                }
                else if (grid.Id == "QuitarEquipo_grid_2")
                {
                    var dbQuery = new QuitarEquipoPreferenciaQuery(nuPreferencia);
                    uow.HandleQuery(dbQuery);

                    context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex, "GridExportExcel");
                throw ex;
            }
            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "AgregarEquipo_grid_1" && context.ButtonId == "btnAgregar")
                this.ProcesarAgregar(context);
            else if (context.GridId == "QuitarEquipo_grid_2" && context.ButtonId == "btnQuitar")
                this.ProcesarQuitar(context);

            return context;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            if (context.ButtonId == "btnCerrar")
                this._concurrencyControl.ClearToken();

            return base.FormButtonAction(form, context);
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

                List<PreferenciaAsociarEquipo> equipos = new List<PreferenciaAsociarEquipo>();

                var dbQuery = new AgregarEquipoPreferenciaQuery(nuPreferencia);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                if (context.Selection.AllSelected)
                    equipos = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, nuPreferencia);
                else
                    equipos = dbQuery.GetSelectedKeys(context.Selection.Keys, nuPreferencia);

                uow.PreferenciaRepository.AddEquiposPreferencia(equipos);

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
                uow.Rollback();
                throw ex;
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

                List<PreferenciaAsociarEquipo> equipos = new List<PreferenciaAsociarEquipo>();

                var dbQuery = new QuitarEquipoPreferenciaQuery(nuPreferencia);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                if (context.Selection.AllSelected)
                    equipos = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, nuPreferencia);
                else
                    equipos = dbQuery.GetSelectedKeys(context.Selection.Keys, nuPreferencia);

                uow.PreferenciaRepository.RemoveEquiposPreferencia(equipos);

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
