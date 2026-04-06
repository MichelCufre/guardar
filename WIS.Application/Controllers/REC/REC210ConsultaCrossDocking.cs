using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
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

namespace WIS.Application.Controllers.REC
{
    public class REC210ConsultaCrossDocking : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;

        protected List<string> GridKeysCrossDock { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC210ConsultaCrossDocking(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService, 
            IFilterInterpreter filterInterpreter)
        {
            this.GridKeysCrossDock = new List<string>
            {
                "NU_PEDIDO",
                "CD_EMPRESA",
                "CD_CLIENTE",
                "NU_AGENDA",
                "NU_PREPARACION",
                "CD_PRODUTO",
                "CD_FAIXA",
                "NU_IDENTIFICADOR",
                "ID_ESPECIFICA_IDENTIFICADOR",
                "NU_CARGA",
                "NU_PREPARACION_PICKEO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Descending)
            };

            this._uowFactory = uowFactory;
            this._identity = identity;
            this._gridService = gridService;
            this._excelService = excelService;
            _filterInterpreter = filterInterpreter;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            List<GridButton> items = new List<GridButton>
            {
                new GridButton("btnDetallePreparacion", "REC210_Sec0_btn_DetallePreparacion", "fas fa-list")
            };

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_LIST", items));

            return this.GridFetchRows(grid, context.FetchContext);
        }
        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();
            CrossDockingDetailQuery dbQuery = null;

            if (context.Parameters.Count == 0)
            {
                dbQuery = new CrossDockingDetailQuery();
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysCrossDock);
            }
            else
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int nroAgenda))
                    throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

                Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

                dbQuery = new CrossDockingDetailQuery(agenda.IdEmpresa, agenda.Id);
                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysCrossDock);

                string empresaDesc = uow.EmpresaRepository.GetNombre(agenda.IdEmpresa);
                ICrossDocking crossDock = uow.CrossDockingRepository.GetCrossDockingByAgenda(agenda.Id);

                context.AddParameter("REC210_NU_AGENDA", nroAgenda.ToString());
                context.AddParameter("REC210_CD_EMPRESA", agenda.IdEmpresa.ToString());
                context.AddParameter("REC210_NM_EMPRESA", empresaDesc);
                context.AddParameter("REC210_NU_PREPARACION", crossDock.Preparacion.ToString());                
            }



            return grid;
        }
        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(query.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int nroAgenda))
                nroAgenda = -1;

            CrossDockingDetailQuery dbQuery = null;

            if (nroAgenda == -1)
            {
                dbQuery = new CrossDockingDetailQuery();
            }
            else
            {
                Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

                dbQuery = new CrossDockingDetailQuery(agenda.IdEmpresa, agenda.Id);
            }

            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }
        public override GridButtonActionContext GridButtonAction(GridButtonActionContext data)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (data.ButtonId == "btnDetallePreparacion")
            {
                data.Redirect("/preparacion/PRE130", new List<ComponentParameter>()
                {
                    new ComponentParameter("preparacion", data.Row.GetCell("NU_PREPARACION").Value),
                    new ComponentParameter("carga",data.Row.GetCell("NU_CARGA").Value),
                    new ComponentParameter("pedido", data.Row.GetCell("NU_PEDIDO").Value),
                    new ComponentParameter("empresa", data.Row.GetCell("CD_EMPRESA").Value),
                    new ComponentParameter("cliente", data.Row.GetCell("CD_CLIENTE").Value),
                    new ComponentParameter("producto",data.Row.GetCell("CD_PRODUTO").Value),
                    new ComponentParameter("faixa", data.Row.GetCell("CD_FAIXA").Value),
                    new ComponentParameter("identificador", data.Row.GetCell("NU_IDENTIFICADOR").Value),
                });
            }
            return data;
        }
        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            //Emi: Cambio de recepcion de parametros a por URL  
            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "agenda")?.Value, out int nroAgenda))
                nroAgenda = -1;
            
            CrossDockingDetailQuery dbQuery = null;

            if (nroAgenda == -1)
            {
                dbQuery = new CrossDockingDetailQuery();
            }
            else
            {
                Agenda agenda = uow.AgendaRepository.GetAgenda(nroAgenda);

                dbQuery = new CrossDockingDetailQuery(agenda.IdEmpresa, agenda.Id);
            }

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application +"-"+ DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
        }
    }
}
