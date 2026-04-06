using DocumentFormat.OpenXml.InkML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Application.Security;
using WIS.Application.Validation;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.General;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Dtos;
using WIS.Domain.Services.Interfaces;
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
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;
using WIS.Validation;

namespace WIS.Application.Controllers.PRE
{
    public class PRE100DetallePedidoLpnAtributo : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridValidationService _gridValidationService;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IParameterService _paramService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE100DetallePedidoLpnAtributo(IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridValidationService gridValidationService,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            IParameterService paramService)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "NU_IDENTIFICADOR", "CD_FAIXA", "ID_ESPECIFICA_IDENTIFICADOR", "TP_LPN_TIPO", "ID_LPN_EXTERNO", "NU_DET_PED_SAI_ATRIB"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridValidationService = gridValidationService;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._paramService = paramService;
        }

        public override PageContext PageLoad(PageContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = GetDatos(context.Parameters);
            var editable = PermiteEdicion(uow, datos, btnAgregarAtributos: true);

            context.Parameters.Add(new ComponentParameter("editable", editable ? "S" : "N"));

            var empresa = uow.EmpresaRepository.GetEmpresa(datos.Empresa);
            var cliente = uow.AgenteRepository.GetAgente(datos.Empresa, datos.Cliente);

            context.AddParameter("empresaNombre", empresa.Nombre);
            context.AddParameter("agenteDescripcion", cliente.Descripcion);
            context.AddParameter("agenteCodigo", cliente.Codigo);
            context.AddParameter("agenteTipo", cliente.Tipo);

            return context;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            context.IsAddEnabled = false;
            context.IsRemoveEnabled = false;
            context.IsCommitEnabled = false;
            context.IsEditingEnabled = false;

            var datos = GetDatos(context.Parameters);
            var editable = PermiteEdicion(uow, datos);

			if (editable)
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
                {
                    new GridButton("btnEditar", "PRE100DetallePedidoLpnAtributo_grid1_btn_Atributos", "fas fa-edit"),
                }));
            }
            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = GetDatos(context.Parameters);

            var dbQuery = new DetallePedidoLpnAtributoQuery(datos);
            uow.HandleQuery(dbQuery);
            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = GetDatos(context.Parameters);

            var dbQuery = new DetallePedidoLpnAtributoQuery(datos);
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var datos = GetDatos(context.Parameters);

            var dbQuery = new DetallePedidoLpnAtributoQuery(datos);
            uow.HandleQuery(dbQuery);
            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public virtual DetallePedidoLpnEspecifico GetDatos(List<ComponentParameter> parametros)
        {
            return new DetallePedidoLpnEspecifico()
            {
                Pedido = parametros.FirstOrDefault(p => p.Id == "pedido").Value,
                Cliente = parametros.FirstOrDefault(p => p.Id == "cliente").Value,
                Empresa = int.Parse(parametros.FirstOrDefault(p => p.Id == "empresa").Value),
                Producto = parametros.FirstOrDefault(p => p.Id == "producto").Value,
                Faixa = decimal.Parse(parametros.FirstOrDefault(p => p.Id == "faixa").Value, _identity.GetFormatProvider()),
                Identificador = parametros.FirstOrDefault(p => p.Id == "identificador").Value,
                IdEspecificaIdentificador = parametros.FirstOrDefault(p => p.Id == "idEspecificaIdentificador").Value,
                TipoLpn = parametros.FirstOrDefault(p => p.Id == "tipoLpn").Value,
                IdExternoLpn = parametros.FirstOrDefault(p => p.Id == "idExternoLpn").Value,
            };
        }

        public virtual bool PermiteEdicion(IUnitOfWork uow, DetallePedidoLpnEspecifico datos, bool btnAgregarAtributos = false)
        {
            var permiteAgregar = true;
            var pedido = uow.PedidoRepository.GetPedido(datos.Empresa, datos.Cliente, datos.Pedido);
            var lpnHabilitados = (_paramService.GetValueByEmpresa(ParamManager.IE_503_HAB_LPN, datos.Empresa) ?? "N") == "S";
            var atributosHabilitados = (_paramService.GetValueByEmpresa(ParamManager.IE_503_HAB_ATRIBUTOS, datos.Empresa) ?? "N") == "S";

            if (btnAgregarAtributos)
            {
                var detallePedidoLpn = uow.ManejoLpnRepository.GetDetallePedidoLpn(datos.Pedido, datos.Cliente, datos.Empresa, datos.Producto, datos.Faixa,
                    datos.Identificador, datos.IdEspecificaIdentificador, datos.TipoLpn, datos.IdExternoLpn);

                var detsPedidoLpnAtributos = uow.ManejoLpnRepository.GetDetallesPedidoLpnAtributo(datos.Pedido, datos.Cliente, datos.Empresa, datos.Producto, datos.Faixa,
                    datos.Identificador, datos.IdEspecificaIdentificador, datos.TipoLpn, datos.IdExternoLpn);

                permiteAgregar = ((detallePedidoLpn.CantidadPedida ?? 0) - (detsPedidoLpnAtributos.Sum(d => (d.CantidadPedida)))) > 0;
            }

            return pedido.IsManual && atributosHabilitados && lpnHabilitados && permiteAgregar;
        }
    }
}
