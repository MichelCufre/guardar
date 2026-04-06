using Newtonsoft.Json;
using NLog;
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
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.PRE
{
    public class PRE812ZoominModal : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }
        protected readonly Logger _logger;

        public PRE812ZoominModal(
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IIdentityService identity)
        {

            this.GridKeys = new List<string>
            {
                "CD_PRODUTO","CD_EMPRESA","NU_IDENTIFICADOR","CD_FAIXA","NU_PREPARACION","NU_PEDIDO","CD_CLIENTE","CD_ENDERECO","NU_SEQ_PREPARACION",
            };

            this.DefaultSort = new List<SortCommand>() {
                new SortCommand("NU_SEQ_PREPARACION",SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._identity = identity;

            this._logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!(uow.ManejoLpnRepository.AnyLpnActivo() || uow.ManejoLpnRepository.AnyLpnGeneradoConUbicacion()))
            {
                grid.MenuItems.Add(new GridButton("btnDesasociar", "PRE812_grid1_btn_Desasociar", string.Empty, new ConfirmMessage("¿Desasociar funcionarios de las lineas seleccionadas?")));
                grid.MenuItems.Add(new GridButton("btnAsociar", "PRE812_grid1_btn_Asociar"));
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            string strEmpresa = context.GetParameter("cdEmpresa");
            string strCliente = context.GetParameter("cdCliente");
            string strPedido = context.GetParameter("nuPedido");

            if (string.IsNullOrEmpty(strPedido) || string.IsNullOrEmpty(strCliente) || !int.TryParse(strEmpresa, out int cdEmpresa))
            {
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
            }

            var dbQuery = new SeguimientoZoominColaDeTrabajoQuery(cdEmpresa, strCliente, strPedido);

            uow.HandleQuery(dbQuery);

            List<SortCommand> defaultSort = new List<SortCommand>() {
                new SortCommand("NU_SEQ_PREPARACION",SortDirection.Descending)
            };

            List<string> listKey = new List<string>
            {
                    "CD_PRODUTO","CD_EMPRESA","NU_IDENTIFICADOR","CD_FAIXA","NU_PREPARACION","NU_PEDIDO","CD_CLIENTE","CD_ENDERECO","NU_SEQ_PREPARACION",
            };

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, defaultSort, listKey);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string strEmpresa = context.GetParameter("cdEmpresa");
            string strCliente = context.GetParameter("cdCliente");
            string strPedido = context.GetParameter("nuPedido");
            int cdEmpresa = -1;

            if (!string.IsNullOrEmpty(strPedido) && !string.IsNullOrEmpty(strCliente) && int.TryParse(strEmpresa, out cdEmpresa) && cdEmpresa >= 0)
            {
                var dbQuery = new SeguimientoZoominColaDeTrabajoQuery(cdEmpresa, strCliente, strPedido);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);
                dbQuery.ApplyFilter(this._filterInterpreter, context.ExplicitFilter);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            return null;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            string strEmpresa = context.GetParameter("cdEmpresa");
            string strCliente = context.GetParameter("cdCliente");
            string strPedido = context.GetParameter("nuPedido");

            if (string.IsNullOrEmpty(strPedido) || string.IsNullOrEmpty(strCliente) || !int.TryParse(strEmpresa, out int cdEmpresa))
            {
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");
            }

            var dbQuery = new SeguimientoZoominColaDeTrabajoQuery(cdEmpresa, strCliente, strPedido);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.GridId == "PRE812Zoo_grid_1")
            {
                try
                {
                    int cdEmpresa = int.Parse(context.GetParameter("cdEmpresa"));
                    string cdCliente = context.GetParameter("cdCliente");
                    string nuPedido = context.GetParameter("nuPedido");

                    SeguimientoZoominColaDeTrabajoQuery dbQuery = new SeguimientoZoominColaDeTrabajoQuery(cdEmpresa, cdCliente, nuPedido);
                    uow.HandleQuery(dbQuery);

                    List<string[]> keysRowSelected = dbQuery.GetKeysRowsSelected(uow, context, this._filterInterpreter);

                    if (uow.ManejoLpnRepository.AnyLpnActivo() || uow.ManejoLpnRepository.AnyLpnGeneradoConUbicacion())
                        throw new ValidationFailedException("PRE812_Grid_Error_ExisteLPNActivo");

                    if (context.ButtonId == "btnDesasociar")
                    {
                        uow.BeginTransaction();
                        uow.CreateTransactionNumber("PRE812 Desasignación de funcionarios");

                        if (keysRowSelected.Count > 0)
                        {
                            keysRowSelected.ForEach(linea =>
                            {
                                string producto = linea[0];
                                int empresa = int.Parse(linea[1]);
                                string identificador = linea[2];
                                decimal faixa = decimal.Parse(linea[3]);
                                int preparacion = int.Parse(linea[4]);
                                string pedido = linea[5];
                                string cliente = linea[6];
                                string ubicacion = linea[7];
                                int secPreparacion = int.Parse(linea[8]);

                                uow.ColaDeTrabajoRepository.DesasignarFuncionarios(producto, empresa, identificador, faixa, preparacion, pedido, cliente, ubicacion, secPreparacion);
                            });

                            uow.SaveChanges();
                            uow.Commit();
                            context.AddSuccessNotification("PRE812_Sec0_Msg_desasocioadosConExito");
                        }
                        else
                        {
                            context.AddErrorNotification("PRE812_Sec0_Msg_NoHayRegistrosSeleccioandos");
                        }
                    }

                    if (context.ButtonId == "btnAsociar")
                    {
                        if (keysRowSelected.Count > 0)
                        {
                            bool lineasSinAsignar = true;

                            keysRowSelected.ForEach(linea =>
                            {
                                string producto = linea[0];
                                int empresa = int.Parse(linea[1]);
                                string identificador = linea[2];
                                decimal faixa = decimal.Parse(linea[3]);
                                int preparacion = int.Parse(linea[4]);
                                string pedido = linea[5];
                                string cliente = linea[6];
                                string ubicacion = linea[7];
                                int secPreparacion = int.Parse(linea[8]);

                                if (uow.ColaDeTrabajoRepository.IsFuncionarioAsignado(producto, empresa, identificador, faixa, preparacion, pedido, cliente, ubicacion, secPreparacion))
                                    lineasSinAsignar = false;
                            });

                            if (lineasSinAsignar)
                                context.AddParameter("ListaFilasSeleccionadas", JsonConvert.SerializeObject(keysRowSelected));
                            else
                                context.AddErrorNotification("PRE812_Sec0_Msg_LineaFuncionarioAsignado");
                        }
                        else
                        {
                            context.AddErrorNotification("PRE812_Sec0_Msg_NoHayRegistrosSeleccioandos");
                        }
                    }
                }
                catch (ValidationFailedException ex)
                {
                    _logger.Error(ex, "MenuItemAction");
                    context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "MenuItemAction");
                    context.AddErrorNotification("PRE812_Sec0_Msg_ErrorAlAsociar");
                }
            }

            return context;
        }

    }
}
