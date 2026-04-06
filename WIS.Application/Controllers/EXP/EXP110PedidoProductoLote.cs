using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Domain.General;
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
using WIS.Sorting;

namespace WIS.Application.Controllers.EXP
{
    public class EXP110PedidoProductoLote : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP110PedidoProductoLote(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_PRODUTO", "CD_EMPRESA", "NU_IDENTIFICADOR", "CD_FAIXA", "NU_PREPARACION", "NU_PEDIDO", "CD_CLIENTE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PREPARACION", SortDirection.Ascending),
                new SortCommand("NU_CONTENEDOR", SortDirection.Ascending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_SELECT", new List<GridButton> { new GridButton("btnSelectRow", "General_Sec0_btn_SelectRow", "fas fa-hand-point-right") }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var queryData = GetPedidoProductoLoteQueryData(context);

            if (queryData == null)
                return grid;

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PedidoProductoLoteQuery(queryData);
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            if (grid.Rows.Count == 1)
            {
                if (queryData.Empresa.HasValue)
                {
                    var identificador = grid.Rows.FirstOrDefault().GetCell("NU_IDENTIFICADOR").Value;
                    var qtProduto = grid.Rows.FirstOrDefault().GetCell("QT_PRODUTO").Value;

                    context.AddOrUpdateParameter("AUX_TIENE_UNA_ROW", string.Format("{0}${1}", identificador, qtProduto));
                }

                grid.Rows.FirstOrDefault().DisabledButtons.Add("btnSelectRow");

                return grid;
            }

            var rowSelected = context.GetParameter("AUX_ROW_SELECTED_PEDPRODLOTE");
            if (grid.Rows.Count > 1 && !string.IsNullOrEmpty(rowSelected))
            {
                var keys = rowSelected.Split('$');

                grid.Rows.FirstOrDefault(x => x.GetCell("CD_PRODUTO").Value == keys[0] &&
                                              x.GetCell("CD_EMPRESA").Value == keys[1] &&
                                              x.GetCell("NU_IDENTIFICADOR").Value == keys[2] &&
                                              x.GetCell("CD_FAIXA").Value == keys[3] &&
                                              x.GetCell("NU_PREPARACION").Value == keys[4] &&
                                              x.GetCell("NU_PEDIDO").Value == keys[5] &&
                                              x.GetCell("CD_CLIENTE").Value == keys[6]).CssClass = "blue";
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var queryData = GetPedidoProductoLoteQueryData(context);

            if (queryData == null)
                return new byte[0];

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PedidoProductoLoteQuery(queryData);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var queryData = GetPedidoProductoLoteQueryData(context);

            if (queryData == null)
            {
                return new GridStats
                {
                    Count = 0
                };
            }

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PedidoProductoLoteQuery(queryData);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.ButtonId == "btnSelectRow")
            {
                context.AddOrUpdateParameter("AUX_ROW_SELECTED_PEDPRODLOTE", $"{context.Row.Id}${context.Row.GetCell("QT_PRODUTO").Value}");
            }

            return context;
        }

        protected virtual PedidoProductoLoteQueryData GetPedidoProductoLoteQueryData(ComponentContext context)
        {
            var nuContenedorOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR");
            var nuPreparacionOrigen = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION");
            var confInicial = context.GetParameter("CONF_INICIAL");
            var dataProducto = context.GetParameter("AUX_PROD_LEIDO");
            var dataContenedorDestino = !string.IsNullOrEmpty(context.GetParameter("CONT_DESTINO_DATA")) ? JsonConvert.DeserializeObject<ContenedorDestinoData>(context.GetParameter("CONT_DESTINO_DATA")) : null;

            if (string.IsNullOrEmpty(confInicial) ||
                string.IsNullOrEmpty(nuContenedorOrigen) ||
                string.IsNullOrEmpty(nuPreparacionOrigen) ||
                string.IsNullOrEmpty(dataProducto))
            {
                return null;
            }

            var producto = JsonConvert.DeserializeObject<Producto>(dataProducto);
            var rowSelectedPedProdCont = context.GetParameter("AUX_ROW_SELECTED_PEDPRODCONT");

            var data = new PedidoProductoLoteQueryData
            {
                Contenedor = nuContenedorOrigen.ToNumber<int>(),
                Preparacion = nuPreparacionOrigen.ToNumber<int>(),
                Producto = producto.Codigo,
                FiltrarComparteContenedorEntrega = (dataContenedorDestino != null ? true : false),
                ComparteContenedorEntregaDestino = dataContenedorDestino?.CompartContenedorEntrega,
            };

            if (!string.IsNullOrEmpty(rowSelectedPedProdCont))
            {
                var keys = rowSelectedPedProdCont.Split('$');

                data.Empresa = keys[1].ToNumber<int>();
                data.Pedido = keys[3];
                data.Cliente = keys[4];
            }

            return data;
        }
    }
}
