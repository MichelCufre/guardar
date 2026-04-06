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

namespace WIS.Application.Controllers.PRE
{
    public class PRE811PreferenciaCtrlAcceso : AppController
    {
        protected readonly Logger _logger;

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE811PreferenciaCtrlAcceso(
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IIdentityService identity)
        {
            this.GridKeys = new List<string>
            {
                "CD_CONTROL_ACCESO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_CONTROL_ACCESO",SortDirection.Descending)
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
            if (grid.Id == "AgregarControlAcceso_grid_1")
            {
                grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_Agregar"));
            }
            else if (grid.Id == "QuitarControlAcceso_grid_2")
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
                var codigoPreferencia = context.GetParameter("keyPreferencia");
                if (grid.Id == "AgregarControlAcceso_grid_1")
                {
                    var dbQuery = new AgregarControlAccesoPreferenciaQuery(int.Parse(codigoPreferencia));

                    uow.HandleQuery(dbQuery, false, true, true);

                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, DefaultSort, this.GridKeys);
                }
                else if (grid.Id == "QuitarControlAcceso_grid_2")
                {
                    if (!string.IsNullOrEmpty(codigoPreferencia))
                    {
                        Preferencia preferencia = uow.PreferenciaRepository.GetPreferencia(int.Parse(codigoPreferencia));

                        if (preferencia == null)
                            throw new ValidationFailedException("PRE811_Frm1_Error_PreferenciaNoExiste");

                        var dbQuery = new QuitarControlAccesoPreferenciaQuery(preferencia.NU_PREFERENCIA);

                        uow.HandleQuery(dbQuery, false, true, true);

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

            var codigoPreferencia = context.GetParameter("keyPreferencia");
            if (!string.IsNullOrEmpty(codigoPreferencia))
            {
                if (grid.Id == "AgregarControlAcceso_grid_1")
                {
                    var dbQuery = new AgregarControlAccesoPreferenciaQuery(int.Parse(codigoPreferencia));
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
                else if (grid.Id == "QuitarControlAcceso_grid_2")
                {
                    var dbQuery = new QuitarControlAccesoPreferenciaQuery(int.Parse(codigoPreferencia));
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    return new GridStats
                    {
                        Count = dbQuery.GetCount()
                    };
                }
            }
            return null;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int nuPreferencia = int.Parse(context.GetParameter("keyPreferencia"));

            if (grid.Id == "AgregarControlAcceso_grid_1")
            {
                var dbQuery = new AgregarControlAccesoPreferenciaQuery(nuPreferencia);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else if (grid.Id == "QuitarControlAcceso_grid_2")
            {
                var dbQuery = new QuitarControlAccesoPreferenciaQuery(nuPreferencia);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }

            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "AgregarControlAcceso_grid_1" && context.ButtonId == "btnAgregar")
            {
                this.ProcesarAgregar(context);
            }
            else if (context.GridId == "QuitarControlAcceso_grid_2" && context.ButtonId == "btnQuitar")
            {
                this.ProcesarQuitar(context);
            }

            return context;
        }


        public override Form FormInitialize(Form form, FormInitializeContext context)
        {

            using var uow = this._uowFactory.GetUnitOfWork();

            if (form.Id == "PRE811CoAc_form_1")
            {
                FormField pedidoCompletoTogle = form.GetField("habilitarPedCompleto");

                var codigoPreferencia = context.GetParameter("keyPreferencia");

                if (!string.IsNullOrEmpty(codigoPreferencia))
                {
                    Preferencia preferencia = uow.PreferenciaRepository.GetPreferencia(int.Parse(codigoPreferencia));

                    if (preferencia == null)
                        throw new EntityNotFoundException("PRE811_Frm1_Error_PreferenciaNoExiste");

                    pedidoCompletoTogle.Value = preferencia.FL_HABILITADO_PEDIDO_COMPLETO;
                }
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();

            try
            {
                if (form.Id == "PRE811CoAc_form_1")
                {
                    var codigoPreferencia = context.GetParameter("keyPreferencia");

                    Preferencia preferencia = uow.PreferenciaRepository.GetPreferencia(int.Parse(codigoPreferencia));

                    if (preferencia == null)
                        throw new ValidationFailedException("PRE811_Frm1_Error_PreferenciaNoExiste");

                    if (form.GetField("habilitarPedCompleto").Value == "true")
                        preferencia.FL_HABILITADO_PEDIDO_COMPLETO = "S";
                    else
                        preferencia.FL_HABILITADO_PEDIDO_COMPLETO = "N";

                    uow.PreferenciaRepository.UpdatePreferencia(preferencia);

                    uow.SaveChanges();

                    context.AddSuccessNotification("PRE811CoAc_Frm1_Succes_Edicion", new List<string> { preferencia.NU_PREFERENCIA.ToString() });
                }

                uow.SaveChanges();
                uow.Commit();

            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ProcesarQuitar");
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            return form;
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

                List<PreferenciaAsociarContolAcceso> ctrlAcceso = new List<PreferenciaAsociarContolAcceso>();

                var dbQuery = new AgregarControlAccesoPreferenciaQuery(nuPreferencia);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                if (context.Selection.AllSelected)
                    ctrlAcceso = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, nuPreferencia);
                else
                    ctrlAcceso = dbQuery.GetSelectedKeys(context.Selection.Keys, nuPreferencia);

                uow.PreferenciaRepository.AddControlAccesoPreferencia(ctrlAcceso);
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
                context.AddErrorNotification("General_Sec0_Error_Operacion");
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

                List<PreferenciaAsociarContolAcceso> ctrlAcceso = new List<PreferenciaAsociarContolAcceso>();

                var dbQuery = new QuitarControlAccesoPreferenciaQuery(nuPreferencia);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                if (context.Selection.AllSelected)
                    ctrlAcceso = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys, nuPreferencia);
                else
                    ctrlAcceso = dbQuery.GetSelectedKeys(context.Selection.Keys, nuPreferencia);

                uow.PreferenciaRepository.RemoveControlAccesoPreferencia(ctrlAcceso);
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
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }
        }

        #endregion
    }
}
