using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.OrdenTarea;
using WIS.Domain.OrdenTarea;
using WIS.Domain.OrdenTarea.Constants;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Extension;
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
using WIS.Session;
using WIS.Sorting;

namespace WIS.Application.Controllers.ORT
{
    public class ORT030OrdenTrabajo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<ORT030OrdenTrabajo> _logger;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IParameterService _parameterService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public ORT030OrdenTrabajo(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<ORT030OrdenTrabajo> logger,
            IGridValidationService gridValidationService,
            IFormValidationService formValidationService,
            IParameterService parameterService
            )
        {
            this.GridKeys = new List<string>
            {
                "NU_ORT_ORDEN",
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORT_ORDEN", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
            this._formValidationService = formValidationService;
            this._parameterService = parameterService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            var vasHabilitado = _parameterService.GetValue("VAS_HABILITADO");

            if (vasHabilitado == "S")
            {
                query.IsAddEnabled = true;
                query.IsEditingEnabled = true;
                query.IsRemoveEnabled = false;

                grid.SetInsertableColumns(new List<string> {
                    "DS_ORT_ORDEN",
                    "DT_INICIO",
                    "DS_REFERENCIA",
                });

                using var uow = this._uowFactory.GetUnitOfWork();

                grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                    new GridButton("btnCerrarOrden", "ORT030_grid1_btn_CerrarOrden", "fas fa-lock"),
                    new GridButton("btnTareasOrden", "ORT030_grid1_btn_TareasOrden", "fas fa-list"),
                    new GridButton("btnVerTareasOrden", "ORT030_grid1_btn_VerTareasOrden", "fas fa-list"),
                    new GridButton("btnAsignarTareaAmigable", "ORT030_grid1_btn_AsignarTareaAmigable", "fas fa-clipboard"),
                }));

                //Cargo default values
                string fechaActual = DateTimeExtension.ToIsoString(DateTime.Now);

                var defaultColumns = new Dictionary<string, string>();
                defaultColumns.Add("CD_FUNCIONARIO_ADDROW", this._identity.UserId.ToString());
                defaultColumns.Add("DT_INICIO", fechaActual);
                defaultColumns.Add("DT_ULTIMA_OPERACION", fechaActual);
                defaultColumns.Add("DT_ADDROW", fechaActual);
                defaultColumns.Add("ID_ESTADO", "HAB");

