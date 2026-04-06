using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Expedicion.EXP110EmpaquetadoPicking.Dto;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.EXP
{
    public class EXP110PickeosPendientes : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP110PickeosPendientes(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "CD_PRODUTO", "NU_IDENTIFICADOR", "NU_PEDIDO", "CD_FAIXA", "CD_EMPRESA", "NU_PREPARACION", "CD_CLIENTE"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("CD_EMPRESA", SortDirection.Ascending),
                new SortCommand("CD_PRODUTO", SortDirection.Ascending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            var queryData = GetPickeosPendientesQueryData(context);

            if (queryData == null)
                return grid;

            using var uow = this._uowFactory.GetUnitOfWork();

            PickeosPendientesQuery dbQuery = new PickeosPendientesQuery(queryData.Pedido, queryData.Cliente, queryData.Empresa);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            var queryData = GetPickeosPendientesQueryData(context);

            if (queryData == null)
                return new byte[0];

            using var uow = this._uowFactory.GetUnitOfWork();

            PickeosPendientesQuery dbQuery = new PickeosPendientesQuery(queryData.Pedido, queryData.Cliente, queryData.Empresa);
            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            var queryData = GetPickeosPendientesQueryData(context);

            if (queryData == null)
                return new GridStats
                {
                    Count = 0
                };

            using var uow = this._uowFactory.GetUnitOfWork();

            PickeosPendientesQuery dbQuery = new PickeosPendientesQuery(queryData.Pedido, queryData.Cliente, queryData.Empresa);
            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        protected virtual PickeosPendientesQueryData GetPickeosPendientesQueryData(ComponentContext context)
        {
            string nuContenedor = context.GetParameter("AUX_CONT_ORIGEN_NU_CONTENEDOR");
            string nuPreparacion = context.GetParameter("AUX_CONT_ORIGEN_NU_PREPARACION");
            string confInicial = context.GetParameter("CONF_INICIAL");

            if (string.IsNullOrEmpty(confInicial))
                return null;

            ContenedorDestinoData contenedorDestino = !string.IsNullOrEmpty(context.GetParameter("CONT_DESTINO_DATA")) ?
                                                   JsonConvert.DeserializeObject<ContenedorDestinoData>(context.GetParameter("CONT_DESTINO_DATA")) :
                                                   null;

            DatosClientePedidoOriginal contenedorOrigen = !string.IsNullOrEmpty(context.GetParameter("CONT_ORIGEN_DATA")) ?
                                                                JsonConvert.DeserializeObject<DatosClientePedidoOriginal>(context.GetParameter("CONT_ORIGEN_DATA")) :
                                                                null;

            string nuPedido = contenedorOrigen != null ? contenedorOrigen.NumeroPedido : contenedorDestino != null ? contenedorDestino.NumeroPedido : "";
            string cdCliente = contenedorOrigen != null ? contenedorOrigen.CodigoCliente : contenedorDestino != null ? contenedorDestino.CodigoCliente : "";
            int? empresa = contenedorOrigen != null ? contenedorOrigen.Empresa : contenedorDestino != null ? contenedorDestino.CodigoEmpresa : 1;

            if (string.IsNullOrEmpty(nuPedido) || string.IsNullOrEmpty(cdCliente) || empresa == null)
                return null;

            return new PickeosPendientesQueryData
            {
                Pedido = nuPedido,
                Cliente = cdCliente,
                Empresa = empresa.Value
            };
        }
    }

    public class PickeosPendientesQueryData
    {
        public string Pedido { get; set; }
        public string Cliente { get; set; }
        public int Empresa { get; set; }
    }
}
