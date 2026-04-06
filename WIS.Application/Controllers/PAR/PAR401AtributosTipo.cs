using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Parametrizacion;
using WIS.Domain.Parametrizacion;
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

namespace WIS.Application.Controllers.PRE
{
    public class PAR401AtributosTipo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _gridExcelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ILogger<PAR401AtributosTipo> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PAR401AtributosTipo(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService gridExcelService,
            IFilterInterpreter filterInterpreter,
            ILogger<PAR401AtributosTipo> logger)
        {
            this.GridKeys = new List<string>
            {
               "ID_ATRIBUTO", "TP_LPN_TIPO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORDEN", SortDirection.Ascending),
            };

            this._logger = logger;
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._gridExcelService = gridExcelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            context.IsAddEnabled = false;
            context.IsEditingEnabled = true;
            context.IsCommitEnabled = true;

            var lpnEnUso = uow.ManejoLpnRepository.AnyTipoLpnEnUso(context.Parameters.Find(x => x.Id == "LpnTipo")?.Value);

            if (lpnEnUso)
                context.IsRemoveEnabled = false;
            else
                context.IsRemoveEnabled = true;

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_UP", new List<GridButton>
            {
                new GridButton("btnUp", "PAR401_frm1_btn_Subir", "fas fa-arrow-up"),
            }));

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_DOWN", new List<GridButton>
            {
                new GridButton("btnDown", "PAR401_frm1_btn_Bajar", "fas fa-arrow-down"),
            }));

            if (grid.Id == "PAR401Atributos_grid_1")
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
                {
                     new GridButton("btnEditarLpnTipoAtributo", "General_Sec0_btn_Editar", "fas fa-edit")
                }));
            }
            else
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
                {
                     new GridButton("btnEditarLpnTipoAtributoDet", "General_Sec0_btn_Editar", "fas fa-edit")
                }));
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var tpLpn = context.Parameters.Find(x => x.Id == "LpnTipo")?.Value;

            if (grid.Id == "PAR401Atributos_grid_1")
            {
                var dbQuery = new LpnTipoAtributoQuery(tpLpn);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.Rows.ForEach(row =>
                {
                    DisableButtons(row, uow, grid.Rows.Count());
                });
            }
            else
            {
                var dbQuery = new LpnTipoAtributoDetQuery(tpLpn);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                grid.Rows.ForEach(row =>
                {
                    DisableButtons(row, uow, grid.Rows.Count(), detalle: true);
                });
            }
            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            string tpLpn = context.Parameters.Find(x => x.Id == "LpnTipo")?.Value;
            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "PAR401Atributos_grid_1")
            {
                var dbQuery = new LpnTipoAtributoQuery(tpLpn);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                var dbQuery = new LpnTipoAtributoDetQuery(tpLpn);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var tpLpn = context.Parameters.Find(x => x.Id == "LpnTipo")?.Value;

            using var uow = this._uowFactory.GetUnitOfWork();

            if (grid.Id == "PAR401Atributos_grid_1")
            {
                var dbQuery = new LpnTipoAtributoQuery(tpLpn);
                uow.HandleQuery(dbQuery);

                context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";
                return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else
            {
                var dbQuery = new LpnTipoAtributoDetQuery(tpLpn);
                uow.HandleQuery(dbQuery);

                context.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";
                return this._gridExcelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                var idAtributo = int.Parse(context.Row.GetCell("ID_ATRIBUTO").Value);
                var tpLpn = context.Row.GetCell("TP_LPN_TIPO").Value;

                switch (context.ButtonId)
                {
                    case "btnUp":

                        if (context.GridId == "PAR401Atributos_grid_1")
                        {
                            var atributoTipo = uow.ManejoLpnRepository.GetLpnTipoAtributo(idAtributo, tpLpn);
                            uow.ManejoLpnRepository.CambiarOrdenLineaAtributoLpn(atributoTipo);
                        }
                        else
                        {
                            var atributoTipoDet = uow.ManejoLpnRepository.GetLpnAtributoTipoDet(idAtributo, tpLpn);
                            uow.ManejoLpnRepository.CambiarOrdenLineaLpnAtributoDetalle(atributoTipoDet);
                        }

                        uow.SaveChanges();

                        break;
                    case "btnDown":

                        if (context.GridId == "PAR401Atributos_grid_1")
                        {
                            LpnTipoAtributo atributoTipo = uow.ManejoLpnRepository.GetLpnTipoAtributo(idAtributo, tpLpn);
                            uow.ManejoLpnRepository.CambiarOrdenLineaAtributoLpn(atributoTipo, false);
                        }
                        else
                        {
                            LpnTipoAtributoDet atributoTipoDet = uow.ManejoLpnRepository.GetLpnAtributoTipoDet(idAtributo, tpLpn);
                            uow.ManejoLpnRepository.CambiarOrdenLineaLpnAtributoDetalle(atributoTipoDet, false);
                        }

                        uow.SaveChanges();

                        break;
                    case "btnEditarLpnTipoAtributo":

                        context.Parameters.Add(new ComponentParameter
                        {
                            Id = "ID_ATRIBUTO",
                            Value = context.Row.GetCell("ID_ATRIBUTO").Value
                        });

                        break;
                    case "btnEditarLpnTipoAtributoDet":

                        context.Parameters.Add(new ComponentParameter
                        {
                            Id = "ID_ATRIBUTO",
                            Value = context.Row.GetCell("ID_ATRIBUTO").Value
                        });

                        break;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "PAR401AtributosTipo - GridButtonAction");
            }

            return context;
        }

        public override Grid GridCommit(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            uow.BeginTransaction();

            try
            {
                if (grid.Id == "PAR401Atributos_grid_1")
                {
                    if (grid.Rows.Any())
                    {
                        var tpLpn = string.Empty;

                        foreach (var row in grid.Rows)
                        {
                            tpLpn = row.GetCell("TP_LPN_TIPO").Value;

                            if (row.IsDeleted)
                            {
                                var idAtributo = int.Parse(row.GetCell("ID_ATRIBUTO").Value);
                                var lpnEnUso = uow.ManejoLpnRepository.AnyTipoLpnEnUso(tpLpn);

                                if (!lpnEnUso)
                                {
                                    var atributoTipo = uow.ManejoLpnRepository.GetLpnTipoAtributo(idAtributo, tpLpn);
                                    uow.ManejoLpnRepository.DeleteLpnTipoAtributo(atributoTipo);
                                    uow.SaveChanges();
                                }
                                else
                                {
                                    context.AddErrorNotification("PAR401_Sec0_Error_ElTipoDeLpnEstaEnUso");
                                    return grid;
                                }
                            }
                        }
                        uow.ManejoLpnRepository.ReordenarAtributosLpn(tpLpn);
                    }
                }
                else
                {
                    if (grid.Rows.Any())
                    {
                        var tpLpn = string.Empty;

                        foreach (var row in grid.Rows)
                        {
                            tpLpn = row.GetCell("TP_LPN_TIPO").Value;

                            if (row.IsDeleted)
                            {
                                var Id_Atributo = int.Parse(row.GetCell("ID_ATRIBUTO").Value);
                                var lpnEnUso = uow.ManejoLpnRepository.AnyTipoLpnEnUso(tpLpn);

                                if (!lpnEnUso)
                                {
                                    var atributoTipo = uow.ManejoLpnRepository.GetLpnAtributoTipoDet(Id_Atributo, tpLpn);
                                    uow.ManejoLpnRepository.DeleteLpnTipoAtributoDet(atributoTipo);
                                    uow.SaveChanges();
                                }
                                else
                                {
                                    context.AddErrorNotification("PAR401_Sec0_Error_ElTipoDeLpnEstaEnUso");
                                    return grid;
                                }
                            }
                        }

                        uow.ManejoLpnRepository.ReordenarAtributosLpnDetalle(tpLpn);
                    }
                }

                uow.SaveChanges();
                uow.Commit();
                context.AddSuccessNotification("General_Sec0_msg_CambiosGuardados");
            }
            catch (Exception ex)
            {
                uow.Rollback();
                this._logger.LogError(ex, "PAR401AtributosTipo - GridCommit");
                context.AddErrorNotification("General_Sec0_Error_Operacion");
            }

            return grid;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var tpLpn = context.Parameters.Find(x => x.Id == "LpnTipo")?.Value;
            var nmLpn = context.Parameters.Find(x => x.Id == "Nombre")?.Value;
            var lpnEnUso = uow.ManejoLpnRepository.AnyTipoLpnEnUso(tpLpn);

            if (lpnEnUso)
            {
                form.GetButton("btnLpnAtributoDet").Disabled = true;
                form.GetButton("btnLpnAtributo").Disabled = true;
            }

            form.GetField("TP_LPN_TIPO").Value = tpLpn;
            form.GetField("NM_LPN_TIPO").Value = nmLpn;

            return form;
        }

        public override Form FormSubmit(Form form, FormSubmitContext context)
        {
            return form;
        }


        public virtual void DisableButtons(GridRow row, IUnitOfWork uow, int cantidadFilas, bool detalle = false)
        {
            var idAtributo = int.Parse(row.GetCell("ID_ATRIBUTO").Value);
            var tpLpn = row.GetCell("TP_LPN_TIPO").Value;

            short? orden;
            if (detalle)
                orden = uow.ManejoLpnRepository.GetLpnAtributoTipoDet(idAtributo, tpLpn)?.Orden;
            else
                orden = uow.ManejoLpnRepository.GetLpnTipoAtributo(idAtributo, tpLpn)?.Orden;

            if (orden == 1)
                row.DisabledButtons.Add("btnUp");

            //Si es la ultima desactivamos
            if (orden == cantidadFilas)
                row.DisabledButtons.Add("btnDown");
        }
    }
}
