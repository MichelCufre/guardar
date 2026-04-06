using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Application.Security;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Queries.Recepcion;
using WIS.Domain.Recepcion;
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
using WIS.TrafficOfficer;

namespace WIS.Application.Controllers.REC
{
    public class REC170AsociarAgendaFactura : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IFactoryService _factoryService;

        protected List<string> GridKeys { get; }
        protected List<string> GridKeysFactura { get; }
        protected List<SortCommand> DefaultSort { get; }

        public REC170AsociarAgendaFactura(
            IIdentityService identity,
            IFactoryService factoryService,
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService)
        {
            this.GridKeys = new List<string>
            {
                "NU_RECEPCION_FACTURA", "NU_AGENDA", "CD_CLIENTE", "CD_EMPRESA"
            };
            this.GridKeysFactura = new List<string>
            {
                "NU_RECEPCION_FACTURA", "CD_CLIENTE", "CD_EMPRESA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_RECEPCION_FACTURA",SortDirection.Descending)
            };

            this._identity = identity;
            this._factoryService = factoryService;
            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (this._security.IsUserAllowed(SecurityResources.WREC170_grid1_btn_AsociarFacturaAgenda))
            {
                if (grid.Id == "AgregarFactura_grid_1")
                {
                    grid.MenuItems.Add(new GridButton("btnAgregar", "Agregar seleccion"));
                }
                else if (grid.Id == "QuitarFactura_grid_2")
                {
                    grid.MenuItems.Add(new GridButton("btnQuitar", "Quitar seleccion"));
                }
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int idAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            Agenda agenda = uow.AgendaRepository.GetAgendaSinDetalles(idAgenda);

            if (grid.Id == "AgregarFactura_grid_1")
            {
                var dbQuery = new FacturaQuery(cdCliente: agenda.CodigoInternoCliente, predio: agenda.Predio);
                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeysFactura);
            }
            else if (grid.Id == "QuitarFactura_grid_2")
            {
                var dbQuery = new AsociarFacturaAgendaQuitarQuery(idAgenda);
                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_AGENDA", SortDirection.Descending);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }

            return grid;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "AgregarFactura_grid_1" && context.ButtonId == "btnAgregar")
            {
                this.ProcesarAgregar(context);
            }
            else if (context.GridId == "QuitarFactura_grid_2" && context.ButtonId == "btnQuitar")
            {
                this.ProcesarQuitar(context);
            }

            return context;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int idAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            Agenda agenda = uow.AgendaRepository.GetAgendaSinDetalles(idAgenda);

            if (grid.Id == "AgregarFactura_grid_1")
            {
                var dbQuery = new FacturaQuery(cdCliente: agenda.CodigoInternoCliente, predio: agenda.Predio);
                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_RECEPCION_FACTURA", SortDirection.Descending);

                context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
            else if (grid.Id == "QuitarFactura_grid_2")
            {
                var dbQuery = new AsociarFacturaAgendaQuitarQuery(idAgenda);
                uow.HandleQuery(dbQuery);

                var defaultSort = new SortCommand("NU_RECEPCION_FACTURA", SortDirection.Descending);
                context.FileName = this._identity.Application + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, defaultSort);
            }
            return null;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int idAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            Agenda agenda = uow.AgendaRepository.GetAgendaSinDetalles(idAgenda);

            if (grid.Id == "AgregarFactura_grid_1")
            {
                var dbQuery = new FacturaQuery(cdCliente: agenda.CodigoInternoCliente, predio: agenda.Predio);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "QuitarAgenda_grid_2")
            {
                var dbQuery = new AsociarFacturaAgendaQuitarQuery(idAgenda);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }

            return null;
        }

        #region Metodos Auxiliares

        public virtual void ProcesarAgregar(GridMenuItemActionContext context)
        {
            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int idAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("AsociarAgendaFactura: Agregar");
            uow.BeginTransaction();

            try
            {
                Agenda agenda = uow.AgendaRepository.GetAgendaSinDetalles(idAgenda);

                if (agenda == null)
                    throw new EntityNotFoundException("Agenda no existe");
                if (uow.AgendaRepository.IsAgendaFacturaValida(idAgenda))
                    throw new Exception("Agenda ya validada");

                this._concurrencyControl.AddLock("NU_AGENDA", idAgenda.ToString());

                List<Dictionary<string, string>> selection = context.Selection.GetSelection(this.GridKeysFactura);
                List<AsociarAgenda> facturas = selection.Select(item => new AsociarAgenda
                {
                    IdFactura = int.Parse(item["NU_RECEPCION_FACTURA"]),
                    Cliente = item["CD_CLIENTE"],
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new FacturaQuery(cdCliente: agenda.CodigoInternoCliente, predio: agenda.Predio);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    var facturasSeleccion = dbQuery.GetFacturasUnidad();
                    facturas = facturasSeleccion.Except(facturas).ToList();
                }

                ArmadoAsociarAgendaFactura armado = new ArmadoAsociarAgendaFactura(uow, _factoryService, _identity, idAgenda, facturas);
                armado.Armar();

                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        public virtual void ProcesarQuitar(GridMenuItemActionContext context)
        {
            if (!int.TryParse(context.Parameters.FirstOrDefault(s => s.Id == "keyAgenda")?.Value, out int idAgenda))
                throw new MissingParameterException("General_Sec0_Error_ParametrosURI");

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("AsociarFacturaAgenda: Quitar");
            uow.BeginTransaction();

            try
            {
                Agenda agenda = uow.AgendaRepository.GetAgendaSinDetalles(idAgenda);

                if (agenda == null)
                    throw new EntityNotFoundException("Agenda no existe");
                if (uow.AgendaRepository.IsAgendaFacturaValida(idAgenda))
                    throw new Exception("Agenda ya validada");

                this._concurrencyControl.AddLock("NU_AGENDA", idAgenda.ToString());

                List<Dictionary<string, string>> selection = context.Selection.GetSelection(this.GridKeys);
                List<AsociarAgenda> facturas = selection.Select(item => new AsociarAgenda
                {
                    IdFactura = int.Parse(item["NU_RECEPCION_FACTURA"]),
                    Cliente = item["CD_CLIENTE"],
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new AsociarFacturaAgendaQuitarQuery(agenda.Id);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    var facturasSeleccion = dbQuery.GetFacturasUnidad();
                    facturas = facturasSeleccion.Except(facturas).ToList();
                }

                DesarmadoAsociacion desarmadoAsociado = new DesarmadoAsociacion(_uowFactory, _factoryService, _identity, facturas);
                desarmadoAsociado.Desarmar();

                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw;
            }
        }

        #endregion
    }
}
