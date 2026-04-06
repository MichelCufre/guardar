
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Facturacion;
using WIS.Domain.OrdenTarea;
using WIS.Domain.OrdenTarea.Constants;
using WIS.Exceptions;
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
    public class ORT040TareasOrden : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISessionAccessor _session;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<ORT040TareasOrden> _logger;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public ORT040TareasOrden(
            IUnitOfWorkFactory uowFactory,
            ISessionAccessor session,
            IIdentityService identity,
            ISecurityService security,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFormValidationService formValidationService,
            IFilterInterpreter filterInterpreter,
            ILogger<ORT040TareasOrden> logger,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "NU_ORDEN_TAREA",
            };
            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORDEN_TAREA", SortDirection.Descending),
            };
            this._uowFactory = uowFactory;
            this._session = session;
            this._identity = identity;
            this._security = security;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._formValidationService = formValidationService;
            _filterInterpreter = filterInterpreter;
            this._logger = logger;
            this._gridValidationService = gridValidationService;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            TareasOrdenQuery dbQuery = new TareasOrdenQuery();
            if (query.Parameters.Count > 0)
            {
                string numeroOrden = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrden").Value;
                dbQuery = new TareasOrdenQuery(numeroOrden);
            }
            else
            {
                dbQuery = new TareasOrdenQuery();
            }
            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._gridExcelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string numeroOrden = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrden").Value;

            //Validaciones para activar el modo solo lectura
            Orden orden = uow.OrdenRepository.GetOrden(int.Parse(numeroOrden));
            if (orden.Estado == OrdenTareaDb.ESTADO_ORDEN_CERRADA)
            {
                query.AddParameter("ModoLectura", "S");
                query.IsAddEnabled = false;
                query.IsEditingEnabled = false;
                query.IsRemoveEnabled = false;
            }
            else
            {
                query.IsAddEnabled = true;
                query.IsEditingEnabled = true;
                query.IsRemoveEnabled = true;
            }

            grid.SetInsertableColumns(new List<string> {
                "CD_EMPRESA",
                "CD_TAREA",
            });

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                new GridButton("btnFuncionarios", "ORT040_grid1_btn_Funcionarios", "fas fa-user-alt"),
                new GridButton("btnEquipos", "ORT040_grid1_btn_Equipos", "fas fa-user-alt"),
                new GridButton("btnManipuleo", "ORT040_grid1_btn_Manipuleo", "fas fa-clipboard-list"),
            }));

            grid.AddOrUpdateColumn(new GridColumnSelect("CD_TAREA", this.SelectCodigoTarea(uow)));
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_EMPRESA", this.SelectEmpresa(uow)));
            grid.AddOrUpdateColumn(new GridColumnSelect("NU_COMPONENTE", this.SelectNumeroComponente(uow)));

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string numeroOrden = null;

            if (query.Parameters.Count > 1)
                numeroOrden = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrden")?.Value;

            TareasOrdenQuery dbQuery = new TareasOrdenQuery();
            if (numeroOrden != null)
            {
                dbQuery = new TareasOrdenQuery(numeroOrden);
            }
            else
            {
                dbQuery = new TareasOrdenQuery();
            }

            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            foreach (var row in grid.Rows)
            {
                row.DisabledButtons = new List<string>();
            }

            grid.Rows.ForEach(row =>
            {
                Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
                {
                    "WORT040_grid1_btn_Funcionarios",
                    "WORT040_grid1_btn_ManipuleosInsumos",
                    "WORT040_grid1_btn_Equipos"
                });

                DisableButtons(row, uow, result, grid.Rows.Count(), query);
            });

            grid.SetEditableCells(new List<string> {
                "FL_RESUELTA",
            });

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            string numeroOrden = null;

            if (query.Parameters.Count > 1)
                numeroOrden = query.Parameters.FirstOrDefault(s => s.Id == "numeroOrden")?.Value;

            TareasOrdenQuery dbQuery = new TareasOrdenQuery();
            if (numeroOrden != null)
            {
                dbQuery = new TareasOrdenQuery(numeroOrden);
            }
            else
            {
                dbQuery = new TareasOrdenQuery();
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                int numeroOrden = int.Parse(query.Parameters.FirstOrDefault(s => s.Id == "numeroOrden")?.Value);
                var orden = uow.OrdenRepository.GetOrden(numeroOrden);

                if (orden.Estado == OrdenTareaDb.ESTADO_ORDEN_CERRADA)
                {
                    throw new ValidationFailedException("General_Sec0_Error_Error_EstadoDeOrdenNoPermiteIngresarModificar");
                }

                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {

                        if (row.IsNew)
                        {
                            OrdenTareaObjeto ordenTarea = this.CrearOrdenTarea(uow, row, numeroOrden);
                            uow.TareaRepository.AddOrdenTarea(ordenTarea);
                            uow.SaveChanges();
                        }
                        else if (row.IsDeleted)
                        {
                            this.DeleteOrdenTarea(uow, row, query);
                        }
                        else
                        {
                            // rows editadas
                            OrdenTareaObjeto ordenTarea = this.UpdateOrdenTarea(uow, row, numeroOrden);
                            uow.TareaRepository.UpdateOrdenTarea(ordenTarea);
                        }
                    }

                }

                uow.SaveChanges();

                query.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (ValidationFailedException ex)
            {
                this._logger.LogError(ex, "ORT040GridCommit");
                query.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "ORT040GridCommit");
                query.AddErrorNotification(ex.Message);
            }

            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new TareasOrdenValidationModule(uow), grid, row, context);
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            var numOrden = context.Row.GetCell("NU_ORT_ORDEN").Value;
            var numOrdenTarea = context.Row.GetCell("NU_ORDEN_TAREA").Value;
            ComponentParameter numeroOrden = new ComponentParameter("numeroOrden", numOrden);
            ComponentParameter numeroOrdenTarea = new ComponentParameter("numeroOrdenTarea", numOrdenTarea);
            ComponentParameter descripcionOrden = new ComponentParameter("descripcionOrden", context.Row.GetCell("DS_ORT_ORDEN").Value);
            ComponentParameter codigoTarea = new ComponentParameter("codigoTarea", context.Row.GetCell("CD_TAREA").Value);
            ComponentParameter descripcionTarea = new ComponentParameter("descripcionTarea", context.Row.GetCell("DS_TAREA").Value);
            ComponentParameter codigoEmpresa = new ComponentParameter("codigoEmpresa", context.Row.GetCell("CD_EMPRESA").Value);
            ComponentParameter descripcionEmpresa = new ComponentParameter("descripcionEmpresa", context.Row.GetCell("NM_EMPRESA").Value);
            ComponentParameter fechaInicioOrden = new ComponentParameter("fechaInicioOrden", context.Parameters.FirstOrDefault(s => s.Id == "fechaInicioOrden").Value);
            ComponentParameter resuelta = new ComponentParameter("resuelta", context.Row.GetCell("FL_RESUELTA").Value);

            using var uow = this._uowFactory.GetUnitOfWork();
            OrdenTareaObjeto tarea = uow.TareaRepository.GetOrdenTarea(long.Parse(numOrdenTarea));

            switch (context.ButtonId)
            {
                case "btnFuncionarios":
                    context.Redirect("/ordenTarea/ORT060", new List<ComponentParameter>
                    {
                        numeroOrden,
                        numeroOrdenTarea,
                        descripcionOrden,
                        codigoTarea,
                        descripcionTarea,
                        codigoEmpresa,
                        descripcionEmpresa,
                        fechaInicioOrden,
                        resuelta,
                    });
                    break;

                case "btnEquipos":
                    context.Redirect("/ordenTarea/ORT080", new List<ComponentParameter>
                    {
                        numeroOrden,
                        numeroOrdenTarea,
                        descripcionOrden,
                        codigoTarea,
                        descripcionTarea,
                        codigoEmpresa,
                        descripcionEmpresa,
                        fechaInicioOrden,
                        resuelta,
                    });
                    break;

                case "btnManipuleo":
                    context.Redirect("/ordenTarea/ORT070", new List<ComponentParameter>
                    {
                        numeroOrden,
                        numeroOrdenTarea,
                        descripcionOrden,
                        codigoTarea,
                        descripcionTarea,
                        codigoEmpresa,
                        descripcionEmpresa,
                        resuelta
                    });
                    break;
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual OrdenTareaObjeto CrearOrdenTarea(IUnitOfWork uow, GridRow row, int numeroOrden)
        {
            OrdenTareaObjeto ordenTarea = new OrdenTareaObjeto();

            ordenTarea.NuOrden = numeroOrden;
            ordenTarea.CdTarea = row.GetCell("CD_TAREA").Value;
            ordenTarea.Empresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
            ordenTarea.CdFuncionarioAddrow = this._identity.UserId;
            ordenTarea.DtAddrow = DateTime.Now;
            ordenTarea.Resuelta = "N";

            return ordenTarea;
        }

        public virtual OrdenTareaObjeto UpdateOrdenTarea(IUnitOfWork uow, GridRow row, int numeroOrden)
        {
            var nuOrdenTarea = long.Parse(row.GetCell("NU_ORDEN_TAREA").Value);
            var ordenTarea = uow.TareaRepository.GetOrdenTarea(nuOrdenTarea);

            ordenTarea.DtUpdrow = DateTime.Now;
            ordenTarea.Resuelta = row.GetCell("FL_RESUELTA").Value;

            if (ordenTarea.Resuelta == "S" && uow.TareaRepository.AnyOrdenTareaFuncionarioSinFinalizar(nuOrdenTarea))
                throw new ValidationFailedException("General_ORT040_Error_TareaAmigableFuncionarioSinMarcaFin");

            return ordenTarea;
        }

        public virtual void DeleteOrdenTarea(IUnitOfWork uow, GridRow row, GridFetchContext query)
        {
            var nuOrdenTarea = long.Parse(row.GetCell("NU_ORDEN_TAREA").Value);

            if (!uow.TareaRepository.IsOrdenTareaResuelta(nuOrdenTarea))
            {
                throw new ValidationFailedException("General_Sec0_Error_Er006_TareaNoResuelta");
            }
            else if (uow.TareaRepository.AnyOrdenTareaFuncionarioByOrdenTarea(nuOrdenTarea)
                    || uow.TareaRepository.AnyOrdenTareaEquipoByOrdenTarea(nuOrdenTarea)
                    || uow.TareaRepository.AnyOrdenTareaDatoByOrdenTarea(nuOrdenTarea))
            {
                throw new ValidationFailedException("General_ORT040_Error_OrdenTareaTieneDetalles");
            }

            int numeroOrden = int.Parse(row.GetCell("NU_ORDEN_TAREA").Value);
            uow.TareaRepository.DeleteOrdenTarea(numeroOrden);
        }

        public virtual void DisableButtons(GridRow row, IUnitOfWork uow, Dictionary<string, bool> result, int cantidadFilas, GridFetchContext query)
        {

            if (!(result["WORT040_grid1_btn_Funcionarios"]))
            {
                row.DisabledButtons.Add("btnFuncionarios");
            }
            if (!(result["WORT040_grid1_btn_Equipos"]))
            {
                row.DisabledButtons.Add("btnEquipos");
            }
            if (!(result["WORT040_grid1_btn_ManipuleosInsumos"]))
            {
                row.DisabledButtons.Add("btnManipuleo");
            }
        }

        public virtual List<SelectOption> SelectCodigoTarea(IUnitOfWork uow)
        {
            return uow.TareaRepository.GetTareasManuales()
                .Select(w => new SelectOption(w.Id, w.Id + "  -  " + w.Descripcion))
                .ToList();
        }

        public virtual List<SelectOption> SelectEmpresa(IUnitOfWork uow)
        {
            return uow.EmpresaRepository.GetEmpresasParaUsuario(this._identity.UserId)
                .Select(w => new SelectOption(w.Id.ToString(), w.Id.ToString() + "  -  " + w.Nombre))
                .ToList();
        }

        public virtual List<SelectOption> SelectNumeroComponente(IUnitOfWork uow)
        {
            return uow.FacturacionRepository.GetAllComponentes()
                .Select(w => new SelectOption(w.NU_COMPONENTE, w.NU_COMPONENTE + "  -  " + w.DS_SIGNIFICADO))
                .ToList();
        }

        #endregion
    }
}
