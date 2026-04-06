using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.CheckboxListComponent;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Seguridad;
using WIS.Domain.Security;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.SEG
{
    public class SEG030PanelUsuarios : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;
        protected readonly ITrackingService _trackingService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public SEG030PanelUsuarios(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IFormValidationService formValidationService,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security,
            ITrackingService trackingService)
        {
            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("USERID",SortDirection.Ascending)
            };

            this.GridKeys = new List<string>
            {
                "USERID"
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._formValidationService = formValidationService;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
            this._trackingService = trackingService;
        }

        public override PageContext PageLoad(PageContext data)
        {
            return base.PageLoad(data);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = GetEditableCells().Count > 0;
            context.IsCommitEnabled = context.IsEditingEnabled;
            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_ARRAY", new List<IGridItem>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "fas fa-edit"),
                new GridButton("btnAsignarPerfiles", "SEG030_Sec0_btn_Asignar", "fas fa-plus-square"),
                new GridButton("btnConfiEmpre", "SEG030_Sec0_btn_confEmp", "fas fa-list-ul"),
            }));

            grid.MenuItems = new List<IGridItem> 
            {
                    new GridButton("btnImprimir", "IMP050_grid1_btn_imprimir")
            };

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new UsuariosQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            grid.SetEditableCells(GetEditableCells());

            foreach (var row in grid.Rows)
            {
                if (!this._security.IsUserAllowed(SecurityResources.SEG030_Sec0_btn_EditarUsuario))
                    row.DisabledButtons.Add("btnEditar");

                if (!this._security.IsUserAllowed(SecurityResources.SEG030Recursos_Page_Access_Allow))
                    row.DisabledButtons.Add("btnAsignarPerfiles");

                if (!this._security.IsUserAllowed(SecurityResources.SEG030Asignar_Page_Access_Allow))
                    row.DisabledButtons.Add("btnConfiEmpre");
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new UsuariosQuery();
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new UsuariosQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<string> filasSeleccionadas = this.ObtenerKeyLineasSeleccionadas(uow, context);

            context.AddParameter("ListaFilasSeleccionadas", JsonConvert.SerializeObject(filasSeleccionadas));

            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            foreach (var row in grid.Rows)
            {
                if (row.GetCell("ISENABLED").Old != row.GetCell("ISENABLED").Value)
                    uow.SecurityRepository.CambiarEstadoUsuario(int.Parse(row.GetCell("USERID").Value));

            }

            uow.SaveChanges();
            context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");

            return grid;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.Find(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                throw new MissingParameterException("SEC030_Sec0_Error_NoSePudoObtenerUsuario");

            this.Initialize(uow, form, context, idUsuario);

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("SEG030PanelUsuarios - UpdateUsuario");
            uow.BeginTransaction();

            try
            {
                if (!int.TryParse(context.Parameters.Find(x => x.Id == "idUsuario")?.Value, out int idUsuario))
                    throw new ValidationFailedException("SEC030_Sec0_Error_NoSePudoObtenerUsuario");

                var usuario = uow.SecurityRepository.GetUsuario(idUsuario);
                if (usuario == null)
                    throw new ValidationFailedException("SEC030_Sec0_Error_NoSePudoObtenerUsuario");

                string asignarAutoEmpresas = form.GetField("asignarAutoEmpresas")?.Value != "S" ? "N" : "S";

                List<CheckboxListItem> listaPerfiles = JsonConvert.DeserializeObject<List<CheckboxListItem>>(context.Parameters.FirstOrDefault(s => s.Id == "listaPerfiles").Value);

                this.UpdateUsuario(uow, usuario, listaPerfiles, asignarAutoEmpresas);

                uow.Commit();
                context.AddSuccessNotification("SEG030_Sec0_error_usuarioActualizado");
            }
            catch (ValidationFailedException ex)
            {
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                uow.Rollback();
            }
            catch (Exception ex)
            {
                context.AddErrorNotification(ex.Message);
                uow.Rollback();
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new CreateUsuarioValidationModule(uow, this._identity.UserId, this._identity.Application), form, context);
        }

        #region Metodos auxiliares

        public virtual void Initialize(IUnitOfWork uow, Form form, FormInitializeContext context, int idUsuario)
        {
            Usuario usuario = uow.SecurityRepository.GetUsuario(idUsuario);

            if (usuario == null)
                throw new ValidationFailedException("SEC030_Sec0_Error_NoSePudoObtenerUsuario");

            form.GetField("nomUsuario").Value = usuario.Username;
            form.GetField("nomUsuario").ReadOnly = true;

            form.GetField("nomCompleto").Value = usuario.Name;
            form.GetField("nomCompleto").ReadOnly = true;

            form.GetField("email").Value = usuario.Email;
            form.GetField("email").ReadOnly = true;

            //Idioma
            FormField idiomaSelect = form.GetField("idioma");
            var idioma = uow.LocalizationRepository.GetIdioma(usuario.Language);
            idiomaSelect.Options = new List<SelectOption> { new SelectOption(idioma.Valor, idioma.Descripcion) };
            idiomaSelect.Value = usuario.Language;
            idiomaSelect.ReadOnly = true;

            //TipoUsuario
            FormField tipoUsuarioSelect = form.GetField("tipoUsu");
            tipoUsuarioSelect.Options = new List<SelectOption>();

            foreach (var tipo in uow.SecurityRepository.GetUsuarioTipos())
            {
                tipoUsuarioSelect.Options.Add(new SelectOption(tipo.Id.ToString(), tipo.Id + " - " + tipo.Name));
            }
            tipoUsuarioSelect.Value = (usuario.TipoUsuario ?? 1).ToString();
            tipoUsuarioSelect.ReadOnly = true;

            //Asignar automaticamnte nuevas empresas
            InicializarAsigAutoEmpresas(uow, form, context, usuario);

            //Perfiles            
            InicializarPerfiles(uow, context, usuario);
        }
        
        public virtual void InicializarPerfiles(IUnitOfWork uow, FormInitializeContext context, Usuario usuario)
        {
            List<CheckboxListItem> listCheck = new List<CheckboxListItem>();

            var perfiles = uow.SecurityRepository.GetPerfiles();
            var perfilesAsociados = uow.SecurityRepository.GetPerfilesAsociados(usuario.UserId);

            foreach (var perfil in perfiles)
            {
                listCheck.Add(new CheckboxListItem
                {
                    Id = perfil.Id.ToString(),
                    Label = perfil.Descripcion,
                    Selected = false
                });
            }

            foreach (var perfil in perfilesAsociados)
            {
                CheckboxListItem item = listCheck.FirstOrDefault(x => x.Id == perfil.Id.ToString());
                listCheck.Remove(item);
                item.Selected = true;
                listCheck.Add(item);
            }

            context.AddParameter("ListItems", JsonConvert.SerializeObject(listCheck));
        }
        
        public virtual void InicializarAsigAutoEmpresas(IUnitOfWork uow, Form form, FormInitializeContext context, Usuario usuario)
        {
            var editable = (uow.ParametroRepository.GetParameter("SEG030_USUARIO_CONFIG_ENABLED") ?? "N") == "S";

            context.AddParameter("modificarAsignarAutoEmpresas", JsonConvert.SerializeObject(_security.IsUserAllowed("SEG030_User_Config")));

            FormField asignarAutoEmpresas = form.GetField("asignarAutoEmpresas");
            asignarAutoEmpresas.Options = new List<SelectOption>
            {
                new SelectOption("S", "SEG030_Sec0_lbl_frm_HabilitadoS"),
                new SelectOption("N", "SEG030_Sec0_lbl_frm_HabilitadoN"),
            };

            if (!uow.SecurityRepository.AnyConfiguracionUsuario(usuario.UserId))
                asignarAutoEmpresas.Value = "N";
            else
            {
                var configuracionUsuario = uow.SecurityRepository.GetConfiguracionUsuario(usuario.UserId);
                asignarAutoEmpresas.Value = configuracionUsuario.AsignarAutoNuevasEmpresas;
            }

            if (!editable)
                asignarAutoEmpresas.ReadOnly = true;
        }

        public virtual void UpdateUsuario(IUnitOfWork uow, Usuario usuario, List<CheckboxListItem> listaPerfiles, string asignarAutoEmpresas)
        {
            List<int> perfilesQuitar = new List<int>();
            List<int> perfilesAgregar = new List<int>();

            foreach (var perf in listaPerfiles)
            {
                if (perf.Selected)
                    perfilesAgregar.Add(int.Parse(perf.Id));
                else
                    perfilesQuitar.Add(int.Parse(perf.Id));
            }

            new UserService().ActualizarUsuario(uow, usuario, perfilesAgregar, perfilesQuitar, asignarAutoEmpresas);
        }

        public virtual List<string> ObtenerKeyLineasSeleccionadas(IUnitOfWork uow, GridMenuItemActionContext selection)
        {
            var dbQuery = new UsuariosQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, selection.Filters);

            var resultado = new List<string>();

            int idUsuario;
            if (selection.Selection.AllSelected)
            {
                var selectAll = dbQuery.GetResult().Select(g => new { g.USERID }).ToList();

                foreach (var noSeleccionKeys in selection.Selection.Keys)
                {
                    idUsuario = int.Parse(noSeleccionKeys);

                    selectAll.Remove(selectAll.FirstOrDefault(z => z.USERID == idUsuario));
                }

                foreach (var key in selectAll)
                {
                    resultado.Add(string.Join("$", new List<string> { key.USERID.ToString() }));
                }
            }
            else
            {
                foreach (var key in selection.Selection.Keys)
                {
                    resultado.Add(string.Join("$", key));
                }

            }

            return resultado;
        }

        public virtual List<string> GetEditableCells()
        {
            var editableCells = new List<string> { "ISENABLED" };

            if (!this._security.IsUserAllowed(SecurityResources.SEG030_Sec0_btn_HabilitarUsuario))
            {
                editableCells.Remove("ISENABLED");
            }

            return editableCells;
        }

        #endregion
    }
}
