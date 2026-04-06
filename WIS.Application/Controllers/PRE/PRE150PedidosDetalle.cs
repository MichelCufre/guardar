using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Preparacion;
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

namespace WIS.Application.Controllers.PRE
{
    public class PRE150PedidosDetalle : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public PRE150PedidosDetalle(IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
                "NU_PEDIDO", "CD_CLIENTE", "CD_EMPRESA", "CD_PRODUTO", "CD_FAIXA", "NU_IDENTIFICADOR", "ID_ESPECIFICA_IDENTIFICADOR"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Ascending)
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.AddLink("CD_AGENTE", "registro/REG220", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_CLIENTE", "cliente"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            context.AddLink("CD_PRODUTO", "registro/REG009", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_PRODUTO", "producto"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            grid.AddOrUpdateColumn(new GridColumnItemList("BTN_LIST", new List<IGridItem>
            {
                new GridButton("btnLpn", "PRE150_grd1_btn_Lpn", "fas fa-list"),
                new GridButton("btnAtributos", "PRE150_grd1_btn_Atributos", "fas fa-list"),
            }));

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConsultaDetallePedidoQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idCliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value;
                if (string.IsNullOrEmpty(idCliente))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idPedido = context.Parameters.FirstOrDefault(s => s.Id == "pedido").Value;
                if (string.IsNullOrEmpty(idPedido))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ConsultaDetallePedidoQuery(idEmpresa, idPedido, idCliente);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

                var empresa = uow.EmpresaRepository.GetEmpresa(idEmpresa);
                var agente = uow.AgenteRepository.GetAgente(idEmpresa, idCliente);

                context.AddParameter("PRE150_CD_AGENTE", agente.Codigo);
                context.AddParameter("PRE150_DS_AGENTE", agente.Descripcion);
                context.AddParameter("PRE150_DS_TIPO_AGENTE", agente.Tipo.ToString());
                context.AddParameter("PRE150_CD_EMPRESA", idEmpresa.ToString());
                context.AddParameter("PRE150_NM_EMPRESA", empresa.Nombre);
                context.AddParameter("PRE150_NU_PEDIDO", idPedido);

                grid.GetColumn("NU_PEDIDO").Hidden = true;
                grid.GetColumn("DS_TIPO_AGENTE").Hidden = true;
                grid.GetColumn("CD_AGENTE").Hidden = true;
                grid.GetColumn("DS_AGENTE").Hidden = true;
                grid.GetColumn("CD_EMPRESA").Hidden = true;
                grid.GetColumn("NM_EMPRESA").Hidden = true;
            }
            else
            {
                dbQuery = new ConsultaDetallePedidoQuery();

                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }

            VisibilidadBotones(grid);

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConsultaDetallePedidoQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idCliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value;
                if (string.IsNullOrEmpty(idCliente))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idPedido = context.Parameters.FirstOrDefault(s => s.Id == "pedido").Value;
                if (string.IsNullOrEmpty(idPedido))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ConsultaDetallePedidoQuery(idEmpresa, idPedido, idCliente);
            }
            else
                dbQuery = new ConsultaDetallePedidoQuery();

            uow.HandleQuery(dbQuery);
            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            ConsultaDetallePedidoQuery dbQuery;

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "empresa").Value, out int idEmpresa))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idCliente = context.Parameters.FirstOrDefault(s => s.Id == "cliente").Value;
                if (string.IsNullOrEmpty(idCliente))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                var idPedido = context.Parameters.FirstOrDefault(s => s.Id == "pedido").Value;
                if (string.IsNullOrEmpty(idPedido))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                dbQuery = new ConsultaDetallePedidoQuery(idEmpresa, idPedido, idCliente);
            }
            else
                dbQuery = new ConsultaDetallePedidoQuery();

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

            try
            {
                switch (context.ButtonId)
                {
                    case "btnLpn":
                        context.Redirect("/preparacion/PRE152", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "numPed", Value = context.Row.GetCell("NU_PEDIDO").Value },
                            new ComponentParameter(){ Id = "codCli", Value = context.Row.GetCell("CD_CLIENTE").Value },
                            new ComponentParameter(){ Id = "codEmp", Value = context.Row.GetCell("CD_EMPRESA").Value },
                            new ComponentParameter(){ Id = "codPro", Value = context.Row.GetCell("CD_PRODUTO").Value },
                            new ComponentParameter(){ Id = "codFai", Value = context.Row.GetCell("CD_FAIXA").Value },
                            new ComponentParameter(){ Id = "numIde", Value = context.Row.GetCell("NU_IDENTIFICADOR").Value },
                            new ComponentParameter(){ Id = "idEspIde", Value = context.Row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value },
                        });
                        break;

                    case "btnAtributos":
                        context.Redirect("/preparacion/PRE155", true, new List<ComponentParameter>()
                        {
                            new ComponentParameter(){ Id = "numPed", Value = context.Row.GetCell("NU_PEDIDO").Value },
                            new ComponentParameter(){ Id = "codCli", Value = context.Row.GetCell("CD_CLIENTE").Value },
                            new ComponentParameter(){ Id = "codEmp", Value = context.Row.GetCell("CD_EMPRESA").Value },
                            new ComponentParameter(){ Id = "codPro", Value = context.Row.GetCell("CD_PRODUTO").Value },
                            new ComponentParameter(){ Id = "codFai", Value = context.Row.GetCell("CD_FAIXA").Value },
                            new ComponentParameter(){ Id = "numIde", Value = context.Row.GetCell("NU_IDENTIFICADOR").Value },
                            new ComponentParameter(){ Id = "idEspIde", Value = context.Row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value },
                        });
                        break;
                }
            }
            catch (ValidationFailedException ex)
            {
                if (!string.IsNullOrEmpty(ex.Message))
                    context.AddErrorNotification(ex.Message, new List<string>(ex.StrArguments ?? new string[0]));
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return context;
        }

        #region Metodos Auxiliares

        public virtual void VisibilidadBotones(Grid grid)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            foreach (var row in grid.Rows)
            {
                var botonesDeshabilitados = new List<string>();

                var nuPedido = row.GetCell("NU_PEDIDO").Value;
                var codCliente = row.GetCell("CD_CLIENTE").Value;
                var codEmpresa = int.Parse(row.GetCell("CD_EMPRESA").Value);
                var codProducto = row.GetCell("CD_PRODUTO").Value;
                var codFaixa = decimal.Parse(row.GetCell("CD_FAIXA").Value, this._identity.GetFormatProvider());
                var nuIdentificador = row.GetCell("NU_IDENTIFICADOR").Value;
                var idEspecificaIDen = row.GetCell("ID_ESPECIFICA_IDENTIFICADOR").Value;

                if (!uow.PedidoRepository.AnyLpnPedido(nuPedido, codCliente, codEmpresa, codProducto, codFaixa, nuIdentificador, idEspecificaIDen))
                    botonesDeshabilitados.Add("btnLpn");
                else
                    row.DisabledButtons.Remove("btnLpn");

                if (!uow.PedidoRepository.AnyAtributoPedido(nuPedido, codCliente, codEmpresa, codProducto, codFaixa, nuIdentificador, idEspecificaIDen))
                    botonesDeshabilitados.Add("btnAtributos");
                else
                    row.DisabledButtons.Remove("btnAtributos");

                row.DisabledButtons = botonesDeshabilitados;
            }
        }

        #endregion
    }
}
