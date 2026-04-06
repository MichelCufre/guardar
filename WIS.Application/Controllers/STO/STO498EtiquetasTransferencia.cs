using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Stock;
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

namespace WIS.Application.Controllers.STO
{
    public class STO498EtiquetasTransferencia : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IIdentityService _identity;
        protected readonly IGridExcelService _excelService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public STO498EtiquetasTransferencia(IUnitOfWorkFactory uowFactory, IGridService gridService, IFilterInterpreter filterInterpreter, IIdentityService identity, IGridExcelService excelService)
        {
            this.GridKeys = new List<string>
            {
                "NU_ETIQUETA",
                "NU_SEC_ETIQUETA",
                "CD_ENDERECO_ORIGEN",
                "NU_IDENTIFICADOR",
                "CD_PRODUTO",
                "CD_FAIXA",
                "CD_EMPRESA",
                "NU_SEC_DETALLE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_PRODUTO", SortDirection.Ascending)
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._identity = identity;
            this._excelService = excelService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.MenuItems = new List<IGridItem> {
                    new GridButton("btnImprimir", "IMP050_grid1_btn_imprimir")
            };
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EtiquetasTransferenciaQuery();

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            var dbQuery = new EtiquetasTransferenciaQuery();

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

        public virtual List<string> ObtenerKeyLineasSeleccionadas(IUnitOfWork uow, GridMenuItemActionContext selection)
        {
            EtiquetasTransferenciaQuery dbQuery = new EtiquetasTransferenciaQuery();

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, selection.Filters);

            List<string> resultado = new List<string>();

            string[] primaryKeyObjeto = new string[] { "NU_ETIQUETA",
                "NU_SEC_ETIQUETA",
                "CD_ENDERECO_ORIGEN",
                "NU_IDENTIFICADOR",
                "CD_PRODUTO",
                "CD_FAIXA",
                "CD_EMPRESA",
                "NU_SEC_DETALLE" };

            if (selection.Selection.AllSelected)
            {
                var selectAll = dbQuery.GetResult().Select(g => new
                {
                    g.NU_ETIQUETA,
                    g.NU_SEC_ETIQUETA,
                    g.CD_ENDERECO_ORIGEN,
                    g.NU_IDENTIFICADOR,
                    g.CD_PRODUTO,
                    g.CD_FAIXA,
                    g.CD_EMPRESA,
                    g.NU_SEC_DETALLE
                }).ToList();

                foreach (var noSeleccionKeys in selection.Selection.Keys)
                {
                    string[] deselecion = noSeleccionKeys.Split('$');



                    selectAll.Remove(selectAll.FirstOrDefault(z =>
                           z.NU_ETIQUETA == decimal.Parse(deselecion[0], _identity.GetFormatProvider())
                        && z.NU_SEC_ETIQUETA == int.Parse(deselecion[1])
                        && z.CD_ENDERECO_ORIGEN == deselecion[2]
                        && z.NU_IDENTIFICADOR == deselecion[3]
                        && z.CD_PRODUTO == deselecion[4]
                        && z.CD_FAIXA == decimal.Parse(deselecion[5], _identity.GetFormatProvider())
                        && z.CD_EMPRESA == int.Parse(deselecion[6])
                        && z.NU_SEC_DETALLE == int.Parse(deselecion[7])
                    ));
                }

                foreach (var key in selectAll)
                {
                    resultado.Add(string.Join("$", new List<string> {
                        key.NU_ETIQUETA.ToString(),
                        key.NU_SEC_ETIQUETA.ToString(),
                        key.CD_ENDERECO_ORIGEN,
                        key.NU_IDENTIFICADOR,
                        key.CD_PRODUTO,
                        key.CD_FAIXA.ToString(_identity.GetFormatProvider()),
                        key.CD_EMPRESA.ToString(),
                        key.NU_SEC_DETALLE.ToString()
                    }));
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

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EtiquetasTransferenciaQuery();

            uow.HandleQuery(dbQuery);

            query.FileName = $"{this._identity.Application}-{DateTime.Now.ToString("yyyy-MM-dd_HH:mm")}.xlsx";

            return this._excelService.GetExcel(query.FileName, dbQuery, grid.Columns, query, this.DefaultSort);
        }
    }
}
