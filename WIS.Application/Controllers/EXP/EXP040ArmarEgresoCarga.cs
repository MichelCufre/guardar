using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Security;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Expedicion;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
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

namespace WIS.Application.Controllers.EXP
{
    public class EXP040ArmarEgresoCarga : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ISecurityService _security;
        protected readonly ITrafficOfficerService _concurrencyControl;
        protected readonly IGridService _gridService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IGridExcelService _excelService;
        protected readonly IIdentityService _identity;
        protected readonly IParameterService _parameterService;
        protected readonly IFactoryService _factoryService;
        protected readonly ITrackingService _trackingService;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP040ArmarEgresoCarga(
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IIdentityService identity,
            IParameterService parameterService,
            IFactoryService factoryService,
            ITrackingService trackingService)
        {
            this.GridKeys = new List<string>
            {
                "CD_CLIENTE", "CD_EMPRESA", "NU_PREPARACION", "NU_CARGA", "CD_GRUPO_EXPEDICION"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_CARGA",SortDirection.Descending)
            };

            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._identity = identity;
            this._parameterService = parameterService;
            this._factoryService = factoryService;
            _trackingService = trackingService;
        }

        public override PageContext PageLoad(PageContext data)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            var camionId = int.Parse(data.GetParameter("camion"));
            var camion = uow.CamionRepository.GetCamion(camionId);

            if (camion == null)
                throw new EntityNotFoundException("Camion no existe");

            this._concurrencyControl.AddLock("T_CAMION", camion.Id.ToString());

            return base.PageLoad(data);
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            if (this._security.IsUserAllowed(SecurityResources.WEXP040_grid1_btn_ArmarCamion))
            {
                if (grid.Id == "AgregarCarga_grid_1")
                    grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_Agregar"));
                else if (grid.Id == "QuitarCarga_grid_2")
                    grid.MenuItems.Add(new GridButton("btnQuitar", "General_Sec0_btn_Quitar"));
            }

            return base.GridInitialize(grid, context);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int camion = int.Parse(context.GetParameter("camion"));

            int? empresa = null;
            if (int.TryParse(context.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                empresa = idEmpresa;

            short? ruta = null;
            if (short.TryParse(context.Parameters.Find(x => x.Id == "ruta")?.Value, out short cdRuta))
                ruta = cdRuta;

            if (grid.Id == "AgregarCarga_grid_1")
            {
                var dbQuery = new ArmarCamionCargaQuery(camion, empresa, ruta);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            else if (grid.Id == "QuitarCarga_grid_2")
            {
                var dbQuery = new ArmarCamionCargaQuitarQuery(camion, empresa, ruta);
                uow.HandleQuery(dbQuery);
                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }

            return grid;
        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            int camion = int.Parse(context.GetParameter("camion"));

            int? empresa = null;
            if (int.TryParse(context.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                empresa = idEmpresa;

            short? ruta = null;
            if (short.TryParse(context.Parameters.Find(x => x.Id == "ruta")?.Value, out short cdRuta))
                ruta = cdRuta;

            if (grid.Id == "AgregarCarga_grid_1")
            {
                var dbQuery = new ArmarCamionCargaQuery(camion, empresa, ruta);
                uow.HandleQuery(dbQuery);
                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "QuitarCarga_grid_2")
            {
                var dbQuery = new ArmarCamionCargaQuitarQuery(camion, empresa, ruta);
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
            using var uow = this._uowFactory.GetUnitOfWork();

            int camion = int.Parse(context.GetParameter("camion"));

            int? empresa = null;
            if (int.TryParse(context.Parameters.Find(x => x.Id == "empresa")?.Value, out int idEmpresa))
                empresa = idEmpresa;

            short? ruta = null;
            if (short.TryParse(context.Parameters.Find(x => x.Id == "ruta")?.Value, out short cdRuta))
                ruta = cdRuta;

            if (grid.Id == "AgregarCarga_grid_1")
            {
                var dbQuery = new ArmarCamionCargaQuery(camion, empresa, ruta);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else if (grid.Id == "QuitarCarga_grid_2")
            {
                var dbQuery = new ArmarCamionCargaQuitarQuery(camion, empresa, ruta);
                uow.HandleQuery(dbQuery);
                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "AgregarCarga_grid_1" && context.ButtonId == "btnAgregar")
                this.ProcesarAgregar(context);
            else if (context.GridId == "QuitarCarga_grid_2" && context.ButtonId == "btnQuitar")
                this.ProcesarQuitar(context);

            return context;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            if (context.ButtonId == "btnCerrar")
                this._concurrencyControl.ClearToken();
            return base.FormButtonAction(form, context);
        }

        #region Metodos Auxiliares

        public virtual void CheckIfLocked(string camion)
        {
            if (this._concurrencyControl.IsLocked("T_CAMION", camion))
                throw new EntityLockedException("General_Sec0_Error_LockedEntity");
        }

        public virtual void ProcesarAgregar(GridMenuItemActionContext context)
        {
            var camionParam = context.GetParameter("camion");
            this.CheckIfLocked(camionParam);

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("ArmarEgresoCarga: Agregar");
            uow.BeginTransaction();

            try
            {
                var camion = uow.CamionRepository.GetCamionWithCargas(int.Parse(camionParam));
                var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
                var selection = context.Selection.GetSelection(this.GridKeys);
                var cargas = selection.Select(item => new CargaAsociarUnidad
                {
                    Carga = long.Parse(item["NU_CARGA"]),
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                    Preparacion = int.Parse(item["NU_PREPARACION"]),
                    Cliente = item["CD_CLIENTE"],
                    GrupoExpedicion = item["CD_GRUPO_EXPEDICION"]
                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new ArmarCamionCargaQuery(camion.Id, camion.Empresa, camion.Ruta);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    var cargasSeleccion = dbQuery.GetCargasUnidad();
                    cargas = cargasSeleccion.Except(cargas).ToList();
                }

                var armadoEgreso = new ArmadoEgresoCarga(uow, _factoryService, _identity, expedicionService, camion, cargas);
                armadoEgreso.Armar();

                _trackingService.CambiarEstadoSincronizacion(uow, camion, false);
                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }
        }

        public virtual void ProcesarQuitar(GridMenuItemActionContext context)
        {
            var camionParam = context.GetParameter("camion");
            this.CheckIfLocked(camionParam);

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("ArmarEgresoCarga: Quitar");
            uow.BeginTransaction();

            try
            {
                var camion = uow.CamionRepository.GetCamionWithCargas(int.Parse(camionParam));
                var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
                var selection = context.Selection.GetSelection(this.GridKeys);
                var cargas = selection.Select(item => new CargaAsociarUnidad
                {
                    Carga = long.Parse(item["NU_CARGA"]),
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                    Preparacion = int.Parse(item["NU_PREPARACION"]),
                    Cliente = item["CD_CLIENTE"],
                    GrupoExpedicion = item["CD_GRUPO_EXPEDICION"]
                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new ArmarCamionCargaQuitarQuery(camion.Id, camion.Empresa, camion.Ruta);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    var cargasSeleccion = dbQuery.GetCargasUnidad();
                    cargas = cargasSeleccion.Except(cargas).ToList();
                }

                var desarmadoEgreso = new DesarmadoEgresoCarga(uow, _factoryService, _identity, camion, expedicionService, cargas);
                desarmadoEgreso.Desarmar();

                _trackingService.CambiarEstadoSincronizacion(uow, camion, false);
                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                throw ex;
            }
        }

        #endregion
    }
}
