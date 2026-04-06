using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.REC
{
    public class REC170ImpresionEtiquetasRecepcion : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC170ImpresionEtiquetasRecepcion(
            IIdentityService security, 
            IUnitOfWorkFactory uowFactory,
            IGridService gridService, 
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_ETIQUETA_LOTE", "CD_EMPRESA", "NU_AGENDA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_AGENDA", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._identity = security;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext query)
        {
            grid.MenuItems = new List<IGridItem> {
                    new GridButton("btnImprimir", "IMP050_grid1_btn_imprimir")
                };

            return this.GridFetchRows(grid, query.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EtiquetasDeRecepcionQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int numAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new EtiquetasDeRecepcionQuery(numAgenda);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }
            else
            {
                dbQuery = new EtiquetasDeRecepcionQuery();

                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, query, this.DefaultSort, this.GridKeys);
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EtiquetasDeRecepcionQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int numAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new EtiquetasDeRecepcionQuery(numAgenda);
            }
            else
            {
                dbQuery = new EtiquetasDeRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            EtiquetasDeRecepcionQuery dbQuery;

            if (query.Parameters.Count > 0)
            {
                if (!int.TryParse(query.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int numAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new EtiquetasDeRecepcionQuery(numAgenda);
            }
            else
            {
                dbQuery = new EtiquetasDeRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext selection)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            List<string> filasSeleccionadas = this.ObtenerKeyLineasSeleccionadas(uow, selection);

            selection.AddParameter("ListaFilasSeleccionadas", JsonConvert.SerializeObject(filasSeleccionadas));

            return selection;
        }

        public virtual List<string> ObtenerKeyLineasSeleccionadas(UnitOfWorkCore uow, GridMenuItemActionContext selection)
        {
            EtiquetasDeRecepcionQuery dbQuery;

            if (selection.Parameters.Count > 0)
            {
                if (!int.TryParse(selection.Parameters.FirstOrDefault(s => s.Id == "agenda")?.Value, out int numAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new EtiquetasDeRecepcionQuery(numAgenda);
            }
            else
            {
                dbQuery = new EtiquetasDeRecepcionQuery();
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, selection.Filters);

            List<string> resultado = new List<string>();

            if (selection.Selection.AllSelected)
            {
                var selectAll = dbQuery.GetResult().Select(g => new
                {
                    g.NU_ETIQUETA_LOTE,
                    g.CD_EMPRESA,
                    g.NU_AGENDA

                }).ToList();

                foreach (var noSeleccionKeys in selection.Selection.Keys)
                {
                    string[] deselection = noSeleccionKeys.Split('$');

                    selectAll.Remove(selectAll.FirstOrDefault(z =>
                        z.NU_ETIQUETA_LOTE == int.Parse(deselection[0])
                        && z.CD_EMPRESA == int.Parse(deselection[1])
                        && z.NU_AGENDA == int.Parse(deselection[2])
                    ));
                }

                foreach (var key in selectAll)
                {
                    resultado.Add(string.Join("$", new List<string> {
                        key.NU_ETIQUETA_LOTE.ToString(),
                        key.CD_EMPRESA.ToString(),
                        key.NU_AGENDA.ToString(),
                    }));
                }
            }
            else
            {
                foreach (var key in selection.Selection.Keys)
                {
                    resultado.Add(key);
                }
            }

            return resultado;
        }
    }
}