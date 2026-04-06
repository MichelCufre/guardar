using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Domain.Registro;
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
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG070PanelZonas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ILogger<REG070PanelZonas> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG070PanelZonas(IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<REG070PanelZonas> logger,
            IGridExcelService excelService,
            IIdentityService identity,
            IGridService gridService)
        {
            this.GridKeys = new List<string>
            {
                "CD_ZONA_UBICACION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._gridValidationService = gridValidationService;
            this._formValidationService = formValidationService;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._gridService = gridService;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems.Add(new GridButton("btnEliminarZona", "REG070_frm1_btn_EliminarZona", string.Empty, new ConfirmMessage("REG070_Sec0_msg_Confirm")));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ZonasQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                var cdZona = row.GetCell("CD_ZONA_UBICACION").Value;
                var idZona = int.Parse(row.GetCell("ID_ZONA_UBICACION").Value);

                if (uow.ZonaUbicacionRepository.ZonaUtilizada(cdZona, idZona))
                    row.DisabledSelected = true;
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ZonasQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new ZonasQuery();
            uow.HandleQuery(dbQuery);
            context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            try
            {
                if (context.ButtonId == "btnEliminarZona")
                {
                    using var uow = this._uowFactory.GetUnitOfWork();

                    List<string> zonasAEliminar = new List<string>();

                    if (context.Selection.AllSelected)
                        zonasAEliminar = this.GetZonasAllSelected(uow, context);
                    else
                        zonasAEliminar = this.GetZonasSelection(uow, context);

                    var manejoZonas = new ZonasDeUbicacion(uow, _identity.UserId, _identity.Application, zonasAEliminar);
                    manejoZonas.Eliminar();

                    uow.SaveChanges();

                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (form.Id == "REG070_form_1")
            {
                InicializarSelects(uow, form);
            }
            else if (form.Id == "REG070Update_form_1")
            {
                var zona = uow.ZonaUbicacionRepository.GetZona(context.GetParameter("idZonaUbicacion"));
                if (zona == null)
                    throw new EntityNotFoundException("REG070_Frm1_Error_ZonaNoExiste");

                InicializarUpdate(uow, form, zona);
            }

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();

            try
            {
                var msg = string.Empty;

                if (form.Id == "REG070_form_1")
                {
                    CrearZona(uow, form);
                    msg = "REG070_Frm1_Succes_Creacion";
                }
                else if (form.Id == "REG070Update_form_1")
                {
                    UpdateZona(uow, form, context);
                    msg = "REG070_Frm1_Succes_Edicion";
                }

                uow.SaveChanges();
                uow.Commit();

                context.AddSuccessNotification(msg);
            }
            catch (ValidationFailedException ex)
            {
                uow.Rollback();
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                uow.Rollback();
                _logger.LogError(ex, "FormSubmit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new MantenimientoZonaUbicacionFormValidationModule(uow, this._identity), form, context);
        }

        #region Metodos Auxiliares

        public virtual void CrearZona(IUnitOfWork uow, Form form)
        {
            var newZona = new ZonaUbicacion
            {
                Id = form.GetField("idZona").Value.ToUpper(),
                Descripcion = form.GetField("descripcionZona").Value,
                TipoZonaUbicacion = form.GetField("tipoZona").Value,

                ZonaUbicacionPicking = form.GetField("idZonaPicking").Value,
                Estacion = form.GetField("estacion").Value,
                EstacionAlmacenado = form.GetField("estacionAlmacenaje").Value,
                Habilitada = form.GetField("habilitada").Value == "true" ? true : false,
                Alta = DateTime.Now
            };

            uow.ZonaUbicacionRepository.AddZona(newZona);
            uow.DominioRepository.AddDetalleDominioZona(newZona.IdInterno, newZona.Id);
        }

        public virtual void UpdateZona(IUnitOfWork uow, Form form, FormSubmitContext context)
        {
            var codigoZona = form.GetField("idZona").Value;

            var zona = uow.ZonaUbicacionRepository.GetZona(codigoZona);

            if (zona == null)
                throw new EntityNotFoundException("REG070_Frm1_Error_ZonaNoExiste");

            zona.Descripcion = form.GetField("descripcionZona").Value;
            zona.TipoZonaUbicacion = form.GetField("tipoZona").Value;
            zona.ZonaUbicacionPicking = form.GetField("idZonaPicking").Value;
            zona.Estacion = form.GetField("estacion").Value;
            zona.EstacionAlmacenado = form.GetField("estacionAlmacenaje").Value;
            zona.Habilitada = form.GetField("habilitada").Value == "true" ? true : false;
            zona.Modificacion = DateTime.Now;
            zona.IdInterno = int.Parse(form.GetField("idInternoZona").Value);

            uow.ZonaUbicacionRepository.UpdateZona(zona);
        }

        public virtual void InicializarSelects(IUnitOfWork uow, Form form)
        {
            var selectTipoZona = form.GetField("tipoZona");
            var selectZonaPicking = form.GetField("idZonaPicking");

            selectTipoZona.Options = new List<SelectOption>();
            selectZonaPicking.Options = new List<SelectOption>();

            var tiposZonas = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposDeZonas);

            foreach (var tipo in tiposZonas.Where(tp => tp.Valor != TipoUbicacionZonaDb.Automatismo))
            {
                selectTipoZona.Options.Add(new SelectOption(tipo.Valor, tipo.Descripcion));
            }

            var zonas = uow.ZonaUbicacionRepository.GetZonas();
            foreach (var zona in zonas)
            {
                selectZonaPicking.Options.Add(new SelectOption(zona.Id, zona.Descripcion));
            }

            form.GetField("habilitada").Value = "true";
        }

        public virtual void InicializarUpdate(IUnitOfWork uow, Form form, ZonaUbicacion zona)
        {
            var selectTipoZona = form.GetField("tipoZona");
            var selectZonaPicking = form.GetField("idZonaPicking");

            selectTipoZona.Options = new List<SelectOption>();
            selectZonaPicking.Options = new List<SelectOption>();

            var zonas = uow.ZonaUbicacionRepository.GetZonas();
            foreach (var z in zonas)
            {
                selectZonaPicking.Options.Add(new SelectOption(z.Id, z.Descripcion));
            }

            var tiposZonas = uow.DominioRepository.GetDominios(CodigoDominioDb.TiposDeZonas);

            foreach (var tipo in tiposZonas)
            {
                selectTipoZona.Options.Add(new SelectOption(tipo.Valor, tipo.Descripcion));
            }

            form.GetField("idInternoZona").Value = zona.IdInterno.ToString();
            form.GetField("idInternoZona").ReadOnly = true;
            form.GetField("idZona").Value = zona.Id;
            form.GetField("idZona").ReadOnly = true;

            form.GetField("descripcionZona").Value = zona.Descripcion;
            form.GetField("idZonaPicking").Value = zona.ZonaUbicacionPicking;
            form.GetField("estacion").Value = zona.Estacion;
            form.GetField("estacionAlmacenaje").Value = zona.EstacionAlmacenado;

            form.GetField("tipoZona").Value = zona.TipoZonaUbicacion;

            if (zona.TipoZonaUbicacion == TipoUbicacionZonaDb.Automatismo)
                form.GetField("tipoZona").ReadOnly = true;
            else
            {
                form.GetField("tipoZona").ReadOnly = false;
                selectTipoZona.Options.RemoveAll(x => x.Value == TipoUbicacionZonaDb.Automatismo);
            }

            if (zona.Habilitada)
                form.GetField("habilitada").Value = "true";
        }

        public virtual List<string> GetZonasAllSelected(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var dbQuery = new ZonasQuery();
            var zonasNoUsadas = uow.ZonaUbicacionRepository.GetZonasNoUtilizadas();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return dbQuery.GetZonas(zonasNoUsadas);
        }

        public virtual List<string> GetZonasSelection(IUnitOfWork uow, GridMenuItemActionContext context)
        {
            var zonasNoUsadas = uow.ZonaUbicacionRepository.GetZonasNoUtilizadas();
            var zonas = context.Selection.Keys
                .Join(zonasNoUsadas,
                    z => z,
                    znu => znu,
                    (z, znu) => z)
                .ToList();

            return zonas;
        }

        #endregion
    }
}
