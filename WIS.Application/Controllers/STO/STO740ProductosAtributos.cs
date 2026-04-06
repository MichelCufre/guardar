using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.FormModules;
using WIS.Components.Common;
using WIS.Components.Common.Select;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.StockEntities;
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

namespace WIS.Application.Controllers.STO
{
    public class STO740ProductosAtributos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFormValidationService _formValidationService;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly Logger _logger;
        protected List<string> GridKeysLpnDetalle { get; }
        protected List<string> GridKeysAtributo { get; }
        protected List<SortCommand> SortsLpnDetalle { get; }
        protected List<SortCommand> SortsAtributo { get; }
        protected List<string> EditableColumnsAtributo { get; }

        public STO740ProductosAtributos(
            IGridExcelService excelService,
            IUnitOfWorkFactory uowFactory,
        IGridService gridService,
                    IFormValidationService formValidationService,
            IIdentityService identity,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeysLpnDetalle = new List<string>
            {
                "ID_LPN_DET",
                "NU_LPN",
                "CD_PRODUTO",
                "CD_FAIXA",
                "CD_EMPRESA",
                "NU_IDENTIFICADOR",
            };

            this.GridKeysAtributo = new List<string>
            {
                "ID_ATRIBUTO",
            };

            this.SortsLpnDetalle = new List<SortCommand> {
                new SortCommand("ID_LPN_DET", SortDirection.Descending),
                new SortCommand("NU_LPN", SortDirection.Descending),
                new SortCommand("CD_PRODUTO", SortDirection.Descending),
                new SortCommand("CD_FAIXA", SortDirection.Descending),
                new SortCommand("CD_EMPRESA", SortDirection.Descending),
                new SortCommand("NU_IDENTIFICADOR", SortDirection.Descending),
            };

            this.SortsAtributo = new List<SortCommand> {
                new SortCommand("NM_ATRIBUTO", SortDirection.Ascending),
            };

            this.EditableColumnsAtributo = new List<string>
            {
                "VL_ATRIBUTO"
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._formValidationService = formValidationService;
            this._excelService = excelService;
            this._identity = identity;
            this._filterInterpreter = filterInterpreter;
            this._logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override List<SelectOption> FormSelectSearch(Form form, FormSelectSearchContext context)
        {
            switch (context.FieldId)
            {
                case "tipo":
                    return SearchTiposLPN(form, context);
                default:
                    return new List<SelectOption>();
            }
        }

        public virtual List<SelectOption> SearchTiposLPN(Form form, FormSelectSearchContext context)
        {
            var opciones = new List<SelectOption>();

            using (var uow = this._uowFactory.GetUnitOfWork())
            {
                var tipos = uow.ManejoLpnRepository.GetAllTipoLPNByDescriptionOrCodePartial(context.SearchValue);

                foreach (var tipo in tipos)
                {
                    opciones.Add(new SelectOption(tipo.Tipo, $"{tipo.Tipo} - {tipo.Descripcion}"));
                }
            }

            return opciones;
        }

        public override Form FormValidateForm(Form form, FormValidationContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return this._formValidationService.Validate(new STO740ProductosAtributosValidationModule(uow, this._identity.UserId, this._identity.Predio), form, context);
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            return form;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "STO740_grid_AtrCab")
            {
                context.IsEditingEnabled = true;
                context.IsAddEnabled = false;
                context.IsCommitEnabled = false;
                context.IsRemoveEnabled = false;
                context.IsRollbackEnabled = false;
            }
            else if (grid.Id == "STO740_grid_AtrDet")
            {
                context.IsEditingEnabled = true;
                context.IsAddEnabled = false;
                context.IsCommitEnabled = false;
                context.IsRemoveEnabled = false;
                context.IsRollbackEnabled = false;
            }
            else
            {
                grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem> {
                    new GridButton("btnAtributosCabezal", "STO740_grd1_btn_AtributosCabezal", "fas fa-list"),
                    new GridButton("btnAtributosDetalle", "STO740_grd1_btn_AtributosDetalle", "fas fa-list"),
                }));
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var tipo = context.GetParameter("tipo");

            if (grid.Id == "STO740_grid_AtrCab")
            {
                var dbQuery = new ConsultaAtributosLpnQuery(tipo);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.SortsAtributo, this.GridKeysAtributo);

                grid.SetEditableCells(this.EditableColumnsAtributo);
            }
            else if (grid.Id == "STO740_grid_AtrDet")
            {
                var dbQuery = new ConsultaAtributosDetalleLpnQuery(tipo);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.SortsAtributo, this.GridKeysAtributo);

