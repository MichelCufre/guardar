using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Stock;
using WIS.Domain.Logic;
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

namespace WIS.Application.Controllers.STO
{
    public class STO710ConsultaDeLPNPorAtributos : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ISecurityService _security;

        protected List<string> GridCabezalesKeys { get; }
        protected List<SortCommand> CabezalesSorts { get; }

        protected List<string> GridLogsKeys { get; }
        protected List<SortCommand> LogsSorts { get; }

        public STO710ConsultaDeLPNPorAtributos(
            IGridExcelService excelService, 
            IUnitOfWorkFactory uowFactory, 
            IGridService gridService, 
            IIdentityService identity, 
            IFilterInterpreter filterInterpreter, 
            ISecurityService security)
        {
            this.GridCabezalesKeys = new List<string>
            {
                "NU_LPN","ID_LPN_DET", "TP_LPN_TIPO","ID_ATRIBUTO","CD_PRODUTO", "CD_FAIXA", "CD_EMPRESA", "NU_IDENTIFICADOR"
            };

            this.CabezalesSorts = new List<SortCommand>
            {
                new SortCommand("NU_LPN", SortDirection.Descending),
            };

            this.GridLogsKeys = new List<string>
            {
                "NU_LOG_SECUENCIA"
            };

            this.LogsSorts = new List<SortCommand>
            {
                new SortCommand("DT_LOG_ADD_ROW", SortDirection.Descending),
            };

            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._identity = identity;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "STO710_grid_1")
            {
                grid.AddOrUpdateColumn(new GridColumnButton("BTN_ACTIONS", new List<GridButton>
                {
                    new GridButton("btnEditar", "General_Sec0_btn_Editar", "far fa-edit")
                }));
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!string.IsNullOrEmpty(context.GetParameter("numeroLPN")))
            {
                var numeroLpn = context.GetParameter("numeroLPN");
                var detalle = context.GetParameter("detalle");
                var idDetalle = context.GetParameter("idDetalle");
                var lpn = uow.ManejoLpnRepository.GetLpn(long.Parse(numeroLpn));

                context.AddParameter("STO710_NU_LPN", numeroLpn);
                context.AddParameter("STO710_TP_LPN_TIPO", lpn.Tipo);
                context.AddParameter("STO710_ID_LPN_EXTERNO", lpn.IdExterno);

                if (detalle == "true")
                {
                    var detalleLpn = uow.ManejoLpnRepository.GetDetalleLpnByIdDetalle(lpn.NumeroLPN, int.Parse(idDetalle));

                    context.AddParameter("STO710_ID_LPN_DET", idDetalle);
                    context.AddParameter("STO710_ID_LINEA_SISTEMA_EXTERNO", detalleLpn.IdLineaSistemaExterno);
                }

                switch (grid.Id)
                {
                    case "STO710_grid_1":

                        var dbQuery = new ConsultaDeLPNPorAtributoQuery(long.Parse(numeroLpn), detalle, idDetalle);
                        uow.HandleQuery(dbQuery);
                        grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.CabezalesSorts, this.GridCabezalesKeys);
                        break;

                    case "STO710_grid_logsDetalle":

                        var dbQueryDetalles = new ConsultaLogsDetallesAtributosQuery(long.Parse(numeroLpn), idDetalle);
                        uow.HandleQuery(dbQueryDetalles);
                        grid.Rows = _gridService.GetRows(dbQueryDetalles, grid.Columns, context, this.LogsSorts, this.GridLogsKeys);
                        break;

                    case "STO710_grid_logsCabezal":

                        var dbQueryCabezal = new ConsultaLogsCabezalesAtributosQuery(long.Parse(numeroLpn));
                        uow.HandleQuery(dbQueryCabezal);
                        grid.Rows = _gridService.GetRows(dbQueryCabezal, grid.Columns, context, this.LogsSorts, this.GridLogsKeys);
                        break;

                }
            }
            else
            {
                if (grid.Id == "STO710_grid_1")
                {
                    var dbQuery = new ConsultaDeLPNPorAtributoQuery();
                    uow.HandleQuery(dbQuery);
                    grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.CabezalesSorts, this.GridCabezalesKeys);
                }
            }

            ComprobarPermisosEnBotones(uow, grid);

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!string.IsNullOrEmpty(context.GetParameter("numeroLPN")))
            {
                var numeroLpn = context.GetParameter("numeroLPN");
                var detalle = context.GetParameter("detalle");
                var idDetalle = context.GetParameter("idDetalle");

                switch (grid.Id)
                {
                    case "STO710_grid_1":

                        var dbQuery = new ConsultaDeLPNPorAtributoQuery(long.Parse(numeroLpn), detalle, idDetalle);
                        uow.HandleQuery(dbQuery);
                        dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                        return new GridStats
                        {
                            Count = dbQuery.GetCount()
                        };

                    case "STO710_grid_logsDetalle":

                        var dbQueryDetalles = new ConsultaLogsDetallesAtributosQuery(long.Parse(numeroLpn), idDetalle);
                        uow.HandleQuery(dbQueryDetalles);
                        dbQueryDetalles.ApplyFilter(this._filterInterpreter, context.Filters);

                        return new GridStats
                        {
                            Count = dbQueryDetalles.GetCount()
                        };

                    case "STO710_grid_logsCabezal":

                        var dbQueryCabezal = new ConsultaLogsCabezalesAtributosQuery(long.Parse(numeroLpn));
                        uow.HandleQuery(dbQueryCabezal);
                        dbQueryCabezal.ApplyFilter(this._filterInterpreter, context.Filters);

                        return new GridStats
                        {
                            Count = dbQueryCabezal.GetCount()
                        };
                }
            }
            else
            {
                var dbQuery = new ConsultaDeLPNPorAtributoQuery();
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!string.IsNullOrEmpty(context.GetParameter("numeroLPN")))
            {
                var numeroLpn = context.GetParameter("numeroLPN");
                var detalle = context.GetParameter("detalle");
                var idDetalle = context.GetParameter("idDetalle");

                switch (grid.Id)
                {
                    case "STO710_grid_1":

                        var dbQuery = new ConsultaDeLPNPorAtributoQuery(long.Parse(numeroLpn), detalle, idDetalle);
                        uow.HandleQuery(dbQuery);
                        return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.CabezalesSorts);

                    case "STO710_grid_logsDetalle":

                        var dbQueryDetalles = new ConsultaLogsDetallesAtributosQuery(long.Parse(numeroLpn), idDetalle);
                        uow.HandleQuery(dbQueryDetalles);
                        return this._excelService.GetExcel(context.FileName, dbQueryDetalles, grid.Columns, context, this.LogsSorts);

                    case "STO710_grid_logsCabezal":

                        var dbQueryCabezal = new ConsultaLogsCabezalesAtributosQuery(long.Parse(numeroLpn));
                        uow.HandleQuery(dbQueryCabezal);
                        return this._excelService.GetExcel(context.FileName, dbQueryCabezal, grid.Columns, context, this.LogsSorts);

                }
            }
            else
            {
                if (grid.Id == "STO710_grid_1")
                {
                    var dbQuery = new ConsultaDeLPNPorAtributoQuery();
                    uow.HandleQuery(dbQuery);
                    return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.CabezalesSorts);

                }
            }

            return null;
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            try
            {
                switch (context.ButtonId)
                {
                    case "btnEditar":

                        if (LManejoLpn.ValidarEdicionAtributo(uow, context.Row, _identity))
                            throw new ValidationFailedException("STO710_grid1_Error_AtributoConReserva");

                        context.Parameters.Add(new ComponentParameter { Id = "NU_LPN", Value = context.Row.GetCell("NU_LPN").Value });
                        context.Parameters.Add(new ComponentParameter { Id = "CD_EMPRESA", Value = context.Row.GetCell("CD_EMPRESA").Value });
                        context.Parameters.Add(new ComponentParameter { Id = "TP_LPN_TIPO", Value = context.Row.GetCell("TP_LPN_TIPO").Value });
                        context.Parameters.Add(new ComponentParameter { Id = "ID_ATRIBUTO", Value = context.Row.GetCell("ID_ATRIBUTO").Value });
                        context.Parameters.Add(new ComponentParameter { Id = "VL_LPN_ATRIBUTO", Value = context.Row.GetCell("VL_LPN_ATRIBUTO").Value });

                        if (string.IsNullOrEmpty(context.Row.GetCell("TP_ATRIBUTO_ASOCIADO").Value) || context.Row.GetCell("TP_ATRIBUTO_ASOCIADO").Value == "C")
                        {
                            context.Parameters.Add(new ComponentParameter { Id = "CD_PRODUTO", Value = "" });
                        }
                        else
                        {
                            context.Parameters.Add(new ComponentParameter { Id = "CD_FAIXA", Value = context.Row.GetCell("CD_FAIXA").Value });
                            context.Parameters.Add(new ComponentParameter { Id = "CD_PRODUTO", Value = context.Row.GetCell("CD_PRODUTO").Value });
                            context.Parameters.Add(new ComponentParameter { Id = "ID_LPN_DET", Value = context.Row.GetCell("ID_LPN_DET").Value });
                            context.Parameters.Add(new ComponentParameter { Id = "NU_IDENTIFICADOR", Value = context.Row.GetCell("NU_IDENTIFICADOR").Value });
                        }

                        break;
                }
            }
            catch (ValidationFailedException ex)
            {
                context.Parameters.Add(new ComponentParameter { Id = "error", Value = "true" });
                context.AddErrorNotification(ex.Message, ex.StrArguments?.ToList());
            }
            catch (Exception ex)
            {
                context.Parameters.Add(new ComponentParameter { Id = "error", Value = "true" });
                context.AddErrorNotification(ex.Message);
            }

            return context;
        }

        public virtual void ComprobarPermisosEnBotones(IUnitOfWork uow, Grid grid)
        {
            if (grid.Id == "STO710_grid_1")
            {
                var result = this._security.CheckPermissions(new List<string>
                {
                    SecurityResources.STO710_Page_btn_EditAtributo,
                });

                foreach (var row in grid.Rows)
                {
                    var atributo = uow.AtributoRepository.GetAtributo(int.Parse(row.GetCell("ID_ATRIBUTO").Value));
                    var estadoLpn = row.GetCell("ID_ESTADO_LPN").Value;

                    if (!result[SecurityResources.STO710_Page_btn_EditAtributo] || (atributo != null && atributo.IdTipo == TipoAtributoDb.SISTEMA) || estadoLpn != EstadosLPN.Activo)
                        row.DisabledButtons.Add("btnEditar");

                }
            }
        }
    }
}
