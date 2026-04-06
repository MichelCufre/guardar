using System;
using System.Collections.Generic;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Picking;
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
    public class EXP150PedidoComplSinEmpaque : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP150PedidoComplSinEmpaque(IIdentityService identity, IUnitOfWorkFactory uowFactory, IGridService gridService, IGridExcelService excelService, IFilterInterpreter filterInterpreter)
        {
            this.GridKeys = new List<string>
            {
               "CD_EMPRESA","CD_CLIENTE","TP_PEDIDO","NU_PEDIDO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_ORDEN", SortDirection.Descending),
                new SortCommand("DT_ENTREGA", SortDirection.Descending),
                new SortCommand("CD_ROTA", SortDirection.Descending)
            };
            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;

        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
                    {
                        new GridButton("BtnAsignar", "EXP150_Sec0_btn_Asignar", "fa fa-plus-circle"),
                        new GridButton("BtnVerContenedor", "EXP150_Sec0_btn_VerContenedor", "fas fa-list"),
                    }));


            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EXP150PedidoSinEmpaquetarQuery();
            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            grid.Rows.ForEach(row =>
            {
                string ds_anexo = row.GetCell("DS_ANEXO3").Value;
                if (string.IsNullOrEmpty(ds_anexo))
                {
                    row.DisabledButtons.Add("BtnVerContenedor");
                }
                string DS_FUNCAO = uow.FuncionarioRepository.GetFuncionario(this._identity.UserId).Descripcion;
                string cond_lib = row.GetCell("CD_CONDICION_LIBERACION").Value;
                if (DS_FUNCAO == "PINTURA" && cond_lib == "DIA-PI")
                {

                }
                else if (DS_FUNCAO == "BAZAR" && cond_lib == "DIA-BA")
                {

                }
                else if (DS_FUNCAO == "FERRETERIA" && (cond_lib == "DIA-FE" || cond_lib == "DIA"))
                {

                }
                else
                {
                    string dsAnexo3 = row.GetCell("DS_ANEXO3").Value;
                    if (!string.IsNullOrEmpty(dsAnexo3))
                    {
                        row.CssClass = "yellow";
                    }
                    else
                    {
                        row.CssClass = "green";
                    }
                    if (cond_lib == "DIA-PI")
                    {
                        row.GetCell("AUX_OBSERVACION").Value = "Incluye Pintura";
                    }
                }

            });

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EXP150PedidoSinEmpaquetarQuery();

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);

        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new EXP150PedidoSinEmpaquetarQuery();
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

            if (context.ButtonId == "BtnAsignar")
            {
                string pedido = context.Row.GetCell("NU_PEDIDO").Value;
                string cliente = context.Row.GetCell("CD_CLIENTE").Value;
                int empresa = int.Parse(context.Row.GetCell("CD_EMPRESA").Value);
                Pedido ped = uow.PedidoRepository.GetPedido(empresa, cliente, pedido);
                if (!string.IsNullOrEmpty(ped.Anexo3))
                {
                    context.AddErrorNotification("EXP150_Sec0_Error_EsePedidoyaEstaTomado", new List<string> { this._identity.UserId.ToString() });
                }
                else
                {
                    string vDs_funcionario = uow.SecurityRepository.GetUserFullname(this._identity.UserId);
                    ped.Anexo3 = vDs_funcionario + " Id: " + this._identity.UserId;
                    uow.PedidoRepository.UpdatePedido(ped);
                    uow.SaveChanges();
                }

            }
            else
            {
                string pedido = context.Row.GetCell("NU_PEDIDO").Value;
                string cliente = context.Row.GetCell("CD_CLIENTE").Value;

                context.AddParameter("NU_PEDIDO", pedido);
                context.AddParameter("CD_CLIENTE", cliente);
                context.AddParameter("CD_EMPRESA", context.Row.GetCell("CD_EMPRESA").Value);

            }
            return context;
        }
    }
}