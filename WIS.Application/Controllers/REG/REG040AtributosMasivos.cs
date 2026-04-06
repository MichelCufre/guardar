using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
using WIS.Domain.General;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG040AtributosMasivos : AppController
    {
        protected readonly IGridService _gridService;
        protected readonly ISecurityService _security;
        protected readonly IIdentityService _identity;
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IFormValidationService _formValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REG040AtributosMasivos(IGridService gridService, ISecurityService security, IIdentityService identity, IUnitOfWorkFactory uowFactory, IFilterInterpreter filterInterpreter, IFormValidationService formValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_ENDERECO",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_ENDERECO", SortDirection.Ascending)
            };

            this._gridService = gridService;
            this._security = security;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._filterInterpreter = filterInterpreter;
            this._formValidationService = formValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> { new GridButton("btnAsignarAtributos", "IMP050_grid1_btn_AsignarAtributos") };

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            UbicacionesQuery dbQuery = new UbicacionesQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "REG040AtributosMasivos_grid_1" && context.ButtonId == "btnAsignarAtributos")
                this.ProcesarAsignar(context);

            return context;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            UbicacionesQuery dbQuery = new UbicacionesQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Form FormInitialize(Form form, FormInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            this.InicializarSelects(form, uow);
            return form;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new REG040AtributosMasivosFormValidationModule(uow), form, context);
        }

        public virtual void ProcesarAsignar(GridMenuItemActionContext selection)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();
            uow.CreateTransactionNumber("PRE052 Crear Preparacion manual por Pedidos");
            long transaccion = uow.GetTransactionNumber();
            try
            {
                short? tpUbic = null;
                short? rotatividad = null;
                string zona = string.Empty;
                string controlAcceso = string.Empty;

                if (short.TryParse(selection.Parameters.Find(x => x.Id == "tpUbicacion")?.Value, out short valueTpUbic))
                    tpUbic = valueTpUbic;

                if (short.TryParse(selection.Parameters.Find(x => x.Id == "rotatividad")?.Value, out short valueRotatividad))
                    rotatividad = valueRotatividad;

                if (!string.IsNullOrEmpty(selection.Parameters.Find(x => x.Id == "zona")?.Value))
                    zona = selection.Parameters.Find(x => x.Id == "zona")?.Value;

                if (!string.IsNullOrEmpty(selection.Parameters.Find(x => x.Id == "controlAcceso")?.Value))
                    controlAcceso = selection.Parameters.Find(x => x.Id == "controlAcceso")?.Value;

                var filasSeleccionadas = this.ObtenerKeyLineasSeleccionadas(uow, selection);
                if (tpUbic != null || rotatividad != null || !string.IsNullOrEmpty(zona) || !string.IsNullOrEmpty(controlAcceso))
                {
                    AsignarAtributos(uow, filasSeleccionadas, tpUbic, rotatividad, zona, controlAcceso, transaccion);
                    selection.AddSuccessNotification("REG040AtributosMasivos_Sucess_msg_AtributosAsignados");
                }
                else
                    selection.AddSuccessNotification("REG040AtributosMasivos_Sucess_msg_AtributosNOAsignados");

                uow.SaveChanges();
                uow.Commit();

                selection.AddSuccessNotification("REG040AtributosMasivos_Sucess_msg_AtributosAsignados");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        protected virtual void InicializarSelects(Form form, IUnitOfWork uow)
        {

            //Inicializar selects
            FormField selectTipoUbicacion = form.GetField("tpUbicacion");
            FormField selectRotatividad = form.GetField("rotatividad");
            FormField selectZonas = form.GetField("zona");
            FormField selectControlAcceso = form.GetField("controlAcceso");

            selectTipoUbicacion.Options = new List<SelectOption>();
            selectRotatividad.Options = new List<SelectOption>();
            selectZonas.Options = new List<SelectOption>();

            // Tipos ubicación
            List<UbicacionTipo> tiposUbicaciones = uow.UbicacionTipoRepository.GetUbicacionTipos();
            foreach (var tipo in tiposUbicaciones)
            {
                selectTipoUbicacion.Options.Add(new SelectOption(tipo.Id.ToString(), $"{tipo.Id} - {tipo.Descripcion}"));
            }

            // Rotatividad
            List<ProductoRotatividad> rotatividades = uow.ProductoRotatividadRepository.GetProductoRotatividades();
            foreach (var rotatividad in rotatividades)
            {
                selectRotatividad.Options.Add(new SelectOption(rotatividad.Id.ToString(), $"{rotatividad.Id} - { rotatividad.Descripcion}")); ;
            }

            // Zonas
            List<ZonaUbicacion> zonas = uow.ZonaUbicacionRepository.GetZonasHabilitadas();
            foreach (var zona in zonas)
            {
                selectZonas.Options.Add(new SelectOption(zona.Id, $"{zona.Id} - {zona.Descripcion}"));
            }

            // Control Acceso
            List<ControlAcceso> controlesAcceso = uow.ZonaUbicacionRepository.GetControlesAcceso();
            foreach (var controlAcceso in controlesAcceso)
            {
                selectControlAcceso.Options.Add(new SelectOption(controlAcceso.Id, $"{controlAcceso.Id} - {controlAcceso.Descripcion}"));
            }
        }

        public virtual List<string> ObtenerKeyLineasSeleccionadas(UnitOfWorkCore uow, GridMenuItemActionContext selection)
        {
            string ubicacion;
            List<string> resultado = new List<string>();

            UbicacionesQuery dbQuery = new UbicacionesQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, selection.Filters);

            if (selection.Selection.AllSelected)
            {
                var selectAll = dbQuery.GetResult().Select(g => new { g.CD_ENDERECO }).ToList();

                foreach (var noSeleccionKeys in selection.Selection.Keys)
                {
                    ubicacion = noSeleccionKeys;
                    selectAll.Remove(selectAll.FirstOrDefault(z => z.CD_ENDERECO == ubicacion));
                }

                foreach (var key in selectAll)
                {
                    resultado.Add(string.Join("$", new List<string> { key.CD_ENDERECO }));
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

        public virtual void AsignarAtributos(IUnitOfWork uow, List<string> ubicaciones, short? tpUbic, short? rotaividad, string zona, string controlAcceso, long? transaccion)
        {
            foreach (var ubic in ubicaciones)
            {
                Ubicacion ubicacion = uow.UbicacionRepository.GetUbicacion(ubic);

                if (tpUbic != null)
                    ubicacion.IdUbicacionTipo = tpUbic ?? 91;

                if (rotaividad != null)
                    ubicacion.IdProductoRotatividad = rotaividad ?? 1;

                if (!string.IsNullOrEmpty(zona))
                    ubicacion.IdUbicacionZona = zona;

                if (!string.IsNullOrEmpty(controlAcceso))
                    ubicacion.IdControlAcceso = controlAcceso;

                uow.UbicacionRepository.UpdateUbicacion(ubicacion);
            }
        }
    }
}
