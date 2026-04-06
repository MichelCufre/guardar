using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Application.Controllers.INV;
using WIS.Application.Security;
using WIS.Components.Common;
using WIS.Domain.DataModel;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.DataModel.Queries.Expedicion;
using WIS.Domain.Documento;
using WIS.Domain.Documento.Integracion.Egreso;
using WIS.Domain.Expedicion;
using WIS.Domain.Picking;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Filtering;
using WIS.FormComponent;
using WIS.FormComponent.Execution.Configuration;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Build.Configuration;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.GridComponent.Excel.Configuration;
using WIS.GridComponent.Execution.Configuration;
using WIS.GridComponent.Items;
using WIS.Persistence.Database;
using WIS.Security;
using WIS.Sorting;

namespace WIS.Application.Controllers.EXP
{
    public class EXP050PedidosInvolucrados : AppController
    {
        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly IIdentityService _identity;
        protected readonly ISecurityService _security;
        protected readonly IGridService _gridService;
        protected readonly IGridExcelService _excelService;
        protected readonly IFilterInterpreter _filterInterpreter;
        protected readonly IParameterService _parameterService;
        protected readonly IFactoryService _factoryService;
        protected readonly ITrackingService _trackingService;
        protected readonly ILogger<EXP050PedidosInvolucrados> _logger;

        protected List<string> GridKeys { get; }
        protected List<SortCommand> DefaultSort { get; }

        public EXP050PedidosInvolucrados(
            IIdentityService identity,
            IUnitOfWorkFactory uowFactory,
            IGridService gridService,
            IGridExcelService excelService,
            IFilterInterpreter filterInterpreter,
            ISecurityService security,
            IParameterService parameterService,
            IFactoryService factoryService,
            ITrackingService trackingService,
            ILogger<EXP050PedidosInvolucrados> logger)
        {
            this.GridKeys = new List<string>
            {
                "CD_CAMION","NU_PEDIDO","CD_CLIENTE","CD_EMPRESA"
            };

            this.DefaultSort = new List<SortCommand>
            {
                new SortCommand("NU_PEDIDO", SortDirection.Ascending),
                new SortCommand("CD_CLIENTE", SortDirection.Ascending),
                new SortCommand("CD_EMPRESA", SortDirection.Ascending),
            };

            this._identity = identity;
            this._uowFactory = uowFactory;
            this._gridService = gridService;
            this._excelService = excelService;
            this._filterInterpreter = filterInterpreter;
            this._security = security;
            this._parameterService = parameterService;
            this._factoryService = factoryService;
            this._trackingService = trackingService;
            this._logger = logger;
        }

        public override Grid GridInitialize(Grid grid, GridInitializeContext context)
        {
            context.AddLink("CD_CLIENTE", "registro/REG220", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_CLIENTE", "cliente"), new GridColumnLinkMapping("CD_EMPRESA", "empresa") });
            context.AddLink("CD_EMPRESA", "registro/REG100", new List<GridColumnLinkMapping> { new GridColumnLinkMapping("CD_EMPRESA", "empresa") });

            grid.AddOrUpdateColumn(new GridColumnButton("BTN_ARRAY", new List<GridButton>
            {
                new GridButton("btnAlerta", "EXP050_Sec0_btn_Alerta", "fas fa-exclamation-triangle"),
            }));

            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "camion")?.Value, out int idCamion))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            using var uow = this._uowFactory.GetUnitOfWork();

            var camion = uow.CamionRepository.GetCamion(idCamion);

            if (IsCamionEditable(uow, camion, out IDocumentoEgreso documentoEgreso))
            {
                grid.MenuItems = new List<IGridItem>
                {
                    new GridButton("btnDesAsociar", "EXP050_frm1_btn_btnDesAsociar"),
                };
            }

