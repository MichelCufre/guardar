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
    public class EXP040ArmarEgresoPedido : AppController
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

        public EXP040ArmarEgresoPedido(
            ITrafficOfficerService concurrencyControl,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            ISecurityService security,
            IFilterInterpreter filterInterpreter,
            IGridExcelService excelService,
            IIdentityService identity,
            IFactoryService factoryService,
            IParameterService parameterService,
            ITrackingService trackingService)
        {
            this.GridKeys = new List<string>
            {
                "CD_CLIENTE", "CD_EMPRESA", "NU_PEDIDO"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("DT_ADDROW",SortDirection.Descending)
            };

            this._concurrencyControl = concurrencyControl;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._security = security;
            this._filterInterpreter = filterInterpreter;
            this._excelService = excelService;
            this._identity = identity;
            this._factoryService = factoryService;
            this._parameterService = parameterService;
            this._trackingService = trackingService;
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
            if (this._security.IsUserAllowed(SecurityResources.WEXP013_Page_Access_ArmarEgresoPedido))
            {
                if (grid.Id == "AgregarPedido_grid_1")
                {
                    grid.MenuItems.Add(new GridButton("btnAgregar", "General_Sec0_btn_Agregar"));
                }
                else if (grid.Id == "QuitarPedido_grid_2")
                {
                    grid.MenuItems.Add(new GridButton("btnQuitar", "General_Sec0_btn_Quitar"));
                }
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

            if (grid.Id == "AgregarPedido_grid_1")
            {
                var dbQuery = new ArmarCamionPedidoQuery(camion, empresa, ruta);

                uow.HandleQuery(dbQuery);

                grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);
            }
            else if (grid.Id == "QuitarPedido_grid_2")
            {
                var dbQuery = new ArmarCamionPedidoQuitarQuery(camion, empresa, ruta);

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

            if (grid.Id == "AgregarPedido_grid_1")
            {
                var dbQuery = new ArmarCamionPedidoQuery(camion, empresa, ruta);

                uow.HandleQuery(dbQuery);

                dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                return new GridStats
                {
                    Count = dbQuery.GetCount()
                };
            }
            else if (grid.Id == "QuitarPedido_grid_2")
            {
                var dbQuery = new ArmarCamionPedidoQuitarQuery(camion, empresa, ruta);

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

            if (grid.Id == "AgregarPedido_grid_1")
            {
                var dbQuery = new ArmarCamionPedidoQuery(camion, empresa, ruta);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            else if (grid.Id == "QuitarPedido_grid_2")
            {
                var dbQuery = new ArmarCamionPedidoQuitarQuery(camion, empresa, ruta);
                uow.HandleQuery(dbQuery);

                context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";
                return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);
            }
            return null;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (context.GridId == "AgregarPedido_grid_1" && context.ButtonId == "btnAgregar")
                this.ProcesarAgregar(context);
            else if (context.GridId == "QuitarPedido_grid_2" && context.ButtonId == "btnQuitar")
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

            uow.CreateTransactionNumber("ArmarEgresoPedido: Agregar");
            uow.BeginTransaction();

            try
            {
                var camion = uow.CamionRepository.GetCamionWithCargas(int.Parse(camionParam));
                var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
                var selection = context.Selection.GetSelection(this.GridKeys);
                var pedidos = selection.Select(item => new PedidoAsociarUnidad
                {
                    Pedido = item["NU_PEDIDO"],
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                    Cliente = item["CD_CLIENTE"]
                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new ArmarCamionPedidoQuery(camion.Id, camion.Empresa, camion.Ruta);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    var pedidosSeleccion = dbQuery.GetPedidosUnidad();
                    pedidos = pedidosSeleccion.Except(pedidos).ToList();
                }

                var armadoEgreso = new ArmadoEgresoPedido(uow, _factoryService, _identity, expedicionService, camion, pedidos);
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
            string camionParam = context.GetParameter("camion");
            this.CheckIfLocked(camionParam);

            using var uow = this._uowFactory.GetUnitOfWork();
            uow.CreateTransactionNumber("ArmarEgresoPedido: Quitar");
            uow.BeginTransaction();

            try
            {
                var camion = uow.CamionRepository.GetCamionWithCargas(int.Parse(camionParam));
                var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());
                var selection = context.Selection.GetSelection(this.GridKeys);
                var pedidos = selection.Select(item => new PedidoAsociarUnidad
                {
                    Pedido = item["NU_PEDIDO"],
                    Empresa = int.Parse(item["CD_EMPRESA"]),
                    Cliente = item["CD_CLIENTE"]
                }).ToList();

                if (context.Selection.AllSelected)
                {
                    var dbQuery = new ArmarCamionPedidoQuitarQuery(camion.Id, camion.Empresa, camion.Ruta);
                    uow.HandleQuery(dbQuery);
                    dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

                    var pedidosSeleccion = dbQuery.GetPedidosUnidad();
                    pedidos = pedidosSeleccion.Except(pedidos).ToList();
                }

                var desarmadoEgreso = new DesarmadoEgresoPedido(uow, _factoryService, _identity, camion, expedicionService, pedidos);
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
