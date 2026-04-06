using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.DataModel.Queries.Preparacion;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Expedicion;
using WIS.Domain.Recepcion;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.PageComponent.Execution;
using WIS.Security;
using WIS.Sorting;
using WIS.TrafficOfficer;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WIS.Application.Controllers.REC
{
    public class REC170SeleccionLpn : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly ITrafficOfficerService _concurrencyControl;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC170SeleccionLpn(
            IUnitOfWorkFactory uowFactory,
            IIdentityService identity,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ITrafficOfficerService concurrencyControl)
        {
            this.GridKeys = new List<string>
            {
                "NU_LPN"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW",SortDirection.Descending)
            };

            _uowFactory = uowFactory;
            _concurrencyControl = concurrencyControl;
            _gridService = gridService;
            _filterInterpreter = filterInterpreter;
            _excelService = excelService;
            _identity = identity;
        }

        public override PageContext PageLoad(PageContext data)
        {
            return base.PageLoad(data);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (grid.Id == "REC170SeleccionLpn_grid_1")
                grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_AgregarSeleccion"));
            else if (grid.Id == "REC170SeleccionLpn_grid_2")
                grid.MenuItems.Add(new GridButton("btnQuitar", "General_Sec0_btn_QuitarSeleccion"));

            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.Find(x => x.Id == "keyAgenda")?.Value, out int nuAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var agenda = uow.AgendaRepository.GetAgenda(nuAgenda);

            if (grid.Id == "REC170SeleccionLpn_grid_1")
            {
                var dbQuery = new SeleccionLpnQuery(agenda.IdEmpresa);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            else if (grid.Id == "REC170SeleccionLpn_grid_2")
            {
                var dbQuery = new SeleccionLpnQuery(agenda.IdEmpresa, agenda.Id);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext query)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(query.Parameters.Find(x => x.Id == "keyAgenda")?.Value, out int nuAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var agenda = uow.AgendaRepository.GetAgenda(nuAgenda);

            if (grid.Id == "REC170SeleccionLpn_grid_1")
            {
                var dbQuery = new SeleccionLpnQuery(agenda.IdEmpresa);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "REC170SeleccionLpn_grid_2")
            {
                var dbQuery = new SeleccionLpnQuery(agenda.IdEmpresa, agenda.Id);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, query.Filters);

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

            if (!int.TryParse(context.Parameters.Find(x => x.Id == "keyAgenda")?.Value, out int nuAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            var agenda = uow.AgendaRepository.GetAgenda(nuAgenda);

            if (grid.Id == "REC170SeleccionLpn_grid_1")
            {
                var dbQuery = new SeleccionLpnQuery(agenda.IdEmpresa);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else if (grid.Id == "REC170SeleccionLpn_grid_2")
            {
                var dbQuery = new SeleccionLpnQuery(agenda.IdEmpresa, agenda.Id);

                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "REC170SeleccionLpn_grid_1" && context.ButtonId == "btnAgregar")
                this.ProcesarAgregar(context);
            else if (context.GridId == "REC170SeleccionLpn_grid_2" && context.ButtonId == "btnQuitar")
                this.ProcesarQuitar(context);

            return context;
        }

        public virtual void ProcesarAgregar(GridMenuItemActionContext context)
        {
            var nuAgenda = int.Parse(context.GetParameter("keyAgenda"));

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("REC170SeleccionLpn: Agregar");

            var datos = GetKeys(uow, context, nuAgenda);
            uow.AgendaRepository.GenerarDetalleAgendaLpn(nuAgenda, datos.Lpns, datos.Planificaciones, uow.GetTransactionNumber());
        }

        public virtual void ProcesarQuitar(GridMenuItemActionContext context)
        {
            var nuAgenda = int.Parse(context.GetParameter("keyAgenda"));

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("REC170SeleccionLpn: Quitar");

            var datos = GetKeys(uow, context, nuAgenda, agregar: false);
            uow.AgendaRepository.GenerarDetalleAgendaLpn(nuAgenda, datos.Lpns, datos.Planificaciones, uow.GetTransactionNumber(), desasociar: true);
        }

        public virtual (List<Lpn> Lpns, List<AgendaLpnPlanificacion> Planificaciones) GetKeys(IUnitOfWork uow, GridMenuItemActionContext context, int nuAgenda, bool agregar = true)
        {
            var lpns = new List<Lpn>();
            var planificaciones = new List<AgendaLpnPlanificacion>();

            var nuLpns = new List<long>();

            var agenda = uow.AgendaRepository.GetAgenda(nuAgenda);

            var dbQuery = new SeleccionLpnQuery(agenda.IdEmpresa, (agregar ? null : agenda.Id));
            uow.HandleQuery(dbQuery);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                nuLpns = dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);
            else
                nuLpns = dbQuery.GetSelectedKeys(context.Selection.Keys);

            if (agregar)
                nuLpns.AddRange(uow.ManejoLpnRepository.GetNumeroLpnByAgenda(nuAgenda));
            else
            {
                nuLpns = uow.ManejoLpnRepository.GetNumeroLpnByAgenda(nuAgenda)
                    .Select(r => r)
                    .Except(nuLpns)
                    .Select(w => w)
                    .ToList();
            }

            foreach (var nuLpn in nuLpns)
            {
                lpns.Add(new Lpn()
                {
                    NumeroLPN = nuLpn,
                    NroAgenda = nuAgenda,
                    FechaModificacion = DateTime.Now,
                    NumeroTransaccion = uow.GetTransactionNumber(),
                });

                planificaciones.Add(new AgendaLpnPlanificacion()
                {
                    NroLPN = nuLpn,
                    NroAgenda = nuAgenda,
                    Planificado = "S",
                    Recibido = "N",
                    Funcionario = _identity.UserId,
                    FuncionarioRecepcion = null,
                    FechaRecepcion = null,
                    FechaInsercion = DateTime.Now,
                    FechaModificacion = DateTime.Now,
                    NumeroTransaccion = uow.GetTransactionNumber(),
                });
            }

            return (lpns, planificaciones);
        }
    }
}
