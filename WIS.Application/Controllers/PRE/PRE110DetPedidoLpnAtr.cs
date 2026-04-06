using DocumentFormat.OpenXml.InkML;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Validation;
using WIS.Application.Validation.Modules.GridModules;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Filtering;
using WIS.FormComponent.Execution.Configuration;
using WIS.FormComponent;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    //Modificado Mauro 2021-02-25
    public class PRE110DetPedidoLpnAtr : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly ILogger<PRE110DetPedidoLpnAtr> _logger;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrackingService _trackingService;
        protected readonly ITaskQueueService _taskQueue;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE110DetPedidoLpnAtr(
            IIdentityService identity,
            ILogger<PRE110DetPedidoLpnAtr> logger,
            IUnitOfWorkFactory uowFactory,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrackingService trackingService,
            ITaskQueueService taskQueue)
        {
            this.GridKeys = new List<string>
            {"NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR", "TP_LPN_TIPO", "ID_LPN_EXTERNO", "NU_DET_PED_SAI_ATRIB", "ID_ATRIBUTO", "FL_CABEZAL"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("ID_ATRIBUTO", SortDirection.Descending),
                new SortCommand("FL_CABEZAL", SortDirection.Descending),
            };

            this._logger = logger;
            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._trackingService = trackingService;
            _taskQueue = taskQueue;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.IsEditingEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsAddEnabled = false;
            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            PedidosLpnAtributosPendientesQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                var pedido = context.Parameters.FirstOrDefault(x => x.Id == "Pedido").Value.ToString();
                var empresa = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Empresa").Value.ToString());
                var cliente = context.Parameters.FirstOrDefault(x => x.Id == "Cliente").Value.ToString();
                var idEspecificaIdentificador = context.Parameters.FirstOrDefault(x => x.Id == "IdEspecificaIdentificador").Value.ToString();
                var idLpnExteno = context.Parameters.FirstOrDefault(x => x.Id == "IdLpnExteno").Value.ToString();
                var lpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value.ToString();
                var producto = context.Parameters.FirstOrDefault(x => x.Id == "Producto").Value.ToString();
                var identificador = context.Parameters.FirstOrDefault(x => x.Id == "Identificador").Value.ToString();
                var faixa = decimal.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Faixa").Value.ToString(), _identity.GetFormatProvider());
                var nuDetPedSaiAtrib = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "NuDetPedSaiAtrib").Value.ToString());

                dbQuery = new PedidosLpnAtributosPendientesQuery(pedido, empresa, cliente, idEspecificaIdentificador, idLpnExteno, lpnTipo, producto, identificador, faixa, nuDetPedSaiAtrib);

                uow.HandleQuery(dbQuery);


                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            grid.SetEditableCells(new List<string> { "AUXQT_ANULADO", "AUXDS_MOTIVO" });

            return grid;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var pedido = context.Parameters.FirstOrDefault(x => x.Id == "Pedido").Value.ToString();
            var empresa = int.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Empresa").Value.ToString());
            var cliente = context.Parameters.FirstOrDefault(x => x.Id == "Cliente").Value.ToString();
            var idEspecificaIdentificador = context.Parameters.FirstOrDefault(x => x.Id == "IdEspecificaIdentificador").Value.ToString();
            var idLpnExteno = context.Parameters.FirstOrDefault(x => x.Id == "IdLpnExteno").Value.ToString();
            var lpnTipo = context.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value.ToString();
            var producto = context.Parameters.FirstOrDefault(x => x.Id == "Producto").Value.ToString();
            var identificador = context.Parameters.FirstOrDefault(x => x.Id == "Identificador").Value.ToString();
            var faixa = decimal.Parse(context.Parameters.FirstOrDefault(x => x.Id == "Faixa").Value.ToString(), _identity.GetFormatProvider());
            var nuDetPedSaiAtrib = long.Parse(context.Parameters.FirstOrDefault(x => x.Id == "NuDetPedSaiAtrib").Value.ToString());

            var dbQuery = new PedidosLpnAtributosPendientesQuery(pedido, empresa, cliente, idEspecificaIdentificador, idLpnExteno, lpnTipo, producto, identificador, faixa, nuDetPedSaiAtrib);

            uow.HandleQuery(dbQuery);
            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);



        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();


            var pedido = query.Parameters.FirstOrDefault(x => x.Id == "Pedido").Value.ToString();
            var empresa = int.Parse(query.Parameters.FirstOrDefault(x => x.Id == "Empresa").Value.ToString());
            var cliente = query.Parameters.FirstOrDefault(x => x.Id == "Cliente").Value.ToString();
            var idEspecificaIdentificador = query.Parameters.FirstOrDefault(x => x.Id == "IdEspecificaIdentificador").Value.ToString();
            var idLpnExteno = query.Parameters.FirstOrDefault(x => x.Id == "IdLpnExteno").Value.ToString();
            var lpnTipo = query.Parameters.FirstOrDefault(x => x.Id == "LpnTipo").Value.ToString();
            var producto = query.Parameters.FirstOrDefault(x => x.Id == "Producto").Value.ToString();
            var identificador = query.Parameters.FirstOrDefault(x => x.Id == "Identificador").Value.ToString();
            var faixa = decimal.Parse(query.Parameters.FirstOrDefault(x => x.Id == "Faixa").Value.ToString(), _identity.GetFormatProvider());
            var nuDetPedSaiAtrib = long.Parse(query.Parameters.FirstOrDefault(x => x.Id == "NuDetPedSaiAtrib").Value.ToString());
            var dbQuery = new PedidosLpnAtributosPendientesQuery(pedido, empresa, cliente, idEspecificaIdentificador, idLpnExteno, lpnTipo, producto, identificador, faixa, nuDetPedSaiAtrib);

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);
            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var pedidoId = context.GetParameter("Pedido");
            var productoId = context.GetParameter("Producto");
            var clienteId = context.GetParameter("Cliente");
            var empresaId = int.Parse(context.GetParameter("Empresa"));
            var identificador = context.GetParameter("Identificador");
            var nuDetPedSaiAtrib = context.GetParameter("NuDetPedSaiAtrib");


            var pedido = uow.PedidoRepository.GetPedido(empresaId, clienteId, pedidoId);
            var empresa = uow.EmpresaRepository.GetEmpresa(empresaId);
            var cliente = uow.AgenteRepository.GetAgente(empresaId, clienteId);
            var dsproducto = uow.ProductoRepository.GetDescripcion(empresaId, productoId);

            form.GetField("pedido").Value = pedidoId;
            form.GetField("codEmpresa").Value = empresaId.ToString();
            form.GetField("empresa").Value = empresa.Nombre;
            form.GetField("codCliente").Value = clienteId;
            form.GetField("cliente").Value = cliente.Descripcion;
            form.GetField("codProducto").Value = productoId;
            form.GetField("producto").Value = dsproducto;
            form.GetField("identificador").Value = identificador;
            form.GetField("nuDetPedSaiAtrib").Value = nuDetPedSaiAtrib;
            return form;
        }
    }
}