            return this.GridFetchRows(grid, context.FetchContext);
        }

        public override Grid GridFetchRows(Grid grid, GridFetchContext context)
        {
            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "camion")?.Value, out int idCamion))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PedidosCamionQuery(idCamion);

            uow.HandleQuery(dbQuery);

            grid.Rows = _gridService.GetRows(dbQuery, grid.Columns, context, this.DefaultSort, this.GridKeys);

            context.AddParameter("camion", idCamion.ToString());

            var permiso = this._security.IsUserAllowed(SecurityResources.WEXP050_grid1_btn_ProductosEnPedido);

            foreach (var row in grid.Rows)
            {
                if (!permiso || row.GetCell("FL_PROBLEMA").Value != "S")
                    row.DisabledButtons.Add("btnAlerta");
            }

            var camionDescripcion = uow.CamionRepository.GetCamionDescripcion(idCamion);

            var respetaOrden = context.Parameters.FirstOrDefault(x => x.Id == "respetaOrden")?.Value;

            if (camionDescripcion != null)
            {
                context.AddParameter("EXP050_RESPETA_ORDEN", respetaOrden);
                context.AddParameter("EXP050_CD_CAMION", idCamion.ToString());
                context.AddParameter("EXP050_PLACA", camionDescripcion.Matricula);
                context.AddParameter("EXP050_SITUACION", $"{(short)camionDescripcion.Estado} - {camionDescripcion.DescSituacion}");
                context.AddParameter("EXP050_DT_INGRESO", camionDescripcion.FechaCreacion?.ToString(CDateFormats.DATE_ONLY));

                context.AddParameter("EXP050_RUTA", camionDescripcion.Ruta == null ? string.Empty : $"{camionDescripcion.Ruta} - {camionDescripcion.DescRuta}");
                context.AddParameter("EXP050_PUERTA", camionDescripcion.Puerta == null ? string.Empty : $"{camionDescripcion.Puerta} - {camionDescripcion.DescPuerta}");
                context.AddParameter("EXP050_EMPRESA", camionDescripcion.Empresa == null ? string.Empty : $"{camionDescripcion.Empresa} - {camionDescripcion.DescEmpresa}");
            }

            return grid;
        }

        public override byte[] GridExportExcel(Grid grid, GridExportExcelContext context)
        {
            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "camion")?.Value, out int idCamion))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PedidosCamionQuery(idCamion);

            uow.HandleQuery(dbQuery);

            context.FileName = this._identity.Application + "-" + DateTime.Now.ToString("yyyy-MM-dd_HH:mm") + ".xlsx";

            return this._excelService.GetExcel(context.FileName, dbQuery, grid.Columns, context, this.DefaultSort);

        }

        public override GridStats GridFetchStats(Grid grid, GridFetchStatsContext context)
        {
            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "camion")?.Value, out int idCamion))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            using var uow = this._uowFactory.GetUnitOfWork();

            var dbQuery = new PedidosCamionQuery(idCamion);
            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            return new GridStats
            {
                Count = dbQuery.GetCount()
            };
        }

        public override GridButtonActionContext GridButtonAction(GridButtonActionContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.ButtonId == "btnAlerta")
            {
                context.Redirect("/expedicion/EXP051", new List<ComponentParameter>()
                {
                    new ComponentParameter("pedido",context.Row.GetCell("NU_PEDIDO").Value),
                    new ComponentParameter("camion", context.Row.GetCell("CD_CAMION").Value),
                    new ComponentParameter("respetaOrden", context.GetParameter("EXP050_RESPETA_ORDEN")),
                });
            }

            return context;
        }

        public override GridMenuItemActionContext GridMenuItemAction(GridMenuItemActionContext context)
        {
            if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "camion")?.Value, out int cdCamion))
                throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

            using var uow = this._uowFactory.GetUnitOfWork();

            uow.CreateTransactionNumber(this._identity.Application);
            uow.BeginTransaction();

            try
            {
                var keys = this.GetSelectedPedidos(uow, cdCamion, context);

                var camion = uow.CamionRepository.GetCamion(cdCamion);

                if (context.ButtonId == "btnDesAsociar" && IsCamionEditable(uow, camion, out IDocumentoEgreso documentoEgreso))
                {
                    var pedidos = keys.Select(key => new PedidoAsociarUnidad
                    {
                        Pedido = key[1],
                        Cliente = key[2],
                        Empresa = int.Parse(key[3])
                    }).ToList();

                    var expedicionService = new ExpedicionConfiguracionService(uow, this._parameterService, new ParametroMapper());

                    var desarmadoEgreso = new DesarmadoEgresoPedido(uow, _factoryService, _identity, camion, expedicionService, pedidos);
                    desarmadoEgreso.Desarmar();

                    _trackingService.CambiarEstadoSincronizacion(uow, camion, false);

                    uow.SaveChanges();

                    if (documentoEgreso != null)
                    {
                        var egresoDocumental = new EgresoDocumental(_factoryService);
                        egresoDocumental.GenerarEgresoDocumental(uow, _identity.UserId, cdCamion, documentoEgreso.Tipo, documentoEgreso.Numero);
                    }
                }

                uow.SaveChanges();
                uow.Commit();
            }
            catch (Exception ex)
            {
                uow.Rollback();
                context.AddErrorNotification("General_Sec0_Error_Operacion");
                _logger.LogError(ex, "EXP050PedidosInvolucrados - GridMenuItemAction");
            }

            return context;
        }

        public override Form FormInitialize(Form form, FormInitializeContext context)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            if (context.Parameters.Count > 0)
            {
                if (!int.TryParse(context.Parameters.FirstOrDefault(x => x.Id == "camion")?.Value, out int idCamion))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                var respetaOrden = context.Parameters.FirstOrDefault(x => x.Id == "respetaOrden")?.Value;

                if (string.IsNullOrEmpty(respetaOrden))
                    throw new MissingParameterException("General_Sec0_Error_Sin_ParametrosURI");

                var camionDescripcion = uow.CamionRepository.GetCamionDescripcion(idCamion);

                if (camionDescripcion == null)
                    throw new EntityNotFoundException("EXP050_Sec0_error_NoHayCamionDisponible", new string[] { idCamion.ToString() });
            }

            return form;
        }

        public override Form FormButtonAction(Form form, FormButtonActionContext context)
        {
            context.Redirect("expedicion/EXP052");

            return form;
        }

        #region Metodos Auxiliares

        public virtual List<string[]> GetSelectedPedidos(IUnitOfWork uow, int camion, GridMenuItemActionContext context)
        {
            var dbQuery = new PedidosCamionQuery(camion);

            uow.HandleQuery(dbQuery);

            dbQuery.ApplyFilter(this._filterInterpreter, context.Filters);

            if (context.Selection.AllSelected)
                return dbQuery.GetSelectedKeysAndExclude(context.Selection.Keys);

            return dbQuery.GetSelectedKeys(context.Selection.Keys);
        }

        public virtual bool IsCamionEditable(IUnitOfWork uow, Camion camion, out IDocumentoEgreso documentoEgreso)
        {
            var manejoDocumentalActivo = camion.Empresa.HasValue && this._parameterService.GetValueByEmpresa(ParamManager.MANEJO_DOCUMENTAL, camion.Empresa.Value) == "S";

            documentoEgreso = null;

            if (!_security.IsUserAllowed(SecurityResources.WEXP013_Page_Access_ArmarEgresoPedido))
                return false;

            if (!camion.IsAguarandoCarga())
                return false;

            if (camion.NumeroInterfazEjecucionFactura != null)
                return false;

            if (manejoDocumentalActivo)
            {
                documentoEgreso = uow.DocumentoRepository.GetEgresoPorCamion(camion.Id);

                if (documentoEgreso == null)
                    return true;

                return uow.DocumentoTipoRepository.PermiteEditarCamion(documentoEgreso.Tipo, documentoEgreso.Estado);
            }

            return true;
        }

        #endregion
    }
}
