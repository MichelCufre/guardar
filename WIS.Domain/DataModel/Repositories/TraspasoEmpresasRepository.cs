using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Documento.TiposDocumento;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Helpers;
using WIS.Domain.Parametrizacion;
using WIS.Domain.Picking;
using WIS.Domain.Picking.Logic.Bulks;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Domain.StockEntities.Constants;
using WIS.Exceptions;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class TraspasoEmpresasRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly TraspasoEmpresasMapper _mapper;
        protected readonly EmpresaMapper _empresaMapper;
        protected readonly IDapper _dapper;
        protected readonly EmpresaMapper _mapperEmpresa;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public TraspasoEmpresasRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new TraspasoEmpresasMapper();
            this._dapper = dapper;
            this._mapperEmpresa = new EmpresaMapper();
        }

        #region Add

        public virtual void AddConfiguracionTraspaso(TraspasoEmpresasConfiguracion config)
        {
            config.Id = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_TRASPASO_CONFIGURACION);
            this._context.T_TRASPASO_CONFIGURACION.Add(this._mapper.MapConfigToEntity(config));
        }

        public virtual void AgregarEmpresasDestino(long idConfig, List<int> empresas)
        {
            foreach (var emp in empresas?.Distinct())
            {
                if (!this._context.T_TRASPASO_CONF_DESTINO.Any(x => x.NU_TRASPASO_CONFIGURACION == idConfig && x.CD_EMPRESA == emp))
                {
                    this._context.T_TRASPASO_CONF_DESTINO.Add(new T_TRASPASO_CONF_DESTINO
                    {
                        NU_TRASPASO_CONF_DESTINO = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_TRASPASO_CONF_DESTINO),
                        NU_TRASPASO_CONFIGURACION = idConfig,
                        CD_EMPRESA = emp
                    });
                }
            }
        }

        public virtual void AgregarTiposTraspaso(long idConfig, List<string> tiposTraspaso)
        {
            foreach (var t in tiposTraspaso?.Distinct())
            {
                if (!this._context.T_TRASPASO_CONF_TIPO_TRASPASO.Any(x => x.NU_TRASPASO_CONFIGURACION == idConfig && x.TP_TRASPASO == t))
                {
                    this._context.T_TRASPASO_CONF_TIPO_TRASPASO.Add(new T_TRASPASO_CONF_TIPO_TRASPASO
                    {
                        NU_TRASPASO_CONF_TIPO_TRASPASO = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_TRASPASO_CONF_TIPO_TRASPASO),
                        NU_TRASPASO_CONFIGURACION = idConfig,
                        TP_TRASPASO = t
                    });
                }
            }
        }

        public virtual void AddMapeoProductos(MapeoProducto mapeo)
        {
            this._context.T_TRASPASO_MAPEO_PRODUTO.Add(this._mapper.MapMapeoToEntity(mapeo));
        }

        public virtual void AddTraspaso(TraspasoEmpresas traspaso)
        {
            traspaso.Id = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_TRASPASO_EMPRESAS);

            if (string.IsNullOrEmpty(traspaso.IdExterno))
                traspaso.IdExterno = "WIS" + traspaso.Id.ToString();

            this._context.T_TRASPASO.Add(this._mapper.MapToEntity(traspaso));
        }

        public virtual void AddDetallePedido(TraspasoEmpresasDetallePedido det)
        {
            det.Id = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_TRASPASO_DET_PEDIDO);
            this._context.T_TRASPASO_DET_PEDIDO.Add(this._mapper.MapDetPedidoToEntity(det));
        }

        #endregion

        #region Any

        public virtual bool AnyConfiguracionTraspaso(long idConfig)
        {
            return this._context.T_TRASPASO_CONFIGURACION.Any(x => x.NU_TRASPASO_CONFIGURACION == idConfig);
        }

        public virtual bool IsEmpresaOrigenConfigurada(int empresa)
        {
            return this._context.T_TRASPASO_CONFIGURACION.Any(x => x.CD_EMPRESA == empresa);
        }

        public virtual bool AnyMapeoProducto(int empresaOrigen, string cdProducto, decimal faixa, int empresaDestino)
        {
            return this._context.T_TRASPASO_MAPEO_PRODUTO
                .Any(x => x.CD_EMPRESA == empresaOrigen
                    && x.CD_EMPRESA_DESTINO == empresaDestino
                    && x.CD_PRODUTO == cdProducto
                    && x.CD_FAIXA == faixa);
        }

        public virtual bool AnyTraspaso(long idTraspaso)
        {
            return this._context.T_TRASPASO.Any(x => x.NU_TRASPASO == idTraspaso);
        }

        public virtual TraspasoEmpresasPreparacionPendiente GetPreparacionPendienteValida(int nuPreparacion)
        {
            return _mapper.MapToObject(this._context.V_STO820_PREPARACION_PENDIENTE.FirstOrDefault(x => x.NU_PREPARACION == nuPreparacion));
        }

        public virtual TraspasoEmpresasPreparacionPreparado GetPreparacionPreparadoValida(int nuPreparacion)
        {
            return _mapper.MapToObject(this._context.V_STO820_PREPARACION_PREPARADO.FirstOrDefault(x => x.NU_PREPARACION == nuPreparacion));
        }

        public virtual bool AnyTraspasoActivoEmpresa(int empresa)
        {
            List<string> situacionesActiva = new List<string> {
            TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION,
            TraspasoEmpresasDb.ESTADO_TRASPASO_EN_PROCESO,
            };

            return this._context.T_TRASPASO.Any(w => w.CD_EMPRESA == empresa
                    && situacionesActiva.Contains(w.ID_ESTADO));
        }


        #endregion

        #region Get

        public virtual TraspasoEmpresasConfiguracion GetConfiguracionTraspaso(long idConfig)
        {
            return this._context.T_TRASPASO_CONFIGURACION.AsNoTracking()
                .Where(x => x.NU_TRASPASO_CONFIGURACION == idConfig)
                .Select(x => this._mapper.MapConfigToObject(x))
                .FirstOrDefault();
        }

        public virtual TraspasoEmpresasConfiguracion GetConfiguracionTraspasoByEmpresa(int cdEmpresa)
        {
            return this._context.T_TRASPASO_CONFIGURACION.AsNoTracking()
                .Where(x => x.CD_EMPRESA == cdEmpresa)
                .Select(x => this._mapper.MapConfigToObject(x))
                .FirstOrDefault();
        }

        public virtual MapeoProducto GetMapeoProducto(int cdEmpresaOrigen, string cdProducto, decimal faixa, int cdEmpresaDestino)
        {
            return this._context.T_TRASPASO_MAPEO_PRODUTO.AsNoTracking()
                .Where(x => x.CD_EMPRESA == cdEmpresaOrigen
                    && x.CD_EMPRESA_DESTINO == cdEmpresaDestino
                    && x.CD_PRODUTO == cdProducto
                    && x.CD_FAIXA == faixa)
                .Select(x => this._mapper.MapMapeoToObject(x))
                .FirstOrDefault();
        }

        public virtual TraspasoEmpresas GetTraspaso(long idTraspaso)
        {
            return this._context.T_TRASPASO.AsNoTracking()
                .Where(x => x.NU_TRASPASO == idTraspaso)
                .Select(x => this._mapper.MapToObject(x))
                .FirstOrDefault();
        }

        public virtual List<KeyValuePair<string, string>> GetTiposTraspaso()
        {
            return this._context.V_STO800_TIPOS_TRASPASO
                .Select(x => new KeyValuePair<string, string>(x.TP_TRASPASO, x.DS_TRASPASO))
                .ToList();
        }

        public virtual List<KeyValuePair<int, string>> GetPreparacionesPendientes(int cdEmpresa)
        {
            List<string> situacionesActiva = new List<string> {
            TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION,
            TraspasoEmpresasDb.ESTADO_TRASPASO_EN_PROCESO,
            };

            return this._context.V_STO820_PREPARACION_PENDIENTE
                .Where(x => x.CD_EMPRESA == cdEmpresa && !this._context.T_TRASPASO.Any(w => w.NU_PREPARACION == x.NU_PREPARACION &&
                    situacionesActiva.Contains(w.ID_ESTADO)
                ))
                .Select(x => new KeyValuePair<int, string>(x.NU_PREPARACION, x.DS_PREPARACION))
                .ToList();
        }

        public virtual List<KeyValuePair<int, string>> GetPreparacionesTodoPreparado(int cdEmpresa)
        {
            List<string> situacionesActiva = new List<string> {
            TraspasoEmpresasDb.ESTADO_TRASPASO_EN_EDICION,
            TraspasoEmpresasDb.ESTADO_TRASPASO_EN_PROCESO,
            };

            return this._context.V_STO820_PREPARACION_PREPARADO
                .Where(x => x.CD_EMPRESA == cdEmpresa && !this._context.T_TRASPASO.Any(w => w.NU_PREPARACION == x.NU_PREPARACION
                    && situacionesActiva.Contains(w.ID_ESTADO)))
                .Select(x => new KeyValuePair<int, string>(x.NU_PREPARACION, x.DS_PREPARACION))
                .ToList();
        }

        public virtual TraspasoEmpresasDetallePedido GetDetallePedido(long idTraspaso, string nuPedido, string cdCliente, int cdEmpresa)
        {
            return this._context.T_TRASPASO_DET_PEDIDO.AsNoTracking()
                .Where(x => x.NU_TRASPASO == idTraspaso && x.NU_PEDIDO == nuPedido && x.CD_CLIENTE == cdCliente && x.CD_EMPRESA == cdEmpresa)
                .Select(x => this._mapper.MapDetPedidoToObject(x))
                .FirstOrDefault();
        }

        public virtual List<KeyValuePair<string, string>> GetTiposTraspaso(long idConfig)
        {
            return this._context.T_TRASPASO_CONF_TIPO_TRASPASO.Where(x => x.NU_TRASPASO_CONFIGURACION == idConfig)
                .Join(_context.V_STO800_TIPOS_TRASPASO,
                    ctt => ctt.TP_TRASPASO,
                    tt => tt.TP_TRASPASO,
                    (ctt, tt) => tt)
                .Select(x => new KeyValuePair<string, string>(x.TP_TRASPASO, x.DS_TRASPASO))
                .ToList();
        }

        public virtual List<Empresa> GetEmpresasByConfiguracionEmpresa(long nroConfiguracion, string value)
        {
            var empresasUsuario = this._context.T_TRASPASO_CONF_DESTINO.Where(x => x.NU_TRASPASO_CONFIGURACION == nroConfiguracion)
                 .Join(this._context.T_EMPRESA,
                 tt => tt.CD_EMPRESA,
                 e => e.CD_EMPRESA,
                 (tt, e) => e)
           .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == _userId),
               e => e.CD_EMPRESA,
               ef => ef.CD_EMPRESA,
               (e, ef) => e)
           .AsNoTracking();

            int cdEmpresa;

            if (int.TryParse(value, out cdEmpresa))
            {
                empresasUsuario = empresasUsuario
                    .Where(d => (d.CD_EMPRESA == cdEmpresa
                        || (d.CD_EMPRESA.ToString().Contains(cdEmpresa.ToString()))
                        || (d.NM_EMPRESA.ToLower().Contains(value.ToLower()))));
            }
            else
            {
                empresasUsuario = empresasUsuario
                    .Where(d => d.NM_EMPRESA.ToLower().Contains(value.ToLower()));
            }

            return empresasUsuario
                .Select(d => this._mapperEmpresa.MapToObject(d))
                .ToList();

        }

        public virtual List<Empresa> GetEmpresasByConfiguracionEmpresa(long nroConfiguracion)
        {
            var empresasUsuario = this._context.T_TRASPASO_CONF_DESTINO.Where(x => x.NU_TRASPASO_CONFIGURACION == nroConfiguracion)
                 .Join(this._context.T_EMPRESA,
                 tt => tt.CD_EMPRESA,
                 e => e.CD_EMPRESA,
                 (tt, e) => e)
           .Join(this._context.T_EMPRESA_FUNCIONARIO.Where(ef => ef.USERID == _userId),
               e => e.CD_EMPRESA,
               ef => ef.CD_EMPRESA,
               (e, ef) => e)
           .AsNoTracking();

            return empresasUsuario
                .Select(d => this._mapperEmpresa.MapToObject(d))
                .ToList();

        }

        #endregion

        #region Update

        public virtual void UpdateConfiguracionTraspaso(TraspasoEmpresasConfiguracion config)
        {
            T_TRASPASO_CONFIGURACION entity = this._mapper.MapConfigToEntity(config);
            T_TRASPASO_CONFIGURACION attachedEntity = _context.T_TRASPASO_CONFIGURACION.Local.FirstOrDefault(w => w.NU_TRASPASO_CONFIGURACION == entity.NU_TRASPASO_CONFIGURACION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_TRASPASO_CONFIGURACION.Attach(entity);
                _context.Entry<T_TRASPASO_CONFIGURACION>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateMapeoProducto(MapeoProducto mapeo)
        {
            T_TRASPASO_MAPEO_PRODUTO entity = this._mapper.MapMapeoToEntity(mapeo);
            T_TRASPASO_MAPEO_PRODUTO attachedEntity = _context.T_TRASPASO_MAPEO_PRODUTO.Local
                .FirstOrDefault(w => w.CD_EMPRESA == mapeo.EmpresaOrigen
                    && w.CD_EMPRESA_DESTINO == mapeo.EmpresaDestino
                    && w.CD_PRODUTO == mapeo.ProductoOrigen
                    && w.CD_FAIXA == mapeo.FaixaOrigen);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_TRASPASO_MAPEO_PRODUTO.Attach(entity);
                _context.Entry<T_TRASPASO_MAPEO_PRODUTO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateTraspaso(TraspasoEmpresas traspaso)
        {
            T_TRASPASO entity = this._mapper.MapToEntity(traspaso);
            T_TRASPASO attachedEntity = _context.T_TRASPASO.Local.FirstOrDefault(w => w.NU_TRASPASO == entity.NU_TRASPASO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_TRASPASO.Attach(entity);
                _context.Entry<T_TRASPASO>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetallePedido(TraspasoEmpresasDetallePedido detalle)
        {
            T_TRASPASO_DET_PEDIDO entity = this._mapper.MapDetPedidoToEntity(detalle);
            T_TRASPASO_DET_PEDIDO attachedEntity = _context.T_TRASPASO_DET_PEDIDO.Local.FirstOrDefault(w => w.NU_TRASPASO_DET_PEDIDO == entity.NU_TRASPASO_DET_PEDIDO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_TRASPASO_DET_PEDIDO.Attach(entity);
                _context.Entry<T_TRASPASO_DET_PEDIDO>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoverEmpresasDestino(long idConfig, List<int> empresas)
        {
            foreach (var emp in empresas)
            {
                var entity = this._context.T_TRASPASO_CONF_DESTINO
                    .FirstOrDefault(x => x.NU_TRASPASO_CONFIGURACION == idConfig && x.CD_EMPRESA == emp);
                var attachedEntity = this._context.T_TRASPASO_CONF_DESTINO.Local
                    .FirstOrDefault(x => x.NU_TRASPASO_CONFIGURACION == idConfig && x.CD_EMPRESA == emp);

                if (attachedEntity != null)
                {
                    this._context.T_TRASPASO_CONF_DESTINO.Remove(attachedEntity);
                }
                else
                {
                    this._context.T_TRASPASO_CONF_DESTINO.Remove(entity);
                }

            }
        }

        public virtual void RemoverTiposTraspaso(long idConfig, List<string> tiposTraspaso)
        {
            foreach (var t in tiposTraspaso)
            {
                var entity = this._context.T_TRASPASO_CONF_TIPO_TRASPASO
                    .FirstOrDefault(x => x.NU_TRASPASO_CONFIGURACION == idConfig && x.TP_TRASPASO == t);
                var attachedEntity = this._context.T_TRASPASO_CONF_TIPO_TRASPASO.Local
                    .FirstOrDefault(x => x.NU_TRASPASO_CONFIGURACION == idConfig && x.TP_TRASPASO == t);

                if (attachedEntity != null)
                {
                    this._context.T_TRASPASO_CONF_TIPO_TRASPASO.Remove(attachedEntity);
                }
                else
                {
                    this._context.T_TRASPASO_CONF_TIPO_TRASPASO.Remove(entity);
                }

            }
        }

        public virtual void RemoverEmpresasDestino(long idConfig)
        {
            var entities = this._context.T_TRASPASO_CONF_DESTINO
                .Where(x => x.NU_TRASPASO_CONFIGURACION == idConfig)
                .ToList();

            foreach (var entity in entities)
            {
                var attachedEntity = this._context.T_TRASPASO_CONF_DESTINO.Local
                    .FirstOrDefault(x => x.NU_TRASPASO_CONFIGURACION == idConfig && x.CD_EMPRESA == entity.CD_EMPRESA);

                this._context.T_TRASPASO_CONF_DESTINO.Remove(attachedEntity ?? entity);
            }
        }

        public virtual void RemoverTiposTraspaso(long idConfig)
        {
            var entities = this._context.T_TRASPASO_CONF_TIPO_TRASPASO
                .Where(x => x.NU_TRASPASO_CONFIGURACION == idConfig)
                .ToList();

            foreach (var entity in entities)
            {
                var attachedEntity = this._context.T_TRASPASO_CONF_TIPO_TRASPASO.Local
                    .FirstOrDefault(x => x.NU_TRASPASO_CONFIGURACION == idConfig && x.TP_TRASPASO == entity.TP_TRASPASO);

                this._context.T_TRASPASO_CONF_TIPO_TRASPASO.Remove(attachedEntity ?? entity);
            }
        }

        public virtual void DeleteConfiguracionTraspaso(long idConfig)
        {
            T_TRASPASO_CONFIGURACION entity = this._context.T_TRASPASO_CONFIGURACION
               .FirstOrDefault(x => x.NU_TRASPASO_CONFIGURACION == idConfig);

            T_TRASPASO_CONFIGURACION attachedEntity = _context.T_TRASPASO_CONFIGURACION.Local
                .FirstOrDefault(x => x.NU_TRASPASO_CONFIGURACION == idConfig);

            if (attachedEntity != null)
            {
                _context.T_TRASPASO_CONFIGURACION.Remove(attachedEntity);
            }
            else
            {
                _context.T_TRASPASO_CONFIGURACION.Attach(entity);
                _context.T_TRASPASO_CONFIGURACION.Remove(entity);
            }
        }

        public virtual void DeleteMapeoProducto(int empresaOrigen, string cdProducto, decimal faixa, int empresaDestino)
        {
            T_TRASPASO_MAPEO_PRODUTO entity = this._context.T_TRASPASO_MAPEO_PRODUTO
               .FirstOrDefault(x => x.CD_EMPRESA == empresaOrigen
                    && x.CD_EMPRESA_DESTINO == empresaDestino
                    && x.CD_PRODUTO == cdProducto
                    && x.CD_FAIXA == faixa);

            T_TRASPASO_MAPEO_PRODUTO attachedEntity = _context.T_TRASPASO_MAPEO_PRODUTO.Local
                .FirstOrDefault(x => x.CD_EMPRESA == empresaOrigen
                    && x.CD_EMPRESA_DESTINO == empresaDestino
                    && x.CD_PRODUTO == cdProducto
                    && x.CD_FAIXA == faixa);

            if (attachedEntity != null)
            {
                _context.T_TRASPASO_MAPEO_PRODUTO.Remove(attachedEntity);
            }
            else
            {
                _context.T_TRASPASO_MAPEO_PRODUTO.Attach(entity);
                _context.T_TRASPASO_MAPEO_PRODUTO.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #region TraspasoEntreEmpresa 

        public virtual void PuedeTraspasarEmpresa(IUnitOfWork uow, TraspasoBulkOperationContext contextBulk, TraspasoEmpresas traspaso, bool isEmpresaDocumentalEmpresaOrigen, bool isTraspasoEmpresaDejarPreparado, bool isTraspasoEmpresaPreparacionPendiente, bool isPropagarLpn, int empresaDest, long nuTransaccion, out List<TraspasoEmpresasDetalleTemp> detallesTraspaso, out List<Pedido> pedidosNew)
        {
            pedidosNew = null;
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var detalleTraspaso = contextBulk.DetallePreparacion.Select(x => new DetallePreparacion()
            {
                NumeroPreparacion = x.NumeroPreparacion,
                Producto = x.Producto,
                Lote = x.Lote,
                Faixa = x.Faixa,
                Empresa = x.Empresa,
                Ubicacion = !isTraspasoEmpresaDejarPreparado ? x.Ubicacion : x.Contenedor?.Ubicacion,
                Pedido = x.Pedido,
                Cliente = x.Cliente,
                Cantidad = x.Cantidad,
                UbicacionPicking = x.Ubicacion,
                IdDetallePickingLpn = x.IdDetallePickingLpn,
                NroContenedor = x.Contenedor?.Numero,
                NroLpn = x.Contenedor?.NroLpn,
                NumeroSecuencia = x.NumeroSecuencia,
                Agrupacion = x.Agrupacion,
                NroContenedorDestino = x.NroContenedorDestino
            })
            .GroupBy(x => new { x.NumeroPreparacion, x.Producto, x.Lote, x.Faixa, x.Ubicacion, x.Empresa, x.Pedido, x.Cliente, x.UbicacionPicking, x.NumeroSecuencia, x.Agrupacion, x.IdDetallePickingLpn, x.NroContenedor, x.NroLpn, x.NroContenedorDestino })
            .Select(x => new TraspasoEmpresasDetalleTemp()
            {
                NumeroPreparacion = x.Key.NumeroPreparacion,
                UbicacionPicking = x.Key.UbicacionPicking,
                Producto = x.Key.Producto,
                Identificador = x.Key.Lote,
                Faixa = x.Key.Faixa,
                Ubicacion = x.Key.Ubicacion,
                Empresa = x.Key.Empresa,
                Cantidad = x.Sum(d => d.Cantidad),
                Vencimiento = x.Min(d => d.VencimientoPickeo),
                Pedido = x.Key.Pedido,
                Cliente = x.Key.Cliente,
                IdDetallePickingLpn = x.Key.IdDetallePickingLpn,
                NroContenedor = x.Key.NroContenedor,
                NroLpn = x.Key.NroLpn,
                NumeroSecuencia = x.Key.NumeroSecuencia,
                Agrupacion = x.Key.Agrupacion,
                NroContenedorDestino = x.Key.NroContenedorDestino
            }).ToList();

            AddDetalleTraspasoSueltoTemporal(connection, tran, detalleTraspaso, traspaso, empresaDest);
            UpdateVencimientoLoteProdutoTraspasoTemp(connection, tran);

            UpdateDatosProdutoTraspasoTemp(connection, tran);
            UpdateProdutoEstadoAveriaTraspasoTemp(connection, tran);

            UpdateProdutoDestinoTraspasoTemp(connection, tran);
            UpdateDatosProdutoDestinoTraspasoTemp(connection, tran);
            UpdateVencimientoLoteProdutoDestinoTraspasoTemp(connection, tran, nuTransaccion);

            if (isTraspasoEmpresaDejarPreparado || isTraspasoEmpresaPreparacionPendiente)
                UpdatePedidoAndAgenteDestinoTraspasoTemp(connection, tran, traspaso.Id);

            var productos = GetProductoTraspasoTemp(connection, tran);

            var productosMapeoTraspaso = GetProductoTraspasoMapeoTemp(connection, tran);

            if (isTraspasoEmpresaDejarPreparado || isTraspasoEmpresaPreparacionPendiente)
            {
                ProcesarPedidoTraspasoEmpresaTemp(uow, connection, tran, productos, contextBulk.DetallePreparacionLpn, isTraspasoEmpresaPreparacionPendiente, out pedidosNew);

                ControlPedidoTraspasoEmpresa(connection, tran);
            }

            if (isPropagarLpn)
            {
                GenerarTraspasoEmpresaLpnTemp(uow, contextBulk, traspaso, empresaDest, connection, tran, productos, productosMapeoTraspaso);
            }
            else
            {
                AddDetalleTraspasoLpnTemporal(connection, tran, contextBulk.DetallePreparacionLpn, traspaso, empresaDest);
            }

            ControlProductosTraspasoEmpresa(uow, contextBulk, traspaso, isEmpresaDocumentalEmpresaOrigen, connection, tran, productos);

            ControlAgenteTraspasoEmpresa(uow, traspaso, connection, tran);

            detallesTraspaso = productos;

        }

        public virtual void UpdateVencimientoLoteProdutoTraspasoTemp(DbConnection connection, DbTransaction tran)
        {
            var alias = "tdt";
            var from = $@"
                T_TRASPASO_DET_TEMP tdt
                INNER JOIN (
                         SELECT 
                            td.CD_ENDERECO,
                            td.CD_EMPRESA,
                            td.CD_PRODUTO,
                            td.CD_FAIXA,
                            td.NU_IDENTIFICADOR,
                            MIN(st.DT_FABRICACAO) DT_FABRICACAO_PROD
                         FROM  T_TRASPASO_DET_TEMP  td
                         INNER JOIN T_STOCK st on 
                            st.CD_ENDERECO = td.CD_ENDERECO AND
                            st.CD_EMPRESA = td.CD_EMPRESA AND
                            st.CD_PRODUTO = td.CD_PRODUTO AND
                            st.CD_FAIXA = td.CD_FAIXA AND
                            st.NU_IDENTIFICADOR = td.NU_IDENTIFICADOR
                         WHERE td.DT_FABRICACAO is null
                         GROUP BY td.CD_ENDERECO,
                            td.CD_EMPRESA,
                            td.CD_PRODUTO,
                            td.CD_FAIXA,
                            td.NU_IDENTIFICADOR
                ) tdtm ON 
                    tdt.CD_ENDERECO = tdtm.CD_ENDERECO AND
                    tdt.CD_EMPRESA = tdtm.CD_EMPRESA AND
                    tdt.CD_PRODUTO = tdtm.CD_PRODUTO AND
                    tdt.CD_FAIXA = tdtm.CD_FAIXA AND
                    tdt.NU_IDENTIFICADOR = tdtm.NU_IDENTIFICADOR";
            var set = @"DT_FABRICACAO = DT_FABRICACAO_PROD";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void ControlProductosTraspasoEmpresa(IUnitOfWork uow, TraspasoBulkOperationContext contextBulk, TraspasoEmpresas traspaso, bool isEmpresaDocumentalEmpresaOrigen, DbConnection connection, DbTransaction tran, List<TraspasoEmpresasDetalleTemp> productos)
        {
            var producto = AnyProductoInexistente(connection, tran);

            if (!traspaso.ReplicaProductos && producto != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_04", new string[] { producto.ProductoDest, producto.EmpresaDest.ToString() });
            }

            producto = productos.FirstOrDefault(x => x.ManejoIdentificadorDest != x.ManejoIdentificador ||
                x.ManejoFecha != x.ManejoFechaDest || x.AceptaDecimales != x.AceptaDecimalesDest);

            if (traspaso.ControlaCaractIguales && producto != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_13", new string[] { producto.Producto, producto.Empresa.ToString(), producto.ProductoDest, producto.EmpresaDest.ToString() });
            }

            producto = productos.FirstOrDefault(x => x.AceptaDecimalesDest == "N" && x.CantidadDestino != (int)x.CantidadDestino);

            if (producto != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_05", new string[] { producto.ProductoDest, producto.EmpresaDest.ToString() });
            }

            producto = productos.GroupBy(x => new { x.ProductoDest, x.IdentificadorDest, x.Ubicacion, x.ManejoIdentificadorDest })
                .Select(x => new TraspasoEmpresasDetalleTemp
                {
                    ProductoDest = x.Key.ProductoDest,
                    IdentificadorDest = x.Key.IdentificadorDest,
                    Ubicacion = x.Key.Ubicacion,
                    ManejoIdentificadorDest = x.Key.ManejoIdentificadorDest,
                    CantidadDestino = x.Sum(d => d.CantidadDestino)
                })
                .FirstOrDefault(x => x.ManejoIdentificadorDest == ManejoIdentificadorDb.Serie && x.CantidadDestino != 1);

            if (producto != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_06", new string[] { producto.ProductoDest, traspaso.EmpresaDestino.ToString() });
            }

            producto = AnyProductoSerieExistenteStock(connection, tran);

            if (producto != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_07", new string[] { producto.ProductoDest, traspaso.EmpresaDestino.ToString(), producto.ManejoIdentificadorDest });
            }

            producto = AnySuficienteStockParaTransferir(connection, tran);

            if (producto != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_08", new string[] { producto.Producto, producto.Empresa.ToString(), producto.Identificador, producto.Ubicacion, });
            }

            producto = AnySuficienteStockSueltoParaTransferir(connection, tran);

            if (producto != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_15", new string[] { producto.Producto, producto.Empresa.ToString(), producto.Identificador, producto.Ubicacion, });
            }

            producto = AnyTransferirProductoDestinoNoAsociadoUbicacionPicking(connection, tran);

            if (producto != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_14", new string[] { producto.Ubicacion, producto.Producto, producto.Empresa.ToString() });
            }

            if (isEmpresaDocumentalEmpresaOrigen)
            {
                uow.DocumentoRepository.AddPreparacionReservaTemp(connection, tran, contextBulk.DetallePreparacionReservaDocumental);

                producto = AnySuficienteReservaDocumental(connection, tran);

                if (producto != null)
                {
                    throw new ValidationFailedException("PRE052_Sec0_Error_09", new string[] { producto.Producto, producto.Empresa.ToString(), producto.Identificador });
                }

            }
        }

        public virtual TraspasoEmpresasDetalleTemp AnyProductoInexistente(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            st.CD_EMPRESA_DEST as EmpresaDest,
                            st.CD_PRODUTO_DEST as ProductoDest
                        FROM
                            T_TRASPASO_DET_TEMP st
                        LEFT JOIN T_PRODUTO s on 
                             s.CD_EMPRESA = st.CD_EMPRESA_DEST AND
                             s.CD_PRODUTO = st.CD_PRODUTO_DEST 
                        WHERE s.CD_PRODUTO is null
            ";

            return _dapper.Query<TraspasoEmpresasDetalleTemp>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual TraspasoEmpresasDetalleTemp AnyTransferirProductoDestinoNoAsociadoUbicacionPicking(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                                st.CD_ENDERECO as Ubicacion,
                                st.CD_EMPRESA as Empresa,
                                st.CD_PRODUTO as Producto
                            FROM
                                T_TRASPASO_DET_TEMP st
                            INNER JOIN T_ENDERECO_ESTOQUE ee on ee.CD_ENDERECO = st.CD_ENDERECO
                            INNER JOIN T_TIPO_AREA ta on ta.CD_AREA_ARMAZ = ee.CD_AREA_ARMAZ
                            LEFT JOIN (
                                        SELECT 
                                            dp.CD_ENDERECO_SEPARACAO CD_ENDERECO,
                                            dp.CD_PRODUTO,
                                            dp.CD_EMPRESA
                                        FROM T_PICKING_PRODUTO dp
                            ) s on s.CD_ENDERECO = st.CD_ENDERECO AND
                                 s.CD_EMPRESA = st.CD_EMPRESA AND
                                 s.CD_PRODUTO = st.CD_PRODUTO
                            WHERE ta.ID_AREA_PICKING = 'S' AND
                                s.CD_PRODUTO is null";

            return _dapper.Query<TraspasoEmpresasDetalleTemp>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual void ControlPedidoTraspasoEmpresa(DbConnection connection, DbTransaction tran)
        {
            var pedidoExistente = AnyPedidoExistenteDestino(connection, tran);

            if (pedidoExistente != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_12", new string[] { pedidoExistente.Pedido, pedidoExistente.Cliente, pedidoExistente.EmpresaDest.ToString() });
            }
        }

        public virtual void ControlAgenteTraspasoEmpresa(IUnitOfWork uow, TraspasoEmpresas traspaso, DbConnection connection, DbTransaction tran)
        {
            Agente agenteInexistente = uow.TraspasoEmpresasRepository.GetAgenteNotExistingByTraspaso(connection, tran);

            if (!traspaso.ReplicaAgentes && agenteInexistente != null)
            {
                throw new ValidationFailedException("PRE052_Sec0_Error_10", new string[] { agenteInexistente.Codigo, agenteInexistente.Empresa.ToString() });
            }
        }

        public virtual void ProcesarPedidoTraspasoEmpresaTemp(IUnitOfWork uow, DbConnection connection, DbTransaction tran, List<TraspasoEmpresasDetalleTemp> productos, List<DetallePreparacionLpn> detallePreparacionLpn, bool isTraspasoPedidoPendiente, out List<Pedido> pedidosNew)
        {
            var pedidos = productos.GroupBy(x => new { x.Pedido, x.Cliente, x.Empresa, x.EmpresaDest, x.ClienteDest, x.PedidoDest })
                .Select(x => new TraspasoEmpresasDetalleTemp()
                {
                    Pedido = x.Key.Pedido,
                    Cliente = x.Key.Cliente,
                    Empresa = x.Key.Empresa,
                    ClienteDest = x.Key.ClienteDest,
                    EmpresaDest = x.Key.EmpresaDest,
                    PedidoDest = x.Key.PedidoDest
                }).ToList();

            List<long> nrosPedidos = new List<long>();
            if (pedidos.Where(x => x.PedidoDest == null).Count() > 0)
                nrosPedidos = uow.PedidoRepository.GetNewIdPedido(pedidos.Where(x => x.PedidoDest == null).Count());

            foreach (var pedido in pedidos)
            {
                if (pedido.PedidoDest == null)
                {
                    var id = nrosPedidos.FirstOrDefault();
                    pedido.PedidoDest = id.ToString();
                    nrosPedidos.Remove(id);
                }
            }

            UpdatePedidoDestinoTraspasoTemp(connection, tran, pedidos);

            if (isTraspasoPedidoPendiente)
            {
                foreach (var det in detallePreparacionLpn)
                {
                    var pedido = pedidos.FirstOrDefault(x => x.Pedido == det.Pedido && x.Cliente == det.Cliente && x.Empresa == det.Empresa);
                    det.ClienteDestino = pedido.ClienteDest;
                    det.EmpresaDestino = pedido.EmpresaDest;
                    det.PedidoDestino = pedido.PedidoDest;
                }
            }

            pedidosNew = pedidos.Select(x => new Pedido() { Id = x.PedidoDest, Cliente = x.ClienteDest, Empresa = x.EmpresaDest }).ToList();
        }

        public virtual void GenerarTraspasoEmpresaLpnTemp(IUnitOfWork uow, TraspasoBulkOperationContext contextBulk, TraspasoEmpresas traspaso, int empresaDest, DbConnection connection, DbTransaction tran, List<TraspasoEmpresasDetalleTemp> productos, List<MapeoProducto> productosMapeoTraspaso)
        {
            var lpns = contextBulk.DetallePreparacionLpn.GroupBy(x => new { x.NroLpn }).Select(x => new DetallePreparacionLpn() { NroLpn = x.Key.NroLpn });
            var idsLpn = uow.ManejoLpnRepository.GetNewNroLpn(lpns.Count());

            foreach (var lpn in lpns)
            {
                var id = idsLpn.FirstOrDefault();
                foreach (var det in contextBulk.DetallePreparacionLpn.Where(x => x.NroLpn == lpn.NroLpn))
                {
                    det.NumeroLPNDestino = id;
                }
                idsLpn.Remove(id);
            }

            var idsLpns = contextBulk.DetallePreparacionLpn.GroupBy(x => new { x.IdDetalleLpn }).Select(x => new DetallePreparacionLpn() { IdDetalleLpn = x.Key.IdDetalleLpn });
            var idsDetalleLpn = uow.ManejoLpnRepository.GetNewIdsDetalleLpn(idsLpns.Count());

            foreach (var idLpn in idsLpns)
            {
                foreach (var det in contextBulk.DetallePreparacionLpn.Where(x => x.IdDetalleLpn == idLpn.IdDetalleLpn))
                {
                    var id = idsDetalleLpn.FirstOrDefault();
                    det.IdDetalleDestino = id;
                    det.EmpresaDestino = empresaDest;

                    var productoMapeoTraspaso = productosMapeoTraspaso.FirstOrDefault(x => x.ProductoOrigen == det.Producto &&
                        x.EmpresaOrigen == det.Empresa);
                    var productoTraspaso = productos.FirstOrDefault(x => x.Producto == det.Producto &&
                        x.Empresa == det.Empresa &&
                        x.Identificador == det.Lote);

                    det.CodigoProductoDestino = productoMapeoTraspaso == null ? det.Producto : productoMapeoTraspaso.ProductoDestino;
                    det.IdentificadorDestino = productoTraspaso.IdentificadorDest;
                    det.CantidadDestino = productoMapeoTraspaso == null ? det.Cantidad : ((det.Cantidad / productoMapeoTraspaso.CantidadOrigen) * productoMapeoTraspaso.CantidadDestino);
                    det.VencimientoDestino = (productoTraspaso.ManejoFechaDest == ManejoFechaProductoDb.Expirable && productoTraspaso.ManejoFecha == ManejoFechaProductoDb.Expirable)
                        ? det.Vencimiento : productoTraspaso.VencimientoDest;
                    idsDetalleLpn.Remove(id);


                    if (productoTraspaso.AceptaDecimalesDest == "N" && det.CantidadDestino != (int)det.CantidadDestino)
                    {
                        throw new ValidationFailedException("PRE052_Sec0_Error_05", new string[] { productoTraspaso.ProductoDest, productoTraspaso.EmpresaDest.ToString() });
                    }

                    if (productoTraspaso.ManejoIdentificadorDest == ManejoIdentificadorDb.Serie && det.CantidadDestino != 1)
                    {
                        throw new ValidationFailedException("PRE052_Sec0_Error_06", new string[] { productoTraspaso.ProductoDest, productoTraspaso.Empresa.ToString() });
                    }

                }
            }

            AddDetalleTraspasoLpnTemporal(connection, tran, contextBulk.DetallePreparacionLpn, traspaso, empresaDest);
        }

        public virtual string GetSerializadoAtributos(IUnitOfWork uow, DetallePreparacionLpn detalle)
        {
            var formaterDateTime = uow.ParametroRepository.GetParameter(ParamManager.DATETIME_FORMAT_DATE_SECONDS);
            var separador = uow.ParametroRepository.GetParameter(ParamManager.NUMBER_DECIMAL_SEPARATOR);

            var atributos = uow.AtributoRepository.GetAtributos();

            var serializado = new DatosAjusteStockLpnAtributos(detalle.NroLpn ?? -1, detalle.IdExternoLpn, detalle.TipoLpn);

            var atributosCabezal = uow.ManejoLpnRepository.GetAllLpnAtributo(detalle.NroLpn ?? -1)
                .Select(a => new AtributoValor()
                {
                    Nombre = a.Nombre,
                    Valor = AtributoHelper.GetValorDisplayByIdTipo(a.Valor, atributos.FirstOrDefault(at => at.Id == a.Id), separador, formaterDateTime, true)

                })
                .ToList();

            serializado.AtributosCabezal = atributosCabezal;

            var atributosDetalleLpn = uow.ManejoLpnRepository.GetAllLpnDetalleAtributo(detalle.NroLpn ?? -1, detalle.IdDetalleLpn ?? -1, detalle.Empresa ?? -1, detalle.Producto, detalle.Lote, detalle.Faixa ?? 1);

            var atributosDetalle = new List<AtributoValor>();
            if (atributosDetalleLpn != null && atributosDetalleLpn.Count > 0)
            {
                atributosDetalle = atributosDetalleLpn
               .Select(a => new AtributoValor()
               {
                   Nombre = atributos.FirstOrDefault(at => at.Id == a.IdAtributo)?.Nombre,
                   Valor = AtributoHelper.GetValorDisplayByIdTipo(a.ValorAtributo, atributos.FirstOrDefault(at => at.Id == a.IdAtributo), separador, formaterDateTime, true)
               })
               .ToList();
            }
            else if (detalle != null)
            {
                atributosDetalle = uow.ManejoLpnRepository.GetAllLpnDetalleAtributo(detalle.NroLpn ?? -1, detalle.IdDetalleLpn ?? -1, detalle.Empresa ?? -1, detalle.Producto, detalle.Lote, detalle.Faixa ?? 1)

                .Select(a => new AtributoValor()
                {
                    Nombre = a.NombreAtributo,
                    Valor = AtributoHelper.GetValorDisplayByIdTipo(a.ValorAtributo, atributos.FirstOrDefault(at => at.Id == a.IdAtributo), separador, formaterDateTime, true)
                })
                .ToList();
            }

            serializado.AtributosDetalle = atributosDetalle;

            return JsonConvert.SerializeObject(serializado, new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml
            });
        }

        public virtual void AddDetalleTraspasoLpnTemporal(DbConnection connection, DbTransaction tran, List<DetallePreparacionLpn> detallePreparacion, TraspasoEmpresas traspaso, int empresaDest)
        {
            _dapper.BulkInsert(connection, tran, detallePreparacion, "T_TRASPASO_DET_LPN_TEMP", new Dictionary<string, Func<DetallePreparacionLpn, ColumnInfo>>
            {
                { "NU_TRASPASO", x => new ColumnInfo(traspaso.Id)},
                { "NU_LPN", x => new ColumnInfo(x.NroLpn)},
                { "ID_LPN_DET", x => new ColumnInfo(x.IdDetalleLpn)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
                { "QT_ESTOQUE", x => new ColumnInfo(x.Cantidad)},

                { "ID_DET_PICKING_LPN", x => new ColumnInfo(x.IdDetallePickingLpn, DbType.Int64)},
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido,DbType.String)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente,DbType.String)},
                { "NU_SEQ_PREPARACION", x => new ColumnInfo(x.NumeroSecuencia, DbType.Int32)},
                { "ID_ESPECIFICA_IDENTIFICADOR", x => new ColumnInfo(x.EspecificaLote,DbType.String)},
                { "ID_AGRUPACION", x => new ColumnInfo(x.Agrupacion,DbType.String)},
                { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion,DbType.String)},

                { "DT_FABRICACAO", x => new ColumnInfo(x.Vencimiento,DbType.DateTime)},
                { "NU_LPN_DEST", x => new ColumnInfo(x.NumeroLPNDestino,DbType.Int64)},
                { "ID_LPN_DET_DEST", x => new ColumnInfo(x.IdDetalleDestino,DbType.Int32)},
                { "CD_EMPRESA_DEST", x => new ColumnInfo(empresaDest)},
                { "CD_PRODUTO_DEST", x => new ColumnInfo(x.CodigoProductoDestino,DbType.String)},
                { "NU_IDENTIFICADOR_DEST", x => new ColumnInfo(x.IdentificadorDestino,DbType.String)},
                { "DT_FABRICACAO_DEST", x => new ColumnInfo(x.VencimientoDestino,DbType.DateTime)},
                { "QT_ESTOQUE_DEST", x => new ColumnInfo(x.CantidadDestino,DbType.Decimal)},

                { "ID_DET_PICKING_LPN_DEST", x => new ColumnInfo(x.IdDetallePickingLpnDest, DbType.Int64)},
                { "NU_PEDIDO_DEST", x => new ColumnInfo(x.PedidoDestino, DbType.String)},
                { "CD_CLIENTE_DEST", x => new ColumnInfo(x.ClienteDestino,DbType.String)},
                { "VL_ATRIBUTOS_LPN" , x => new ColumnInfo(x.Atributos,DbType.String)}
            });
        }

        public virtual List<MapeoProducto> GetProductoTraspasoMapeoTemp(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                             temp.CD_EMPRESA EmpresaOrigen,
                             temp.CD_PRODUTO ProductoOrigen,
                             QT_ORIGEN CantidadOrigen,
                             QT_DESTINO CantidadDestino,
                             CD_PRODUTO_DESTINO ProductoDestino
                        FROM
                            T_TRASPASO_DET_TEMP temp
                        INNER JOIN T_TRASPASO_MAPEO_PRODUTO mp ON mp.CD_PRODUTO = temp.CD_PRODUTO AND 
                        mp.CD_EMPRESA = temp.CD_EMPRESA
            ";

            return _dapper.Query<MapeoProducto>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual void UpdateLpnTraspasoTemp(DbConnection connection, DbTransaction tran, IEnumerable<TraspasoEmpresasDetalleTemp> lpns)
        {
            _dapper.BulkUpdate(connection, tran, lpns, "T_TRASPASO_DET_TEMP", new Dictionary<string, Func<TraspasoEmpresasDetalleTemp, ColumnInfo>>
        {
            { "NU_LPN_DEST", x => new ColumnInfo(x.NroLpnDest)},
        }, new Dictionary<string, Func<TraspasoEmpresasDetalleTemp, ColumnInfo>>
        {
            { "NU_LPN", x => new ColumnInfo(x.NroLpn)}
        });
        }

        public virtual void UpdateIdsDetalleLpnTraspasoTemp(DbConnection connection, DbTransaction tran, IEnumerable<TraspasoEmpresasDetalleTemp> lpns)
        {
            _dapper.BulkUpdate(connection, tran, lpns, "T_TRASPASO_DET_TEMP", new Dictionary<string, Func<TraspasoEmpresasDetalleTemp, ColumnInfo>>
        {
            { "ID_LPN_DET_DEST", x => new ColumnInfo(x.IdLpnDetDest)},
        }, new Dictionary<string, Func<TraspasoEmpresasDetalleTemp, ColumnInfo>>
        {
            { "ID_LPN_DET", x => new ColumnInfo(x.IdLpnDet)}
        });
        }

        public virtual TraspasoEmpresasDetalleTemp AnyPedidoExistenteDestino(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                                st.NU_PEDIDO_DEST as Pedido,
                                st.CD_CLIENTE_DEST as Cliente,
                                st.CD_EMPRESA_DEST as EmpresaDest
                            FROM
                                T_TRASPASO_DET_TEMP st
                            INNER JOIN T_PEDIDO_SAIDA ps on ps.NU_PEDIDO = st.NU_PEDIDO_DEST AND
                                 ps.CD_CLIENTE = st.CD_CLIENTE_DEST AND
                                 ps.CD_EMPRESA = st.CD_EMPRESA_DEST";

            return _dapper.Query<TraspasoEmpresasDetalleTemp>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual void UpdatePedidoDestinoTraspasoTemp(DbConnection connection, DbTransaction tran, IEnumerable<TraspasoEmpresasDetalleTemp> pedidos)
        {
            _dapper.BulkUpdate(connection, tran, pedidos, "T_TRASPASO_DET_TEMP", new Dictionary<string, Func<TraspasoEmpresasDetalleTemp, ColumnInfo>>
            {
                { "NU_PEDIDO_DEST", x => new ColumnInfo(x.PedidoDest,DbType.String)},
            }, new Dictionary<string, Func<TraspasoEmpresasDetalleTemp, ColumnInfo>>
            {
                { "NU_PEDIDO", x => new ColumnInfo(x.Pedido)},
                { "CD_CLIENTE", x => new ColumnInfo(x.Cliente)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)}
            });
        }

        public virtual Agente GetAgenteNotExistingByTraspaso(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            tdt.CD_CLIENTE_DEST as Cliente,
                            tdt.CD_EMPRESA_DEST as Empresa
                        FROM
                            T_TRASPASO_DET_TEMP tdt
                        LEFT JOIN T_CLIENTE c on c.CD_CLIENTE = tdt.CD_CLIENTE_DEST AND
                             c.CD_EMPRESA = tdt.CD_EMPRESA_DEST 
                        WHERE c.CD_CLIENTE is null
                        AND tdt.CD_CLIENTE_DEST is not null
            ";

            return _dapper.Query<Agente>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual TraspasoEmpresasDetalleTemp AnySuficienteStockParaTransferir(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            tdt.CD_ENDERECO as Ubicacion,
                            tdt.CD_EMPRESA as Empresa,
                            tdt.CD_PRODUTO as Producto,
                            tdt.CD_FAIXA as Faixa,
                            tdt.NU_IDENTIFICADOR as Identificador
                        FROM (
                                SELECT 
                                    tdt.CD_ENDERECO,
                                    tdt.CD_EMPRESA,
                                    tdt.CD_PRODUTO,
                                    tdt.CD_FAIXA,
                                    tdt.NU_IDENTIFICADOR,
                                    SUM(tdt.QT_ESTOQUE) QT_ESTOQUE
                                FROM T_TRASPASO_DET_TEMP tdt 
                                GROUP BY 
                                    tdt.CD_ENDERECO,
                                    tdt.CD_EMPRESA,
                                    tdt.CD_PRODUTO,
                                    tdt.CD_FAIXA,
                                    tdt.NU_IDENTIFICADOR
                             ) tdt
                        LEFT JOIN T_STOCK s on s.CD_ENDERECO = tdt.CD_ENDERECO AND
                             s.CD_EMPRESA = tdt.CD_EMPRESA AND
                             s.CD_PRODUTO = tdt.CD_PRODUTO AND
                             s.CD_FAIXA = tdt.CD_FAIXA AND
                             s.NU_IDENTIFICADOR = tdt.NU_IDENTIFICADOR
                        WHERE (tdt.QT_ESTOQUE > s.QT_ESTOQUE OR tdt.QT_ESTOQUE > s.QT_RESERVA_SAIDA) OR  s.CD_ENDERECO is null
            ";

            return _dapper.Query<TraspasoEmpresasDetalleTemp>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual TraspasoEmpresasDetalleTemp AnySuficienteStockSueltoParaTransferir(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            tdt.CD_ENDERECO as Ubicacion,
                            tdt.CD_EMPRESA as Empresa,
                            tdt.CD_PRODUTO as Producto,
                            tdt.CD_FAIXA as Faixa,
                            tdt.NU_IDENTIFICADOR as Identificador
                        FROM (
                                SELECT 
                                    tdt.CD_ENDERECO,
                                    tdt.CD_EMPRESA,
                                    tdt.CD_PRODUTO,
                                    tdt.CD_FAIXA,
                                    tdt.NU_IDENTIFICADOR,
                                    SUM(tdt.QT_ESTOQUE) QT_ESTOQUE
                                FROM T_TRASPASO_DET_TEMP tdt 
                                WHERE tdt.ID_DET_PICKING_LPN is null AND tdt.NU_LPN is null
                                GROUP BY 
                                    tdt.CD_ENDERECO,
                                    tdt.CD_EMPRESA,
                                    tdt.CD_PRODUTO,
                                    tdt.CD_FAIXA,
                                    tdt.NU_IDENTIFICADOR
                             ) tdt
                        LEFT JOIN V_STOCK_SUELTO s on s.CD_ENDERECO = tdt.CD_ENDERECO AND
                             s.CD_EMPRESA = tdt.CD_EMPRESA AND
                             s.CD_PRODUTO = tdt.CD_PRODUTO AND
                             s.CD_FAIXA = tdt.CD_FAIXA AND
                             s.NU_IDENTIFICADOR = tdt.NU_IDENTIFICADOR
                        WHERE tdt.QT_ESTOQUE > s.QT_ESTOQUE OR  s.CD_ENDERECO is null
            ";

            return _dapper.Query<TraspasoEmpresasDetalleTemp>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual TraspasoEmpresasDetalleTemp AnyProductoSerieExistenteStock(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                                tmp.CD_EMPRESA_DEST EmpresaDest,
                                tmp.CD_PRODUTO_DEST ProductoDest,
                                tmp.NU_IDENTIFICADOR_DEST ManejoIdentificadorDest
                            FROM
                                T_TRASPASO_DET_TEMP tmp
                                INNER JOIN T_STOCK s on s.CD_ENDERECO = tmp.CD_ENDERECO AND
                                    s.CD_PRODUTO = tmp.CD_PRODUTO_DEST AND 
                                    s.CD_FAIXA= tmp.CD_FAIXA AND 
                                    s.NU_IDENTIFICADOR = tmp.NU_IDENTIFICADOR_DEST AND 
                                    s.CD_EMPRESA = tmp.CD_EMPRESA_DEST
                                WHERE tmp.ID_MANEJO_IDENTIFICADOR_DEST = 'S'
            ";

            return _dapper.Query<TraspasoEmpresasDetalleTemp>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual List<TraspasoEmpresasDetalleTemp> GetProductoTraspasoTemp(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                                NU_TRASPASO NroTraspaso,
                                CD_ENDERECO Ubicacion,
                                CD_EMPRESA Empresa,
                                CD_PRODUTO Producto,
                                CD_FAIXA Faixa,
                                NU_IDENTIFICADOR Identificador,
                                DT_FABRICACAO Vencimiento,
                                QT_ESTOQUE Cantidad,
                                ID_MANEJO_IDENTIFICADOR ManejoIdentificador,
                                TP_MANEJO_FECHA ManejoFecha,
                                FL_ACEPTA_DECIMALES AceptaDecimales,
                                CD_EMPRESA_DEST EmpresaDest,
                                NU_IDENTIFICADOR_DEST IdentificadorDest,
                                DT_FABRICACAO_DEST VencimientoDest,
                                CD_PRODUTO_DEST ProductoDest,
                                QT_ESTOQUE_DEST CantidadDestino,
                                CD_ENDERECO_PICKING UbicacionPicking,
                                ID_MANEJO_IDENTIFICADOR_DEST ManejoIdentificadorDest,
                                TP_MANEJO_FECHA_DEST ManejoFechaDest,
                                FL_ACEPTA_DECIMALES_DEST AceptaDecimalesDest,
                                NU_PEDIDO_DEST PedidoDest,
                                CD_CLIENTE_DEST ClienteDest,
                                NU_PEDIDO Pedido,
                                CD_CLIENTE Cliente,
                                NU_LPN NroLpn,
                                ID_DET_PICKING_LPN IdDetallePickingLpn
                            FROM
                                T_TRASPASO_DET_TEMP 
            ";

            return _dapper.Query<TraspasoEmpresasDetalleTemp>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();

        }

        public virtual void UpdateProdutoDestinoTraspasoTemp(DbConnection connection, DbTransaction tran)
        {
            var alias = "tdt";
            var from = $@"
                T_TRASPASO_DET_TEMP tdt
                INNER JOIN (
                         SELECT 
                              td.NU_TRASPASO,
                              td.CD_ENDERECO,
                              td.CD_EMPRESA,
                              td.CD_PRODUTO,
                              td.CD_FAIXA,
                              td.NU_IDENTIFICADOR,
                              td.NU_SEQ_PREPARACION,
                              td.NU_PEDIDO,
                              td.CD_CLIENTE,
                              MIN(COALESCE(tmp.CD_PRODUTO_DESTINO, td.CD_PRODUTO)) CD_PRODUTO_PROD,
                              SUM((td.QT_ESTOQUE / COALESCE(tmp.QT_ORIGEN,td.QT_ESTOQUE)) * COALESCE(tmp.QT_DESTINO,td.QT_ESTOQUE)) QT_TRANFERENCIA
                         FROM  T_TRASPASO_DET_TEMP  td
                         LEFT JOIN T_TRASPASO_MAPEO_PRODUTO tmp on 
                           tmp.CD_PRODUTO = td.CD_PRODUTO AND
                           tmp.CD_EMPRESA = td.CD_EMPRESA AND
                           tmp.CD_EMPRESA_DESTINO = td.CD_EMPRESA_DEST 
                         GROUP BY td.NU_TRASPASO,
                              td.CD_ENDERECO,
                              td.CD_EMPRESA,
                              td.CD_PRODUTO,
                              td.CD_FAIXA,
                              td.NU_IDENTIFICADOR,
                              td.NU_SEQ_PREPARACION,
                              td.NU_PEDIDO,
                              td.CD_CLIENTE
                ) tdtm ON 
                    tdt.NU_TRASPASO = tdtm.NU_TRASPASO AND
                    tdt.CD_ENDERECO = tdtm.CD_ENDERECO AND
                    tdt.CD_EMPRESA = tdtm.CD_EMPRESA AND
                    tdt.CD_PRODUTO = tdtm.CD_PRODUTO AND
                    tdt.CD_FAIXA = tdtm.CD_FAIXA AND
                    tdt.NU_IDENTIFICADOR = tdtm.NU_IDENTIFICADOR AND
                    tdt.NU_SEQ_PREPARACION = tdtm.NU_SEQ_PREPARACION AND
                    tdt.NU_PEDIDO = tdtm.NU_PEDIDO AND
                    tdt.CD_CLIENTE = tdtm.CD_CLIENTE";
            var set = @"
                CD_PRODUTO_DEST = CD_PRODUTO_PROD,
                QT_ESTOQUE_DEST = QT_TRANFERENCIA";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateProdutoEstadoAveriaTraspasoTemp(DbConnection connection, DbTransaction tran)
        {
            var alias = "tdt";
            var from = $@"
                T_TRASPASO_DET_TEMP tdt
                INNER JOIN (
                        SELECT 
                             st.CD_ENDERECO,
                             st.CD_EMPRESA,
                             st.CD_PRODUTO,
                             st.CD_FAIXA,
                             st.NU_IDENTIFICADOR,
                             MIN(s.ID_AVERIA) ID_AVERIA_DEST
                        FROM  T_TRASPASO_DET_TEMP  st
                        LEFT JOIN T_STOCK s on 
                            s.CD_ENDERECO = st.CD_ENDERECO AND 
                            s.CD_EMPRESA = st.CD_EMPRESA AND 
                            s.CD_PRODUTO = st.CD_PRODUTO AND
                            s.CD_FAIXA = st.CD_FAIXA AND
                            s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR 
                        GROUP by 
                           st.CD_ENDERECO,
                             st.CD_EMPRESA,
                             st.CD_PRODUTO,
                             st.CD_FAIXA,
                             st.NU_IDENTIFICADOR
                ) st ON 
                    tdt.CD_ENDERECO = st.CD_ENDERECO AND 
                    tdt.CD_EMPRESA = st.CD_EMPRESA AND 
                    tdt.CD_PRODUTO = st.CD_PRODUTO AND
                    tdt.CD_FAIXA = st.CD_FAIXA AND
                    tdt.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR ";
            var set = @"
                ID_AVERIA = ID_AVERIA_DEST";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateDatosProdutoTraspasoTemp(DbConnection connection, DbTransaction tran)
        {
            var alias = "tdt";
            var from = $@"
                T_TRASPASO_DET_TEMP tdt
                INNER JOIN (
                        SELECT 
                            st.CD_EMPRESA,
                            st.CD_PRODUTO,
                            MIN(s.ID_MANEJO_IDENTIFICADOR) ID_MANEJO_IDENTIFICADOR_PROD,
                            MIN(s.TP_MANEJO_FECHA) TP_MANEJO_FECHA_PROD,
                            MIN(s.FL_ACEPTA_DECIMALES) FL_ACEPTA_DECIMALES_PROD
                        FROM  T_TRASPASO_DET_TEMP  st
                        LEFT JOIN T_PRODUTO s on 
                             s.CD_PRODUTO = st.CD_PRODUTO AND
                             s.CD_EMPRESA = st.CD_EMPRESA 
                        GROUP by 
                            st.CD_EMPRESA,
                            st.CD_PRODUTO
                ) st ON 
                    tdt.CD_EMPRESA = st.CD_EMPRESA AND
                    tdt.CD_PRODUTO = st.CD_PRODUTO";
            var set = @"
                ID_MANEJO_IDENTIFICADOR = ID_MANEJO_IDENTIFICADOR_PROD,
                TP_MANEJO_FECHA = TP_MANEJO_FECHA_PROD,
                FL_ACEPTA_DECIMALES = FL_ACEPTA_DECIMALES_PROD";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdatePedidoAndAgenteDestinoTraspasoTemp(DbConnection connection, DbTransaction tran, long traspaso)
        {
            var alias = "tdt";
            var from = $@"
                T_TRASPASO_DET_TEMP tdt
                INNER JOIN (
                    SELECT 
                        st.NU_PEDIDO,
                        st.CD_CLIENTE,
                        MIN(s.NU_PEDIDO_DESTINO) PEDIDO_DEST,
                        MIN(COALESCE(s.CD_CLIENTE_DESTINO ,  st.CD_CLIENTE)) CLIENTE_DEST,
                        MIN(s.TP_PEDIDO_DESTINO) TP_PEDIDO,
                        MIN(s.TP_EXPEDICION_DESTINO) TP_EXPEDICION
                    FROM  T_TRASPASO_DET_TEMP  st
                    INNER JOIN T_TRASPASO_DET_PEDIDO s on 
                         s.NU_PEDIDO = st.NU_PEDIDO AND
                         s.CD_CLIENTE = st.CD_CLIENTE AND
                         s.NU_TRASPASO = :traspaso
                    GROUP by 
                        st.NU_PEDIDO,
                        st.CD_CLIENTE
                ) tdtmp ON 
                    tdt.NU_PEDIDO = tdtmp.NU_PEDIDO AND
                    tdt.CD_CLIENTE = tdtmp.CD_CLIENTE";
            var set = @"
                NU_PEDIDO_DEST = PEDIDO_DEST,
                CD_CLIENTE_DEST = CLIENTE_DEST,
                TP_PEDIDO_DEST = TP_PEDIDO,
                TP_EXPEDICION_DEST = TP_EXPEDICION";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { traspaso = traspaso }, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateDatosProdutoDestinoTraspasoTemp(DbConnection connection, DbTransaction tran)
        {
            var alias = "tdt";
            var from = $@"
                T_TRASPASO_DET_TEMP tdt
                INNER JOIN (
                        SELECT 
                            st.CD_EMPRESA_DEST,
                            st.CD_PRODUTO_DEST,
                            MIN(COALESCE(s.ID_MANEJO_IDENTIFICADOR,st.ID_MANEJO_IDENTIFICADOR)) ID_MANEJO_IDENTIFICADOR_PROD,
                            MIN(COALESCE(s.TP_MANEJO_FECHA,st.TP_MANEJO_FECHA)) TP_MANEJO_FECHA_PROD,
                            MIN(COALESCE(s.FL_ACEPTA_DECIMALES,st.FL_ACEPTA_DECIMALES)) FL_ACEPTA_DECIMALES_PROD
                        FROM  T_TRASPASO_DET_TEMP  st
                        LEFT JOIN T_PRODUTO s on 
                             s.CD_PRODUTO = st.CD_PRODUTO_DEST AND
                             s.CD_EMPRESA = st.CD_EMPRESA_DEST 
                        GROUP by 
                            st.CD_EMPRESA_DEST,
                            st.CD_PRODUTO_DEST
                ) st ON 
                    tdt.CD_EMPRESA_DEST = st.CD_EMPRESA_DEST AND
                    tdt.CD_PRODUTO_DEST = st.CD_PRODUTO_DEST";
            var set = @"
                ID_MANEJO_IDENTIFICADOR_DEST = ID_MANEJO_IDENTIFICADOR_PROD,
                TP_MANEJO_FECHA_DEST = TP_MANEJO_FECHA_PROD,
                FL_ACEPTA_DECIMALES_DEST = FL_ACEPTA_DECIMALES_PROD";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateVencimientoLoteProdutoDestinoTraspasoTemp(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "tdt";
            var from = $@"
                T_TRASPASO_DET_TEMP tdt
                INNER JOIN (
                         SELECT 
                              td.NU_TRASPASO,
                              td.CD_ENDERECO,
                              td.CD_EMPRESA,
                              td.CD_PRODUTO,
                              td.CD_FAIXA,
                              td.NU_IDENTIFICADOR,
                              td.NU_SEQ_PREPARACION,
                              td.NU_PEDIDO,
                              td.CD_CLIENTE,
                              MIN(CASE WHEN td.ID_MANEJO_IDENTIFICADOR_DEST = 'P' THEN '*' WHEN NU_IDENTIFICADOR = '*' AND td.ID_MANEJO_IDENTIFICADOR_DEST != 'P' THEN :NU_IDENTIFICADOR ELSE td.NU_IDENTIFICADOR END) NU_IDENTIFICADOR_PROD,
                              MIN(CASE WHEN td.TP_MANEJO_FECHA_DEST = 'F' THEN :FechaActual WHEN td.TP_MANEJO_FECHA_DEST = 'E' AND td.DT_FABRICACAO is NULL THEN :FechaActual WHEN td.TP_MANEJO_FECHA_DEST = 'D' THEN NULL ELSE td.DT_FABRICACAO END) DT_FABRICACAO_PROD
                         FROM  T_TRASPASO_DET_TEMP  td
                         LEFT JOIN T_TRASPASO_MAPEO_PRODUTO tmp on 
                           tmp.CD_PRODUTO = td.CD_PRODUTO AND
                           tmp.CD_EMPRESA = td.CD_EMPRESA AND
                           tmp.CD_EMPRESA_DESTINO = td.CD_EMPRESA_DEST 
                         GROUP BY td.NU_TRASPASO,
                              td.CD_ENDERECO,
                              td.CD_EMPRESA,
                              td.CD_PRODUTO,
                              td.CD_FAIXA,
                              td.NU_IDENTIFICADOR,
                              td.NU_SEQ_PREPARACION,
                              td.NU_PEDIDO,
                              td.CD_CLIENTE
                ) tdtm ON 
                    tdt.NU_TRASPASO = tdtm.NU_TRASPASO AND
                    tdt.CD_ENDERECO = tdtm.CD_ENDERECO AND
                    tdt.CD_EMPRESA = tdtm.CD_EMPRESA AND
                    tdt.CD_PRODUTO = tdtm.CD_PRODUTO AND
                    tdt.CD_FAIXA = tdtm.CD_FAIXA AND
                    tdt.NU_IDENTIFICADOR = tdtm.NU_IDENTIFICADOR AND
                    tdt.NU_SEQ_PREPARACION = tdtm.NU_SEQ_PREPARACION AND
                    tdt.NU_PEDIDO = tdtm.NU_PEDIDO AND
                    tdt.CD_CLIENTE = tdtm.CD_CLIENTE";
            var set = @"
                NU_IDENTIFICADOR_DEST = NU_IDENTIFICADOR_PROD,
                DT_FABRICACAO_DEST = DT_FABRICACAO_PROD";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new DynamicParameters(new { FechaActual = DateTime.Now, NU_IDENTIFICADOR = nuTransaccion.ToString() }), commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void AddDetalleTraspasoSueltoTemporal(DbConnection connection, DbTransaction tran, List<TraspasoEmpresasDetalleTemp> detallePreparacion, TraspasoEmpresas traspaso, int empresaDest)
        {
            _dapper.BulkInsert(connection, tran, detallePreparacion, "T_TRASPASO_DET_TEMP", new Dictionary<string, Func<TraspasoEmpresasDetalleTemp, ColumnInfo>>
            {
                    { "NU_TRASPASO", x => new ColumnInfo(traspaso.Id)},
                    { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                    { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                    { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                    { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                    { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
                    { "DT_FABRICACAO", x => new ColumnInfo(x.Vencimiento,DbType.DateTime)},
                    { "QT_ESTOQUE", x => new ColumnInfo(x.Cantidad)},
                    { "CD_EMPRESA_DEST", x => new ColumnInfo(empresaDest)},
                    { "NU_PEDIDO" , x => new ColumnInfo(x.Pedido)},
                    { "CD_CLIENTE" , x => new ColumnInfo(x.Cliente)},
                    { "CD_ENDERECO_PICKING" , x => new ColumnInfo(x.UbicacionPicking)},
                    { "NU_SEQ_PREPARACION" , x => new ColumnInfo(x.NumeroSecuencia) },
                    { "ID_AGRUPACION" , x => new ColumnInfo(x.Agrupacion) },
                    { "NU_CONTENEDOR" , x => new ColumnInfo(x.NroContenedor,DbType.Int32) },
                    { "NU_CONTENEDOR_DEST" , x => new ColumnInfo(x.NroContenedorDestino,DbType.Int32) },
                    { "ID_DET_PICKING_LPN" , x => new ColumnInfo(x.IdDetallePickingLpn,DbType.Int64) },
                    { "NU_LPN" , x => new ColumnInfo(x.NroLpn,DbType.Int64) }
            });
        }

        public virtual TraspasoEmpresasDetalleTemp AnySuficienteReservaDocumental(DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            dprt.NU_DOCUMENTO as NroDocumento,
                            dprt.TP_DOCUMENTO as TipoDocumento,
                            dprt.CD_EMPRESA as Empresa,
                            dprt.CD_PRODUTO as Producto,
                            dprt.CD_FAIXA as Faixa,
                            dprt.NU_IDENTIFICADOR as Identificador
                        FROM
                            T_DOC_PREPARACION_RESERV_TEMP dprt
                        LEFT JOIN (
                                    SELECT 
                                        DD.NU_DOCUMENTO,
                                        DD.TP_DOCUMENTO,
                                        DD.CD_EMPRESA,
                                        DD.CD_PRODUTO,
                                        DD.NU_IDENTIFICADOR,
                                        DD.CD_FAIXA,
                                        SUM(COALESCE(DD.QT_RESERVADA,0)) QT_RESERVADA          
                                    FROM T_DET_DOCUMENTO  DD 
                                    GROUP BY DD.NU_DOCUMENTO,
                                        DD.TP_DOCUMENTO,
                                        DD.CD_EMPRESA,
                                        DD.CD_PRODUTO,
                                        DD.NU_IDENTIFICADOR, 
                                        DD.CD_FAIXA
                        ) D on D.NU_DOCUMENTO = dprt.NU_DOCUMENTO AND
                             D.TP_DOCUMENTO = dprt.TP_DOCUMENTO AND
                             D.CD_EMPRESA = dprt.CD_EMPRESA AND
                             D.CD_PRODUTO = dprt.CD_PRODUTO AND
                             D.CD_FAIXA = dprt.CD_FAIXA AND
                             D.NU_IDENTIFICADOR = dprt.NU_IDENTIFICADOR
                        WHERE dprt.QT_PRODUTO > D.QT_RESERVADA";

            return _dapper.Query<TraspasoEmpresasDetalleTemp>(connection, sql, commandType: CommandType.Text, transaction: tran).FirstOrDefault();
        }

        #region Baja Stock

        public virtual void ProcesarBajaStockTraspaso(IUnitOfWork uow, long nuTransaccion, TraspasoBulkOperationContext bulkContext, bool isTraspasoEmpresaDejarPreparado)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            UpdateBajaStock(connection, tran, nuTransaccion);

            UpdateBajaStockLpn(connection, tran, bulkContext.DetallePreparacionLpn, nuTransaccion, isTraspasoEmpresaDejarPreparado);

            UpdateEstadoLpn(connection, tran, nuTransaccion);

            uow.AjusteRepository.BulkInsertAjusteStock(connection, tran, bulkContext.AjustesStock.Where(x => x.QtMovimiento < 0).ToList());
        }

        public virtual void UpdateEstadoLpn(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "l";
            var from = $@"
                T_LPN l
                INNER JOIN (
                    SELECT 
                        tmp.NU_LPN
                    FROM  T_TRASPASO_DET_LPN_TEMP  tmp
                    LEFT JOIN T_LPN_DET ld on ld.NU_LPN = tmp.NU_LPN AND ld.QT_ESTOQUE > 0
                    WHERE ld.NU_LPN is null
                    GROUP BY tmp.NU_LPN
                ) tmp ON 
                     tmp.NU_LPN  = l.NU_LPN";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                ID_ESTADO = :Estado";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { Estado = EstadosLPN.Finalizado, FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void UpdateBajaStockLpn(DbConnection connection, DbTransaction tran, List<DetallePreparacionLpn> detallePreparacionLpn, long nuTransaccion, bool isTraspasoEmpresaDejarPreparado)
        {
            _dapper.BulkUpdate(connection, tran, detallePreparacionLpn, "T_LPN_DET", new Dictionary<string, Func<DetallePreparacionLpn, ColumnInfo>>
            {
                { "QT_RESERVA_SAIDA", x => new ColumnInfo(isTraspasoEmpresaDejarPreparado ? 0 :x.Cantidad,DbType.Decimal,OperacionDb.OperacionMenos)},
                { "QT_ESTOQUE", x => new ColumnInfo(x.Cantidad,OperacionDb.OperacionMenos)},
                { "NU_TRANSACCION", x => new ColumnInfo(nuTransaccion)},
            }, new Dictionary<string, Func<DetallePreparacionLpn, ColumnInfo>>
            {
                { "ID_LPN_DET", x => new ColumnInfo(x.IdDetalleLpn)},
                { "NU_LPN", x => new ColumnInfo(x.NroLpn)},
                { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
            });
        }

        public virtual void UpdateBajaStock(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "s";
            var from = $@"
                T_STOCK s
                INNER JOIN (
                    SELECT 

                        s.CD_ENDERECO,
                        s.CD_EMPRESA,
                        s.CD_PRODUTO,
                        s.CD_FAIXA,
                        s.NU_IDENTIFICADOR,
                        SUM(st.QT_ESTOQUE) QT_PRODUTO_TEMP
                    FROM  T_TRASPASO_DET_TEMP  st
                    INNER JOIN T_STOCK s on 
                         s.CD_ENDERECO = st.CD_ENDERECO AND
                         s.CD_PRODUTO = st.CD_PRODUTO AND
                         s.CD_FAIXA = st.CD_FAIXA AND
                         s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR AND
                         s.CD_EMPRESA = st.CD_EMPRESA 
                    GROUP by   s.CD_ENDERECO,
                        s.CD_EMPRESA,
                        s.CD_PRODUTO,
                        s.CD_FAIXA,
                        s.NU_IDENTIFICADOR
                ) st ON 
                     st.CD_ENDERECO  = s.CD_ENDERECO AND
                     st.CD_EMPRESA  = s.CD_EMPRESA AND
                     st.CD_PRODUTO  = s.CD_PRODUTO AND
                     st.CD_FAIXA  = s.CD_FAIXA AND
                     st.NU_IDENTIFICADOR  = s.NU_IDENTIFICADOR";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                QT_RESERVA_SAIDA = QT_RESERVA_SAIDA - QT_PRODUTO_TEMP,
                QT_ESTOQUE = QT_ESTOQUE - QT_PRODUTO_TEMP";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: tran);
        }

        #endregion

        #region Alta Stock

        public virtual void ProcesarAltaStockTraspaso(IUnitOfWork uow, long nuTransaccion, TraspasoBulkOperationContext bulkContext, bool isTraspasoEmpresaStockConReserva, bool propagarLpn, bool isTraspasoPreparacionPendiente)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            BulkUpdateStock(connection, tran, nuTransaccion, isTraspasoEmpresaStockConReserva);
            BulkInsertStock(connection, tran, nuTransaccion, isTraspasoEmpresaStockConReserva);
            uow.AjusteRepository.BulkInsertAjusteStock(connection, tran, bulkContext.AjustesStock.Where(x => x.QtMovimiento > 0).ToList());

            if (propagarLpn)
            {
                BulkInsertLpns(connection, tran, nuTransaccion);
                BulkInsertBarrasLpns(connection, tran, nuTransaccion);
                BulkInsertAtributosLpns(connection, tran, nuTransaccion);
                BulkInsertDetallesLpns(connection, tran, nuTransaccion, isTraspasoPreparacionPendiente);
                BulkInsertAtributosDetallesLpns(connection, tran, nuTransaccion);
            }
        }

        public virtual void BulkInsertLpns(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new
            {
                NU_TRANSACCION = nuTransaccion,
                DT_ADDROW = DateTime.Now,
                DT_ACTIVACION = DateTime.Now,
            };

            var sqlInsertNew = $@"
                INSERT INTO T_LPN (NU_LPN,ID_LPN_EXTERNO,TP_LPN_TIPO,ID_ESTADO,CD_ENDERECO,DT_ADDROW,
                     DT_ACTIVACION,NU_TRANSACCION,CD_EMPRESA,ID_PACKING,FL_DISPONIBLE_LIBERACION)
                SELECT d.NU_LPN_DEST,l.ID_LPN_EXTERNO,l.TP_LPN_TIPO,l.ID_ESTADO,l.CD_ENDERECO,:DT_ADDROW,
                     :DT_ACTIVACION,:NU_TRANSACCION,d.CD_EMPRESA_DEST,l.ID_PACKING,l.FL_DISPONIBLE_LIBERACION
                FROM  (SELECT NU_LPN_DEST,CD_EMPRESA_DEST,NU_LPN FROM T_TRASPASO_DET_LPN_TEMP GROUP BY  NU_LPN_DEST,CD_EMPRESA_DEST,NU_LPN) d
                INNER JOIN T_LPN l ON l.NU_LPN = d.NU_LPN
                WHERE d.NU_LPN is not null";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BulkInsertBarrasLpns(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new
            {
                NU_TRANSACCION = nuTransaccion,
                DT_ADDROW = DateTime.Now,
            };

            var sqlInsertNew = $@"
                INSERT INTO T_LPN_BARRAS (NU_LPN,CD_BARRAS,NU_ORDEN,TP_BARRAS)
                SELECT d.NU_LPN_DEST,CD_BARRAS,NU_ORDEN,TP_BARRAS
                FROM (SELECT NU_LPN_DEST,NU_LPN FROM T_TRASPASO_DET_LPN_TEMP GROUP BY  NU_LPN_DEST,NU_LPN) d
                INNER JOIN T_LPN_BARRAS l ON l.NU_LPN = d.NU_LPN
                WHERE d.NU_LPN is not null";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BulkInsertAtributosLpns(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new
            {
                NU_TRANSACCION = nuTransaccion,
            };

            var sqlInsertNew = $@"
                INSERT INTO T_LPN_ATRIBUTO (NU_LPN,TP_LPN_TIPO,ID_ATRIBUTO,VL_LPN_ATRIBUTO,
                     ID_ESTADO,NU_TRANSACCION)
                SELECT d.NU_LPN_DEST,l.TP_LPN_TIPO,l.ID_ATRIBUTO,l.VL_LPN_ATRIBUTO,l.ID_ESTADO,:NU_TRANSACCION
                FROM (SELECT NU_LPN_DEST,NU_LPN FROM T_TRASPASO_DET_LPN_TEMP GROUP BY  NU_LPN_DEST,NU_LPN) d
                INNER JOIN T_LPN_ATRIBUTO l ON l.NU_LPN = d.NU_LPN
                WHERE d.NU_LPN is not null";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BulkInsertDetallesLpns(DbConnection connection, DbTransaction tran, long nuTransaccion, bool isTraspasoPreparacionPendiente)
        {
            var param = new
            {
                NU_TRANSACCION = nuTransaccion,
            };

            var sqlInsertNew = $@"
                INSERT INTO T_LPN_DET (ID_LPN_DET,NU_LPN,CD_PRODUTO,CD_FAIXA,CD_EMPRESA,
                     NU_IDENTIFICADOR,NU_TRANSACCION,DT_FABRICACAO,QT_ESTOQUE,
                     QT_DECLARADA,QT_RECIBIDA,ID_LINEA_SISTEMA_EXTERNO,QT_RESERVA_SAIDA,
                     QT_EXPEDIDA,ID_AVERIA,ID_INVENTARIO,ID_CTRL_CALIDAD,CD_MOTIVO_AVERIA)
                SELECT d.ID_LPN_DET_DEST,d.NU_LPN_DEST,d.CD_PRODUTO_DEST,d.CD_FAIXA,d.CD_EMPRESA_DEST,
                     d.NU_IDENTIFICADOR_DEST,:NU_TRANSACCION,d.DT_FABRICACAO_DEST,d.QT_ESTOQUE_DEST,
                     0,0,l.ID_LINEA_SISTEMA_EXTERNO,{(isTraspasoPreparacionPendiente ? "d.QT_ESTOQUE_DEST" : "0")},
                     0,l.ID_AVERIA,l.ID_INVENTARIO,l.ID_CTRL_CALIDAD,l.CD_MOTIVO_AVERIA
                FROM (SELECT MIN(ID_LPN_DET_DEST) ID_LPN_DET_DEST,
                            NU_LPN,
                            NU_LPN_DEST,
                            MIN(ID_LPN_DET) ID_LPN_DET,
                            CD_EMPRESA,
                            CD_PRODUTO,
                            CD_FAIXA,
                            CD_PRODUTO_DEST,
                            CD_EMPRESA_DEST,
                            NU_IDENTIFICADOR_DEST,
                            MIN(DT_FABRICACAO_DEST) DT_FABRICACAO_DEST,
                            SUM(QT_ESTOQUE_DEST) QT_ESTOQUE_DEST,
                            VL_ATRIBUTOS_LPN
                        FROM T_TRASPASO_DET_LPN_TEMP 
                        GROUP BY  NU_LPN,
                            NU_LPN_DEST,
                            CD_EMPRESA,
                            CD_PRODUTO,
                            CD_FAIXA,
                            CD_PRODUTO_DEST,
                            CD_EMPRESA_DEST,
                            NU_IDENTIFICADOR_DEST,
                            VL_ATRIBUTOS_LPN) d
                INNER JOIN T_LPN_DET l ON l.NU_LPN = d.NU_LPN AND 
                 l.ID_LPN_DET = d.ID_LPN_DET AND
                 l.CD_EMPRESA = d.CD_EMPRESA AND
                 l.CD_PRODUTO = d.CD_PRODUTO AND
                 l.CD_FAIXA = d.CD_FAIXA
                WHERE d.ID_LPN_DET_DEST is not null";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BulkInsertAtributosDetallesLpns(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {

            var param = new
            {
                NU_TRANSACCION = nuTransaccion,
            };

            var sqlInsertNew = $@"
                INSERT INTO T_LPN_DET_ATRIBUTO (ID_LPN_DET,TP_LPN_TIPO,ID_ATRIBUTO,VL_LPN_DET_ATRIBUTO,
                     CD_PRODUTO,CD_FAIXA,CD_EMPRESA,NU_IDENTIFICADOR,NU_LPN,ID_ESTADO,NU_TRANSACCION)
                SELECT d.ID_LPN_DET_DEST,l.TP_LPN_TIPO,l.ID_ATRIBUTO,l.VL_LPN_DET_ATRIBUTO,
                     d.CD_PRODUTO_DEST,d.CD_FAIXA,d.CD_EMPRESA_DEST,d.NU_IDENTIFICADOR_DEST,d.NU_LPN_DEST,l.ID_ESTADO,:NU_TRANSACCION
                FROM (SELECT MIN(ID_LPN_DET_DEST) ID_LPN_DET_DEST,
                         NU_LPN,
                         NU_LPN_DEST,
                         MIN(ID_LPN_DET) ID_LPN_DET,
                         CD_EMPRESA,
                         CD_PRODUTO,
                         CD_FAIXA,
                         CD_PRODUTO_DEST,
                         CD_EMPRESA_DEST,
                         NU_IDENTIFICADOR_DEST,
                         VL_ATRIBUTOS_LPN
                       FROM T_TRASPASO_DET_LPN_TEMP 
                       GROUP BY  NU_LPN,
                          NU_LPN_DEST,
                          CD_EMPRESA,
                          CD_PRODUTO,
                          CD_FAIXA,
                          CD_PRODUTO_DEST,
                          CD_EMPRESA_DEST,
                          NU_IDENTIFICADOR_DEST,
                          VL_ATRIBUTOS_LPN ) d
                INNER JOIN T_LPN_DET_ATRIBUTO l ON l.NU_LPN = d.NU_LPN AND 
                 l.ID_LPN_DET = d.ID_LPN_DET AND
                 l.CD_EMPRESA = d.CD_EMPRESA AND
                 l.CD_PRODUTO = d.CD_PRODUTO AND
                 l.CD_FAIXA = d.CD_FAIXA 
                WHERE d.ID_LPN_DET_DEST is not null";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BulkUpdateStock(DbConnection connection, DbTransaction tran, long nuTransaccion, bool isTraspasoEmpresaStockConReserva)
        {
            var alias = "s";
            var from = $@"
                T_STOCK s
                INNER JOIN (
                            SELECT 
                                s.CD_ENDERECO,
                                s.CD_EMPRESA,
                                s.CD_PRODUTO,
                                s.CD_FAIXA,
                                s.NU_IDENTIFICADOR,
                                SUM(st.QT_ESTOQUE_DEST) QT_PRODUTO_TEMP,
                                MIN(CASE WHEN st.TP_MANEJO_FECHA_DEST = 'E' AND s.DT_FABRICACAO > st.DT_FABRICACAO_DEST  THEN st.DT_FABRICACAO_DEST ELSE s.DT_FABRICACAO END) DT_FABRICACAO_DEST ,
                                MIN(CASE WHEN st.ID_AVERIA = 'S' THEN st.ID_AVERIA ELSE s.ID_AVERIA END) ID_AVERIA_DEST 
                            FROM  T_TRASPASO_DET_TEMP  st
                            INNER JOIN T_STOCK s on 
                                 s.CD_ENDERECO = st.CD_ENDERECO AND
                                 s.CD_PRODUTO = st.CD_PRODUTO_DEST AND
                                 s.CD_FAIXA = st.CD_FAIXA AND
                                 s.NU_IDENTIFICADOR = st.NU_IDENTIFICADOR_DEST AND
                                 s.CD_EMPRESA = st.CD_EMPRESA_DEST
                            GROUP by   s.CD_ENDERECO,
                                s.CD_EMPRESA,
                                s.CD_PRODUTO,
                                s.CD_FAIXA,
                                s.NU_IDENTIFICADOR
                      ) st ON 
                           st.CD_ENDERECO  = s.CD_ENDERECO AND
                           st.CD_EMPRESA  = s.CD_EMPRESA AND
                           st.CD_PRODUTO  = s.CD_PRODUTO AND
                           st.CD_FAIXA  = s.CD_FAIXA AND
                           st.NU_IDENTIFICADOR  = s.NU_IDENTIFICADOR";
            var set = @"
                ID_AVERIA = ID_AVERIA_DEST,
                DT_UPDROW = :FechaModificacion,
                NU_TRANSACCION = :Transaccion,
                QT_ESTOQUE = QT_ESTOQUE + QT_PRODUTO_TEMP,
                DT_FABRICACAO = DT_FABRICACAO_DEST
                ";

            if (isTraspasoEmpresaStockConReserva)
                set = set + @" ,QT_RESERVA_SAIDA = QT_RESERVA_SAIDA + QT_PRODUTO_TEMP";

            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void BulkInsertStock(DbConnection connection, DbTransaction tran, long nuTransaccion, bool isTraspasoEmpresaStockConReserva)
        {
            var param = new
            {
                NU_TRANSACCION = nuTransaccion,
                DT_UPDROW = DateTime.Now,
                cantidadCero = 0
            };

            var sqlInsertNew = $@"
                    INSERT INTO T_STOCK (CD_ENDERECO, CD_EMPRESA, CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR,
                        DT_FABRICACAO, DT_UPDROW, ID_AVERIA, ID_INVENTARIO, ID_CTRL_CALIDAD,
                        QT_ESTOQUE, QT_RESERVA_SAIDA, QT_TRANSITO_ENTRADA, NU_TRANSACCION)
                    SELECT d.CD_ENDERECO,d.CD_EMPRESA_DEST,d.CD_PRODUTO_DEST,d.CD_FAIXA,d.NU_IDENTIFICADOR_DEST,
                        d.DT_FABRICACAO_DEST,MIN(:DT_UPDROW),d.ID_AVERIA,MIN('R'), MIN('C'),
                        SUM(d.QT_ESTOQUE_DEST) QT_PRODUTO_DEST,{(isTraspasoEmpresaStockConReserva == true ? "SUM(d.QT_ESTOQUE_DEST) QT_RESERVA_SAIDA" : "MIN(:cantidadCero)")},
                        MIN(:cantidadCero),MIN(:NU_TRANSACCION)
                    FROM T_TRASPASO_DET_TEMP d
                    LEFT JOIN T_STOCK st ON st.CD_ENDERECO = d.CD_ENDERECO
                        AND st.CD_EMPRESA = d.CD_EMPRESA_DEST
                        AND st.CD_PRODUTO = d.CD_PRODUTO_DEST
                        AND st.CD_FAIXA = d.CD_FAIXA
                        AND st.NU_IDENTIFICADOR = d.NU_IDENTIFICADOR_DEST
                        WHERE st.CD_ENDERECO IS NULL 
                    GROUP BY d.CD_ENDERECO,d.CD_EMPRESA_DEST,d.CD_PRODUTO_DEST,
                        d.CD_FAIXA,d.NU_IDENTIFICADOR_DEST, d.DT_FABRICACAO_DEST,d.ID_AVERIA";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion

        #region Productos Faltantes

        public virtual void ProcesarProductosFaltantes(int empresaDestino, long nuTransaccion, bool replicaProductos, bool replicaCodBarras)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            if (replicaProductos)
            {
                List<Producto> productoEmbalageFaltante = GetProductosFaltantes(connection, tran);
                BulkInsertProductosFaltantes(connection, tran, nuTransaccion, empresaDestino);
                BulkInsertProductosEmbalageFaltantes(connection, tran, nuTransaccion, productoEmbalageFaltante);

                if (replicaCodBarras)
                {
                    BulkInsertCodigoBarrasProductosFaltantes(connection, tran, nuTransaccion);
                }
            }
        }

        public virtual void BulkInsertCodigoBarrasProductosFaltantes(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {

            var param = new
            {
                transaction = nuTransaccion,
                FechaAlta = DateTime.Now
            };
            var sqlInsertNew = $@"
                INSERT INTO T_CODIGO_BARRAS (CD_BARRAS,CD_EMPRESA,CD_PRODUTO,TP_CODIGO_BARRAS,
                    DT_ADDROW,QT_EMBALAGEM,NU_PRIORIDADE_USO,NU_TRANSACCION)
                SELECT c.CD_BARRAS,temp.CD_EMPRESA_DEST,temp.CD_PRODUTO_DEST,c.TP_CODIGO_BARRAS,
                    :FechaAlta,c.QT_EMBALAGEM,c.NU_PRIORIDADE_USO,:transaction
                FROM T_TRASPASO_DET_TEMP temp
                INNER JOIN T_CODIGO_BARRAS c ON c.CD_EMPRESA = temp.CD_EMPRESA
                    AND c.CD_PRODUTO = temp.CD_PRODUTO
                LEFT JOIN T_CODIGO_BARRAS cb ON cb.CD_EMPRESA = temp.CD_EMPRESA_DEST
                    AND cb.CD_PRODUTO = temp.CD_PRODUTO_DEST
                WHERE cb.CD_PRODUTO IS NULL
    ";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BulkInsertProductosEmbalageFaltantes(DbConnection connection, DbTransaction tran, long nuTransaccion, List<Producto> productoEmbalageFaltante)
        {
            _dapper.BulkInsert(connection, tran, productoEmbalageFaltante, "T_PRODUTO_FAIXA", new Dictionary<string, Func<Producto, ColumnInfo>>
            {
                { "CD_PRODUTO", x => new ColumnInfo(x.Codigo)},
                { "CD_EMPRESA", x => new ColumnInfo(x.CodigoEmpresa)},
                { "CD_FAIXA", x => new ColumnInfo(1)},
                { "CD_EMBALAGEM_FAIXA", x => new ColumnInfo("UND")},
                { "QT_UNIDADE_EMBALAGEM", x => new ColumnInfo(1)},
                { "DT_ADDROW", x => new ColumnInfo(DateTime.Now)},
                { "DT_UPDROW", x => new ColumnInfo(DateTime.Now)},
            });
        }

        public virtual List<Producto> GetProductosFaltantes(DbConnection connection, DbTransaction tran)
        {
            var sql = $@"
                 SELECT temp.CD_EMPRESA_DEST  CodigoEmpresa,
                     temp.CD_PRODUTO_DEST Codigo
                 FROM T_TRASPASO_DET_TEMP temp
                 LEFT JOIN T_PRODUTO p ON p.CD_EMPRESA = temp.CD_EMPRESA_DEST
                     AND p.CD_PRODUTO = temp.CD_PRODUTO_DEST
                 WHERE  temp.CD_PRODUTO = temp.CD_PRODUTO_DEST AND
                 p.CD_PRODUTO IS NULL
                 GROUP BY  temp.CD_EMPRESA_DEST ,
                     temp.CD_PRODUTO_DEST";

            return _dapper.Query<Producto>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();

        }

        public virtual void BulkInsertProductosFaltantes(DbConnection connection, DbTransaction tran, long nuTransaccion, int empresa)
        {
            var param = new
            {
                transaction = nuTransaccion,
                empresa = empresa,
                fechaAlta = DateTime.Now,
                fechaModificacion = DateTime.Now,
                fechaSituacion = DateTime.Now,
            };
            var sqlInsertNew = $@"
                INSERT INTO T_PRODUTO (CD_EMPRESA,CD_PRODUTO,CD_PRODUTO_EMPRESA,CD_NAM,CD_MERCADOLOGICO,SG_PRODUTO,
                    TP_PESO_PRODUTO,DS_DIFER_PESO_QTDE,DS_PRODUTO,CD_UNIDADE_MEDIDA,CD_FAMILIA_PRODUTO,
                    CD_ROTATIVIDADE,CD_CLASSE,QT_ESTOQUE_MINIMO,QT_ESTOQUE_MAXIMO,PS_LIQUIDO,
                    PS_BRUTO,FT_CONVERSAO,VL_CUBAGEM,VL_PRECO_VENDA,VL_CUSTO_ULT_ENT,CD_ORIGEM,
                    DS_REDUZIDA,CD_NIVEL,CD_UNID_EMB,CD_SITUACAO,DT_SITUACAO,QT_DIAS_VALIDADE,
                    QT_DIAS_DURACAO,ID_CROSS_DOCKING,ID_REDONDEO_VALIDEZ,ID_AGRUPACION,ID_MANEJO_IDENTIFICADOR,
                    TP_DISPLAY,DS_ANEXO1,DS_ANEXO2,DS_ANEXO3,DS_ANEXO4,DT_UPDROW,DT_ADDROW,
                    VL_ALTURA,VL_LARGURA,VL_PROFUNDIDADE,TP_MANEJO_FECHA,VL_AVISO_AJUSTE,DS_HELP_COLECTOR,
                    QT_SUBBULTO,CD_EXCLUSIVO,QT_UND_DISTRIBUCION,QT_DIAS_VALIDADE_LIBERACION,QT_UND_BULTO,
                    ID_MANEJA_TOMA_DATO,DS_ANEXO5,CD_GRUPO_CONSULTA,DS_DISPLAY,VL_PRECIO_SEG_DISTR,VL_PRECIO_SEG_STOCK,
                    VL_PRECIO_DISTRIB,VL_PRECIO_EGRESO,VL_PRECIO_INGRESO,VL_PRECIO_STOCK,CD_UND_MEDIDA_FACT,
                    CD_PRODUTO_UNICO,CD_RAMO_PRODUTO,FL_ACEPTA_DECIMALES,ND_FACTURACION_COMP1,ND_FACTURACION_COMP2,
                    ND_MODALIDAD_INGRESO_LOTE,QT_PADRON_STOCK,QT_GENERICO,CODIGO_BASE,TALLE,COLOR,TEMPORADA,VL_CATEGORIA_01,
                    VL_CATEGORIA_02,NU_TRANSACCION)
                SELECT :empresa, d.CD_PRODUTO, d.CD_PRODUTO_EMPRESA, d.CD_NAM, d.CD_MERCADOLOGICO, d.SG_PRODUTO, 
                    d.TP_PESO_PRODUTO, d.DS_DIFER_PESO_QTDE, d.DS_PRODUTO, d.CD_UNIDADE_MEDIDA, d.CD_FAMILIA_PRODUTO, 
                    d.CD_ROTATIVIDADE, d.CD_CLASSE, d.QT_ESTOQUE_MINIMO, d.QT_ESTOQUE_MAXIMO, d.PS_LIQUIDO, 
                    d.PS_BRUTO, d.FT_CONVERSAO, d.VL_CUBAGEM, d.VL_PRECO_VENDA, d.VL_CUSTO_ULT_ENT, d.CD_ORIGEM, 
                    d.DS_REDUZIDA, d.CD_NIVEL, d.CD_UNID_EMB, d.CD_SITUACAO, :fechaSituacion, d.QT_DIAS_VALIDADE, 
                    d.QT_DIAS_DURACAO, d.ID_CROSS_DOCKING, d.ID_REDONDEO_VALIDEZ, d.ID_AGRUPACION, d.ID_MANEJO_IDENTIFICADOR, 
                    d.TP_DISPLAY, d.DS_ANEXO1, d.DS_ANEXO2, d.DS_ANEXO3, d.DS_ANEXO4, :fechaModificacion, :fechaAlta, 
                    d.VL_ALTURA, d.VL_LARGURA, d.VL_PROFUNDIDADE, d.TP_MANEJO_FECHA, d.VL_AVISO_AJUSTE, d.DS_HELP_COLECTOR, 
                    d.QT_SUBBULTO, d.CD_EXCLUSIVO, d.QT_UND_DISTRIBUCION, d.QT_DIAS_VALIDADE_LIBERACION, d.QT_UND_BULTO, 
                    d.ID_MANEJA_TOMA_DATO, d.DS_ANEXO5, d.CD_GRUPO_CONSULTA, d.DS_DISPLAY, d.VL_PRECIO_SEG_DISTR, d.VL_PRECIO_SEG_STOCK,
                    d.VL_PRECIO_DISTRIB, d.VL_PRECIO_EGRESO, d.VL_PRECIO_INGRESO, d.VL_PRECIO_STOCK, d.CD_UND_MEDIDA_FACT, d.CD_PRODUTO_UNICO, 
                    d.CD_RAMO_PRODUTO, d.FL_ACEPTA_DECIMALES, d.ND_FACTURACION_COMP1, d.ND_FACTURACION_COMP2, d.ND_MODALIDAD_INGRESO_LOTE, 
                    d.QT_PADRON_STOCK, d.QT_GENERICO, d.CODIGO_BASE, d.TALLE, d.COLOR, d.TEMPORADA, d.VL_CATEGORIA_01, d.VL_CATEGORIA_02, :transaction
                FROM (SELECT CD_EMPRESA,
                        CD_EMPRESA_DEST,
                        CD_PRODUTO_DEST 
                        FROM T_TRASPASO_DET_TEMP
                    WHERE CD_PRODUTO = CD_PRODUTO_DEST
                    GROUP BY CD_EMPRESA,
                        CD_EMPRESA_DEST,
                        CD_PRODUTO_DEST ) temp
                LEFT JOIN T_PRODUTO ps ON ps.CD_EMPRESA = temp.CD_EMPRESA_DEST
                    AND ps.CD_PRODUTO = temp.CD_PRODUTO_DEST
                INNER JOIN T_PRODUTO d ON d.CD_EMPRESA = temp.CD_EMPRESA
                    AND d.CD_PRODUTO = temp.CD_PRODUTO_DEST
                WHERE ps.CD_PRODUTO IS NULL";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion

        #region Agentes Faltantes

        public virtual void ProcesarAgentesFaltantes(int empresaDestino, long nuTransaccion, bool replicaAgentes, out List<Agente> agentes)
        {
            agentes = new List<Agente>();
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            if (replicaAgentes)
            {
                agentes = GetAgentesFaltantes(connection, tran, nuTransaccion, empresaDestino);
                BulkInsertAgentesFaltantes(connection, tran, nuTransaccion, empresaDestino);
                BulkInsertContactoAgentesFaltantes(connection, tran, nuTransaccion, empresaDestino);
            }
        }

        public virtual List<Agente> GetAgentesFaltantes(DbConnection connection, DbTransaction tran, long nuTransaccion, int empresaDestino)
        {
            string sql = @"SELECT
                                temp.CD_EMPRESA_DEST Empresa,
                                c.CD_CLIENTE CodigoInterno,
                                c.DS_CLIENTE Descripcion,
                                c.TP_AGENTE Tipo
                            FROM (SELECT CD_EMPRESA,
                                    CD_CLIENTE,
                                    CD_EMPRESA_DEST
                                    FROM T_TRASPASO_DET_TEMP
                                WHERE CD_CLIENTE = CD_CLIENTE_DEST
                                GROUP BY CD_EMPRESA,
                                    CD_CLIENTE,
                                    CD_EMPRESA_DEST) temp
                            LEFT JOIN T_CLIENTE d ON d.CD_EMPRESA = temp.CD_EMPRESA_DEST
                                AND d.CD_CLIENTE = temp.CD_CLIENTE
                            INNER JOIN T_CLIENTE c ON  c.CD_EMPRESA = temp.CD_EMPRESA
                                AND  c.CD_CLIENTE = temp.CD_CLIENTE
                            WHERE d.CD_CLIENTE IS NULL";

            return _dapper.Query<Agente>(connection, sql, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual void BulkInsertAgentesFaltantes(DbConnection connection, DbTransaction tran, long nuTransaccion, int empresaDestino)
        {

            var sqlInsertNew = $@"
                INSERT INTO T_CLIENTE (CD_EMPRESA,CD_CLIENTE,CD_ROTA,DS_CLIENTE,DS_ENDERECO,DS_BAIRRO,
                    CD_CEP,NU_TELEFONE,NU_DDD,NU_FAX,NU_INSCRICAO,CD_CGC_CLIENTE,
                    CD_SITUACAO,DT_SITUACAO,DT_CADASTRAMENTO,DT_ALTERACAO,TP_ATIVIDADE,
                    NU_PRIOR_CARGA,NU_DV_CLIENTE,DS_ANEXO1,DS_ANEXO2,DS_ANEXO3,DS_ANEXO4,
                    ID_CLIENTE_FILIAL,CD_FORNECEDOR,CD_CLIENTE_EN_CONSOLIDADO,CD_EMPRESA_CONSOLIDADA,CD_AGENTE,
                    TP_AGENTE,FL_ACEPTA_DEVOLUCION,CD_GLN,CD_PUNTO_ENTREGA,CD_CATEGORIA,ND_TIPO_FISCAL,
                    ID_LOCALIDAD,VL_PORCENTAJE_VIDA_UTIL,CD_LUGAR,CD_GRUPO_CONSULTA,FL_SYNC_REALIZADA,
                    DS_EMAIL) 
                SELECT temp.CD_EMPRESA_DEST,c.CD_CLIENTE,c.CD_ROTA,c.DS_CLIENTE,c.DS_ENDERECO,c.DS_BAIRRO,
                    c.CD_CEP,c.NU_TELEFONE,c.NU_DDD,c.NU_FAX,c.NU_INSCRICAO,c.CD_CGC_CLIENTE,
                    c.CD_SITUACAO,c.DT_SITUACAO,c.DT_CADASTRAMENTO,c.DT_ALTERACAO,c.TP_ATIVIDADE,
                    c.NU_PRIOR_CARGA,c.NU_DV_CLIENTE,c.DS_ANEXO1,c.DS_ANEXO2,c.DS_ANEXO3,c.DS_ANEXO4,
                    c.ID_CLIENTE_FILIAL,c.CD_FORNECEDOR,c.CD_CLIENTE_EN_CONSOLIDADO,c.CD_EMPRESA_CONSOLIDADA,c.CD_AGENTE,
                    c.TP_AGENTE,c.FL_ACEPTA_DEVOLUCION,c.CD_GLN,c.CD_PUNTO_ENTREGA,c.CD_CATEGORIA,c.ND_TIPO_FISCAL,
                    c.ID_LOCALIDAD,c.VL_PORCENTAJE_VIDA_UTIL,c.CD_LUGAR,c.CD_GRUPO_CONSULTA,c.FL_SYNC_REALIZADA,
                    c.DS_EMAIL
                FROM (SELECT CD_EMPRESA,
                        CD_CLIENTE,
                        CD_EMPRESA_DEST
                        FROM T_TRASPASO_DET_TEMP
                    WHERE CD_CLIENTE = CD_CLIENTE_DEST
                    GROUP BY CD_EMPRESA,
                        CD_CLIENTE,
                        CD_EMPRESA_DEST) temp
                LEFT JOIN T_CLIENTE d ON d.CD_EMPRESA = temp.CD_EMPRESA_DEST
                    AND d.CD_CLIENTE = temp.CD_CLIENTE
                INNER JOIN T_CLIENTE c ON  c.CD_EMPRESA = temp.CD_EMPRESA
                    AND  c.CD_CLIENTE = temp.CD_CLIENTE
                WHERE d.CD_CLIENTE IS NULL";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BulkInsertContactoAgentesFaltantes(DbConnection connection, DbTransaction tran, long nuTransaccion, int empresaDestino)
        {
            var param = new
            {
                NU_TRANSACCION = nuTransaccion,
                DT_ADDROW = DateTime.Now,
            };

            var sqlInsertNew = $@"
                INSERT INTO T_CONTACTO (CD_EMPRESA,CD_CLIENTE,DS_CONTACTO,
                     NM_CONTACTO,DS_EMAIL,NU_TELEFONO,DT_ADDROW,NU_TRANSACCION)
                SELECT temp.CD_EMPRESA_DEST,temp.CD_CLIENTE_DEST,d.DS_CONTACTO,
                     d.NM_CONTACTO,d.DS_EMAIL,d.NU_TELEFONO,:DT_ADDROW,:NU_TRANSACCION
                FROM (SELECT CD_EMPRESA,
                        CD_CLIENTE,
                        CD_EMPRESA_DEST,
                        CD_CLIENTE_DEST
                        FROM T_TRASPASO_DET_TEMP
                    WHERE CD_CLIENTE = CD_CLIENTE_DEST
                    GROUP BY CD_EMPRESA,
                        CD_CLIENTE,
                        CD_EMPRESA_DEST,
                        CD_CLIENTE_DEST ) temp
                LEFT JOIN T_CONTACTO ps ON ps.CD_EMPRESA = temp.CD_EMPRESA_DEST
                    AND ps.CD_CLIENTE = temp.CD_CLIENTE_DEST
                INNER JOIN T_CONTACTO d ON d.CD_EMPRESA = temp.CD_EMPRESA
                    AND d.CD_CLIENTE = temp.CD_CLIENTE
                WHERE ps.CD_CLIENTE IS NULL";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion

        #region Pedido

        public virtual void ProcesarPedidoEnDestinoLiberado(long nuTransaccion, int preparacion, string aplicacion, bool isGenerarDetallePedidoLpn = false)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            BulkUpdateTraspasoPedido(connection, tran);
            BulkInsertPedidos(connection, tran, nuTransaccion, preparacion, aplicacion);
            BulkInsertDetallesPedidos(connection, tran, nuTransaccion);

            if (isGenerarDetallePedidoLpn)
                BulkInsertDetallesPedidosLpns(connection, tran, nuTransaccion);
        }

        public virtual void BulkInsertDetallesPedidosLpns(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new
            {
                NU_TRANSACCION = nuTransaccion,
                DT_ADDROW = DateTime.Now,
                ID_ESPECIFICA_IDENTIFICADOR = "S",
                QT_ANULADO = 0
            };

            var sqlInsertNew = $@"
                INSERT INTO T_DET_PEDIDO_SAIDA_LPN (NU_PEDIDO,CD_CLIENTE,CD_EMPRESA,CD_PRODUTO,
                                CD_FAIXA,NU_IDENTIFICADOR,ID_ESPECIFICA_IDENTIFICADOR,
                                QT_PEDIDO,DT_ADDROW,NU_TRANSACCION,ID_LPN_EXTERNO,
                                TP_LPN_TIPO,QT_LIBERADO,QT_ANULADO,NU_LPN)
                SELECT t.NU_PEDIDO_DEST,
                   t.CD_CLIENTE_DEST,
                   t.CD_EMPRESA_DEST,
                   t.CD_PRODUTO_DEST,
                   t.CD_FAIXA,
                   t.NU_IDENTIFICADOR_DEST,
                   MIN(:ID_ESPECIFICA_IDENTIFICADOR),
                   SUM(t.QT_ESTOQUE_DEST) QT_PEDIDO,
                   MIN(:DT_ADDROW),
                   MIN(:NU_TRANSACCION),
                   l.ID_LPN_EXTERNO,
                   l.TP_LPN_TIPO,
                   SUM(t.QT_ESTOQUE_DEST) QT_LIBERADO,
                   MIN(:QT_ANULADO),
                   t.NU_LPN_DEST
                FROM T_TRASPASO_DET_LPN_TEMP t
                INNER JOIN T_LPN l on l.NU_LPN = t.NU_LPN_DEST
                GROUP BY t.NU_PEDIDO_DEST,
                    t.CD_CLIENTE_DEST,
                    t.CD_EMPRESA_DEST,
                    t.CD_PRODUTO_DEST,
                    t.CD_FAIXA, 
                    t.NU_IDENTIFICADOR_DEST,
                    l.ID_LPN_EXTERNO,
                    l.TP_LPN_TIPO,
                    t.NU_LPN_DEST";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BulkUpdateTraspasoPedido(DbConnection connection, DbTransaction tran)
        {
            var alias = "dd";
            var from = $@"
                T_TRASPASO_DET_PEDIDO dd
                          INNER JOIN ( 
                                SELECT 
                                   tdp.NU_PEDIDO,
                                   tdp.CD_CLIENTE,
                                   tdp.CD_EMPRESA,
                                   MIN(temp.NU_PEDIDO_DEST) NU_PEDIDO_DEST,
                                   MIN(temp.CD_CLIENTE_DEST) CD_CLIENTE_DEST
                                FROM T_TRASPASO_DET_TEMP temp
                                INNER JOIN T_TRASPASO_DET_PEDIDO tdp on  tdp.NU_PEDIDO = temp.NU_PEDIDO AND
                                    tdp.CD_CLIENTE = temp.CD_CLIENTE AND
                                    tdp.CD_EMPRESA = temp.CD_EMPRESA
                                GROUP BY
                                   tdp.NU_PEDIDO,
                                   tdp.CD_CLIENTE,
                                   tdp.CD_EMPRESA
                                   ) tmp ON 
                            tmp.NU_PEDIDO = dd.NU_PEDIDO AND
                            tmp.CD_CLIENTE = dd.CD_CLIENTE AND
                            tmp.CD_EMPRESA = dd.CD_EMPRESA";

            var set = @"
                NU_PEDIDO_DESTINO = NU_PEDIDO_DEST,
                CD_CLIENTE_DESTINO = CD_CLIENTE_DEST";


            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void BulkInsertPedidos(DbConnection connection, DbTransaction tran, long nuTransaccion, int preparacion, string aplicacion)
        {

            var param = new
            {
                ID_AGRUPACION = Agrupacion.Pedido,
                NU_TRANSACCION = nuTransaccion,
                DT_ADDROW = DateTime.Now,
                NU_PREPARACION_DEST = preparacion,
                CD_ORIGEN = aplicacion
            };

            var sqlInsertNew = $@"
                INSERT INTO T_PEDIDO_SAIDA (NU_PEDIDO,CD_CLIENTE,CD_EMPRESA,CD_ROTA,CD_SITUACAO,
                     ID_MANUAL,ID_AGRUPACION,DT_ADDROW,NU_ULT_PREPARACION,TP_PEDIDO,TP_EXPEDICION,
                     NU_TRANSACCION,ND_ACTIVIDAD,CD_ORIGEN)
                SELECT d.NU_PEDIDO_DEST,d.CD_CLIENTE_DEST,d.CD_EMPRESA_DEST,l.CD_ROTA,l.CD_SITUACAO,
                     l.ID_MANUAL,:ID_AGRUPACION,:DT_ADDROW,:NU_PREPARACION_DEST,d.TP_PEDIDO,d.TP_EXPEDICION,
                     :NU_TRANSACCION,l.ND_ACTIVIDAD,:CD_ORIGEN
                FROM  (SELECT NU_PEDIDO,
                         CD_CLIENTE,
                         CD_EMPRESA,
                         NU_PEDIDO_DEST,
                         CD_CLIENTE_DEST,
                         CD_EMPRESA_DEST,
                         TP_EXPEDICION_DEST TP_EXPEDICION,
                         TP_PEDIDO_DEST TP_PEDIDO
                     FROM T_TRASPASO_DET_TEMP 
                     GROUP BY  NU_PEDIDO,
                         CD_CLIENTE,
                         CD_EMPRESA,
                         NU_PEDIDO_DEST,
                         CD_CLIENTE_DEST,
                         CD_EMPRESA_DEST,
                         TP_EXPEDICION_DEST,
                         TP_PEDIDO_DEST
                     ) d
                INNER JOIN T_PEDIDO_SAIDA l ON l.NU_PEDIDO = d.NU_PEDIDO AND
                     l.CD_EMPRESA = d.CD_EMPRESA AND
                     l.CD_CLIENTE = d.CD_CLIENTE";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void BulkInsertDetallesPedidos(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var param = new
            {
                ID_AGRUPACION = Agrupacion.Pedido,
                NU_TRANSACCION = nuTransaccion,
                DT_ADDROW = DateTime.Now,
                ID_ESPECIFICA_IDENTIFICADOR = "S",
                QT_ANULADO = 0
            };

            var sqlInsertNew = $@"
                INSERT INTO T_DET_PEDIDO_SAIDA (NU_PEDIDO,CD_EMPRESA,CD_CLIENTE,CD_PRODUTO,CD_FAIXA,
                     QT_PEDIDO,QT_PEDIDO_ORIGINAL,NU_IDENTIFICADOR,ID_AGRUPACION,QT_LIBERADO,
                     QT_ANULADO,ID_ESPECIFICA_IDENTIFICADOR,DT_ADDROW,NU_TRANSACCION)
                SELECT d.NU_PEDIDO_DEST,d.CD_EMPRESA_DEST,d.CD_CLIENTE_DEST,d.CD_PRODUTO_DEST,d.CD_FAIXA,
                     d.QT_ESTOQUE_DEST,d.QT_ESTOQUE_DEST,d.NU_IDENTIFICADOR_DEST,:ID_AGRUPACION,d.QT_ESTOQUE_DEST,
                     :QT_ANULADO,:ID_ESPECIFICA_IDENTIFICADOR,:DT_ADDROW,:NU_TRANSACCION
                FROM  (SELECT NU_PEDIDO_DEST,
                         CD_CLIENTE_DEST,
                         CD_EMPRESA_DEST,
                         CD_PRODUTO_DEST,
                         CD_FAIXA,
                         NU_IDENTIFICADOR_DEST,
                         SUM(QT_ESTOQUE_DEST) QT_ESTOQUE_DEST
                     FROM T_TRASPASO_DET_TEMP 
                     GROUP BY NU_PEDIDO_DEST,
                         CD_CLIENTE_DEST,
                         CD_EMPRESA_DEST,
                         CD_PRODUTO_DEST,
                         CD_FAIXA,
                         NU_IDENTIFICADOR_DEST
                     ) d";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }


        #endregion

        #region Preparacion 

        public virtual void GenerarPreparacionEnDestino(IUnitOfWork uow, Preparacion preparacion, Carga carga, List<Contenedor> contenedoresDestino, DocumentoIngreso documentoEntrada, bool isDocumentalEmpresaDestino, long nuTransaccion, bool isGenerateDetallesLpns)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            uow.PreparacionRepository.AddPreparacion(connection, tran, preparacion);
            uow.CargaRepository.AddCarga(connection, tran, carga);

            AddDetallePreparacionPendiente(connection, tran, preparacion.Id, carga.Id, nuTransaccion, isGenerateDetallesLpns);

            if (isGenerateDetallesLpns)
            {
                AddDetallePreparacionLpnsPendiente(connection, tran, preparacion.Id, carga.Id, nuTransaccion);
            }

            if (isDocumentalEmpresaDestino)
            {
                AddPreparacionReservaPreparado(connection, tran, preparacion.Id, nuTransaccion, documentoEntrada, true);
                UpdateDetalleDocumentoPreparacionReserva(connection, tran, nuTransaccion, documentoEntrada);
            }
        }

        public virtual void AddDetallePreparacionLpnsPendiente(DbConnection connection, DbTransaction tran, int preparacion, long idCarga, long nuTransaccion)
        {
            var param = new
            {
                NU_PREPARACION = preparacion,
                NU_CARGA = idCarga,
                NU_TRANSACCION = nuTransaccion,
                DT_ADDROW = DateTime.Now,
                ND_ESTADO = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE,
                ID_ESPECIFICA_IDENTIFICADOR = "S",
            };

            var sqlInsertNew = $@"
                   INSERT INTO T_DET_PICKING (NU_PREPARACION,CD_PRODUTO,CD_FAIXA,NU_IDENTIFICADOR,CD_EMPRESA,CD_ENDERECO,
                        NU_PEDIDO,CD_CLIENTE,NU_SEQ_PREPARACION,ID_ESPECIFICA_IDENTIFICADOR,NU_CARGA,ID_AGRUPACION,
                        QT_PRODUTO,DT_ADDROW,NU_TRANSACCION,ND_ESTADO,ID_DET_PICKING_LPN)
                   SELECT MIN(:NU_PREPARACION),temp.CD_PRODUTO_DEST,temp.CD_FAIXA,temp.NU_IDENTIFICADOR_DEST,temp.CD_EMPRESA_DEST,temp.CD_ENDERECO,
                        temp.NU_PEDIDO_DEST,temp.CD_CLIENTE_DEST,MIN(temp.NU_SEQ_PREPARACION) NU_SEQ_PREPARACION,MIN(:ID_ESPECIFICA_IDENTIFICADOR),MIN(:NU_CARGA),temp.ID_AGRUPACION,
                        SUM(temp.QT_ESTOQUE_DEST) QT_ESTOQUE_DEST ,MIN(:DT_ADDROW),MIN(:NU_TRANSACCION),MIN(:ND_ESTADO),ID_DET_PICKING_LPN_DEST
                    FROM  (SELECT MIN(ID_LPN_DET_DEST) ID_LPN_DET_DEST,
                                NU_LPN_DEST,
                                MIN(ID_LPN_DET) ID_LPN_DET,
                                CD_FAIXA,
                                CD_PRODUTO_DEST,
                                CD_EMPRESA_DEST,
                                NU_IDENTIFICADOR_DEST,
                                VL_ATRIBUTOS_LPN,
                                SUM(QT_ESTOQUE_DEST) QT_ESTOQUE_DEST,
                                MIN(ID_DET_PICKING_LPN_DEST)ID_DET_PICKING_LPN_DEST,
                                ID_AGRUPACION,
                                CD_CLIENTE_DEST,
                                NU_PEDIDO_DEST,
                                CD_ENDERECO,
                                MIN(NU_SEQ_PREPARACION) NU_SEQ_PREPARACION
                          FROM T_TRASPASO_DET_LPN_TEMP 
                          GROUP BY NU_LPN_DEST,
                            CD_FAIXA,
                            CD_PRODUTO_DEST,
                            CD_EMPRESA_DEST,
                            NU_IDENTIFICADOR_DEST,
                            VL_ATRIBUTOS_LPN,
                            ID_AGRUPACION,
                            CD_CLIENTE_DEST,
                            NU_PEDIDO_DEST,
                            CD_ENDERECO)temp                
                    GROUP BY temp.CD_PRODUTO_DEST, temp.CD_FAIXA, temp.NU_IDENTIFICADOR_DEST, temp.CD_EMPRESA_DEST, 
                    temp.CD_ENDERECO, temp.NU_PEDIDO_DEST, temp.CD_CLIENTE_DEST,
                    temp.ID_AGRUPACION,ID_DET_PICKING_LPN_DEST";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

            var param1 = new
            {
                NU_PREPARACION = preparacion,
                DT_ADDROW = DateTime.Now,
                ND_ESTADO = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE,
                NU_TRANSACCION = nuTransaccion,
            };
            string sql = @" INSERT INTO T_DET_PICKING_LPN
                    (
                        NU_PREPARACION,
                        ID_DET_PICKING_LPN,
                        ID_LPN_DET,
                        NU_LPN,
                        CD_EMPRESA,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        QT_RESERVA,
                        TP_LPN_TIPO,
                        CD_ENDERECO,
                        DT_ADDROW,
                        NU_TRANSACCION,
                        ID_LPN_EXTERNO
                    )
                    SELECT
                        :NU_PREPARACION,
                        tdlt.ID_DET_PICKING_LPN_DEST,
                        tdlt.ID_LPN_DET_DEST,
                        tdlt.NU_LPN_DEST,
                        tdlt.CD_EMPRESA_DEST,
                        tdlt.CD_PRODUTO_DEST,
                        tdlt.CD_FAIXA,
                        tdlt.NU_IDENTIFICADOR_DEST,
                        tdlt.QT_ESTOQUE_DEST,
                        l.TP_LPN_TIPO,
                        tdlt.CD_ENDERECO,
                        :DT_ADDROW,
                        :NU_TRANSACCION,
                        l.ID_LPN_EXTERNO
                    FROM (SELECT MIN(ID_LPN_DET_DEST) ID_LPN_DET_DEST,
                            NU_LPN_DEST,
                            MIN(ID_LPN_DET) ID_LPN_DET,
                            CD_FAIXA,
                            CD_PRODUTO_DEST,
                            CD_EMPRESA_DEST,
                            NU_IDENTIFICADOR_DEST,
                            VL_ATRIBUTOS_LPN,
                            SUM(QT_ESTOQUE_DEST) QT_ESTOQUE_DEST,
                            MIN(ID_DET_PICKING_LPN_DEST)ID_DET_PICKING_LPN_DEST,
                            CD_ENDERECO
                          FROM T_TRASPASO_DET_LPN_TEMP 
                              GROUP BY NU_LPN_DEST,
                                CD_FAIXA,
                                CD_PRODUTO_DEST,
                                CD_EMPRESA_DEST,
                                NU_IDENTIFICADOR_DEST,
                                VL_ATRIBUTOS_LPN,
                                CD_ENDERECO) tdlt
                    INNER JOIN T_LPN l on l.NU_LPN = tdlt.NU_LPN_DEST";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, param1, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void AddDetallePreparacionPendiente(DbConnection connection, DbTransaction tran, int preparacion, long idCarga, long nuTransaccion, bool isGenerateDetallesLpns)
        {
            var param = new
            {
                NU_PREPARACION = preparacion,
                NU_CARGA = idCarga,
                NU_TRANSACCION = nuTransaccion,
                DT_ADDROW = DateTime.Now,
                ND_ESTADO = EstadoDetallePreparacion.ESTADO_PREP_PENDIENTE,
                ID_ESPECIFICA_IDENTIFICADOR = "S"
            };

            string condicion = (isGenerateDetallesLpns == true) ? " WHERE temp.ID_DET_PICKING_LPN is null " : "";
            var sqlInsertNew = $@"
                INSERT INTO T_DET_PICKING (NU_PREPARACION,CD_PRODUTO,CD_FAIXA,NU_IDENTIFICADOR,CD_EMPRESA,CD_ENDERECO,
                    NU_PEDIDO,CD_CLIENTE,NU_SEQ_PREPARACION,ID_ESPECIFICA_IDENTIFICADOR,NU_CARGA,ID_AGRUPACION,
                    QT_PRODUTO,DT_ADDROW,NU_TRANSACCION,ND_ESTADO)
                SELECT MIN(:NU_PREPARACION),temp.CD_PRODUTO_DEST,temp.CD_FAIXA,temp.NU_IDENTIFICADOR_DEST,temp.CD_EMPRESA_DEST,temp.CD_ENDERECO_PICKING,
                    temp.NU_PEDIDO_DEST,temp.CD_CLIENTE_DEST,MIN(temp.NU_SEQ_PREPARACION) NU_SEQ_PREPARACION,MIN(:ID_ESPECIFICA_IDENTIFICADOR),MIN(:NU_CARGA),temp.ID_AGRUPACION,
                    SUM(temp.QT_ESTOQUE_DEST) QT_ESTOQUE_DEST ,MIN(:DT_ADDROW),MIN(:NU_TRANSACCION),MIN(:ND_ESTADO)
                FROM  T_TRASPASO_DET_TEMP temp 
                {condicion}
                GROUP BY temp.CD_PRODUTO_DEST, temp.CD_FAIXA, temp.NU_IDENTIFICADOR_DEST, temp.CD_EMPRESA_DEST, 
                temp.CD_ENDERECO_PICKING, temp.NU_PEDIDO_DEST, temp.CD_CLIENTE_DEST, 
                temp.ID_AGRUPACION";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void GenerarPreparacionEnDestinoPreparada(IUnitOfWork uow, Preparacion preparacion, Carga carga, List<Contenedor> contenedores, DocumentoIngreso documento, bool isDocumentalEmpresaDestino, long nuTransaccion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            uow.PreparacionRepository.AddPreparacion(connection, tran, preparacion);
            uow.CargaRepository.AddCarga(connection, tran, carga);

            AddContenedores(connection, tran, contenedores);
            AddDetallePreparacionPreparado(connection, tran, preparacion.Id, carga.Id, nuTransaccion);

            if (isDocumentalEmpresaDestino)
            {
                AddPreparacionReservaPreparado(connection, tran, preparacion.Id, nuTransaccion, documento);
                UpdateDetalleDocumentoPreparacionReserva(connection, tran, nuTransaccion, documento);
            }
        }

        public virtual void AddContenedores(DbConnection connection, DbTransaction tran, List<Contenedor> contenedores)
        {
            _dapper.BulkInsert(connection, tran, contenedores, "T_CONTENEDOR", new Dictionary<string, Func<Contenedor, ColumnInfo>>
            {
                    { "NU_PREPARACION", x => new ColumnInfo(x.NumeroPreparacion)},
                    { "NU_CONTENEDOR", x => new ColumnInfo(x.Numero)},
                    { "TP_CONTENEDOR", x => new ColumnInfo(x.TipoContenedor)},
                    { "CD_SITUACAO", x => new ColumnInfo(x.EstadoId)},
                    { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                    { "CD_SUB_CLASSE", x => new ColumnInfo(x.CodigoSubClase,DbType.String)},
                    { "DT_ADDROW", x => new ColumnInfo(x.FechaAgregado)},
                    { "ID_CONTENEDOR_EMPAQUE", x => new ColumnInfo(x.IdContenedorEmpaque)},
                    { "CD_BARRAS", x => new ColumnInfo(x.CodigoBarras)},
                    { "ID_EXTERNO_CONTENEDOR", x => new ColumnInfo(x.IdExterno)},
                    { "NU_TRANSACCION", x => new ColumnInfo(x.NumeroTransaccion)},
                    { "NU_LPN", x => new ColumnInfo(x.NroLpn,DbType.Int64)},
            });
        }

        public virtual void AddDetallePreparacionPreparado(DbConnection connection, DbTransaction tran, int preparacion, long idCarga, long nuTransaccion)
        {
            var param = new
            {
                NU_PREPARACION = preparacion,
                NU_CARGA = idCarga,
                NU_TRANSACCION = nuTransaccion,
                DT_PICKEO = DateTime.Now,
                DT_ADDROW = DateTime.Now,
                ND_ESTADO = EstadoDetallePreparacion.ESTADO_PREPARADO,
                ID_ESPECIFICA_IDENTIFICADOR = "S",
                ID_AVERIA_PICKEO = "N"
            };

            var sqlInsertNew = $@"
                INSERT INTO T_DET_PICKING (NU_PREPARACION,CD_PRODUTO,CD_FAIXA,NU_IDENTIFICADOR,CD_EMPRESA,CD_ENDERECO,
                    NU_PEDIDO,CD_CLIENTE,NU_SEQ_PREPARACION,ID_ESPECIFICA_IDENTIFICADOR,NU_CARGA,ID_AGRUPACION,
                    QT_PRODUTO,QT_PREPARADO,NU_CONTENEDOR,DT_PICKEO,DT_ADDROW,DT_FABRICACAO_PICKEO
                    ,ID_AVERIA_PICKEO,NU_TRANSACCION,ND_ESTADO)
                SELECT :NU_PREPARACION,temp.CD_PRODUTO_DEST,temp.CD_FAIXA,temp.NU_IDENTIFICADOR_DEST,temp.CD_EMPRESA_DEST,temp.CD_ENDERECO_PICKING,
                    temp.NU_PEDIDO_DEST,temp.CD_CLIENTE_DEST,temp.NU_SEQ_PREPARACION,:ID_ESPECIFICA_IDENTIFICADOR,:NU_CARGA,temp.ID_AGRUPACION,
                    temp.QT_ESTOQUE_DEST,temp.QT_ESTOQUE_DEST,temp.NU_CONTENEDOR_DEST,:DT_PICKEO,:DT_ADDROW,temp.DT_FABRICACAO_DEST
                    ,:ID_AVERIA_PICKEO,:NU_TRANSACCION,:ND_ESTADO
                FROM  T_TRASPASO_DET_TEMP temp";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, param: param, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void ProcesarDetallePreparacionOrigen(int preparacion, long nuTransaccion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var alias = "dp";
            var from = $@"
                T_DET_PICKING dp
                INNER JOIN (
                         SELECT 
                             tdp.NU_PREPARACION,
                             tdp.CD_ENDERECO,
                             tdp.CD_PRODUTO,
                             tdp.CD_FAIXA,
                             tdp.NU_IDENTIFICADOR,
                             tdp.CD_EMPRESA,
                             tdp.NU_PEDIDO,
                             tdp.CD_CLIENTE,
                             tdp.NU_SEQ_PREPARACION
                         FROM  T_TRASPASO_DET_TEMP  td
                         INNER JOIN T_DET_PICKING tdp on 
                             tdp.NU_PREPARACION = :NU_PREPARACION AND
                             tdp.CD_ENDERECO = td.CD_ENDERECO_PICKING AND 
                             tdp.CD_PRODUTO = td.CD_PRODUTO AND 
                             tdp.CD_FAIXA = td.CD_FAIXA AND 
                             tdp.NU_IDENTIFICADOR = td.NU_IDENTIFICADOR AND 
                             tdp.CD_EMPRESA = td.CD_EMPRESA AND
                             tdp.NU_PEDIDO = td.NU_PEDIDO AND 
                             tdp.CD_CLIENTE = td.CD_CLIENTE AND 
                             tdp.NU_SEQ_PREPARACION = td.NU_SEQ_PREPARACION
                         GROUP BY  tdp.NU_PREPARACION,
                             tdp.CD_ENDERECO,
                             tdp.CD_PRODUTO,
                             tdp.CD_FAIXA,
                             tdp.NU_IDENTIFICADOR,
                             tdp.CD_EMPRESA,
                             tdp.NU_PEDIDO,
                             tdp.CD_CLIENTE,
                             tdp.NU_SEQ_PREPARACION
                     ) tdtmp ON 
                         dp.NU_PREPARACION = tdtmp.NU_PREPARACION AND
                         dp.CD_ENDERECO = tdtmp.CD_ENDERECO AND 
                         dp.CD_PRODUTO = tdtmp.CD_PRODUTO AND 
                         dp.CD_FAIXA = tdtmp.CD_FAIXA AND 
                         dp.NU_IDENTIFICADOR = tdtmp.NU_IDENTIFICADOR AND 
                         dp.CD_EMPRESA = tdtmp.CD_EMPRESA AND
                         dp.NU_PEDIDO = tdtmp.NU_PEDIDO AND 
                         dp.CD_CLIENTE = tdtmp.CD_CLIENTE AND 
                         dp.NU_SEQ_PREPARACION = tdtmp.NU_SEQ_PREPARACION";
            var set = @"
                NU_TRANSACCION = :NU_TRANSACCION,
                ND_ESTADO = :ND_ESTADO";
            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new DynamicParameters(new { NU_PREPARACION = preparacion, NU_TRANSACCION = nuTransaccion, ND_ESTADO = EstadoDetallePreparacion.ESTAD_TRASPASADO }), commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void ProcesarTraspasoContenedor(List<DetallePreparacion> detallePreparacion, long nuTransaccion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            _dapper.BulkUpdate(connection, tran, detallePreparacion, "T_CONTENEDOR", new Dictionary<string, Func<DetallePreparacion, ColumnInfo>>
            {
                { "CD_SITUACAO", x => new ColumnInfo(SituacionDb.ContenedorTransferido)},
                { "NU_UNIDAD_TRANSPORTE", x => new ColumnInfo(null,DbType.Int32)},
                { "NU_TRANSACCION", x => new ColumnInfo(nuTransaccion)},
            }, new Dictionary<string, Func<DetallePreparacion, ColumnInfo>>
            {
                { "NU_PREPARACION", x => new ColumnInfo(x.NumeroPreparacion)},
                { "NU_CONTENEDOR", x => new ColumnInfo(x.NroContenedor)}
            });
        }

        public virtual void ProcesarTraspasoDeleteUnidadTransporte(List<UnidadTransporte> uts, long nuTransaccion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            _dapper.BulkInsert(connection, tran, uts, "T_CONTENEDOR_TEMP", new Dictionary<string, Func<UnidadTransporte, ColumnInfo>>
            {
                    { "NU_UNIDAD_TRANSPORTE", x => new ColumnInfo(x.NumeroUnidadTransporte,DbType.Int32)},
            });

            var alias = "ut";
            var from = @" T_UNIDAD_TRANSPORTE ut
                          INNER JOIN ( 
                                SELECT ct.NU_UNIDAD_TRANSPORTE
                                FROM (
                                    SELECT 
                                       ct.NU_UNIDAD_TRANSPORTE,
                                       COUNT(c.NU_CONTENEDOR) QT_CONTENEDOR
                                    FROM T_CONTENEDOR_TEMP ct 
                                    LEFT JOIN T_CONTENEDOR c on c.NU_UNIDAD_TRANSPORTE = ct.NU_UNIDAD_TRANSPORTE
                                    GROUP BY
                                       ct.NU_UNIDAD_TRANSPORTE
                                ) ct WHERE ct.QT_CONTENEDOR = 0
                            ) tmp ON 
                            tmp.NU_UNIDAD_TRANSPORTE = ut.NU_UNIDAD_TRANSPORTE";

            var where = "";

            _dapper.ExecuteDelete(connection, alias, from, where, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());

        }

        #endregion

        #region Documento Egreso

        public virtual void GenerarDocumentoSalidaTraspaso(DocumentoEgreso documentoSalida, long nuTransaccion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            GenerarDocumentoSalida(connection, tran, documentoSalida, nuTransaccion);
            UpdateDeleteReservaPreparacion(connection, tran, nuTransaccion);
            DeleteReservaPreparacion(connection, tran);
            UpdateReservaPreparacion(connection, tran, nuTransaccion);
            UpdateDetalleDocumento(connection, tran, nuTransaccion);
            UpdateDetalleDocumentoSalida(connection, tran, nuTransaccion, documentoSalida);
            AddDetalleDocumentoSalida(connection, tran, documentoSalida);

        }

        public virtual void GenerarDocumentoSalida(DbConnection connection, DbTransaction tran, DocumentoEgreso documentoSalida, long nuTransaccion)
        {
            var sqlInsertNew = $@"
                INSERT INTO T_DOCUMENTO (NU_DOCUMENTO,TP_DOCUMENTO,CD_EMPRESA,CD_FUNCIONARIO,VL_ARBITRAJE,
                    CD_SITUACAO,DT_ADDROW,ID_ESTADO,ID_GENERAR_AGENDA,VL_VALIDADO,VL_DATO_AUDITORIA,NU_PREDIO)
                VALUES (:Numero,:Tipo,:Empresa,:Usuario,:ValorArbitraje,:Situacion,:FechaAlta,:Estado,'N','N',:Auditoria,:Predio)";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, documentoSalida, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateDetalleDocumentoSalida(DbConnection connection, DbTransaction tran, long nuTransaccion, DocumentoEgreso documentoSalida)
        {
            var alias = "dd";
            var from = $@"
                T_DET_DOCUMENTO_EGRESO dd
                          INNER JOIN ( 
                                SELECT 
                                   dde.NU_DOCUMENTO,
                                   dde.TP_DOCUMENTO,
                                   dde.NU_DOCUMENTO_INGRESO ,
                                   dde.TP_DOCUMENTO_INGRESO ,
                                   dde.CD_EMPRESA,
                                   dde.CD_PRODUTO,
                                   dde.CD_FAIXA,
                                   dde.NU_IDENTIFICADOR,
                                   SUM(temp.QT_PRODUTO) QT_PRODUTO_TEMP
                                FROM T_DOC_PREPARACION_RESERV_TEMP temp
                                INNER JOIN T_DET_DOCUMENTO_EGRESO dde ON  dde.NU_DOCUMENTO = :Numero AND
                                    dde.TP_DOCUMENTO = :Tipo AND
                                    dde.NU_DOCUMENTO_INGRESO = temp.NU_DOCUMENTO AND
                                    dde.TP_DOCUMENTO_INGRESO = temp.TP_DOCUMENTO AND  
                                    dde.CD_EMPRESA = temp.CD_EMPRESA AND
                                    dde.CD_PRODUTO = temp.CD_PRODUTO AND
                                    dde.CD_FAIXA = temp.CD_FAIXA AND
                                    dde.NU_IDENTIFICADOR = temp.NU_IDENTIFICADOR 
                                    GROUP BY dde.NU_DOCUMENTO,
                                        dde.TP_DOCUMENTO,
                                        dde.NU_DOCUMENTO_INGRESO,
                                        dde.TP_DOCUMENTO_INGRESO,
                                        dde.CD_EMPRESA, 
                                        dde.CD_PRODUTO,
                                        dde.CD_FAIXA,
                                        dde.NU_IDENTIFICADOR
                               ) tmp ON 
                            tmp.NU_DOCUMENTO = dd.NU_DOCUMENTO AND
                            tmp.TP_DOCUMENTO = dd.TP_DOCUMENTO AND
                            tmp.NU_DOCUMENTO_INGRESO = dd.NU_DOCUMENTO_INGRESO AND
                            tmp.TP_DOCUMENTO_INGRESO = dd.TP_DOCUMENTO_INGRESO AND  
                            tmp.CD_EMPRESA = dd.CD_EMPRESA AND
                            tmp.CD_PRODUTO = dd.CD_PRODUTO AND
                            tmp.CD_FAIXA = dd.CD_FAIXA AND
                            tmp.NU_IDENTIFICADOR = dd.NU_IDENTIFICADOR";

            var set = @"
                DT_UPDROW = :FechaModificacion,
                VL_DATO_AUDITORIA = :Transaccion,
                QT_DESAFECTADA = QT_DESAFECTADA + QT_PRODUTO_TEMP";


            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion, Numero = documentoSalida.Numero, Tipo = documentoSalida.Tipo }, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void AddDetalleDocumentoSalida(DbConnection connection, DbTransaction tran, DocumentoEgreso documentoSalida)
        {
            var sqlInsertNew = @"
                INSERT INTO T_DET_DOCUMENTO_EGRESO (
                   NU_DOCUMENTO,TP_DOCUMENTO,NU_SECUENCIA,NU_DOCUMENTO_INGRESO,TP_DOCUMENTO_INGRESO,
                   CD_EMPRESA,CD_PRODUTO,CD_FAIXA,NU_IDENTIFICADOR,QT_DESAFECTADA,VL_DATO_AUDITORIA,DT_ADDROW)
                SELECT 
                    MIN(:Numero) NU_DOCUMENTO,
                    MIN(:Tipo) TP_DOCUMENTO,
                    MIN((COALESCE(ddem.NU_SECUENCIA, 0) + temp.NU_SECUENCIA)) NU_SECUENCIA,
                    temp.NU_DOCUMENTO NU_DOCUMENTO_INGRESO,
                    temp.TP_DOCUMENTO TP_DOCUMENTO_INGRESO,
                    temp.CD_EMPRESA,
                    temp.CD_PRODUTO,
                    temp.CD_FAIXA,
                    temp.NU_IDENTIFICADOR,
                    SUM(temp.QT_PRODUTO) QT_PRODUTO_TEMP,
                    MIN(:Auditoria),
                    MIN(:FechaAlta)
                FROM T_DOC_PREPARACION_RESERV_TEMP temp
                LEFT JOIN (SELECT 
                                NU_DOCUMENTO,
                                TP_DOCUMENTO,
                                MAX(NU_SECUENCIA) NU_SECUENCIA
                            FROM T_DET_DOCUMENTO_EGRESO GROUP BY  NU_DOCUMENTO, TP_DOCUMENTO
                    ) ddem on ddem.NU_DOCUMENTO = :Numero AND
                    ddem.TP_DOCUMENTO = :Tipo
                LEFT JOIN (SELECT 
                                NU_DOCUMENTO,
                                TP_DOCUMENTO,
                                NU_DOCUMENTO_INGRESO,
                                TP_DOCUMENTO_INGRESO,
                                CD_EMPRESA,
                                CD_FAIXA,
                                CD_PRODUTO,
                                NU_IDENTIFICADOR 
                            FROM T_DET_DOCUMENTO_EGRESO
                    ) dde on dde.NU_DOCUMENTO = :Numero AND 
                    dde.TP_DOCUMENTO = :Tipo AND
                    dde.NU_DOCUMENTO_INGRESO = temp.NU_DOCUMENTO AND
                    dde.TP_DOCUMENTO_INGRESO = temp.TP_DOCUMENTO AND
                    dde.CD_EMPRESA = temp.CD_EMPRESA AND
                    dde.CD_PRODUTO = temp.CD_PRODUTO AND
                    dde.CD_FAIXA = temp.CD_FAIXA AND
                    dde.NU_IDENTIFICADOR = temp.NU_IDENTIFICADOR
                WHERE dde.NU_DOCUMENTO is null
                GROUP BY temp.NU_DOCUMENTO,
                    temp.TP_DOCUMENTO,
                    temp.CD_EMPRESA,
                    temp.CD_PRODUTO,
                    temp.CD_FAIXA,
                    temp.NU_IDENTIFICADOR";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, documentoSalida, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateDetalleDocumento(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "dd";
            var from = $@"
                T_DET_DOCUMENTO dd
                          INNER JOIN ( 
                                SELECT 
                                   NU_DOCUMENTO,
                                   TP_DOCUMENTO,
                                   CD_EMPRESA,
                                   CD_PRODUTO,
                                   CD_FAIXA,
                                   NU_IDENTIFICADOR,
                                   SUM(QT_PRODUTO) QT_PRODUTO_TEMP
                                FROM T_DOC_PREPARACION_RESERV_TEMP 
                                GROUP BY
                                    NU_DOCUMENTO,
                                   TP_DOCUMENTO,
                                   CD_EMPRESA,
                                   CD_PRODUTO,
                                   CD_FAIXA,
                                   NU_IDENTIFICADOR) tmp ON 
                            tmp.NU_DOCUMENTO = dd.NU_DOCUMENTO AND
                            tmp.TP_DOCUMENTO = dd.TP_DOCUMENTO AND
                            tmp.CD_EMPRESA = dd.CD_EMPRESA AND
                            tmp.CD_PRODUTO = dd.CD_PRODUTO AND
                            tmp.CD_FAIXA = dd.CD_FAIXA AND
                            tmp.NU_IDENTIFICADOR = dd.NU_IDENTIFICADOR";

            var set = @"
                DT_UPDROW = :FechaModificacion,
                VL_DATO_AUDITORIA = :Transaccion,
                QT_RESERVADA = QT_RESERVADA - QT_PRODUTO_TEMP,
                QT_DESAFECTADA = COALESCE(QT_DESAFECTADA,0) + QT_PRODUTO_TEMP";


            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void UpdateReservaPreparacion(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "dpr";
            var from = $@"
            T_DOCUMENTO_PREPARACION_RESERV dpr
                          INNER JOIN ( 
                                SELECT 
                                   NU_DOCUMENTO,
                                   TP_DOCUMENTO,
                                   NU_PREPARACION,
                                   CD_EMPRESA,
                                   CD_PRODUTO,
                                   CD_FAIXA,
                                   NU_IDENTIFICADOR,
                                   ID_ESPECIFICA_IDENTIFICADOR,
                                   NU_IDENTIFICADOR_PICKING_DET,
                                   SUM(QT_PRODUTO) QT_PRODUTO_TEMP
                                FROM T_DOC_PREPARACION_RESERV_TEMP 
                                GROUP BY
                                    NU_DOCUMENTO,
                                   TP_DOCUMENTO,
                                   NU_PREPARACION,
                                   CD_EMPRESA,
                                   CD_PRODUTO,
                                   CD_FAIXA,
                                   NU_IDENTIFICADOR,
                                   ID_ESPECIFICA_IDENTIFICADOR,
                                   NU_IDENTIFICADOR_PICKING_DET ) tmp ON 
                            tmp.NU_DOCUMENTO = dpr.NU_DOCUMENTO AND
                            tmp.TP_DOCUMENTO = dpr.TP_DOCUMENTO AND
                            tmp.NU_PREPARACION = dpr.NU_PREPARACION AND
                            tmp.CD_EMPRESA = dpr.CD_EMPRESA AND
                            tmp.CD_PRODUTO = dpr.CD_PRODUTO AND
                            tmp.CD_FAIXA = dpr.CD_FAIXA AND
                            tmp.NU_IDENTIFICADOR = dpr.NU_IDENTIFICADOR AND
                            tmp.ID_ESPECIFICA_IDENTIFICADOR = dpr.ID_ESPECIFICA_IDENTIFICADOR AND
                            tmp.NU_IDENTIFICADOR_PICKING_DET = dpr.NU_IDENTIFICADOR_PICKING_DET";
            var set = @"
                DT_UPDROW = :FechaModificacion,
                VL_DATO_AUDITORIA = :Transaccion,
                QT_PRODUTO = COALESCE(QT_PRODUTO,0) - QT_PRODUTO_TEMP,
                QT_PREPARADO = COALESCE(QT_PREPARADO,0) - QT_PRODUTO_TEMP";


            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void UpdateDeleteReservaPreparacion(DbConnection connection, DbTransaction tran, long nuTransaccion)
        {
            var alias = "dpr";
            var from = @" T_DOCUMENTO_PREPARACION_RESERV dpr
                          INNER JOIN ( 
                                SELECT 
                                   NU_DOCUMENTO,
                                   TP_DOCUMENTO,
                                   NU_PREPARACION,
                                   CD_EMPRESA,
                                   CD_PRODUTO,
                                   CD_FAIXA,
                                   NU_IDENTIFICADOR,
                                   ID_ESPECIFICA_IDENTIFICADOR,
                                   NU_IDENTIFICADOR_PICKING_DET,
                                   SUM(QT_PRODUTO) QT_PRODUTO_TEMP
                                FROM T_DOC_PREPARACION_RESERV_TEMP 
                                GROUP BY
                                    NU_DOCUMENTO,
                                   TP_DOCUMENTO,
                                   NU_PREPARACION,
                                   CD_EMPRESA,
                                   CD_PRODUTO,
                                   CD_FAIXA,
                                   NU_IDENTIFICADOR,
                                   ID_ESPECIFICA_IDENTIFICADOR,
                                   NU_IDENTIFICADOR_PICKING_DET ) tmp ON 
                            tmp.NU_DOCUMENTO = dpr.NU_DOCUMENTO AND
                            tmp.TP_DOCUMENTO = dpr.TP_DOCUMENTO AND
                            tmp.NU_PREPARACION = dpr.NU_PREPARACION AND
                            tmp.CD_EMPRESA = dpr.CD_EMPRESA AND
                            tmp.CD_PRODUTO = dpr.CD_PRODUTO AND
                            tmp.CD_FAIXA = dpr.CD_FAIXA AND
                            tmp.NU_IDENTIFICADOR = dpr.NU_IDENTIFICADOR AND
                            tmp.ID_ESPECIFICA_IDENTIFICADOR = dpr.ID_ESPECIFICA_IDENTIFICADOR AND
                            tmp.NU_IDENTIFICADOR_PICKING_DET = dpr.NU_IDENTIFICADOR_PICKING_DET";

            var set = @"NU_TRANSACCION_DELETE = :Transaccion,
                DT_UPDROW = :FechaModificacion";

            var where = "QT_PRODUTO = QT_PRODUTO_TEMP";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion }, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void DeleteReservaPreparacion(DbConnection connection, DbTransaction tran)
        {
            var alias = "dpr";
            var from = @" T_DOCUMENTO_PREPARACION_RESERV dpr
                          INNER JOIN ( 
                                SELECT 
                                   NU_DOCUMENTO,
                                   TP_DOCUMENTO,
                                   NU_PREPARACION,
                                   CD_EMPRESA,
                                   CD_PRODUTO,
                                   CD_FAIXA,
                                   NU_IDENTIFICADOR,
                                   ID_ESPECIFICA_IDENTIFICADOR,
                                   NU_IDENTIFICADOR_PICKING_DET,
                                   SUM(QT_PRODUTO) QT_PRODUTO_TEMP
                                FROM T_DOC_PREPARACION_RESERV_TEMP 
                                GROUP BY
                                    NU_DOCUMENTO,
                                   TP_DOCUMENTO,
                                   NU_PREPARACION,
                                   CD_EMPRESA,
                                   CD_PRODUTO,
                                   CD_FAIXA,
                                   NU_IDENTIFICADOR,
                                   ID_ESPECIFICA_IDENTIFICADOR,
                                   NU_IDENTIFICADOR_PICKING_DET ) tmp ON 
                            tmp.NU_DOCUMENTO = dpr.NU_DOCUMENTO AND
                            tmp.TP_DOCUMENTO = dpr.TP_DOCUMENTO AND
                            tmp.NU_PREPARACION = dpr.NU_PREPARACION AND
                            tmp.CD_EMPRESA = dpr.CD_EMPRESA AND
                            tmp.CD_PRODUTO = dpr.CD_PRODUTO AND
                            tmp.CD_FAIXA = dpr.CD_FAIXA AND
                            tmp.NU_IDENTIFICADOR = dpr.NU_IDENTIFICADOR AND
                            tmp.ID_ESPECIFICA_IDENTIFICADOR = dpr.ID_ESPECIFICA_IDENTIFICADOR AND
                            tmp.NU_IDENTIFICADOR_PICKING_DET = dpr.NU_IDENTIFICADOR_PICKING_DET";

            var where = "QT_PRODUTO = QT_PRODUTO_TEMP";

            _dapper.ExecuteDelete(connection, alias, from, where, param: new DynamicParameters(new { FechaActual = DateTime.Now }), commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion

        #region Documento Ingreso

        public virtual void GenerarDocumentoEntradaTraspaso(DocumentoIngreso documentoIngreso, long nuTransaccion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            GenerarDocumentoIngreso(connection, tran, documentoIngreso, nuTransaccion);
            UpdateDetalleDocumentoIngreso(connection, tran, nuTransaccion, documentoIngreso);
            AddDetalleDocumentoIngreso(connection, tran, documentoIngreso);
        }

        public virtual void GenerarDocumentoIngreso(DbConnection connection, DbTransaction tran, DocumentoIngreso documentoingreso, long nuTransaccion)
        {
            var sqlInsertNew = $@"
                INSERT INTO T_DOCUMENTO (NU_DOCUMENTO,TP_DOCUMENTO,CD_EMPRESA,CD_FUNCIONARIO,VL_ARBITRAJE,
                    CD_SITUACAO,DT_ADDROW,ID_ESTADO,ID_GENERAR_AGENDA,VL_VALIDADO,VL_DATO_AUDITORIA,NU_PREDIO)
                VALUES (:Numero,:Tipo,:Empresa,:Usuario,:ValorArbitraje,:Situacion,:FechaAlta,:Estado,'N','N',:Auditoria,:Predio)";

            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, documentoingreso, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateDetalleDocumentoIngreso(DbConnection connection, DbTransaction tran, long nuTransaccion, DocumentoIngreso documentoIngreso)
        {
            var alias = "dd";
            var from = $@"
                T_DET_DOCUMENTO dd
                          INNER JOIN ( 
                                SELECT 
                                   dd.NU_DOCUMENTO,
                                   dd.TP_DOCUMENTO,
                                   dd.CD_EMPRESA,
                                   dd.CD_PRODUTO,
                                   dd.CD_FAIXA,
                                   dd.NU_IDENTIFICADOR,
                                   SUM(temp.QT_ESTOQUE_DEST) QT_PRODUTO_TEMP
                                FROM T_TRASPASO_DET_TEMP temp
                                INNER JOIN T_DET_DOCUMENTO dd ON  dd.NU_DOCUMENTO = :Numero AND
                                    dd.TP_DOCUMENTO = :Tipo AND
                                    dd.CD_EMPRESA = temp.CD_EMPRESA_DEST AND
                                    dd.CD_PRODUTO = temp.CD_PRODUTO_DEST AND
                                    dd.CD_FAIXA = temp.CD_FAIXA AND
                                    dd.NU_IDENTIFICADOR = temp.NU_IDENTIFICADOR_DEST
                                GROUP BY
                                   dd.NU_DOCUMENTO,
                                   dd.TP_DOCUMENTO,
                                   dd.CD_EMPRESA,
                                   dd.CD_PRODUTO,
                                   dd.CD_FAIXA,
                                   dd.NU_IDENTIFICADOR) tmp ON 
                            tmp.NU_DOCUMENTO = dd.NU_DOCUMENTO AND
                            tmp.TP_DOCUMENTO = dd.TP_DOCUMENTO AND
                            tmp.CD_EMPRESA = dd.CD_EMPRESA AND
                            tmp.CD_PRODUTO = dd.CD_PRODUTO AND
                            tmp.CD_FAIXA = dd.CD_FAIXA AND
                            tmp.NU_IDENTIFICADOR = dd.NU_IDENTIFICADOR";

            var set = @"
                DT_UPDROW = :FechaModificacion,
                VL_DATO_AUDITORIA = :Transaccion,
                QT_INGRESADA = QT_INGRESADA + QT_PRODUTO_TEMP";


            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion, Numero = documentoIngreso.Numero, Tipo = documentoIngreso.Tipo }, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void AddDetalleDocumentoIngreso(DbConnection connection, DbTransaction tran, DocumentoIngreso documentoIngreso)
        {
            var sqlInsertNew = @"
                INSERT INTO T_DET_DOCUMENTO (
                   NU_DOCUMENTO,TP_DOCUMENTO,
                   CD_EMPRESA,CD_PRODUTO,CD_FAIXA,NU_IDENTIFICADOR,QT_INGRESADA,QT_RESERVADA,QT_DESAFECTADA,VL_DATO_AUDITORIA,DT_ADDROW)
                SELECT 
                    MIN(:Numero) NU_DOCUMENTO,
                    MIN(:Tipo) TP_DOCUMENTO,
                    temp.CD_EMPRESA_DEST,
                    temp.CD_PRODUTO_DEST,
                    temp.CD_FAIXA,
                    temp.NU_IDENTIFICADOR_DEST,
                    SUM(temp.QT_ESTOQUE_DEST) QT_PRODUTO_TEMP,
                    MIN(0),
                    MIN(0),
                    MIN(:Auditoria),
                    MIN(:FechaAlta)
                FROM T_TRASPASO_DET_TEMP temp
                LEFT JOIN (SELECT 
                                NU_DOCUMENTO,
                                TP_DOCUMENTO,
                                CD_EMPRESA,
                                CD_PRODUTO,
                                CD_FAIXA,
                                NU_IDENTIFICADOR 
                            FROM T_DET_DOCUMENTO
                    ) dde on dde.NU_DOCUMENTO = :Numero AND 
                    dde.TP_DOCUMENTO = :Tipo AND
                    dde.CD_EMPRESA = temp.CD_EMPRESA AND
                    dde.CD_PRODUTO = temp.CD_PRODUTO AND
                    dde.CD_FAIXA = temp.CD_FAIXA AND
                    dde.NU_IDENTIFICADOR = temp.NU_IDENTIFICADOR
                WHERE dde.NU_DOCUMENTO is null
                GROUP BY 
                    temp.CD_EMPRESA_DEST,
                    temp.CD_PRODUTO_DEST,
                    temp.CD_FAIXA,
                    temp.NU_IDENTIFICADOR_DEST";


            _dapper.Execute(_context.Database.GetDbConnection(), sqlInsertNew, documentoIngreso, commandType: CommandType.Text, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual void UpdateDetalleDocumentoPreparacionReserva(DbConnection connection, DbTransaction tran, long nuTransaccion, DocumentoIngreso documentoIngreso)
        {
            var alias = "dd";
            var from = $@"
                T_DET_DOCUMENTO dd
                          INNER JOIN ( 
                                SELECT 
                                   dd.NU_DOCUMENTO,
                                   dd.TP_DOCUMENTO,
                                   dd.CD_EMPRESA,
                                   dd.CD_PRODUTO,
                                   dd.CD_FAIXA,
                                   dd.NU_IDENTIFICADOR,
                                   SUM(temp.QT_ESTOQUE_DEST) QT_PRODUTO_TEMP
                                FROM T_TRASPASO_DET_TEMP temp
                                INNER JOIN T_DET_DOCUMENTO dd on  dd.NU_DOCUMENTO = :Numero AND
                                    dd.TP_DOCUMENTO = :Tipo AND
                                    dd.CD_EMPRESA = temp.CD_EMPRESA_DEST AND
                                    dd.CD_PRODUTO = temp.CD_PRODUTO_DEST AND
                                    dd.CD_FAIXA = temp.CD_FAIXA AND
                                    dd.NU_IDENTIFICADOR = temp.NU_IDENTIFICADOR_DEST
                                GROUP BY
                                   dd.NU_DOCUMENTO,
                                   dd.TP_DOCUMENTO,
                                   dd.CD_EMPRESA,
                                   dd.CD_PRODUTO,
                                   dd.CD_FAIXA,
                                   dd.NU_IDENTIFICADOR) tmp ON 
                            tmp.NU_DOCUMENTO = dd.NU_DOCUMENTO AND
                            tmp.TP_DOCUMENTO = dd.TP_DOCUMENTO AND
                            tmp.CD_EMPRESA = dd.CD_EMPRESA AND
                            tmp.CD_PRODUTO = dd.CD_PRODUTO AND
                            tmp.CD_FAIXA = dd.CD_FAIXA AND
                            tmp.NU_IDENTIFICADOR = dd.NU_IDENTIFICADOR";

            var set = @"
                DT_UPDROW = :FechaModificacion,
                VL_DATO_AUDITORIA = :Transaccion,
                QT_RESERVADA = QT_RESERVADA + QT_PRODUTO_TEMP";


            var where = "";

            _dapper.ExecuteUpdate(connection, alias, from, set, where, param: new { FechaModificacion = DateTime.Now, Transaccion = nuTransaccion, Numero = documentoIngreso.Numero, Tipo = documentoIngreso.Tipo }, commandType: CommandType.Text, transaction: tran);
        }

        public virtual void AddPreparacionReservaPreparado(DbConnection connection, DbTransaction tran, int preparacion, long nuTransaccion, DocumentoIngreso documentoIngreso, bool isPendientePreparar = false)
        {
            string sql = $@"INSERT INTO T_DOCUMENTO_PREPARACION_RESERV
                        (NU_DOCUMENTO,
                        TP_DOCUMENTO,
                        NU_PREPARACION,
                        CD_EMPRESA,
                        CD_PRODUTO,
                        CD_FAIXA,
                        NU_IDENTIFICADOR,
                        ID_ESPECIFICA_IDENTIFICADOR,
                        NU_IDENTIFICADOR_PICKING_DET,
                        QT_PRODUTO,
                        QT_ANULAR,
                        VL_DATO_AUDITORIA,
                        QT_PREPARADO,
                        DT_ADDROW) 
                        (SELECT  
                            MIN(:NU_DOCUMENTO),                          
                            MIN(:TP_DOCUMENTO),                     
                            MIN(:NU_PREPARACION),                     
                            dprt.CD_EMPRESA_DEST,                       
                            dprt.CD_PRODUTO_DEST,                                           
                            dprt.CD_FAIXA,                             
                            dprt.NU_IDENTIFICADOR_DEST,                  
                            MIN(:ID_ESPECIFICA_IDENTIFICADOR),         
                            dprt.NU_IDENTIFICADOR_DEST,               
                            SUM(dprt.QT_ESTOQUE_DEST) QT_ESTOQUE_DEST,             
                            MIN(0) QT_ANULAR,            
                            MIN(:VL_DATO_AUDITORIA),                    
                            {(isPendientePreparar ? "MIN(0)" : "SUM(dprt.QT_ESTOQUE_DEST) QT_PREPARADO")},            
                            MIN(:FechaAlta)
                        FROM T_TRASPASO_DET_TEMP dprt
                        GROUP BY                     
                            dprt.CD_EMPRESA_DEST,                       
                            dprt.CD_PRODUTO_DEST,                                           
                            dprt.CD_FAIXA,                             
                            dprt.NU_IDENTIFICADOR_DEST,                  
                            dprt.NU_IDENTIFICADOR_DEST
                        )";

            _dapper.Execute(connection, sql, param: new { NU_PREPARACION = preparacion, NU_DOCUMENTO = documentoIngreso.Numero, TP_DOCUMENTO = documentoIngreso.Tipo, VL_DATO_AUDITORIA = nuTransaccion, FechaAlta = DateTime.Now, ID_ESPECIFICA_IDENTIFICADOR = "S" }, transaction: tran);
        }

        #endregion

        public virtual void BorrarTablasTemporalesTraspasoEmpresa()
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();


            string sql = @"DELETE FROM T_TRASPASO_DET_TEMP";
            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);

            sql = @"DELETE FROM T_TRASPASO_DET_LPN_TEMP";
            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);

            sql = @"DELETE FROM T_DOC_PREPARACION_RESERV_TEMP";
            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);

            sql = @"DELETE FROM T_CONTENEDOR_TEMP";
            _dapper.Execute(connection, sql, commandType: CommandType.Text, transaction: tran);

        }

        #endregion

        #endregion

    }
}