                grid.SetEditableCells(this.EditableColumnsAtributo);
            }
            else
            {
                GetAtributos(context, out tipo, out Dictionary<int, string> atributosCabezal, out Dictionary<int, string> atributosDetalle);
                var dbQuery = new ConsultaProductosAtributosQuery(tipo, atributosCabezal, atributosDetalle);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.SortsLpnDetalle, this.GridKeysLpnDetalle);
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var tipo = context.GetParameter("tipo");

            if (grid.Id == "STO740_grid_AtrCab")
            {
                var dbQuery = new ConsultaAtributosLpnQuery(tipo);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "STO740_grid_AtrDet")
            {
                var dbQuery = new ConsultaAtributosDetalleLpnQuery(tipo);

                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else
            {
                GetAtributos(context, out tipo, out Dictionary<int, string> atributosCabezal, out Dictionary<int, string> atributosDetalle);
                var dbQuery = new ConsultaProductosAtributosQuery(tipo, atributosCabezal, atributosDetalle);

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
            using var uow = this._uowFactory.GetUnitOfWork();

            var tipo = context.GetParameter("tipo");

            if (grid.Id == "STO740_grid_AtrCab")
            {
                var dbQuery = new ConsultaAtributosLpnQuery(tipo);

                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.SortsAtributo);
            }
            else if (grid.Id == "STO740_grid_AtrDet")
            {
                var dbQuery = new ConsultaAtributosDetalleLpnQuery(tipo);

                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.SortsAtributo);
            }
            else
            {
                GetAtributos(context, out tipo, out Dictionary<int, string> atributosCabezal, out Dictionary<int, string> atributosDetalle);
                var dbQuery = new ConsultaProductosAtributosQuery(tipo, atributosCabezal, atributosDetalle);

                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.SortsLpnDetalle);
            }
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            if (context.GridId == "STO740_grid_LpnDet")
            {
                switch (context.ButtonId)
                {
                    case "btnAtributosCabezal":
                        context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter(){ Id = "detalle", Value = "false" },
                        });
                        break;

                    case "btnAtributosDetalle":
                        context.Redirect("/stock/STO710", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "numeroLPN", Value = context.Row.GetCell("NU_LPN").Value },
                            new ComponentParameter(){ Id = "detalle", Value = "true" },
                            new ComponentParameter(){ Id = "idDetalle", Value = context.Row.GetCell("ID_LPN_DET").Value },
                        });
                        break;
                }
            }

            return context;
        }

        public static void GetAtributos(ComponentContext context, out string tipo, out Dictionary<int, string> atributosCabezal, out Dictionary<int, string> atributosDetalle)
        {
            var jsonFiltro = context.GetParameter("filtro");
            var filtro = JsonConvert.DeserializeObject<FiltroLpn>(jsonFiltro);

            tipo = filtro.TipoLpn;
            atributosCabezal = new Dictionary<int, string>();
            atributosDetalle = new Dictionary<int, string>();

            foreach (var atributo in filtro.AtributosCabezal)
            {
                atributosCabezal[int.Parse(atributo.Id)] = atributo.Value;
            }

            foreach (var atributo in filtro.AtributosDetalle)
            {
                atributosDetalle[int.Parse(atributo.Id)] = atributo.Value;
            }
        }
    }
}