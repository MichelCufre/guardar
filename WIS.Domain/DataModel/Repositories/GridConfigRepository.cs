using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General.API.Dtos.Entrada;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.GridComponent;
using WIS.GridComponent.Build;
using WIS.GridComponent.Columns;
using WIS.GridComponent.Excel;
using WIS.Persistence.Database;
using WIS.WMS_API.Controllers.Entrada;

namespace WIS.Domain.DataModel.Repositories
{
    public class GridConfigRepository
    {
        protected readonly WISDB _context;
        protected readonly string Application;
        protected readonly int User;
        protected readonly GridConfigMapper _mapper;
        protected readonly IDapper _dapper;

        public GridConfigRepository(WISDB context, string application, int user, IDapper dapper)
        {
            this._context = context;
            this.Application = application.Replace("Custom", "");
            this.User = user;
            this._mapper = new GridConfigMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyFilter(string name, int user)
        {
            return this._context.T_GRID_FILTER
                .AsNoTracking()
                .Any(d => d.NM_FILTRO == name && (d.USERID == user || d.FL_GLOBAL == "S"));
        }

        #endregion

        #region Get

        public virtual List<GridFilterData> GetFilterList(string gridId)
        {
            var filters = this._context.T_GRID_FILTER.Include("T_GRID_FILTER_DET").AsNoTracking()
                .Where(d => (d.USERID == this.User || d.FL_GLOBAL == "S")
                    && d.CD_APLICACION == this.Application
                    && d.CD_BLOQUE == gridId)
                .OrderByDescending(d => d.FL_GLOBAL)
                .ThenBy(d => d.NM_FILTRO).ToList();

            var filterData = new List<GridFilterData>();

            foreach (var filter in filters)
            {
                filterData.Add(this._mapper.MapToFilterObject(filter));
            }

            return filterData;
        }

        public virtual GridFilterData GetDefaultFilter(string gridId)
        {
            var filter = this._context.T_GRID_FILTER
                .Include("T_GRID_FILTER_DET")
                .AsNoTracking()
                .Where(d => (d.USERID == this.User || d.FL_GLOBAL == "S")
                    && d.CD_APLICACION == this.Application
                    && d.CD_BLOQUE == gridId
                    && d.FL_INICIAL == "S")
                .FirstOrDefault();

            return filter != null ? this._mapper.MapToFilterObject(filter) : null;
        }

        public virtual List<IGridColumn> GetColumns(string gridId, GridColumnFactory columnFactory)
        {
            var userConfig = this._context.T_GRID_USER_CONFIG
                .Where(d => d.CD_APLICACION == this.Application
                    && d.CD_BLOQUE == gridId
                    && d.USERID == this.User)
                .OrderBy(d => d.NU_ORDEN_VISUAL)
                .ToList();

            if (userConfig.Count > 0)
                return this._mapper.MapToUserColumns(columnFactory, userConfig);

            var defaultConfig = this._context.T_GRID_DEFAULT_CONFIG
                .Where(d => d.CD_APLICACION == this.Application
                    && d.CD_BLOQUE == gridId)
                .OrderBy(d => d.NU_ORDEN_VISUAL)
                .ToList();

            return this._mapper.MapToDefaultColumns(columnFactory, defaultConfig);
        }

        public virtual Dictionary<string, List<IGridColumn>> GetApiColumns(int interfazExterna, out string filename)
        {
            var columnFactory = new GridColumnFactory();
            var properties = GetApiProperties(interfazExterna, out filename);
            var columns = new Dictionary<string, List<IGridColumn>>();

            foreach (var key in properties.Keys)
            {
                columns[key] = this._mapper.MapApiColumns(columnFactory, properties[key]);
            }

            return columns;
        }

        public virtual Dictionary<string, List<string>> GetApiProperties(int interfazExterna, out string filename)
        {
            filename = string.Empty;

            var names = new Dictionary<string, List<string>>();
            Dictionary<string, PropertyInfo[]> properties = new Dictionary<string, PropertyInfo[]>();

            switch (interfazExterna)
            {

                case CInterfazExterna.Producto:
                    properties[GridExcelSheetNames.SheetDataName] = new ProductoRequest().GetType().GetProperties();
                    filename = "Productos";
                    break;

                case CInterfazExterna.CodigoDeBarras:
                    properties[GridExcelSheetNames.SheetDataName] = new CodigoBarraRequest().GetType().GetProperties();
                    filename = "CodigosDeBarras";
                    break;

                case CInterfazExterna.Empresas:
                    properties[GridExcelSheetNames.SheetDataName] = new EmpresaRequest().GetType().GetProperties();
                    filename = "Empresas";
                    break;

                case CInterfazExterna.Agentes:
                    properties[GridExcelSheetNames.SheetDataName] = new AgenteRequest().GetType().GetProperties();
                    filename = "Agentes";
                    break;

                case CInterfazExterna.ProductoProveedor:
                    properties[GridExcelSheetNames.SheetDataName] = new ProductoProveedorRequest().GetType().GetProperties();
                    filename = "Producto proveedor";
                    break;

                case CInterfazExterna.Agendas:
                    properties[GridExcelSheetNames.SheetDataName] = new AgendaRequest().GetType().GetProperties();
                    filename = "Agenda";
                    break;

                case CInterfazExterna.Pedidos:

                    PedidoRequest pedidoVacio = new PedidoRequest();
                    PropertyInfo[] propiedadesPedidoData = new PedidoRequest().GetType().GetProperties();
                    properties[GridExcelSheetNames.SheetDataName] = propiedadesPedidoData
                        .Where(p => p.Name != nameof(pedidoVacio.Detalles)
                            && p.Name != nameof(pedidoVacio.Lpns)).ToArray();

                    List<PropertyInfo> propiedadesPedidoDetail = new List<PropertyInfo>();
                    propiedadesPedidoDetail.AddRange(new PedidoDetalleWithKeysRequest().GetType().GetProperties()
                        .Where(p => p.Name != nameof(PedidoDetalleWithKeysRequest.Duplicados)
                            && p.Name != nameof(PedidoDetalleWithKeysRequest.DetallesLpn)
                            && p.Name != nameof(PedidoDetalleWithKeysRequest.Atributos)));
                    properties[GridExcelSheetNames.SheetDetailName] = propiedadesPedidoDetail.ToArray();

                    List<PropertyInfo> propiedadesDuplicateDetail = new List<PropertyInfo>();
                    propiedadesDuplicateDetail.AddRange(new PedidoDuplicadoWithKeysRequest().GetType().GetProperties());
                    properties[GridExcelSheetNames.SheetDuplicateName] = propiedadesDuplicateDetail.ToArray();

                    List<PropertyInfo> propiedadesLpnDetailPedido = new List<PropertyInfo>();
                    propiedadesLpnDetailPedido.AddRange(new PedidoDetalleLpnWithKeysRequest().GetType().GetProperties()
                        .Where(p => p.Name != nameof(PedidoDetalleWithKeysRequest.Atributos)));
                    properties[GridExcelSheetNames.SheetDetalleLpnName] = propiedadesLpnDetailPedido.ToArray();

                    List<PropertyInfo> propiedadesLpnPedido = new List<PropertyInfo>();
                    propiedadesLpnPedido.AddRange(new PedidoLpnWithKeysRequest().GetType().GetProperties());
                    properties[GridExcelSheetNames.SheetLpnName] = propiedadesLpnPedido.ToArray();

                    List<PropertyInfo> propiedadesLpnAttributesDetail = new List<PropertyInfo>();
                    propiedadesLpnAttributesDetail.AddRange(new PedidoAtributosLpnWithKeysRequest().GetType().GetProperties()
                        .Where(p => p.Name != nameof(PedidoDetalleLpnWithKeysRequest.Atributos)));
                    properties[GridExcelSheetNames.SheetAtributosLpnName] = propiedadesLpnAttributesDetail.ToArray();

                    List<PropertyInfo> propiedadesLpnAttributeDetailPedido = new List<PropertyInfo>();
                    propiedadesLpnAttributeDetailPedido.AddRange(new PedidoAtributoLpnWithKeysRequest().GetType().GetProperties());
                    properties[GridExcelSheetNames.SheetAtributosLpnDetailName] = propiedadesLpnAttributeDetailPedido.ToArray();

                    List<PropertyInfo> propiedadesAttributesDetail = new List<PropertyInfo>();
                    propiedadesAttributesDetail.AddRange(new PedidoAtributosWithKeysRequest().GetType().GetProperties()
                        .Where(p => p.Name != nameof(PedidoDetalleWithKeysRequest.Atributos)));
                    properties[GridExcelSheetNames.SheetAtributosName] = propiedadesAttributesDetail.ToArray();

                    List<PropertyInfo> propiedadesAttributeDetailPedido = new List<PropertyInfo>();
                    propiedadesAttributeDetailPedido.AddRange(new PedidoAtributoWithKeysRequest().GetType().GetProperties());
                    properties[GridExcelSheetNames.SheetAtributosDetailName] = propiedadesAttributeDetailPedido.ToArray();

                    filename = "Pedidos";

                    break;

                case CInterfazExterna.ReferenciaDeRecepcion:

                    ReferenciaRecepcionRequest referenciaVacia = new ReferenciaRecepcionRequest();
                    PropertyInfo[] propiedadesData = new ReferenciaRecepcionRequest().GetType().GetProperties();
                    properties[GridExcelSheetNames.SheetDataName] = propiedadesData.Where(p => p.Name != nameof(referenciaVacia.Detalles)).ToArray();

                    List<PropertyInfo> propiedadesDetail = new List<PropertyInfo>();
                    propiedadesDetail.AddRange(new ReferenciaRecepcionDetalleWithKeysRequest().GetType().GetProperties());
                    properties[GridExcelSheetNames.SheetDetailName] = propiedadesDetail.ToArray();

                    filename = "Referencia recepción";

                    break;

                case CInterfazExterna.Lpn:

                    LpnRequest lpnVacio = new LpnRequest();
                    PropertyInfo[] propiedadesLpnData = new LpnRequest().GetType().GetProperties();
                    properties[GridExcelSheetNames.SheetDataName] = propiedadesLpnData.Where(p => p.Name != nameof(lpnVacio.Detalles) && p.Name != nameof(lpnVacio.Atributos) && p.Name != nameof(lpnVacio.Barras)).ToArray();

                    List<PropertyInfo> propiedadesLpnAtributos = new List<PropertyInfo>();
                    propiedadesLpnAtributos.AddRange(new LpnAtributoWithKeysRequest().GetType().GetProperties());
                    properties[GridExcelSheetNames.SheetAtributosName] = propiedadesLpnAtributos.ToArray();

                    List<PropertyInfo> propiedadesLpnBarras = new List<PropertyInfo>();
                    propiedadesLpnBarras.AddRange(new LpnBarrasWithKeysRequest().GetType().GetProperties());
                    properties[GridExcelSheetNames.SheetBarrasName] = propiedadesLpnBarras.ToArray();

                    List<PropertyInfo> propiedadesLpnDetail = new List<PropertyInfo>();
                    propiedadesLpnDetail.AddRange(new LpnDetalleWithKeysRequest().GetType().GetProperties().Where(p => p.Name != nameof(LpnDetalleWithKeysRequest.Atributos)));
                    properties[GridExcelSheetNames.SheetDetailName] = propiedadesLpnDetail.ToArray();

                    List<PropertyInfo> propiedadesLpnAtributosDetail = new List<PropertyInfo>();
                    propiedadesLpnAtributosDetail.AddRange(new LpnAtributoDetalleWithKeysRequest().GetType().GetProperties());
                    properties[GridExcelSheetNames.SheetAtributosDetailName] = propiedadesLpnAtributosDetail.ToArray();

                    filename = "LPN";

                    break;

                case CInterfazExterna.Produccion:

                    List<PropertyInfo> listProduccion = new ProduccionRequest()
                        .GetType()
                        .GetProperties()
                        .Where(x =>
                            x.Name != nameof(ProduccionRequest.Insumos) &&
                            x.Name != nameof(ProduccionRequest.Productos))
                        .ToList();

                    List<PropertyInfo> listInsumos = new ProduccionInsumoRequestWithKeyProduccion()
                        .GetType()
                        .GetProperties()
                        .ToList();

                    List<PropertyInfo> listProductos = new ProduccionProductoFinalesRequestWithKeyProduccion()
                        .GetType()
                        .GetProperties()
                        .ToList();

                    properties[GridExcelSheetNames.SheetProduccion] = listProduccion.ToArray();
                    properties[GridExcelSheetNames.SheetProduccionInsumo] = listInsumos.ToArray();
                    properties[GridExcelSheetNames.SheetProduccionProducto] = listProductos.ToArray();

                    filename = "Produccion";
                    break;

                case CInterfazExterna.ControlDeCalidad:

                    List<PropertyInfo> listControles = new ControlCalidadItemRequest()
                        .GetType()
                        .GetProperties()
                        .Where(x => x.Name != nameof(ControlCalidadItemRequest.CriteriosDeSeleccion))
                        .ToList();

                    List<PropertyInfo> listCriterios = new CriterioSeleccionItemRequestWithKey()
                        .GetType()
                        .GetProperties()
                        .ToList();

                    properties[GridExcelSheetNames.SheetControles] = listControles.ToArray();
                    properties[GridExcelSheetNames.SheetCriterios] = listCriterios.ToArray();

                    filename = "Controles De Calidad";
                    break;
                case CInterfazExterna.Facturas:

                    FacturaRequest facturaVacia = new FacturaRequest();
                    PropertyInfo[] propiedadesFactura = new FacturaRequest().GetType().GetProperties();
                    properties[GridExcelSheetNames.SheetDataName] = propiedadesFactura.Where(p => p.Name != nameof(facturaVacia.Detalles)).ToArray();

                    List<PropertyInfo> propiedadesDetailFactura = new List<PropertyInfo>();
                    propiedadesDetailFactura.AddRange(new FacturaDetalleWithKeysRequest().GetType().GetProperties());
                    properties[GridExcelSheetNames.SheetDetailName] = propiedadesDetailFactura.ToArray();

                    filename = "Facturas";

                    break;

                case CInterfazExterna.PickingProducto:
                    properties[GridExcelSheetNames.SheetDataName] = new PickingProductoRequest().GetType().GetProperties();
                    filename = "UbicacionesPicking";
                    break;
            }

            foreach (var key in properties.Keys)
            {
                names[key] = properties[key].Select(p => p.Name).ToList();
            }

            return names;
        }

        #endregion

        #region Add

        public virtual void AddDefaultConfig(string gridId, IGridColumn column)
        {
            this._context.T_GRID_DEFAULT_CONFIG.Add(this._mapper.MapToDefaultEntity(gridId, this.Application, column));
        }

        public virtual void AddUserConfig(string gridId, IGridColumn column)
        {
            this._context.T_GRID_USER_CONFIG.Add(this._mapper.MapToUserEntity(gridId, this.Application, this.User, column));
        }

        public virtual void SaveFilter(GridFilterData data)
        {
            T_GRID_FILTER filter = this._mapper.MapToFilterEntity(data, this.Application, this.User);

            filter.CD_FILTRO = this._context.GetNextSequenceValueLong(_dapper, "S_GRID_FILTER");

            this._context.T_GRID_FILTER.Add(filter);
        }

        #endregion

        #region Update

        public virtual void Update(string gridId, List<GridColumn> columns)
        {
            var configLines = this._context.T_GRID_DEFAULT_CONFIG
                .AsNoTracking()
                .Where(d => d.CD_APLICACION == this.Application
                    && d.CD_BLOQUE == gridId)
                .ToList();
            var userConfigLines = this._context.T_GRID_USER_CONFIG
                .AsNoTracking()
                .Where(d => d.CD_APLICACION == this.Application
                    && d.CD_BLOQUE == gridId
                    && d.USERID == this.User)
                .ToList();

            foreach (var column in columns)
            {
                var userConfig = userConfigLines.Where(d => d.NM_DATAFIELD == column.Id).FirstOrDefault();

                if (userConfig == null)
                {
                    var defaultConfig = configLines.Where(d => d.NM_DATAFIELD == column.Id).FirstOrDefault();

                    if (defaultConfig == null)
                        continue;

                    this.AddUserConfig(gridId, column);
                }
                else
                {
                    var entity = this._mapper.MapToUserEntity(gridId, this.Application, this.User, column);
                    var attachedEntity = this._context.T_GRID_USER_CONFIG.Local
                        .FirstOrDefault(c => c.CD_APLICACION == entity.CD_APLICACION
                            && c.CD_BLOQUE == entity.CD_BLOQUE
                            && c.NM_DATAFIELD == entity.NM_DATAFIELD
                            && c.USERID == entity.USERID);

                    if (attachedEntity != null)
                    {
                        var attachedEntry = _context.Entry(attachedEntity);
                        attachedEntry.CurrentValues.SetValues(entity);
                        attachedEntry.State = EntityState.Modified;
                    }
                    else
                    {
                        this._context.T_GRID_USER_CONFIG.Attach(entity);
                        this._context.Entry(entity).State = EntityState.Modified;
                    }
                }
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveFilter(long filterId, int user)
        {
            var filter = this._context.T_GRID_FILTER
                .Include("T_GRID_FILTER_DET")
                .Where(d => (d.USERID == this.User || d.FL_GLOBAL == "S")
                    && d.CD_FILTRO == filterId)
                .FirstOrDefault();

            if (filter == null)
                return;

            if (filter.USERID != user)
                throw new OperationNotAllowedException($"Este filtro solo lo puede borrar el usuario {filter.USERID}.");

            if (filter.T_GRID_FILTER_DET.Count > 0)
                this._context.T_GRID_FILTER_DET.RemoveRange(filter.T_GRID_FILTER_DET);

            this._context.T_GRID_FILTER.Remove(filter);
        }

        public virtual void Reset(string gridId)
        {
            var userConfigLines = this._context.T_GRID_USER_CONFIG.Where(d => d.CD_APLICACION == this.Application && d.CD_BLOQUE == gridId && d.USERID == this.User).ToList();

            foreach (var line in userConfigLines)
            {
                this._context.T_GRID_USER_CONFIG.Remove(line);
            }
        }


        #endregion

        #region Dapper

        #endregion
    }
}
