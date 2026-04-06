using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Eventos;
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

namespace WIS.Application.Controllers.EVT
{
    public class EVT030ModificarGrupo : AppController
    {
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EVT030ModificarGrupo(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService)
        {
            this.GridKeys = new List<string>
            {
                "NU_CONTACTO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_CONTACTO",SortDirection.Descending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "EVT030_grid_Contactos")
            {
                grid.MenuItems = new List<IGridItem> {
                    new GridButton("btnAgregar", "General_Sec0_btn_Agregar"),
                };
            }
            else if (grid.Id == "EVT030_grid_ContactosSel")
            {
                grid.MenuItems = new List<IGridItem> {
                    new GridButton("btnQuitar", "General_Sec0_btn_Quitar")
                };
            }

            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            context.IsEditingEnabled = false;
            context.IsCommitEnabled = false;

            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "EVT030_grid_Contactos")
            {
                if (!int.TryParse(context.GetParameter("grupo"), out int nuGrupo))
                {
                    grid.Rows.Clear();
                    return grid;
                }
                ContactosGruposNoSelQuery dbQuery = new ContactosGruposNoSelQuery(nuGrupo);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, new SortCommand("NU_CONTACTO", SortDirection.Descending), new List<string> { "NU_CONTACTO" });

            }
            else if (grid.Id == "EVT030_grid_ContactosSel")
            {
                if (!int.TryParse(context.GetParameter("grupo"), out int nuGrupo))
                {
                    grid.Rows.Clear();
                    return grid;
                }

                ContactosGruposQuery dbQuery = new ContactosGruposQuery(nuGrupo);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, new List<string> { "NU_CONTACTO", "NU_CONTACTO_GRUPO" });

            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "EVT030_grid_Contactos")
            {
                if (!int.TryParse(context.GetParameter("grupo"), out int nuGrupo))
                    throw new MissingParameterException("General_Sec0_Error_ParametroNoEncontrado");

                ContactosGruposNoSelQuery dbQuery = new ContactosGruposNoSelQuery(nuGrupo);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };

            }
            else if (grid.Id == "EVT030_grid_ContactosSel")
            {
                if (!int.TryParse(context.GetParameter("grupo"), out int nuGrupo))
                    throw new MissingParameterException("General_Sec0_Error_ParametroNoEncontrado");

                ContactosGruposQuery dbQuery = new ContactosGruposQuery(nuGrupo);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int nuGrupo = int.Parse(context.GetParameter("grupo"));

            if (grid.Id == "EVT030_grid_Contactos")
            {
                var dbQuery = new ContactosGruposNoSelQuery(nuGrupo);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else if (grid.Id == "EVT030_grid_ContactosSel")
            {
                var dbQuery = new ContactosGruposQuery(nuGrupo);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (!int.TryParse(context.GetParameter("grupo"), out int nuGrupo))
                throw new ValidationFailedException("EVT030_Grid_Error_GrupoIncorrecto");

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EVT030 Modificar Grupo");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                if (context.ButtonId == "btnAgregar")
                {
                    List<int> keys = this.GetSelectedKeys(uow, nuGrupo, context);

                    foreach (var key in keys)
                    {
                        var nuevoContactoGrupo = new ContactoGrupo();

                        nuevoContactoGrupo.NumeroContactoGrupo = nuGrupo;
                        nuevoContactoGrupo.NumeroContacto = key;
                        nuevoContactoGrupo.FechaAlta = DateTime.Now;
                        nuevoContactoGrupo.NumeroTransaccion = nuTransaccion;

                        uow.DestinatarioRepository.AddContactoToGrupo(nuevoContactoGrupo);
                    }
                }
                else if (context.ButtonId == "btnQuitar")
                {
                    List<int> keys = this.GetSelectedKeysQuitar(uow, nuGrupo, context);

                    foreach (var key in keys)
                    {
                        var contactoGrupo = uow.DestinatarioRepository.GetContactoGrupo(nuGrupo, key);

                        contactoGrupo.FechaModificacion = DateTime.Now;
                        contactoGrupo.NumeroTransaccion = nuTransaccion;
                        contactoGrupo.NumeroTransaccionDelete = nuTransaccion;

                        uow.DestinatarioRepository.UpdateContactoGrupo(contactoGrupo);
                        uow.SaveChanges();

                        uow.DestinatarioRepository.RemoveContactoOfGrupo(contactoGrupo);
                    }
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
            }


            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            if (!int.TryParse(context.GetParameter("grupo"), out int nuGrupo))
                throw new ValidationFailedException("EVT030_Grid_Error_GrupoIncorrecto");

            using var uow = this._uowFactory.GetUnitOfWork();

            GrupoContacto grupo = uow.DestinatarioRepository.GetGrupo(nuGrupo);

            form.GetField("NU_CONTACTO_GRUPO").Value = nuGrupo.ToString();
            form.GetField("NM_GRUPO").Value = grupo.Nombre;

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("EVT030 Modificar Grupo");
            var nuTransaccion = uow.GetTransactionNumber();

            try
            {
                if (context.ButtonId == "btnSubmitModificarGrupo")
                {
                    if (form.IsValid())
                    {

                        var numeroGrupo = int.Parse(form.GetField("NU_CONTACTO_GRUPO").Value);
                        var grupo = uow.DestinatarioRepository.GetGrupo(numeroGrupo);

                        grupo.Nombre = form.GetField("NM_GRUPO").Value;
                        grupo.FechaModificacion = DateTime.Now;
                        grupo.NumeroTransaccion = nuTransaccion;

                        uow.DestinatarioRepository.UpdateGrupo(grupo);

                        uow.SaveChanges();
                        uow.Commit();

                        context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                    }
                    else
                    {
                        context.AddInfoNotification("General_Form_Error_NoValido");
                    }
                }
            }
            catch (Exception ex)
            {
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new EVT030ValidationModule(uow), form, context);

        }

        #region Metodos Auxiliares

        public virtual List<int> GetSelectedKeys(IUnitOfWork uow, int nuGrupo, GridMenuItemActionContext context)
        {
            var keys = new List<int>();

            foreach (var key in context.Selection.Keys)
            {
                keys.Add(int.Parse(key));
            }

            var dbQuery = new ContactosGruposNoSelQuery(nuGrupo);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(keys);

            return dbQuery.GetSelectedKeys(keys);
        }
        
        public virtual List<int> GetSelectedKeysQuitar(IUnitOfWork uow, int nuGrupo, GridMenuItemActionContext context)
        {
            var keys = new List<int>();

            foreach (var key in context.Selection.Keys)
            {
                var numeroContacto = key.Split('$')[0];
                keys.Add(int.Parse(numeroContacto));
            }

            var dbQuery = new ContactosGruposQuery(nuGrupo);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(keys);

            return dbQuery.GetSelectedKeys(keys);
        }

        #endregion
    }
}