                grid.SetColumnDefaultValues(defaultColumns);
            }
            else
            {
                query.IsAddEnabled = false;
                query.IsEditingEnabled = false;
                query.IsRemoveEnabled = false;

            }

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            OrdenTrabajoQuery dbQuery = new OrdenTrabajoQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                row.DisabledButtons = new List<string>();
            }
            var vasHabilitado = _parameterService.GetValue("VAS_HABILITADO");

            if (vasHabilitado == "S")
            {
                grid.SetEditableCells(new List<string> {
                 "DS_ORT_ORDEN",
                 "DS_REFERENCIA",
             });
            }
            grid.Rows.ForEach(row =>
            {
                Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
                {
                    "WORT030_grid1_btn_CerrarOrden",
                    "WORT030_grid1_btn_TareasOrden",
                    "WORT030_grid1_btn_VerTareasOrden",
                    "ORT030_grid1_btn_AsignarTareaAmigable",
                });

                DisableButtons(row, uow, result);
            });

            return grid;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                if (grid.Rows.Any())
                {
                    RegistroModificacionOrdenTrabajo registroModificacionOT = new RegistroModificacionOrdenTrabajo(uow, this._identity.UserId, this._identity.Application);

                    //if (grid.HasNewDuplicates(this.GridKeys))
                    //    throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    //if (grid.HasDuplicates(this.GridKeys))
                    //    throw new ValidationFailedException("General_Sec0_Error_Er006_LineasDuplicadas");

                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            Orden orden = this.CrearOrden(uow, row, query);
                            registroModificacionOT.RegistrarOrden(orden);
                        }
                        else
                        {
                            // rows editadas
                            Orden orden = this.UpdateOrden(uow, row, query);
                            registroModificacionOT.ModificarOrden(orden);
                        }
                    }
                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "ORT030GridCommit");
                query.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoOrdenGridValidationModule(uow), grid, row, context);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            string numeroOrden = context.Row.GetCell("NU_ORT_ORDEN").Value;

            string fechaInicio = "";
            string fechaFin = "";
            string ultimaOperacion = "";

            if (context.ButtonId == "btnVerTareasOrden" || context.ButtonId == "btnTareasOrden")
            {
                if (!string.IsNullOrEmpty(context.Row.GetCell("DT_INICIO").Value))
                {
                    DateTime Fecha = DateTime.Parse(context.Row.GetCell("DT_INICIO").Value, this._identity.GetFormatProvider());
                    fechaInicio = Fecha.ToString(CDateFormats.DATETIME_24H);
                }

                if (!string.IsNullOrEmpty(context.Row.GetCell("DT_FIN").Value))
                {
                    DateTime? Fecha = DateTime.Parse(context.Row.GetCell("DT_FIN").Value, this._identity.GetFormatProvider());
                    fechaFin = Fecha.ToString(CDateFormats.DATETIME_24H);
                }

                if (!string.IsNullOrEmpty(context.Row.GetCell("DT_ULTIMA_OPERACION").Value))
                {
                    DateTime? Fecha = DateTime.Parse(context.Row.GetCell("DT_ULTIMA_OPERACION").Value, this._identity.GetFormatProvider());
                    ultimaOperacion = Fecha.ToString(CDateFormats.DATETIME_24H);
                }
            }
            using var uow = this._uowFactory.GetUnitOfWork();
            switch (context.ButtonId)
            {
                case "btnCerrarOrden":
                    int idOrden = int.Parse(context.Row.GetCell("NU_ORT_ORDEN").Value);
                    ControlarTareaCerradas(idOrden);
                    if (context.ButtonId == "btnCerrarOrden" && !uow.OrdenRepository.OrdenEsCerrable(idOrden))
                    {
                        context.AddParameter("SHOW_CONFIRMACION_CERRAR_ORDEN", "true");
                        context.AddParameter("nuOrden", idOrden.ToString());

                        var cantidadTareasNoResueltas = uow.TareaRepository.GetCantOrdenTareasNoResueltas(idOrden);
                        var cantidadTareasAmigablesPorCerrar = uow.TareaRepository.GetCantidadTareasAmigablesPorCerrar(idOrden);

                        context.AddParameter("cantidadTareasNoResueltas", cantidadTareasNoResueltas.ToString());
                        context.AddParameter("cantidadTareasAmigablesPorCerrar", cantidadTareasAmigablesPorCerrar.ToString());

                        return context;
                    }
                    CerrarOrden(uow, idOrden);
                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                    break;

                case "btnTareasOrden":
                    context.Redirect("/ordenTarea/ORT040", new List<ComponentParameter>
                    {
                        new ComponentParameter("numeroOrden", numeroOrden),
                        new ComponentParameter("descripcionOrden", context.Row.GetCell("DS_ORT_ORDEN").Value),
                        new ComponentParameter("fechaInicio", fechaInicio),
                        new ComponentParameter("fechaFin", fechaFin),
                        new ComponentParameter("UltimaOperacion", ultimaOperacion),
                    });
                    break;

                case "btnVerTareasOrden":
                    context.Redirect("/ordenTarea/ORT040", new List<ComponentParameter>
                    {
                        new ComponentParameter("numeroOrden", numeroOrden),
                        new ComponentParameter("descripcionOrden", context.Row.GetCell("DS_ORT_ORDEN").Value),
                        new ComponentParameter("fechaInicio", fechaInicio),
                        new ComponentParameter("fechaFin", fechaFin),
                        new ComponentParameter("UltimaOperacion", ultimaOperacion),
                    });
                    break;

                case "btnAsignarTareaAmigable":
                    context.Redirect("/ordenTarea/ORT090", true, new List<ComponentParameter> { new ComponentParameter("nuOrden", numeroOrden) });
                    break;
            }

            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            switch (context.ButtonId)
            {
                case "btnCerrarOrdenConfirm":
                    var idOrden = int.Parse(context.GetParameter("nuOrden"));
                    ControlarTareaCerradas(idOrden);
                    CerrarOrden(uow, idOrden);
                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
                    break;
            }

            return context;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            OrdenTrabajoQuery dbQuery = new OrdenTrabajoQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            OrdenTrabajoQuery dbQuery = new OrdenTrabajoQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        #region Metodos Auxiliares

        public virtual Orden CrearOrden(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            Orden nuevaOrden = new Orden();

            //Datos de Row
            nuevaOrden.Descripcion = row.GetCell("DS_ORT_ORDEN").Value;

            nuevaOrden.FechaInicio = DateTimeExtension.TryParseFromIso(row.GetCell("DT_INICIO").Value, out DateTime? parsedDate) != false ? parsedDate : null;
            nuevaOrden.DsReferencia = row.GetCell("DS_REFERENCIA").Value;

            //Datos hardcodeados
            nuevaOrden.FechaAgregado = DateTime.Now;
            nuevaOrden.Funcionario = this._identity.UserId;
            nuevaOrden.FechaUltimaOperacion = DateTime.Now;
            nuevaOrden.Estado = OrdenTareaDb.ESTADO_ORDEN_ACTIVA;

            return nuevaOrden;
        }

        public virtual Orden UpdateOrden(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            string idOrden = row.GetCell("NU_ORT_ORDEN").Value;

            Orden orden = new Orden();
            orden = uow.OrdenRepository.GetOrden(int.Parse(idOrden));

            orden.Descripcion = row.GetCell("DS_ORT_ORDEN").Value;
            orden.DsReferencia = row.GetCell("DS_REFERENCIA").Value;
            orden.FechaUltimaOperacion = DateTime.Now;

            return orden;
        }

        public virtual void DisableButtons(GridRow row, IUnitOfWork uow, Dictionary<string, bool> result)
        {
            Orden orden = uow.OrdenRepository.GetOrden(int.Parse(row.GetCell("NU_ORT_ORDEN").Value));
            int cantOrdenTarea = uow.TareaRepository.GetCantOrdenTarea(orden.Id);

            if (!(result["WORT030_grid1_btn_CerrarOrden"] && orden.FechaFin == null))
            {
                //Caso normal
                row.DisabledButtons.Add("btnCerrarOrden");
            }

            if (!(result["ORT030_grid1_btn_AsignarTareaAmigable"] && orden.Estado == OrdenTareaDb.ESTADO_ORDEN_ACTIVA))
            {
                row.DisabledButtons.Add("btnAsignarTareaAmigable");
            }

            if (!(result["WORT030_grid1_btn_TareasOrden"] && cantOrdenTarea >= 0))
            {
                row.DisabledButtons.Add("btnTareasOrden");
                row.DisabledButtons.Add("btnVerTareasOrden");
            }
            else if (orden.FechaFin != null || orden.Estado == OrdenTareaDb.ESTADO_ORDEN_CERRADA)
            {
                row.DisabledButtons.Add("btnTareasOrden");
            }
            else
            {
                row.DisabledButtons.Add("btnVerTareasOrden");
            }


        }

        public virtual void ControlarTareaCerradas(int idOrden)
        {
            /*ORT030 Me fijo que todas las lineas tengan fl_resuelta, 
             * luego me fijo si alguna tiene asociada horas funcionario, 
             * si tiene me fijo que ninguna supera la hora de finalizacion de la orden(order by dt_fin desc => first), en caso de serlo, 
             * apunta al culpable y no deja cerrar la orden hasta que el tiempo sea mayor o se corrija el problema*/

            using var uow = this._uowFactory.GetUnitOfWork();


            int cantTarea = uow.TareaRepository.GetCantOrdenTarea(idOrden);
            if (cantTarea > 0)
            {
                if (uow.TareaRepository.GetCantOrdenTareaFuncionario(idOrden) > 0)
                {
                    KeyValuePair<string, DateTime?> TareaFecha = uow.TareaRepository.GetTareaFechaOrdenTareaFuncionario(idOrden);

                    if (TareaFecha.Value > DateTime.Now)
                        throw new ValidationFailedException("ORT030_Sec0_Error_HorasFuncMayorFechaFin", new string[] { TareaFecha.Key });
                }
            }
        }

        public virtual void CerrarOrden(IUnitOfWork uow, int idOrden)
        {
            Orden orden = new Orden();
            orden = uow.OrdenRepository.GetOrden(idOrden);

            DateTime fechaFin = DateTime.Now;


            orden.Estado = OrdenTareaDb.ESTADO_ORDEN_CERRADA;
            orden.FechaFin = fechaFin;

            uow.OrdenRepository.UpdateOrden(orden);

            var sesionesFuncionariosActiva = uow.OrdenRepository.GetSesionActivaFuncionario(idOrden);

            foreach (var sesionFuncionario in sesionesFuncionariosActiva)
            {
                var equipoSesionActiva = uow.OrdenRepository.GetSesionEquipoAuxiliar(sesionFuncionario.NuOrtOrdenSesion);

                foreach (var equipoSesion in equipoSesionActiva)
                {
                    equipoSesion.DtFin = fechaFin;
                    uow.OrdenRepository.UpdateEquipoSesion(equipoSesion);
                }

                sesionFuncionario.DtFin = fechaFin;

                uow.OrdenRepository.UpdateSesionFuncionario(sesionFuncionario);
            }

            var tareasSinFinalizar = uow.TareaRepository.GetTareasSinFinalizar(idOrden);

            foreach (var tarea in tareasSinFinalizar)
            {
                tarea.Resuelta = "S";
                tarea.DtUpdrow = fechaFin;

                uow.TareaRepository.UpdateOrdenTarea(tarea);

                var ordenTareaFuncionario = uow.TareaRepository.GetOrdenTareaFuncionarioAmigable(tarea.NuTarea);

                if (ordenTareaFuncionario != null)
                {
                    ordenTareaFuncionario.FechaHasta = fechaFin;

                    uow.TareaRepository.UpdateOrdenTareaFuncionario(ordenTareaFuncionario);
                }

            }
            uow.SaveChanges();

        }

        #endregion
    }
}
