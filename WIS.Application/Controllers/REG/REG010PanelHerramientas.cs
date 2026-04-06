using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Registro;
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
using WIS.Sorting;

namespace WIS.Application.Controllers.REG
{
    public class REG010PanelHerramientas : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridValidationService _gridValidationService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }
        protected readonly ISecurityService _security;
        public REG010PanelHerramientas(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            IGridValidationService gridValidationService)
        {
            this.GridKeys = new List<string>
            {
                "CD_EQUIPO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EQUIPO", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
            this._gridValidationService = gridValidationService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            query.IsAddEnabled = false;
            query.IsEditingEnabled = true;
            query.IsRemoveEnabled = true;

            grid.MenuItems = new List<IGridItem> { new GridButton("btnImprimir", "IMP050_grid1_btn_imprimir") };
            grid.AddOrUpdateColumn(new GridColumnSelect("CD_FERRAMENTA", this.OptionSelectHerramienta()));
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
            {
                new GridButton("btnImpPosicion", "General_Sec0_btn_ImpPosicion", "fas fa-print"),
            }));
            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new HerramientasQuery();
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);

            Dictionary<string, bool> result = this._security.CheckPermissions(new List<string>
            {
                SecurityResources.IMP050HerramientasPosicion_Page_Access_Allow,

            });

            grid.SetEditableCells(new List<string> {
                "NU_COMPONENTE"
            });

            foreach (var row in grid.Rows)
            {
                var tipo = row.GetCell("ID_AUTOASIGNADO").Value;

                if (tipo != "S")
                {
                    if (!result[SecurityResources.IMP050HerramientasPosicion_Page_Access_Allow])
                        row.DisabledButtons.Add("btnImpPosicion");
                    row.SetEditableCells(new List<string>
                    {
                        "DS_EQUIPO",
                        "CD_FERRAMENTA",
                        "NU_COMPONENTE"
                    });
                }
                else
                {
                    row.DisabledButtons.Add("btnImpPosicion");
                }
            }

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new HerramientasQuery();
            uow.HandleQuery(dbQuery);
            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, DefaultSort);
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new HerramientasQuery();
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            uow.BeginTransaction();
            try
            {
                bool inhabilitado = false;
                if (grid.Rows.Any())
                {
                    foreach (var row in grid.Rows)
                    {
                        var cdEquipo = row.GetCell("CD_EQUIPO").Value;
                        var equipo = uow.EquipoRepository.GetEquipo(int.Parse(cdEquipo));

                        if (equipo == null)
                            throw new ValidationFailedException("REG010_msg_Error_EquipoNoExiste", new string[] { cdEquipo });

                        if (row.IsDeleted)
                        {
                            var tipo = row.GetCell("ID_AUTOASIGNADO").Value;

                            if (uow.EquipoRepository.PuedeBorrarEquipo(equipo) && tipo != "S")
                                uow.EquipoRepository.RemoveEquipo(equipo);
                            else
                                inhabilitado = true;
                        }
                        else
                        {
                            var descripcion = row.GetCell("DS_EQUIPO").Value;
                            var cdHerramienta = short.Parse(row.GetCell("CD_FERRAMENTA").Value);
                            var nuComponente = row.GetCell("NU_COMPONENTE").Value;

                            equipo.Descripcion = descripcion;
                            equipo.CodigoHerramienta = cdHerramienta;
                            equipo.NuComponente = nuComponente;
                            equipo.FechaModificacion = DateTime.Now;

                            uow.EquipoRepository.UpdateEquipo(equipo);
                        }
                    }
                }

                uow.SaveChanges();
                uow.Commit();

                if (inhabilitado)
                    context.AddInfoNotification("REG010_msg_Error_RemeveParcial");
                else
                    context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
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
            return grid;
        }

        public override Grid GridValidateRow(GridRow row, Grid grid, GridValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._gridValidationService.Validate(new MantenimientoEquipoValidationModule(uow), grid, row, context);
        }
        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<string> filasSeleccionadas = this.ObtenerKeyLineasSeleccionadas(uow, selection);

            selection.AddParameter("ListaFilasSeleccionadas", JsonConvert.SerializeObject(filasSeleccionadas));

            return selection;
        }

        public virtual List<string> ObtenerKeyLineasSeleccionadas(IUnitOfWork uow, GridMenuItemActionContext selection)
        {
            HerramientasQuery dbQuery = new HerramientasQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, selection.Filters);

            List<string> resultado = new List<string>();

            short equipo;
            if (selection.Selection.AllSelected)
            {
                var selectAll = dbQuery.GetResult().Select(g => new { g.CD_EQUIPO }).ToList();

                foreach (var noSeleccionKeys in selection.Selection.Keys)
                {
                    equipo = short.Parse(noSeleccionKeys);

                    selectAll.Remove(selectAll.FirstOrDefault(z => z.CD_EQUIPO == equipo));
                }

                foreach (var key in selectAll)
                {
                    resultado.Add(string.Join("$", new List<string> { key.CD_EQUIPO.ToString() }));
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
        public virtual List<SelectOption> OptionSelectHerramienta()
        {
            List<SelectOption> opciones = new List<SelectOption>();

            using var uow = this._uowFactory.GetUnitOfWork();

            var herramientas = uow.EquipoRepository.GetHerramientas(false);
            foreach (var h in herramientas)
            {
                opciones.Add(new SelectOption(h.Id.ToString(), $"{h.Id} - { h.Descripcion}"));
            }

            return opciones;
        }
    }
}