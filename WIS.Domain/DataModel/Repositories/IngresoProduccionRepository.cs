using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.General.API;
using WIS.Domain.General.API.Bulks;
using WIS.Domain.General.API.Dtos.Salida;
using WIS.Domain.General.Enums;
using WIS.Domain.Interfaces;
using WIS.Domain.Inventario;
using WIS.Domain.Picking;
using WIS.Domain.Produccion;
using WIS.Domain.Produccion.Constants;
using WIS.Domain.Produccion.DTOs;
using WIS.Domain.Produccion.Mappers;
using WIS.Domain.Produccion.Models;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Exceptions;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class IngresoProduccionRepository
    {
        protected readonly IngresoProduccionMapper _mapperIngresoProduccion;
        protected readonly EspacioProduccionMapper _mapperEspacioProduccion;
        protected readonly PreparacionMapper _mapperPreparacion;
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();
        protected readonly TransaccionRepository _transaccionRepository;
        protected readonly StockRepository _stockRepository;
        protected readonly AjusteRepository _ajusteRepository;
        protected readonly ParametroRepository _parametroRepository;

        protected readonly WISDB _context;
        protected readonly IDapper _dapper;
        protected readonly string _application;
        protected readonly int _userId;

        public IngresoProduccionRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._mapperIngresoProduccion = new IngresoProduccionMapper();
            this._mapperEspacioProduccion = new EspacioProduccionMapper();
            this._mapperPreparacion = new PreparacionMapper();
            this._dapper = dapper;
            this._transaccionRepository = new TransaccionRepository(context, cdAplicacion, userId, dapper);
            this._stockRepository = new StockRepository(context, cdAplicacion, userId, dapper);
            this._ajusteRepository = new AjusteRepository(context, cdAplicacion, userId, dapper);
            this._context = context;
            this._application = cdAplicacion;
            this._userId = userId;
            this._parametroRepository = new ParametroRepository(_context, cdAplicacion, userId, dapper);
        }

        #region Any

        public virtual bool ExisteIngresoByIdExternoEmpresa(string idExterno, int empresa)
        {
            return _context.T_PRDC_INGRESO
                .Any(w => w.ID_PRODUCCION_EXTERNO.ToUpper() == idExterno.ToUpper() && w.CD_EMPRESA == empresa);
        }

        public virtual bool IngresoDetalleTieneDetalleSalidaReal(string nuProduccion, string codigoProducto, int empresa, string identificador)
        {
            return _context.T_PRDC_DET_SALIDA_REAL.Any(f => f.NU_PRDC_INGRESO == nuProduccion &&
                                                                             f.CD_PRODUTO == codigoProducto &&
                                                                             f.CD_EMPRESA == empresa &&
                                                                             (f.NU_IDENTIFICADOR == identificador || identificador == ManejoIdentificadorDb.IdentificadorAuto));
        }

        public virtual bool HayDiferenciasEnProduccion(string nuIngresoProduccion)
        {
            return _context.V_PRODUCIDOS_PRODUCCION.AsNoTracking().Any(f => f.NU_PRDC_INGRESO == nuIngresoProduccion && f.FL_DIFERENCIA == "S");
        }

        public virtual bool HayDiferenciasEnInsumosConsumidos(string nuIngresoProduccion)
        {
            return _context.V_CONSUMIDOS_PRODUCCION.AsNoTracking().Any(f => f.NU_PRDC_INGRESO == nuIngresoProduccion && f.FL_DIFERENCIA == "S");
        }

        public virtual bool HayStockProducidoEnEspacio(string ubicacionProduccion)
        {
            return _context.V_PRD113_REMANENTE_PRODUCIDO.AsNoTracking().Any(f => f.CD_ENDERECO == ubicacionProduccion && f.QT_ESTOQUE > 0);
        }

        public virtual bool HayStockInsumosEnEspacio(string nuIngresoProduccion)
        {
            return _context.V_PRD113_STOCK_INSUMOS.AsNoTracking().Any(f => f.NU_PRDC_INGRESO == nuIngresoProduccion && f.QT_REAL > 0);
        }

        public virtual bool PuedeEditarseIngresoTeorico(IngresoProduccion ingreso)
        {
            bool isdetalleTeoricoEditable = false;

            if (ingreso.CantidadIteracionesFormula == 0 && !_context.T_PEDIDO_SAIDA.Any(x => x.NU_PRDC_INGRESO == ingreso.Id) && (ingreso.IdManual ?? "S") == "S")
            {
                isdetalleTeoricoEditable = true;
            }

            return isdetalleTeoricoEditable;
        }

        public virtual bool StockInsumoConsumible(string nuIngresoProduccion, string producto, int empresa, decimal faixa, string identificador)
        {
            return _context.V_PRD113_STOCK_INSUMOS
                .AsNoTracking()
                .Any(f => f.NU_PRDC_INGRESO == nuIngresoProduccion
                    && f.CD_PRODUTO == producto
                    && f.CD_EMPRESA == empresa
                    && f.CD_FAIXA == faixa
                    && f.NU_IDENTIFICADOR == identificador
                    && f.FL_CONSUMIBLE == "S"
                    && f.QT_REAL > 0);
        }

        public virtual bool AnyInsumosReales(string nuPrdcIngreso)
        {
            return _context.T_PRDC_DET_INGRESO_REAL.Any(w => w.NU_PRDC_INGRESO == nuPrdcIngreso && w.QT_REAL > 0);
        }

        public virtual bool AnyConsumoInsumosReales(string nuPrdcIngreso)
        {
            return _context.T_PRDC_DET_INGRESO_REAL.Any(w => w.NU_PRDC_INGRESO == nuPrdcIngreso && w.QT_DESAFECTADA > 0);
        }

        public virtual bool AnySalidasReales(string nuPrdcIngreso)
        {
            return _context.T_PRDC_DET_SALIDA_REAL.Any(w => w.NU_PRDC_INGRESO == nuPrdcIngreso && w.QT_PRODUCIDO > 0);
        }

        #endregion

        #region Get

        public virtual IngresoProduccion GetIngresoByIdConDetalles(string id)
        {
            var ingreso = _context.T_PRDC_INGRESO.FirstOrDefault(w => w.NU_PRDC_INGRESO == id);

            if (ingreso == null)
                throw new ValidationFailedException("General_Sec0_Error_IdProduccionExternoEmpresaNoExiste", new string[] { id });

            return GetIngresoDetalles(ingreso);
        }

        public virtual IngresoProduccion GetIngresoByIdExternoEmpresaConDetalles(string idExterno, int empresa)
        {
            var ingreso = _context.T_PRDC_INGRESO.FirstOrDefault(w => w.ID_PRODUCCION_EXTERNO == idExterno && w.CD_EMPRESA == empresa);

            if (ingreso == null)
                return null;

            return GetIngresoDetalles(ingreso);
        }

        public virtual IngresoProduccion GetIngresoDetalles(T_PRDC_INGRESO ingreso)
        {
            var espacioProduccion = _context.T_PRDC_LINEA.FirstOrDefault(f => f.CD_PRDC_LINEA == ingreso.CD_PRDC_LINEA);

            var detalles = _context.T_PRDC_DET_INGRESO_TEORICO.Where(w => w.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO);

            var tempPedioInsumos = _context.T_PRDC_INGRESO_DET_PEDIDO_TEMP.Where(w => w.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO).ToList();

            var obj = _mapperIngresoProduccion.MapEntityToObject(ingreso);

            var insumos = _context.T_PRDC_DET_INGRESO_REAL.Where(w => w.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO).ToList();
            obj.Consumidos = _mapperIngresoProduccion.MapEntityToObject(insumos);

            var producidos = _context.T_PRDC_DET_SALIDA_REAL.Where(w => w.NU_PRDC_INGRESO == ingreso.NU_PRDC_INGRESO).ToList();
            obj.Producidos = _mapperIngresoProduccion.MapEntityToObject(producidos);

            if (espacioProduccion != null)
                obj.EspacioProduccion = _mapperEspacioProduccion.MapToObjet(espacioProduccion);

            if (detalles != null)
                obj.Detalles = detalles.Select(s => _mapperIngresoProduccion.MapEntityToObject(s)).ToList();
            return obj;
        }

        public virtual IngresoProduccion GetIngresoById(string id)
        {
            var ingreso = _context.T_PRDC_INGRESO.FirstOrDefault(w => w.NU_PRDC_INGRESO == id);

            return _mapperIngresoProduccion.MapEntityToObject(ingreso);
        }

        public virtual IngresoProduccion GetIngresoActivoEnEspacio(string idEspacio)
        {
            try
            {
                var ingreso = _context.T_PRDC_INGRESO.AsNoTracking().FirstOrDefault(w => w.CD_PRDC_LINEA == idEspacio && SituacionDb.SITUACIONES_PRODUCCION_ACTIVA.Contains(w.CD_SITUACAO ?? 0));

                if (ingreso == null)
                    return null;

                return _mapperIngresoProduccion.MapEntityToObject(ingreso);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public virtual int GetNextPosicionEnCola(string idEspacio)
        {
            var ingresos = _context.T_PRDC_INGRESO.Where(w => w.CD_PRDC_LINEA == idEspacio && w.NU_POSICION_EN_COLA != null);

            if (ingresos.Count() > 0) return (int)(ingresos.OrderByDescending(o => o.NU_POSICION_EN_COLA).FirstOrDefault().NU_POSICION_EN_COLA + 1);
            else return 1;
        }

        public virtual long GetNextNuDetalleReal()
        {
            return _context.GetNextSequenceValueLong(_dapper, Secuencias.S_NU_PRDC_INGRESO_REAL);
        }

        public virtual long? GetNextValueNuOrdenDetalleReal(string nuIngreso)
        {
            var ordenVolcadoUltimoValor = _context.T_PRDC_DET_INGRESO_REAL
                                                    .Where(x => x.NU_PRDC_INGRESO == nuIngreso)
                                                    .Max(x => x.NU_ORDEN);

            return ordenVolcadoUltimoValor + 1;

        }

        public virtual long GetNextNuDetalleSalidaReal()
        {
            return _context.GetNextSequenceValueLong(_dapper, Secuencias.S_NU_PRDC_SALIDA_REAL);
        }

        public virtual long GetNextIngresoRealMov()
        {
            return _context.GetNextSequenceValueLong(_dapper, Secuencias.S_INGRESO_REAL_MOV);
        }

        public virtual long GetNextSalidaRealMov()
        {
            return _context.GetNextSequenceValueLong(_dapper, Secuencias.S_SALIDA_REAL_MOV);
        }

        public virtual IngresoProduccionDetalleSalida GetDetalleSalidaReal(string nuProduccion, string codigoProducto, int empresa, string identificador)
        {
            var entity = _context.T_PRDC_DET_SALIDA_REAL.AsNoTracking().FirstOrDefault(f => f.NU_PRDC_INGRESO == nuProduccion &&
                                                                             f.CD_PRODUTO == codigoProducto &&
                                                                             f.CD_EMPRESA == empresa &&
                                                                             f.NU_IDENTIFICADOR == identificador);

            if (entity != null)
                return _mapperIngresoProduccion.MapEntityToObject(entity);
            else
                return null;

        }

        public virtual List<IngresoProduccionDetalleReal> GetInsumosReales(string nuPrdcIngreso)
        {
            var entities = _context.T_PRDC_DET_INGRESO_REAL.Where(w => w.NU_PRDC_INGRESO == nuPrdcIngreso);

            return entities.Select(s => _mapperIngresoProduccion.MapEntityToObject(s)).ToList();
        }

        #endregion

        #region Add

        public virtual void AddIngreso(IngresoProduccion ingreso)
        {
            int secuencia = this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_PRDC_INGRESO_PK);

            ingreso.Id = secuencia.ToString();

            ingreso.NumeroProduccionOriginal = ingreso.Id;

            ingreso.Detalles.ForEach(f => f.IdIngresoProduccion = ingreso.Id);

            if (ingreso.IdProduccionExterno == string.Empty
                || ingreso.IdProduccionExterno == null)
                ingreso.IdProduccionExterno = "WIS" + ingreso.Id.ToString();

            AddDetalles(ingreso.Detalles);

            if (!string.IsNullOrEmpty(ingreso.IdEspacioProducion))
            {
                ingreso.PosicionEnCola = GetNextPosicionEnCola(ingreso.IdEspacioProducion);
            }

            if (ingreso.Instrucciones != null)
            {
                ingreso.Instrucciones.IdIngreso = ingreso.Id;

                AddInstruccion(ingreso.Instrucciones);
            }

            _context.T_PRDC_INGRESO.Add(_mapperIngresoProduccion.MapObjectToEntity(ingreso));
        }

        public virtual void AddDetalles(List<IngresoProduccionDetalleTeorico> detalles)
        {
            foreach (var detalle in detalles)
            {
                AddDetalle(detalle);
            }
        }

        public virtual void AddDetalle(IngresoProduccionDetalleTeorico detalle)
        {
            long secuencia = this._context.GetNextSequenceValueLong(_dapper, Secuencias.S_PRDC_INGRESO_DET_TEORICO_PK);
            detalle.Id = secuencia;

            if (detalle.FechaAlta == null)
            {
                detalle.FechaAlta = DateTime.Now;
            }

            var entity = _mapperIngresoProduccion.MapObjectToEntity(detalle);
            _context.T_PRDC_DET_INGRESO_TEORICO.Add(entity);
        }

        public virtual void AddInstruccion(IngresoProduccionInstruccion instruccion)
        {
            var entity = _mapperIngresoProduccion.MapObjectToEntity(instruccion);

            _context.T_PRDC_INGRESO_INSTRUCCION.Add(entity);
        }

        public virtual void AddDetalleRealProduccion(IngresoProduccionDetalleReal detalle)
        {
            detalle.NuPrdcIngresoReal = GetNextNuDetalleReal();
            detalle.DtAddrow = DateTime.Now;
            if (detalle.NuOrden == null) detalle.NuOrden = GetNextNuDetalleReal();

            var entity = _mapperIngresoProduccion.MapObjectToEntity(detalle);
            _context.T_PRDC_DET_INGRESO_REAL.Add(entity);
        }

        public virtual void AddDetalleSalidaReal(IngresoProduccionDetalleSalida detalle)
        {
            detalle.NuPrdcIngresoSalida = GetNextNuDetalleSalidaReal();
            var entity = _mapperIngresoProduccion.MapObjectToEntity(detalle);

            _context.T_PRDC_DET_SALIDA_REAL.Add(entity);
        }

        public virtual void AddMovimientoIngreso(IngresoProduccionDetalle detalle)
        {
            detalle.Id = GetNextIngresoRealMov();
            var entity = _mapperIngresoProduccion.MapObjectToEntity(detalle);

            _context.T_PRDC_DET_INGRESO_REAL_MOV.Add(entity);
        }

        public virtual void AddDetalleSalidaProducido(SalidaProduccionDetalle detalle)
        {
            detalle.Id = GetNextSalidaRealMov();
            var entity = _mapperIngresoProduccion.MapObjectToEntity(detalle);

            _context.T_PRDC_DET_SALIDA_REAL_MOV.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateIngresoProduccion(IngresoProduccion ingreso)
        {
            var entity = _mapperIngresoProduccion.MapObjectToEntity(ingreso);
            var attachedEntity = _context.T_PRDC_INGRESO.Local.FirstOrDefault(f => f.NU_PRDC_INGRESO == ingreso.Id);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_INGRESO.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetalle(IngresoProduccionDetalleTeorico detalle)
        {
            var entity = _mapperIngresoProduccion.MapObjectToEntity(detalle);
            var attachedEntity = _context.T_PRDC_DET_INGRESO_TEORICO.Local.FirstOrDefault(f => f.NU_PRDC_INGRESO == detalle.IdIngresoProduccion && f.NU_PRDC_DET_TEORICO == detalle.Id);

            if (attachedEntity != null)
            {

                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_DET_INGRESO_TEORICO.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetalleRealProduccion(IngresoProduccionDetalleReal detalle)
        {
            var entity = _mapperIngresoProduccion.MapObjectToEntity(detalle);
            var attachedEntity = _context.T_PRDC_DET_INGRESO_REAL.Local.FirstOrDefault(f => f.NU_PRDC_INGRESO_REAL == detalle.NuPrdcIngresoReal);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_DET_INGRESO_REAL.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateDetalleSalidaProduccion(IngresoProduccionDetalleSalida detalle)
        {
            var entity = _mapperIngresoProduccion.MapObjectToEntity(detalle);
            var attachedEntity = _context.T_PRDC_DET_SALIDA_REAL.Local.FirstOrDefault(f => f.NU_PRDC_SALIDA_REAL == detalle.NuPrdcIngresoSalida);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_PRDC_DET_SALIDA_REAL.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteDetalle(IngresoProduccionDetalleTeorico detalle)
        {
            var entity = _mapperIngresoProduccion.MapObjectToEntity(detalle);
            var attachedEntity = _context.T_PRDC_DET_INGRESO_TEORICO.Local.FirstOrDefault(w => w.NU_PRDC_DET_TEORICO == entity.NU_PRDC_DET_TEORICO);

            if (attachedEntity != null)
            {
                _context.T_PRDC_DET_INGRESO_TEORICO.Remove(attachedEntity);
            }
            else
            {
                _context.T_PRDC_DET_INGRESO_TEORICO.Attach(entity);
                _context.T_PRDC_DET_INGRESO_TEORICO.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        #region API de Produccion

        public virtual async Task<IngresoProduccion> GetProduccionOrNull(string nroIngresoProduccion, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var model = GetIngresoProduccion(nroIngresoProduccion, connection, null);

                Fill(connection, model);

                return model;
            }
        }

        public virtual void Fill(DbConnection connection, IngresoProduccion model)
        {
            if (model != null)
            {
                model.GeneraPedido = model.GeneraPedidoId == "S";

                model.Detalles = GetIngresoDetallesTeoricos(model.Id, connection, null);
                model.Consumidos = GetIngresoInsumos(model.Id, connection, null);
                model.Producidos = GetIngresoProductosFinales(model.Id, connection, null);
            }
        }

        public virtual IEnumerable<IngresoBlackBox> GetIdsExternos(IEnumerable<IngresoBlackBox> ingresos)
        {
            IEnumerable<IngresoBlackBox> resultado = new List<IngresoBlackBox>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRDC_INGRESO_TEMP (ID_PRODUCCION_EXTERNO) VALUES (:IdProduccionExterno)";
                    _dapper.Execute(connection, sql, ingresos, transaction: tran);

                    sql = @"SELECT 
								PI.ID_PRODUCCION_EXTERNO as IdProduccionExterno,
								PI.CD_EMPRESA as Empresa                
                            FROM T_PRDC_INGRESO PI 
                            INNER JOIN T_PRDC_INGRESO_TEMP T ON PI.ID_PRODUCCION_EXTERNO = T.ID_PRODUCCION_EXTERNO
                            GROUP BY 
                                PI.ID_PRODUCCION_EXTERNO,
                                PI.CD_EMPRESA";

                    resultado = _dapper.Query<IngresoBlackBox>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual List<IngresoProduccionDetalleTeorico> GetIngresoDetallesTeoricos(string nroIngresoProduccion, DbConnection connection, DbTransaction tran)
        {
            string sql = @" SELECT 
                                NU_PRDC_DET_TEORICO as Id,
                                NU_PRDC_INGRESO as IdIngresoProduccion,
                                TP_REGISTRO as Tipo,
                                CD_PRODUTO as Producto,
                                CD_EMPRESA as Empresa,
                                CD_FAIXA as Faixa,
                                QT_TEORICO as CantidadTeorica,
                                QT_PEDIDO_GENERADO as CantidadPedidoGenerada,
                                QT_ABASTECIDO as CantidadAbastecido,
                                QT_CONSUMIDO as CantidadConsumida,
                                NU_IDENTIFICADOR as Identificador,
                                NU_TRANSACCION as NumeroTransaccion,
                                DT_ADDROW as FechaAlta,
                                DS_ANEXO1 as Anexo1,
                                DS_ANEXO2 as Anexo2,
                                DS_ANEXO3 as Anexo3,
                                DS_ANEXO4 as Anexo4
                            FROM T_PRDC_DET_INGRESO_TEORICO WHERE NU_PRDC_INGRESO = :nroIngresoProduccion";

            return _dapper.Query<IngresoProduccionDetalleTeorico>(connection, sql, new
            {
                nroIngresoProduccion = nroIngresoProduccion
            }, CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<IngresoProduccionDetalleReal> GetIngresoInsumos(string nroIngresoProduccion, DbConnection connection, DbTransaction tran)
        {
            string sql = @" SELECT 
                                NU_PRDC_INGRESO_REAL as NuPrdcIngresoReal,
                                NU_PRDC_INGRESO as NuPrdcIngreso,
                                CD_PRODUTO as Producto,
                                CD_EMPRESA as Empresa,
                                CD_FAIXA as Faixa,
                                QT_REAL as QtReal,
                                QT_NOTIFICADO as QtNotificado,
                                QT_MERMA as QtMerma,
                                NU_IDENTIFICADOR as Identificador,
                                NU_TRANSACCION as NuTransaccion,
                                DT_ADDROW as DtAddrow,
                                NU_ORDEN as NuOrden,
                                ND_ESTADO as Estado,
                                DS_ANEXO1 as DsAnexo1,
                                DS_ANEXO2 as DsAnexo2,
                                DS_ANEXO3 as DsAnexo3,
                                DS_ANEXO4 as DsAnexo4,
                                QT_DESAFECTADA as QtDesafectado,
                                QT_REAL_ORIGINAL as QtRealOriginal,
                                DS_REFERENCIA as Referencia
                            FROM T_PRDC_DET_INGRESO_REAL WHERE NU_PRDC_INGRESO = :nroIngresoProduccion";

            return _dapper.Query<IngresoProduccionDetalleReal>(connection, sql, new
            {
                nroIngresoProduccion = nroIngresoProduccion
            }, CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<IngresoProduccionDetalleSalida> GetIngresoProductosFinales(string nroIngresoProduccion, DbConnection connection, DbTransaction tran)
        {
            string sql = @" SELECT 
                                NU_PRDC_SALIDA_REAL as NuPrdcIngresoSalida,
                                NU_PRDC_INGRESO as NuPrdcIngreso,
                                CD_PRODUTO as Producto,
                                NU_IDENTIFICADOR as Identificador,
                                CD_EMPRESA as Empresa,
                                CD_FAIXA as Faixa,
                                QT_PRODUCIDO as QtProducido,
                                ND_MOTIVO as NdMotivo,
                                DS_MOTIVO as DsMotivo,
                                NU_TRANSACCION as NuTransaccion,
                                DT_ADDROW as DtAddrow,
                                NU_ORDEN as NuOrden,
                                ND_ESTADO as NdEstado,
                                DS_ANEXO1 as DsAnexo1,
                                DS_ANEXO2 as DsAnexo1,
                                DS_ANEXO3 as DsAnexo1,
                                DS_ANEXO4 as DsAnexo1,
                                DT_VENCIMIENTO as DtVencimiento,
                                NU_PRDC_DET_TEORICO as NuPrdcIngresoTeorico,
                                QT_NOTIFICADO as QtNotificado
                            FROM T_PRDC_DET_SALIDA_REAL WHERE NU_PRDC_INGRESO = :nroIngresoProduccion";

            return _dapper.Query<IngresoProduccionDetalleSalida>(connection, sql, new
            {
                nroIngresoProduccion = nroIngresoProduccion
            }, CommandType.Text, transaction: tran).ToList();
        }

        public virtual async Task AddIngresos(List<IngresoProduccion> ingresos, IProduccionServiceContext context, List<IngresosGeneradosApiProduccion> ingresosGenerados, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var bulkContext = GetBulkOperationContext(ingresos, context, connection, ingresosGenerados);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertPreparaciones(connection, tran, bulkContext.NewPreparacion);
                    await BulkInsertIngresos(connection, tran, bulkContext.NewIngresos);
                    await BulkInsertDetallesTeoricos(connection, tran, bulkContext.NewDetallesTeoricos);
                    await BulkInsertPedidos(connection, tran, bulkContext.NewPedidos);
                    await BulkInsertDetallesPedidos(connection, tran, bulkContext.NewDetallesPedidos);

                    tran.Commit();
                }
            }
        }

        public virtual ProduccionBulkOperationContext GetBulkOperationContext(List<IngresoProduccion> ingresos, IProduccionServiceContext serviceContext, DbConnection connection, List<IngresosGeneradosApiProduccion> colIngresosGenerados)
        {
            var context = new ProduccionBulkOperationContext();
            var ingresosIds = GetNewIngresosIds(ingresos.Count, connection);

            var pedidosFormulasIds = GetNewPedidosIds(ingresos.Where(w => w.GeneraPedido && w.IdFormula != null).Count(), connection);

            var pedidosInsumosIds = GetNewPedidosIds(ingresos.Where(w => w.GeneraPedido && w.IdFormula == null).Count(), connection);

            var preparacionesIds = GetNewPreparacionesIds(ingresos.Where(w => w.GeneraPedido && (w.LiberarPedido ?? false)).Count(), connection);

            var countDetTeoricos = serviceContext.GetCantidadDetallesTeoricos();
            var detalleTeoricoIds = GetNewDetTeoricoIds(countDetTeoricos, connection);
            var posicionEspacio = GetNextPosicionEnCola(serviceContext.Espacios.Values);

            for (int i = 0; i < ingresos.Count; i++)
            {
                IngresosGeneradosApiProduccion nuevoIngresoGenerado = new IngresosGeneradosApiProduccion();
                var ingreso = Map(ingresos[i], ingresosIds[i], serviceContext, ref posicionEspacio);

                nuevoIngresoGenerado.IdIngreso = ingreso.Id;
                context.NewIngresos.Add(ingreso);

                if (!string.IsNullOrEmpty(ingreso.IdFormula) && ingreso.Formula != null)
                    MapDetallesTeoricosConFormula(ingreso, context, serviceContext, ref pedidosFormulasIds, ref detalleTeoricoIds, ref preparacionesIds, nuevoIngresoGenerado);
                else
                    MapDetallesTeoricos(ingreso, context, serviceContext, ref pedidosInsumosIds, ref detalleTeoricoIds, ref preparacionesIds, nuevoIngresoGenerado);

                colIngresosGenerados.Add(nuevoIngresoGenerado);
            }

            return context;
        }

        public virtual List<int> GetNewIngresosIds(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_PRDC_INGRESO_PK, count).ToList();
        }

        public virtual List<int> GetNewPedidosIds(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_NU_PEDI_MANUAL, count).ToList();
        }

        public virtual List<int> GetNewPreparacionesIds(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_NU_PREPARACION, count).ToList();
        }

        public virtual List<long> GetNewDetTeoricoIds(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_PRDC_INGRESO_DET_TEORICO_PK, count).ToList();
        }

        public virtual Dictionary<string, int> GetNextPosicionEnCola(IEnumerable<EstacionDeTrabajo> espacios)
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_PRDC_INGRESO_TEMP (CD_PRDC_LINEA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, espacios, transaction: tran);

                    sql = @"SELECT 
                                PDE.CD_PRDC_LINEA as IdEspacioProducion,
                                MAX(COALESCE(PDE.NU_POSICION_EN_COLA,0)) as PosicionEnCola
                            FROM T_PRDC_INGRESO PDE 
                            INNER JOIN T_PRDC_INGRESO_TEMP T ON PDE.CD_PRDC_LINEA = T.CD_PRDC_LINEA
                            GROUP BY PDE.CD_PRDC_LINEA";

                    var query = _dapper.Query<IngresoBlackBox>(connection, sql, transaction: tran);
                    resultado = query.ToDictionary(q => q.IdEspacioProducion, q => ((q.PosicionEnCola ?? 0) + 1));

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual IngresoProduccion Map(IngresoProduccion request, int nroIngresoProduccion, IProduccionServiceContext context, ref Dictionary<string, int> posicionEspacio)
        {
            if (!string.IsNullOrEmpty(request.IdEspacioProducion) && !posicionEspacio.ContainsKey(request.IdEspacioProducion))
            {
                posicionEspacio[request.IdEspacioProducion] = 1;
            }

            var model = new IngresoBlackBox
            {
                Id = request.Id = nroIngresoProduccion.ToString(),
                Empresa = request.Empresa,
                Formula = request.Formula,
                IdFormula = request.IdFormula,
                CantidadIteracionesFormula = request.CantidadIteracionesFormula,
                GeneraPedidoId = !string.IsNullOrEmpty(request.IdFormula) ? "S" : "N",
                GeneraPedido = request.GeneraPedido,
                LiberarPedido = request.LiberarPedido,
                Situacion = request.GeneraPedido
                    ? SituacionDb.PEDIDO_GENERADO
                    : SituacionDb.PRODUCCION_CREADA,
                Funcionario = request.Funcionario,
                FechaAlta = DateTime.Now,
                NumeroProduccionOriginal = nroIngresoProduccion.ToString(),
                TipoDeFlujo = request.TipoDeFlujo,
                Anexo1 = request.Anexo1,
                Anexo2 = request.Anexo2,
                Anexo3 = request.Anexo3,
                Anexo4 = request.Anexo4,
                Anexo5 = request.Anexo5,
                EjecucionEntrada = request.EjecucionEntrada,
                Predio = request.Predio,
                Tipo = request.Tipo,
                IdManual = request.IdManual,
                IdModalidadLote = request.IdModalidadLote,
                IdEspacioProducion = request.IdEspacioProducion,
                PosicionEnCola = !string.IsNullOrEmpty(request.IdEspacioProducion) ? posicionEspacio[request.IdEspacioProducion] : null,
                IdProduccionExterno = !string.IsNullOrEmpty(request.IdProduccionExterno) ? request.IdProduccionExterno : $"WIS{nroIngresoProduccion}",
                NuTransaccion = request.NuTransaccion,
                Lote = request.Lote,
                PermitirAutoasignarLinea = "N",
                ModalidadTrabajo = request.ModalidadTrabajo,
                ProductoInsumoAnclaLote = request.ProductoInsumoAnclaLote,
                IngresoDirectoProduccion = request.IngresoDirectoProduccion,
                Detalles = request.Detalles
            };

            model.Detalles = request.Detalles;

            if (!string.IsNullOrEmpty(request.IdEspacioProducion))
                posicionEspacio[request.IdEspacioProducion]++;

            return model;
        }

        public virtual void MapDetallesTeoricosConFormula(IngresoProduccion ingreso, ProduccionBulkOperationContext context, IProduccionServiceContext serviceContext, ref List<int> pedidosIds, ref List<long> detalleTeoricoIds, ref List<int> preparacionesIds, IngresosGeneradosApiProduccion ingresoGenerado)
        {
            Pedido pedido = null;
            Preparacion preparacion = null;

            if (ingreso.GeneraPedido)
            {
                GenerarCabezalPedidoPreparacion(ingreso, context, serviceContext, ref pedidosIds, ref preparacionesIds, ref pedido, ref preparacion);

                ingresoGenerado.PedidoGenerado = pedido.Id;

                if (preparacion != null)
                    ingresoGenerado.PreparacionGenerada = preparacion.Id;
            }

            foreach (var entrada in ingreso.Formula.Entrada)
            {
                var producto = serviceContext.GetProducto(entrada.Producto);
                var cantidad = (entrada.CantidadCompleta * ingreso.CantidadIteracionesFormula);

                var identificador = ManejoIdentificadorDb.IdentificadorAuto;
                var idEspIdentificador = "N";

                if (producto.ManejoIdentificador == ManejoIdentificador.Producto)
                {
                    identificador = ManejoIdentificadorDb.IdentificadorProducto;
                    idEspIdentificador = "S";
                }
                if (ingreso.GeneraPedido)
                {
                    context.NewDetallesPedidos.Add(new DetallePedido
                    {
                        Id = pedido.Id,
                        Empresa = pedido.Empresa,
                        Cliente = pedido.Cliente,
                        Producto = entrada.Producto,
                        Faixa = entrada.Faixa,
                        Identificador = identificador,
                        EspecificaIdentificadorId = idEspIdentificador,
                        Agrupacion = Agrupacion.Pedido,
                        Cantidad = cantidad,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        CantidadOriginal = cantidad,
                        FechaAlta = DateTime.Now,
                        Transaccion = pedido.Transaccion,
                    });
                }

                var idDetalleTeorico = detalleTeoricoIds.FirstOrDefault();

                context.NewDetallesTeoricos.Add(new IngresoProduccionDetalleTeorico
                {
                    Id = idDetalleTeorico,
                    IdIngresoProduccion = ingreso.Id,
                    Tipo = CIngresoProduccionDetalleTeorico.TipoDetalleEntrada,
                    Empresa = ingreso.Empresa,
                    Producto = entrada.Producto,
                    Faixa = entrada.Faixa,
                    Identificador = identificador,
                    CantidadTeorica = cantidad,
                    CantidadConsumida = 0,
                    CantidadAbastecido = 0,
                    CantidadPedidoGenerada = 0,
                    FechaAlta = DateTime.Now,
                    NumeroTransaccion = ingreso.NuTransaccion,
                });

                detalleTeoricoIds.Remove(idDetalleTeorico);
            }

            foreach (var entrada in ingreso.Formula.Salida)
            {
                var producto = serviceContext.GetProducto(entrada.Producto);

                var identificador = ManejoIdentificadorDb.IdentificadorAuto;
                if (producto.ManejoIdentificador == ManejoIdentificador.Producto)
                    identificador = ManejoIdentificadorDb.IdentificadorProducto;

                var cantidad = (entrada.CantidadCompleta * ingreso.CantidadIteracionesFormula);

                var idDetalleTeorico = detalleTeoricoIds.FirstOrDefault();

                context.NewDetallesTeoricos.Add(new IngresoProduccionDetalleTeorico
                {
                    Id = idDetalleTeorico,
                    IdIngresoProduccion = ingreso.Id,
                    Tipo = CIngresoProduccionDetalleTeorico.TipoDetalleSalida,
                    Empresa = ingreso.Empresa,
                    Producto = entrada.Producto,
                    Faixa = entrada.Faixa,
                    Identificador = identificador,
                    CantidadTeorica = cantidad,
                    CantidadConsumida = 0,
                    CantidadAbastecido = 0,
                    CantidadPedidoGenerada = 0,
                    FechaAlta = DateTime.Now,
                    NumeroTransaccion = ingreso.NuTransaccion,
                });

                detalleTeoricoIds.Remove(idDetalleTeorico);
            }
        }

        public virtual void GenerarCabezalPedidoPreparacion(IngresoProduccion ingreso, ProduccionBulkOperationContext context, IProduccionServiceContext serviceContext, ref List<int> pedidosIds, ref List<int> preparacionesIds, ref Pedido pedido, ref Preparacion preparacion)
        {
            if (ingreso.GeneraPedido)
            {
                var nroPedido = pedidosIds.FirstOrDefault();

                pedido = new Pedido
                {
                    Id = nroPedido.ToString(),
                    Empresa = ingreso.Empresa.Value,
                    ComparteContenedorPicking = $"{ingreso.Id}.{serviceContext.EmpresaEjecucion.CdClienteArmadoKit}.{ingreso.Empresa.Value}",
                    Cliente = serviceContext.Agente.CodigoInterno,
                    Ruta = serviceContext.Agente?.Ruta?.Id,
                    Estado = SituacionDb.PedidoAbierto,
                    ManualId = "N",
                    Agrupacion = Agrupacion.Pedido,
                    FechaAlta = DateTime.Now,
                    IngresoProduccion = ingreso.Id,
                    Memo1 = $"Pedido generado para producción {ingreso.IdProduccionExterno}.",
                    Origen = "API",
                    CondicionLiberacion = CondicionLiberacionDb.SinCondicion,
                    Tipo = TipoPedidoDb.Produccion,
                    Memo = $"Pedido generado para producción {ingreso.IdProduccionExterno}.",
                    TipoExpedicionId = TipoExpedicion.Produccion,
                    Transaccion = ingreso.NuTransaccion,
                    Predio = ingreso.Predio,
                    Anexo = ingreso.Anexo1,
                    Anexo2 = ingreso.Anexo2,
                    Anexo3 = ingreso.Anexo3,
                    Anexo4 = ingreso.Anexo4,
                    SincronizacionRealizadaId = "N",
                    Actividad = EstadoPedidoDb.Activo,
                };

                context.NewPedidos.Add(pedido);
                pedidosIds.Remove(nroPedido);

                if ((ingreso.LiberarPedido ?? false))
                {
                    var nroPreparacion = preparacionesIds.FirstOrDefault();

                    var codigoContenedorValidado = serviceContext.GetParamValue("PRDC_PED_CD_CON_VAL");
                    var onda = serviceContext.GetParamValue("PRD112_LIB_ONDA");
                    var agrupacion = serviceContext.GetParamValue("PRD112_LIB_AGRUPACION");
                    var respetarFifoEnLoteAUTO = serviceContext.GetParamValue("PRD112_LIB_RESP_FIFO_AUTO");
                    var controlaStockDocumental = serviceContext.GetParamValue("PRD112_LIB_CTRL_STK_DOCUMENTAL");
                    var cursorStock = serviceContext.GetParamValue("PRD112_LIB_CURSOR_STOCK");
                    var cursorPedido = serviceContext.GetParamValue("PRD112_LIB_CURSOR_PEDIDO");
                    var debeLiberarPorCurvas = serviceContext.GetParamValue("PRD112_LIB_LIBERAR_CURVAS");
                    var debeLiberarPorUnidades = serviceContext.GetParamValue("PRD112_LIB_LIBERAR_UNIDADES");
                    var repartirEscasez = serviceContext.GetParamValue("PRD112_LIB_REPARTIR_ESCASEZ");
                    var pickingAgrupCamion = serviceContext.GetParamValue("PRD112_LIB_AGRUP_CAMION");
                    var prepararSoloConCamion = serviceContext.GetParamValue("PRD112_LIB_PREP_SOLO_CAMION");
                    var modalPalletCompleto = serviceContext.GetParamValue("PRD112_LIB_MODO_PALLET_COMPLEO");
                    var modalPalletIncompleto = serviceContext.GetParamValue("PRD112_LIB_MODO_PALLET_INCO");
                    var priorizarDesborde = serviceContext.GetParamValue("PRD112_LIB_PRIORIZAR_DESBORDE");
                    var manejaVidaUtil = serviceContext.GetParamValue("PRD112_LIB_MANEJA_VIDA_UTIL");
                    var requiereUbicacion = serviceContext.GetParamValue("PRD112_LIB_PICKING_DOS_FACES");
                    var excluirPicking = serviceContext.GetParamValue("PRD112_LIB_EXCLUIR_PICKING");

                    preparacion = new Preparacion()
                    {
                        Id = nroPreparacion,
                        Descripcion = $"Lib Fabricación: {ingreso.IdProduccionExterno} Ped: {pedido.Id}",
                        Empresa = ingreso.Empresa,
                        Onda = short.Parse(onda),
                        Agrupacion = agrupacion,
                        RespetarFifoEnLoteAUTO = respetarFifoEnLoteAUTO == "S",
                        ControlaStockDocumental = controlaStockDocumental == "S",
                        CursorStock = cursorStock,
                        DebeLiberarPorCurvas = debeLiberarPorCurvas == "S",
                        DebeLiberarPorUnidades = debeLiberarPorUnidades == "S",
                        RepartirEscasez = repartirEscasez,
                        PickingEsAgrupadoPorCamion = pickingAgrupCamion == "S",
                        PrepararSoloConCamion = prepararSoloConCamion == "S",
                        ModalPalletCompleto = modalPalletCompleto,
                        ModalPalletIncompleto = modalPalletIncompleto,
                        CursorPedido = cursorPedido,
                        UsarSoloStkPicking = priorizarDesborde == "S",
                        ManejaVidaUtil = manejaVidaUtil == "S",
                        RequiereUbicacion = requiereUbicacion == "S",
                        FechaInicio = DateTime.Now,
                        ExcluirUbicacionesPicking = excluirPicking == "S",
                        Predio = pedido.Predio,
                        Transaccion = ingreso.NuTransaccion,
                        Tipo = TipoPreparacionDb.Normal,
                        Usuario = _userId,
                        Situacion = SituacionDb.PreparacionPendiente,

                    };
                    pedido.NumeroOrdenLiberacion = 0;
                    pedido.PreparacionProgramada = nroPreparacion;
                    context.NewPreparacion.Add(preparacion);

                    preparacionesIds.Remove(nroPreparacion);
                }
            }

        }

        public virtual void MapDetallesTeoricos(IngresoProduccion ingreso, ProduccionBulkOperationContext context, IProduccionServiceContext serviceContext, ref List<int> pedidosIds, ref List<long> detalleTeoricoIds, ref List<int> preparacionesIds, IngresosGeneradosApiProduccion ingresoGenerado)
        {
            Pedido pedido = null;
            Preparacion preparacion = null;

            if (ingreso.GeneraPedido)
            {
                GenerarCabezalPedidoPreparacion(ingreso, context, serviceContext, ref pedidosIds, ref preparacionesIds, ref pedido, ref preparacion);

                ingresoGenerado.PedidoGenerado = pedido.Id;

                if (preparacion != null)
                    ingresoGenerado.PreparacionGenerada = preparacion.Id;
            }

            foreach (var det in ingreso.Detalles)
            {
                var idDetalleTeorico = detalleTeoricoIds.FirstOrDefault();
                var producto = serviceContext.GetProducto(det.Producto);

                if (det.Tipo == CIngresoProduccionDetalleTeorico.TipoDetalleEntrada && ingreso.GeneraPedido)
                {
                    var identificador = ManejoIdentificadorDb.IdentificadorAuto;
                    var idEspIdentificador = "N";

                    if (producto.ManejoIdentificador != ManejoIdentificador.Producto && !string.IsNullOrEmpty(det.Identificador) && det.Identificador != ManejoIdentificadorDb.IdentificadorAuto)
                    {
                        identificador = det.Identificador;
                        idEspIdentificador = "S";
                    }
                    else if (producto.ManejoIdentificador == ManejoIdentificador.Producto)
                    {
                        identificador = ManejoIdentificadorDb.IdentificadorProducto;
                        idEspIdentificador = "S";
                    }

                    context.NewDetallesPedidos.Add(new DetallePedido
                    {
                        Id = pedido.Id,
                        Empresa = pedido.Empresa,
                        Cliente = pedido.Cliente,
                        Producto = det.Producto,
                        Faixa = (det.Faixa ?? 1),
                        Identificador = identificador,
                        EspecificaIdentificadorId = idEspIdentificador,
                        Agrupacion = Agrupacion.Pedido,
                        Cantidad = det.CantidadTeorica,
                        CantidadLiberada = 0,
                        CantidadAnulada = 0,
                        CantidadOriginal = det.CantidadTeorica,
                        FechaAlta = DateTime.Now,
                        Transaccion = pedido.Transaccion,
                    });
                }

                context.NewDetallesTeoricos.Add(new IngresoProduccionDetalleTeorico
                {
                    Id = idDetalleTeorico,
                    IdIngresoProduccion = ingreso.Id,
                    Tipo = det.Tipo,
                    Empresa = det.Empresa,
                    Producto = det.Producto,
                    Faixa = det.Faixa,
                    Identificador = det.Identificador,
                    CantidadTeorica = det.CantidadTeorica,
                    CantidadConsumida = 0,
                    CantidadAbastecido = 0,
                    CantidadPedidoGenerada = 0,
                    FechaAlta = DateTime.Now,
                    NumeroTransaccion = ingreso.NuTransaccion,
                });

                detalleTeoricoIds.Remove(idDetalleTeorico);
            }
        }

        public virtual async Task BulkInsertPreparaciones(DbConnection connection, DbTransaction tran, List<Preparacion> preparaciones)
        {
            var entities = this._mapperPreparacion.MapToEntity(preparaciones);

            var sql = @$"INSERT INTO T_PICKING 
                            (NU_PREPARACION,
                            DS_PREPARACION,
                            CD_EMPRESA,
                            CD_ONDA,
                            ID_AGRUPACION,
                            FL_RESPETAR_FIFO_EN_LOTE_AUTO,
                            FL_CONTROLA_STOCK_DOCUMENTO,
                            VL_CURSOR_STOCK,
                            FL_LIBERAR_POR_CURVAS,
                            FL_LIBERAR_POR_UNIDADES,
                            VL_REPARTIR_ESCASEZ,
                            FL_PICK_AGRUPADO_POR_CAMION,
                            FL_PREPARAR_SOLO_CON_CAMION,
                            FL_MODAL_PALLET_COMPLETO,
                            FL_MODAL_PALLET_INCOMPLETO,
                            VL_CURSOR_PEDIDO,
                            FL_USAR_SOLO_STK_PICKING,
                            FL_VENTANA_POR_CLIENTE,
                            FL_REQUIERE_UBICACION,
                            DT_INICIO,
                            FL_EXCLUIR_UBICACIONES_PICKING,
                            NU_PREDIO,
                            NU_TRANSACCION,
                            TP_PREPARACION,
                            CD_FUNCIONARIO,
                            CD_SITUACAO) 
                        VALUES (
                            :NU_PREPARACION,
                            :DS_PREPARACION,
                            :CD_EMPRESA,
                            :CD_ONDA,
                            :ID_AGRUPACION,
                            :FL_RESPETAR_FIFO_EN_LOTE_AUTO,
                            :FL_CONTROLA_STOCK_DOCUMENTO,
                            :VL_CURSOR_STOCK,
                            :FL_LIBERAR_POR_CURVAS,
                            :FL_LIBERAR_POR_UNIDADES,
                            :VL_REPARTIR_ESCASEZ,
                            :FL_PICK_AGRUPADO_POR_CAMION,
                            :FL_PREPARAR_SOLO_CON_CAMION,
                            :FL_MODAL_PALLET_COMPLETO,
                            :FL_MODAL_PALLET_INCOMPLETO,
                            :VL_CURSOR_PEDIDO,
                            :FL_USAR_SOLO_STK_PICKING,
                            :FL_VENTANA_POR_CLIENTE,
                            :FL_REQUIERE_UBICACION,
                            :DT_INICIO,
                            :FL_EXCLUIR_UBICACIONES_PICKING,
                            :NU_PREDIO,
                            :NU_TRANSACCION,
                            :TP_PREPARACION,
                            :CD_FUNCIONARIO,
                            :CD_SITUACAO)";

            await _dapper.ExecuteAsync(connection, sql, entities, transaction: tran);
        }

        public virtual async Task BulkInsertIngresos(DbConnection connection, DbTransaction tran, List<IngresoProduccion> ingresos)
        {
            string sql = @"INSERT INTO T_PRDC_INGRESO 
                            (NU_PRDC_INGRESO,
                            CD_PRDC_DEFINICION,
                            QT_FORMULA,
                            ID_GENERAR_PEDIDO,
                            CD_FUNCIONARIO,
                            CD_SITUACAO,
                            DT_ADDROW,
                            NU_PRDC_ORIGINAL,
                            DS_ANEXO1,
                            DS_ANEXO2,
                            DS_ANEXO3,
                            DS_ANEXO4,
                            DS_ANEXO5,
                            TP_FLUJO,
                            NU_INTERFAZ_EJECUCION_ENTRADA,
                            ND_TIPO,
                            CD_PRDC_LINEA,
                            NU_PREDIO,
                            CD_EMPRESA,
                            NU_POSICION_EN_COLA,
                            ND_ASIGNACION_LOTE,
                            ID_PRODUCCION_EXTERNO,
                            ID_MANUAL,
                            NU_TRANSACCION,
                            FL_PERMITIR_AUTOASIGNAR_LINEA) 
                        VALUES (
                            :Id,                          
                            :IdFormula,                     
                            :CantidadIteracionesFormula,                     
                            :GeneraPedidoId,         
                            :Funcionario,                      
                            :Situacion,                
                            :FechaAlta,                        
                            :NumeroProduccionOriginal,    
                            :Anexo1,
                            :Anexo2,
                            :Anexo3,
                            :Anexo4,
                            :Anexo5,
                            :TipoDeFlujo,
                            :EjecucionEntrada,            
                            :Tipo,                        
                            :IdEspacioProducion,                       
                            :Predio,                           
                            :Empresa,                
                            :PosicionEnCola,                
                            :IdModalidadLote,             
                            :IdProduccionExterno,           
                            :IdManual,           
                            :NuTransaccion, 
                            :PermitirAutoasignarLinea)";

            await _dapper.ExecuteAsync(connection, sql, ingresos, transaction: tran);
        }

        public virtual async Task BulkInsertDetallesTeoricos(DbConnection connection, DbTransaction tran, List<IngresoProduccionDetalleTeorico> detallesTeoricos)
        {
            string sql = @"INSERT INTO T_PRDC_DET_INGRESO_TEORICO 
                            (NU_PRDC_DET_TEORICO,
                            NU_PRDC_INGRESO,
                            TP_REGISTRO,
                            CD_PRODUTO,
                            CD_EMPRESA,
                            CD_FAIXA,
                            QT_TEORICO,
                            QT_PEDIDO_GENERADO,
                            QT_ABASTECIDO,
                            QT_CONSUMIDO,
                            NU_IDENTIFICADOR,
                            NU_TRANSACCION,
                            DT_ADDROW,
                            DS_ANEXO1,
                            DS_ANEXO2,
                            DS_ANEXO3,
                            DS_ANEXO4) 
                           VALUES (
                            :Id,                          
                            :IdIngresoProduccion,                     
                            :Tipo,                     
                            :Producto,         
                            :Empresa,                      
                            :Faixa,                
                            :CantidadTeorica,                        
                            :CantidadPedidoGenerada,               
                            :CantidadAbastecido,        
                            :CantidadConsumida,                     
                            :Identificador,                      
                            :NumeroTransaccion,                      
                            :FechaAlta,                      
                            :Anexo1,                      
                            :Anexo2,                      
                            :Anexo3,                      
                            :Anexo4)";

            await _dapper.ExecuteAsync(connection, sql, detallesTeoricos, transaction: tran);
        }

        public virtual async Task BulkInsertPedidos(DbConnection connection, DbTransaction tran, List<Pedido> pedidos)
        {
            string sql = @"INSERT INTO T_PEDIDO_SAIDA 
                            (NU_PEDIDO, 
                            CD_EMPRESA,
                            VL_COMPARTE_CONTENEDOR_PICKING,
                            CD_CLIENTE,
                            CD_ROTA,
                            CD_SITUACAO,
                            ID_MANUAL,
                            ID_AGRUPACION,
                            DT_ADDROW,
                            NU_PRDC_INGRESO,
                            DS_MEMO_1,
                            CD_ORIGEN,
                            CD_CONDICION_LIBERACION,
                            TP_PEDIDO,
                            DS_MEMO,
                            TP_EXPEDICION,
                            NU_TRANSACCION,
                            NU_PREDIO,
                            DS_ANEXO1,
                            DS_ANEXO2,
                            DS_ANEXO3,
                            DS_ANEXO4,
                            FL_SYNC_REALIZADA,
                            NU_PREPARACION_PROGRAMADA,
                            NU_ORDEN_LIBERACION,
                            ND_ACTIVIDAD) 
                          VALUES (
                            :Id,                          
                            :Empresa,                     
                            :ComparteContenedorPicking,   
                            :Cliente,                     
                            :Ruta,                        
                            :Estado,               
                            :ManualId,                
                            :Agrupacion,                  
                            :FechaAlta,
                            :IngresoProduccion,
                            :Memo1,                       
                            :Origen,                      
                            :CondicionLiberacion,         
                            :Tipo,                        
                            :Memo,                        
                            :TipoExpedicionId,                
                            :Transaccion,
                            :Predio,                      
                            :Anexo,                       
                            :Anexo2,                      
                            :Anexo3,                      
                            :Anexo4,      
                            :SincronizacionRealizadaId,
                            :PreparacionProgramada,
                            :NumeroOrdenLiberacion,
                            :Actividad)";

            await _dapper.ExecuteAsync(connection, sql, pedidos, transaction: tran);
        }

        public virtual async Task BulkInsertDetallesPedidos(DbConnection connection, DbTransaction tran, List<DetallePedido> detallesPedidos)
        {
            var sql = @" INSERT INTO T_DET_PEDIDO_SAIDA
                            (NU_PEDIDO,
                            CD_EMPRESA,
                            CD_CLIENTE,
                            CD_PRODUTO,
                            CD_FAIXA,
                            NU_IDENTIFICADOR,
                            ID_ESPECIFICA_IDENTIFICADOR,
                            ID_AGRUPACION,
                            QT_PEDIDO,
                            QT_LIBERADO,
                            QT_ANULADO,
                            QT_PEDIDO_ORIGINAL,                        
                            DT_ADDROW,
                            NU_TRANSACCION) 
                        VALUES(
                            :Id,                          
                            :Empresa,                     
                            :Cliente,                     
                            :Producto,                    
                            :Faixa,                       
                            :Identificador,               
                            :EspecificaIdentificadorId, 
                            :Agrupacion,                  
                            :Cantidad,                    
                            :CantidadLiberada,            
                            :CantidadAnulada,             
                            :CantidadOriginal,                                    
                            :FechaAlta,             
                            :Transaccion)";

            await _dapper.ExecuteAsync(connection, sql, detallesPedidos, transaction: tran);
        }

        #endregion

        #region API de ProducirProduccion

        public virtual async Task ProducirProduccion(ProducirProduccion produccion, IProducirProduccionServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var nuTransaccion = await CreateTransaction($"Producción para el ingreso {context.GetIngreso().Id}", connection, null);

                var bulkContext = GetBulkOperationContext(produccion, context, connection, nuTransaccion);
                context.KeysAjustes = bulkContext.NewAjustesStocks
                    .Select(a => a.NuAjusteStock.ToString())
                    .ToList();
                context.NotificarProduccion = bulkContext.NotificarProduccion;

                using (var tran = connection.BeginTransaction())
                {
                    await InsertStock(connection, tran, bulkContext.NewStocks.Values);
                    await UpdateStock(connection, tran, bulkContext.UpdateStocks.Values);
                    await AddAjustesStock(connection, tran, bulkContext.NewAjustesStocks);

                    await AddDetallesProductosProducidosReales(connection, tran, bulkContext.NewSalidaProduccionDetalleReales.Values);
                    await UpdateDetallesProductosProducidosReales(connection, tran, bulkContext.UpdateSalidaProduccionDetalleReales.Values);
                    await AddDetallesProductosProducidosMov(connection, tran, bulkContext.NewSalidaProduccionDetalleMov);

                    if (bulkContext.Ingreso != null)
                        await UpdateIngreso(connection, tran, bulkContext.Ingreso);

                    if (produccion.FinalizarProduccion)
                    {
                        await UpdateDetallesInsumosReales(connection, tran, bulkContext.UpdateInsumos);

                        if (!bulkContext.NotificarProduccion && bulkContext.EspacioProduccion != null)
                            await UpdateEspacioProduccion(connection, tran, bulkContext.EspacioProduccion);
                    }

                    tran.Commit();
                }
            }
        }

        public virtual ProducirProduccionBulkOperationContext GetBulkOperationContext(ProducirProduccion produccion, IProducirProduccionServiceContext serviceContext, DbConnection connection, long nuTransaccion)
        {
            var context = new ProducirProduccionBulkOperationContext();

            var keysStock = produccion.Productos.Select(s => new Stock()
            {
                Ubicacion = s.Ubicacion,
                Empresa = s.Empresa,
                Producto = s.Producto,
                Faixa = s.Faixa,
                Identificador = s.Identificador,
            }).ToList();

            var ingreso = serviceContext.GetIngreso();
            var espacioProducion = serviceContext.GetEspacioProduccion();

            var keysStockInsumos = ingreso.Consumidos
                .Where(c => c.QtReal > 0)
                .GroupBy(g => new { Empresa = g.Empresa.Value, g.Producto, Faixa = g.Faixa.Value, g.Identificador })
                .Select(s => new Stock()
                {
                    Ubicacion = espacioProducion.IdUbicacionProduccion,
                    Empresa = s.Key.Empresa,
                    Producto = s.Key.Producto,
                    Faixa = s.Key.Faixa,
                    Identificador = s.Key.Identificador,
                }).ToList();

            keysStock.AddRange(keysStockInsumos);
            keysStock = keysStock.Distinct().ToList();

            var stocks = _stockRepository.GetStocks(keysStock);

            var detallesSalidaReal = GetDetallesSalidaReal(ingreso.Id);
            var detallesSalidaTeorico = GetDetallesTeoricos(ingreso.Id, CIngresoProduccionDetalleTeorico.TipoDetalleSalida);

            var nuOrden = (detallesSalidaReal.Max(x => x.NuOrden) ?? 0) + 1;
            var productosProducidosPorBB = produccion.Productos.Where(x => x.Motivo != TipoIngresoProduccion.MOT_PROD_ADS).ToList();
            var productosProducidosAjusteStock = produccion.Productos.Where(x => x.Motivo == TipoIngresoProduccion.MOT_PROD_ADS).ToList();

            var idsDetalleSalidaMov = GetNewIdsDetalleSalidaMov(productosProducidosPorBB.Count, connection);
            var idsAjustes = _ajusteRepository.GetNewIdsAjusteStock(productosProducidosAjusteStock.Count, connection);
            var idsDetallesSalidaReal = GetNewIdsDetalleSalidaReal(productosProducidosPorBB.Count, connection);

            foreach (var movimientoSalida in produccion.Productos)
            {
                var stock = stocks.FirstOrDefault(s => s.Ubicacion == movimientoSalida.Ubicacion &&
                    s.Empresa == movimientoSalida.Empresa &&
                    s.Producto == movimientoSalida.Producto &&
                    s.Faixa == movimientoSalida.Faixa &&
                    s.Identificador == movimientoSalida.Identificador);

                var keyStock = $"{movimientoSalida.Ubicacion}.{movimientoSalida.Empresa}.{movimientoSalida.Producto}.{movimientoSalida.Faixa}.{movimientoSalida.Identificador}";

                var vencimiento = movimientoSalida.Vencimiento;

                if (stock == null)
                {
                    if (!context.NewStocks.ContainsKey(keyStock))
                    {
                        context.NewStocks.Add(keyStock, new Stock()
                        {
                            Ubicacion = movimientoSalida.Ubicacion,
                            Empresa = movimientoSalida.Empresa,
                            Producto = movimientoSalida.Producto,
                            Faixa = movimientoSalida.Faixa,
                            Identificador = movimientoSalida.Identificador,
                            Cantidad = movimientoSalida.Cantidad,
                            ReservaSalida = 0,
                            CantidadTransitoEntrada = 0,
                            Vencimiento = movimientoSalida.Vencimiento,
                            Inventario = "R",
                            Averia = "N",
                            ControlCalidad = EstadoControlCalidad.Controlado,
                            FechaModificacion = DateTime.Now,
                            FechaInventario = DateTime.Now,
                            NumeroTransaccion = nuTransaccion,
                        });
                    }
                    else
                    {
                        var newStock = context.NewStocks[keyStock];
                        newStock.Cantidad += movimientoSalida.Cantidad;
                        newStock.Vencimiento = vencimiento = InventarioLogic.ResolverVencimiento(newStock.Vencimiento, movimientoSalida.Vencimiento);

                        context.NewStocks[keyStock] = newStock;
                    }
                }
                else
                {
                    stock.Cantidad += movimientoSalida.Cantidad;
                    stock.FechaModificacion = DateTime.Now;
                    stock.NumeroTransaccion = nuTransaccion;
                    stock.Vencimiento = vencimiento = InventarioLogic.ResolverVencimiento(stock.Vencimiento, movimientoSalida.Vencimiento);

                    context.UpdateStocks[keyStock] = stock;
                }

                if (movimientoSalida.Motivo == TipoIngresoProduccion.MOT_PROD_ADS)
                {
                    var idAjusteStock = idsAjustes.FirstOrDefault();

                    var ajuste = new AjusteStock
                    {
                        NuAjusteStock = idAjusteStock,
                        Ubicacion = movimientoSalida.Ubicacion,
                        Empresa = movimientoSalida.Empresa,
                        Producto = movimientoSalida.Producto,
                        Faixa = movimientoSalida.Faixa,
                        Identificador = movimientoSalida.Identificador,
                        QtMovimiento = movimientoSalida.Cantidad,
                        FechaVencimiento = vencimiento,
                        FechaRealizado = DateTime.Now,
                        TipoAjuste = TipoAjusteDb.Stock,
                        CdMotivoAjuste = MotivoAjusteDb.Produccion,
                        DescMotivo = $"Producción para el Ingreso Nro: {ingreso.Id}",
                        NuTransaccion = nuTransaccion,
                        Predio = espacioProducion.Predio,
                        IdAreaAveria = "N",
                        FechaMotivo = DateTime.Now,
                        Funcionario = serviceContext.UserId,
                        Aplicacion = _application,
                        Metadata = ingreso.Id
                    };

                    idsAjustes.Remove(idAjusteStock);

                    context.NewAjustesStocks.Add(ajuste);
                }
                else
                {
                    var productoProducidoMov = Map(movimientoSalida, produccion.Empresa, ref idsDetalleSalidaMov, serviceContext, produccion.ConfirmarMovimiento, nuTransaccion);
                    context.NewSalidaProduccionDetalleMov.Add(productoProducidoMov);

                    var detalleSalidaReal = detallesSalidaReal
                        .FirstOrDefault(d => d.NuPrdcIngreso == ingreso.Id &&
                            d.Empresa == movimientoSalida.Empresa &&
                            d.Producto == movimientoSalida.Producto &&
                            d.Faixa == movimientoSalida.Faixa &&
                            d.Identificador == movimientoSalida.Identificador);

                    var keyDetalleReal = $"{ingreso.Id}.{movimientoSalida.Empresa}.{movimientoSalida.Producto}.{movimientoSalida.Faixa}.{movimientoSalida.Identificador}";

                    if (detalleSalidaReal == null)
                    {
                        var detalleTeorico = detallesSalidaTeorico
                            .FirstOrDefault(x => x.Producto == movimientoSalida.Producto
                                && (x.Identificador == movimientoSalida.Identificador || x.Identificador == ManejoIdentificadorDb.IdentificadorAuto)
                                && x.Faixa == movimientoSalida.Faixa && x.Empresa == movimientoSalida.Empresa);

                        if (!context.NewSalidaProduccionDetalleReales.ContainsKey(keyDetalleReal))
                        {
                            var id = idsDetallesSalidaReal.FirstOrDefault();

                            context.NewSalidaProduccionDetalleReales.Add(keyDetalleReal, new IngresoProduccionDetalleSalida()
                            {
                                NuPrdcIngresoSalida = id,
                                NuPrdcIngreso = ingreso.Id,
                                Producto = movimientoSalida.Producto,
                                Empresa = movimientoSalida.Empresa,
                                Faixa = movimientoSalida.Faixa,
                                Identificador = movimientoSalida.Identificador,
                                QtProducido = movimientoSalida.Cantidad,
                                NdMotivo = movimientoSalida.Motivo,
                                DtVencimiento = vencimiento,
                                NuTransaccion = nuTransaccion,
                                DtAddrow = DateTime.Now,
                                QtNotificado = !produccion.ConfirmarMovimiento ? movimientoSalida.Cantidad : 0,
                                NuOrden = nuOrden,
                                NuPrdcIngresoTeorico = detalleTeorico?.Id
                            });

                            idsDetallesSalidaReal.Remove(id);
                            nuOrden = nuOrden + 1;
                        }
                        else
                        {
                            var newDetalleReal = context.NewSalidaProduccionDetalleReales[keyDetalleReal];
                            newDetalleReal.QtProducido += movimientoSalida.Cantidad;
                            newDetalleReal.QtNotificado += !produccion.ConfirmarMovimiento ? movimientoSalida.Cantidad : 0;
                            newDetalleReal.NdMotivo = movimientoSalida.Motivo;
                            newDetalleReal.DtVencimiento = vencimiento;

                            context.NewSalidaProduccionDetalleReales[keyDetalleReal] = newDetalleReal;
                        }
                    }
                    else
                    {
                        detalleSalidaReal.QtProducido += movimientoSalida.Cantidad;
                        detalleSalidaReal.QtNotificado += !produccion.ConfirmarMovimiento ? movimientoSalida.Cantidad : 0;
                        detalleSalidaReal.NuTransaccion = nuTransaccion;
                        detalleSalidaReal.DtVencimiento = vencimiento;

                        context.UpdateSalidaProduccionDetalleReales[keyDetalleReal] = detalleSalidaReal;
                    }
                }
            }

            context.NotificarProduccion = (produccion.Productos.Any(p => p.Motivo != TipoIngresoProduccion.MOT_PROD_ADS) && produccion.ConfirmarMovimiento);

            if (produccion.FinalizarProduccion)
            {
                var insumosConSaldo = ingreso.Consumidos.Where(w => w.QtReal > 0).ToList();

                foreach (var insumo in insumosConSaldo)
                {
                    var saldoConsumir = (insumo.QtReal ?? 0);

                    insumo.QtReal = 0;
                    insumo.NuTransaccion = nuTransaccion;

                    context.UpdateInsumos.Add(insumo);

                    var stock = stocks.FirstOrDefault(s => s.Ubicacion == espacioProducion.IdUbicacionProduccion &&
                        s.Empresa == insumo.Empresa &&
                        s.Producto == insumo.Producto &&
                        s.Faixa == insumo.Faixa &&
                        s.Identificador == insumo.Identificador);

                    if (stock != null)
                    {
                        if (stock.ReservaSalida > saldoConsumir)
                            stock.ReservaSalida = (stock.ReservaSalida ?? 0) - saldoConsumir;
                        else
                            stock.ReservaSalida = 0;

                        stock.NumeroTransaccion = nuTransaccion;
                        stock.FechaModificacion = DateTime.Now;

                        var keyStock = $"{espacioProducion.IdUbicacionProduccion}.{insumo.Empresa}.{insumo.Producto}.{insumo.Faixa}.{insumo.Identificador}";

                        context.UpdateStocks[keyStock] = stock;
                    }
                }

                if (context.NotificarProduccion)
                    ingreso.Situacion = SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL;
                else
                {
                    ingreso.Situacion = SituacionDb.PRODUCCION_FINALIZADA;
                    ingreso.FechaFinProduccion = DateTime.Now;

                    espacioProducion.NumeroIngreso = null;
                    espacioProducion.NumeroTransaccion = nuTransaccion;

                    context.EspacioProduccion = espacioProducion;
                }
            }
            else if (context.NotificarProduccion)
                ingreso.Situacion = SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_PARCIAL;
            else
                ingreso.Situacion = SituacionDb.PRODUCIENDO;

            ingreso.NuTransaccion = nuTransaccion;
            context.Ingreso = ingreso;

            return context;
        }

        public virtual async Task AddDetallesProductosProducidosReales(DbConnection connection, DbTransaction tran, IEnumerable<IngresoProduccionDetalleSalida> detallesReales)
        {
            string sql = @"INSERT INTO T_PRDC_DET_SALIDA_REAL
                            (NU_PRDC_SALIDA_REAL,
                            NU_PRDC_INGRESO,
                            CD_PRODUTO,
                            NU_IDENTIFICADOR,
                            CD_EMPRESA,
                            CD_FAIXA,
                            QT_PRODUCIDO,
                            QT_NOTIFICADO,
                            ND_MOTIVO,
                            NU_TRANSACCION,
                            DT_ADDROW,
                            NU_ORDEN,
                            DT_VENCIMIENTO,
                            NU_PRDC_DET_TEORICO)
                        VALUES (
                            :NuPrdcIngresoSalida,
                            :NuPrdcIngreso,
                            :Producto,
                            :Identificador,
                            :Empresa,
                            :Faixa,
                            :QtProducido,
                            :QtNotificado,
                            :NdMotivo,
                            :NuTransaccion,
                            :DtAddrow,
                            :NuOrden,
                            :DtVencimiento,
                            :NuPrdcIngresoTeorico
                        )
                        ";

            await _dapper.ExecuteAsync(connection, sql, detallesReales, transaction: tran);
        }

        public virtual async Task UpdateDetallesProductosProducidosReales(DbConnection connection, DbTransaction tran, IEnumerable<IngresoProduccionDetalleSalida> detallesReales)
        {
            string sql = $@"
                UPDATE T_PRDC_DET_SALIDA_REAL 
                    SET QT_PRODUCIDO = :QtProducido,
                    QT_NOTIFICADO = :QtNotificado,
                    NU_TRANSACCION = :NuTransaccion,
                    DT_VENCIMIENTO = :DtVencimiento
                WHERE NU_PRDC_SALIDA_REAL = :NuPrdcIngresoSalida";

            await _dapper.ExecuteAsync(connection, sql, detallesReales, transaction: tran);
        }

        public virtual async Task UpdateDetallesInsumosReales(DbConnection connection, DbTransaction tran, IEnumerable<IngresoProduccionDetalleReal> updateInsumos)
        {
            string sql = $@"
                UPDATE T_PRDC_DET_INGRESO_REAL 
                    SET QT_REAL = :QtReal,
                    QT_NOTIFICADO = :QtNotificado,
                    QT_DESAFECTADA = :QtDesafectado,
                    NU_TRANSACCION = :NuTransaccion
                WHERE NU_PRDC_INGRESO_REAL = :NuPrdcIngresoReal";

            await _dapper.ExecuteAsync(connection, sql, updateInsumos, transaction: tran);
        }

        public virtual async Task AddDetallesProductosProducidosMov(DbConnection connection, DbTransaction tran, IEnumerable<SalidaProduccionDetalle> detallesProduccionMov)
        {
            string sql = @"INSERT INTO T_PRDC_DET_SALIDA_REAL_MOV
                    (NU_SALIDA_REAL_MOV,
                    NU_PRDC_INGRESO,
                    CD_PRODUTO,
                    CD_EMPRESA,
                    CD_FAIXA,
                    CD_ENDERECO,
                    QT_ESTOQUE,
                    NU_IDENTIFICADOR,
                    NU_TRANSACCION,
                    DT_ADDROW,
                    DT_VENCIMIENTO,
                    FL_PENDIENTE_NOTIFICAR,
                    ND_MOTIVO) 
                   VALUES (
                    :Id,
                    :NuPrdcIngreso,                          
                    :Producto,                     
                    :Empresa,                     
                    :Faixa,         
                    :Ubicacion,                      
                    :Cantidad,                
                    :Identificador,                        
                    :NuTransaccion,               
                    :FechaAlta,        
                    :Vencimiento,                     
                    :FlPendienteNotificar,                      
                    :Motivo)";

            await _dapper.ExecuteAsync(connection, sql, detallesProduccionMov, transaction: tran);
        }

        public virtual async Task UpdateEspacioProduccion(DbConnection connection, DbTransaction tran, EspacioProduccion espacioProduccion)
        {
            string sql = $@"
                UPDATE T_PRDC_LINEA 
                    SET NU_PRDC_INGRESO = :NumeroIngreso,
                    NU_TRANSACCION = :NumeroTransaccion
                WHERE CD_PRDC_LINEA = :Id";

            await _dapper.ExecuteAsync(connection, sql, espacioProduccion, transaction: tran);
        }

        public virtual async Task UpdateIngreso(DbConnection connection, DbTransaction tran, IngresoProduccion ingreso)
        {
            string sql = $@"
                UPDATE T_PRDC_INGRESO 
                    SET CD_SITUACAO = :Situacion,
                    DT_FIN_PRODUCCION = :FechaFinProduccion,
                    NU_TRANSACCION = :NuTransaccion
                WHERE NU_PRDC_INGRESO = :Id";

            await _dapper.ExecuteAsync(connection, sql, ingreso, transaction: tran);
        }

        public virtual async Task InsertStock(DbConnection connection, DbTransaction tran, IEnumerable<Stock> stocks)
        {
            string sql = $@"
                INSERT INTO T_STOCK (
                    CD_ENDERECO, 
                    CD_EMPRESA, 
                    CD_PRODUTO, 
                    CD_FAIXA, 
                    NU_IDENTIFICADOR, 
                    QT_ESTOQUE, 
                    QT_RESERVA_SAIDA, 
                    QT_TRANSITO_ENTRADA, 
                    DT_UPDROW, 
                    DT_INVENTARIO,
                    ID_AVERIA,
                    ID_INVENTARIO,
                    ID_CTRL_CALIDAD,
                    NU_TRANSACCION,
                    DT_FABRICACAO
                )
                VALUES(
                 :Ubicacion,
                 :Empresa,
                 :Producto,
                 :Faixa,
                 :Identificador,
                 :Cantidad,
                 :ReservaSalida,
                 :CantidadTransitoEntrada,
                 :FechaModificacion,
                 :FechaInventario,
                 :Averia,
                 :Inventario,
                 :ControlCalidad,
                 :NumeroTransaccion,
                 :Vencimiento)";

            await _dapper.ExecuteAsync(connection, sql, stocks, transaction: tran);
        }

        public virtual async Task UpdateStock(DbConnection connection, DbTransaction tran, IEnumerable<Stock> stocks)
        {
            string sql = $@"
                UPDATE T_STOCK SET 
                    QT_ESTOQUE = :Cantidad,
                    QT_RESERVA_SAIDA = :ReservaSalida,
                    DT_UPDROW = :FechaModificacion,
                    NU_TRANSACCION = :NumeroTransaccion,
                    DT_FABRICACAO = :Vencimiento
                WHERE CD_ENDERECO = :Ubicacion
                    AND CD_PRODUTO = :Producto
                    AND CD_FAIXA = :Faixa
                    AND NU_IDENTIFICADOR = :Identificador 
                    AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(connection, sql, stocks, transaction: tran);
        }

        public virtual async Task AddAjustesStock(DbConnection connection, DbTransaction tran, List<AjusteStock> ajustesStocks)
        {
            string sql = $@"
                INSERT INTO T_AJUSTE_STOCK (
                    NU_AJUSTE_STOCK,
                    CD_ENDERECO,
                    CD_EMPRESA,
                    CD_PRODUTO,
                    CD_FAIXA,
                    NU_IDENTIFICADOR,
                    QT_MOVIMIENTO,
                    DT_FABRICACAO,
                    DT_REALIZADO,
                    TP_AJUSTE,
                    CD_MOTIVO_AJUSTE,
                    DS_MOTIVO,
                    NU_TRANSACCION,
                    NU_PREDIO,
                    ID_AREA_AVERIA,
                    DT_MOTIVO,
                    CD_FUNCIONARIO,
                    CD_APLICACAO,
                    VL_METADATA)
                VALUES (
                    :NuAjusteStock ,
                    :Ubicacion ,
                    :Empresa ,
                    :Producto ,
                    :Faixa ,
                    :Identificador ,
                    :QtMovimiento ,
                    :FechaVencimiento ,
                    :FechaRealizado ,
                    :TipoAjuste ,
                    :CdMotivoAjuste ,
                    :DescMotivo ,
                    :NuTransaccion ,
                    :Predio ,
                    :IdAreaAveria ,
                    :FechaMotivo ,
                    :Funcionario ,
                    :Aplicacion,
                    :Metadata)";

            await _dapper.ExecuteAsync(connection, sql, ajustesStocks, transaction: tran);
        }

        public virtual SalidaProduccionDetalle Map(SalidaProduccionDetalle detalle, int empresa, ref List<long> idsDetalleSalidaMov, IProducirProduccionServiceContext context, bool confirmarMovimiento, long nuTransaccion)
        {
            var id = idsDetalleSalidaMov.FirstOrDefault();

            var model = new SalidaProduccionDetalle
            {
                Id = id,
                NuPrdcIngreso = detalle.NuPrdcIngreso,
                Producto = detalle.Producto,
                Empresa = empresa,
                Faixa = detalle.Faixa,
                Ubicacion = detalle.Ubicacion,
                Cantidad = detalle.Cantidad,
                Identificador = detalle.Identificador,
                NuTransaccion = nuTransaccion,
                FechaAlta = detalle.FechaAlta,
                Vencimiento = detalle.Vencimiento,
                FlPendienteNotificar = confirmarMovimiento ? "S" : "N",
                Motivo = detalle.Motivo
            };

            idsDetalleSalidaMov.Remove(id);

            return model;
        }

        public virtual List<long> GetNewIdsDetalleSalidaMov(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_SALIDA_REAL_MOV, count).ToList();
        }

        public virtual List<long> GetNewIdsDetalleSalidaReal(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_NU_PRDC_SALIDA_REAL, count).ToList();
        }

        public virtual List<IngresoProduccionDetalleSalida> GetDetallesSalidaReal(string nroIngresoProduccion)
        {
            var salidas = _context.T_PRDC_DET_SALIDA_REAL.AsNoTracking().Where(d => d.NU_PRDC_INGRESO == nroIngresoProduccion).ToList();
            return _mapperIngresoProduccion.MapEntityToObject(salidas);
        }

        public virtual List<IngresoProduccionDetalleTeorico> GetDetallesTeoricos(string nroIngresoProduccion, string tipo)
        {
            var detallesTeoricos = _context.T_PRDC_DET_INGRESO_TEORICO.Where(x => x.NU_PRDC_INGRESO == nroIngresoProduccion && x.TP_REGISTRO == tipo).ToList();
            return _mapperIngresoProduccion.MapEntityToObject(detallesTeoricos);
        }

        #endregion

        #region API de ConsumirProduccion

        public virtual async Task ConsumirProduccion(ConsumirProduccion consumo, IConsumirProduccionServiceContext context, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var nuTransaccion = await CreateTransaction($"Consumo para el ingreso {context.GetIngreso().Id}", connection, null);

                var bulkContext = GetBulkOperationContext(consumo, context, connection, nuTransaccion);

                context.KeysAjustes = bulkContext.NewAjustesStocks
                    .Select(a => a.NuAjusteStock.ToString())
                    .ToList();

                context.NotificarProduccion = bulkContext.NotificarProduccion;

                using (var tran = connection.BeginTransaction ())
                {
                    await UpdateStock(connection, tran, bulkContext.UpdateStocks.Values);
                    await AddAjustesStock(connection, tran, bulkContext.NewAjustesStocks);

                    await AddDetallesProductosConsumidosReales(connection, tran, bulkContext.NewIngresoProduccionDetallesReales.Values);
                    await UpdateDetallesInsumosReales(connection, tran, bulkContext.UpdateIngresoProduccionDetallesReales.Values);
                    await AddDetallesProductosConsumidosMov(connection, tran, bulkContext.NewIngresoProduccionDetalleMov);

                    if (consumo.IniciarProduccion && string.IsNullOrEmpty(consumo.IdProduccionExterno))
                        await UpdateEspacioProduccion(connection, tran, bulkContext.EspacioProduccion);

                    if (bulkContext.Ingreso != null)
                        await UpdateIngreso(connection, tran, bulkContext.Ingreso);

                    if ((consumo.FinalizarProduccion || consumo.IniciarProduccion) && bulkContext.EspacioProduccion != null)
                        await UpdateEspacioProduccion(connection, tran, bulkContext.EspacioProduccion);

                    tran.Commit();
                }
            }
        }

        public virtual ConsumirProduccionBulkOperationContext GetBulkOperationContext(ConsumirProduccion consumo, IConsumirProduccionServiceContext serviceContext, DbConnection connection, long nuTransaccion)
        {
            var context = new ConsumirProduccionBulkOperationContext();

            var ingreso = serviceContext.GetIngreso();
            var espacioProducion = serviceContext.GetEspacioProduccion();

            var keysStock = serviceContext.DetallesInsumos
                .GroupBy(g => new { Empresa = g.Empresa.Value, g.Producto, Faixa = g.Faixa.Value, g.Identificador })
                .Select(s => new Stock()
                {
                    Ubicacion = espacioProducion.IdUbicacionProduccion,
                    Empresa = s.Key.Empresa,
                    Producto = s.Key.Producto,
                    Faixa = s.Key.Faixa,
                    Identificador = s.Key.Identificador,
                }).ToList();

            var stocks = _stockRepository.GetStocks(keysStock);

            var detallesIngresoReal = GetInsumosReales(ingreso.Id);

            var nuOrden = (detallesIngresoReal.Max(x => x.NuOrden) ?? 0) + 1;

            var countAjustes = consumo.Insumos
                .Where(x => x.Motivo == TipoIngresoProduccion.MOT_CONS_ADS)
                .ToList()
                .Count;

            var idsAjustes = _ajusteRepository.GetNewIdsAjusteStock(countAjustes, connection);
            var idsDetallesIngresoReal = GetNewIdsDetalleIngresoReal(consumo.Insumos.Count, connection);

            foreach (var movimientoConsumo in consumo.Insumos)
            {
                var stock = stocks.FirstOrDefault(s => s.Ubicacion == movimientoConsumo.Ubicacion &&
                    s.Empresa == movimientoConsumo.Empresa &&
                    s.Producto == movimientoConsumo.Producto &&
                    s.Faixa == movimientoConsumo.Faixa &&
                    s.Identificador == movimientoConsumo.Identificador);

                var keyStock = $"{movimientoConsumo.Ubicacion}.{movimientoConsumo.Empresa}.{movimientoConsumo.Producto}.{movimientoConsumo.Faixa}.{movimientoConsumo.Identificador}";

                if (stock != null)
                {
                    stock.Cantidad -= movimientoConsumo.Cantidad;
                    stock.FechaModificacion = DateTime.Now;
                    stock.NumeroTransaccion = nuTransaccion;

                    context.UpdateStocks[keyStock] = stock;

                    if (movimientoConsumo.Motivo == TipoIngresoProduccion.MOT_CONS_ADS)
                    {
                        var idAjusteStock = idsAjustes.FirstOrDefault();

                        var ajuste = new AjusteStock
                        {
                            NuAjusteStock = idAjusteStock,
                            Ubicacion = movimientoConsumo.Ubicacion,
                            Empresa = movimientoConsumo.Empresa,
                            Producto = movimientoConsumo.Producto,
                            Faixa = movimientoConsumo.Faixa,
                            Identificador = movimientoConsumo.Identificador,
                            QtMovimiento = (-1) * movimientoConsumo.Cantidad,
                            FechaVencimiento = stock.Vencimiento,
                            FechaRealizado = DateTime.Now,
                            TipoAjuste = TipoAjusteDb.Stock,
                            CdMotivoAjuste = MotivoAjusteDb.Produccion,
                            DescMotivo = $"Consumo para el Ingreso Nro: {ingreso.Id}",
                            NuTransaccion = nuTransaccion,
                            Predio = espacioProducion.Predio,
                            IdAreaAveria = "N",
                            FechaMotivo = DateTime.Now,
                            Funcionario = serviceContext.UserId,
                            Aplicacion = _application,
                            Metadata = ingreso.Id
                        };

                        idsAjustes.Remove(idAjusteStock);

                        context.NewAjustesStocks.Add(ajuste);
                    }
                    else
                    {
                        var insumosDisponibles = serviceContext.DetallesInsumos
                            .Where(d => d.Empresa == movimientoConsumo.Empresa
                                && d.Producto == movimientoConsumo.Producto
                                && d.Faixa == movimientoConsumo.Faixa
                               && d.Identificador == movimientoConsumo.Identificador
                               && (!movimientoConsumo.Referencia.IsNullOrEmpty() ? d.Referencia == movimientoConsumo.Referencia : true)
                               && (movimientoConsumo.UsarSoloReserva ? d.Consumible == "N" : true))
                            .OrderBy(i => i.Consumible)
                            .ThenBy(d => d.NuOrden)
                            .ToList();

                        var saldoAconsumir = movimientoConsumo.Cantidad;

                        foreach (var insumo in insumosDisponibles)
                        {
                            if (saldoAconsumir == 0)
                                break;

                            var detalleIngresoReal = detallesIngresoReal
                                .OrderBy(d => d.NuOrden)
                                .FirstOrDefault(d => d.Empresa == insumo.Empresa
                                    && d.Producto == insumo.Producto
                                    && d.Faixa == insumo.Faixa
                                    && d.Identificador == insumo.Identificador
                                    && (!movimientoConsumo.Referencia.IsNullOrEmpty() ? d.Referencia == movimientoConsumo.Referencia : true)
                                    && (d.QtReal ?? 0) > 0);


                            if (detalleIngresoReal == null)
                            {
                                detalleIngresoReal = detallesIngresoReal
                                .FirstOrDefault(d => d.Empresa == insumo.Empresa
                                    && d.Producto == insumo.Producto
                                    && d.Faixa == insumo.Faixa
                                    && d.Identificador == insumo.Identificador
                                    && string.IsNullOrEmpty(d.Referencia)
                                    && (d.QtReal ?? 0) == 0);

                                if (detalleIngresoReal == null)
                                {
                                    var keyNuevoDetalleReal = $"{ingreso.Id}.{insumo.Empresa}.{insumo.Producto}.{insumo.Faixa}.{insumo.Identificador}";

                                    if (!context.NewIngresoProduccionDetallesReales.ContainsKey(keyNuevoDetalleReal))
                                    {
                                        var id = idsDetallesIngresoReal.FirstOrDefault();

                                        context.NewIngresoProduccionDetallesReales.Add(keyNuevoDetalleReal, new IngresoProduccionDetalleReal()
                                        {
                                            NuPrdcIngresoReal = id,
                                            NuPrdcIngreso = ingreso.Id,
                                            Producto = insumo.Producto,
                                            Empresa = insumo.Empresa,
                                            Faixa = insumo.Faixa,
                                            Identificador = insumo.Identificador,
                                            QtReal = 0,
                                            NuTransaccion = nuTransaccion,
                                            DtAddrow = DateTime.Now,
                                            QtNotificado = !consumo.ConfirmarMovimiento ? saldoAconsumir : 0,
                                            QtDesafectado = saldoAconsumir,
                                            NuOrden = nuOrden,
                                        });

                                        movimientoConsumo.NuPrdcIngresoReal = id;

                                        idsDetallesIngresoReal.Remove(id);
                                        nuOrden = nuOrden + 1;
                                    }
                                    else
                                    {
                                        var newDetalleReal = context.NewIngresoProduccionDetallesReales[keyNuevoDetalleReal];

                                        newDetalleReal.QtNotificado += !consumo.ConfirmarMovimiento ? saldoAconsumir : 0;
                                        newDetalleReal.QtDesafectado += saldoAconsumir;

                                        movimientoConsumo.NuPrdcIngresoReal = newDetalleReal.NuPrdcIngresoReal;

                                        context.NewIngresoProduccionDetallesReales[keyNuevoDetalleReal] = newDetalleReal;
                                    }
                                }
                                else
                                {
                                    detalleIngresoReal.QtNotificado = (detalleIngresoReal.QtNotificado ?? 0) + (!consumo.ConfirmarMovimiento ? saldoAconsumir : 0);
                                    detalleIngresoReal.QtDesafectado = (detalleIngresoReal.QtDesafectado ?? 0) + saldoAconsumir;
                                    detalleIngresoReal.NuTransaccion = nuTransaccion;

                                    movimientoConsumo.NuPrdcIngresoReal = detalleIngresoReal.NuPrdcIngresoReal;

                                    context.UpdateIngresoProduccionDetallesReales[detalleIngresoReal.NuPrdcIngresoReal] = detalleIngresoReal;
                                }

                                movimientoConsumo.Cantidad = saldoAconsumir;
                                saldoAconsumir = 0;
                            }
                            else
                            {
                                decimal cantidadMovimiento;
                                if (saldoAconsumir > detalleIngresoReal.QtReal)
                                {
                                    cantidadMovimiento = (detalleIngresoReal.QtReal ?? 0);

                                    saldoAconsumir -= (detalleIngresoReal.QtReal ?? 0);
                                    detalleIngresoReal.QtDesafectado = (detalleIngresoReal.QtDesafectado ?? 0) + detalleIngresoReal.QtReal;
                                    detalleIngresoReal.QtNotificado = (detalleIngresoReal.QtNotificado ?? 0) + (!consumo.ConfirmarMovimiento ? detalleIngresoReal.QtReal : 0);
                                    detalleIngresoReal.QtReal = 0;

                                }
                                else
                                {
                                    cantidadMovimiento = saldoAconsumir;

                                    detalleIngresoReal.QtNotificado = (detalleIngresoReal.QtNotificado ?? 0) + (!consumo.ConfirmarMovimiento ? saldoAconsumir : 0);
                                    detalleIngresoReal.QtDesafectado = (detalleIngresoReal.QtDesafectado ?? 0) + saldoAconsumir;
                                    detalleIngresoReal.QtReal -= saldoAconsumir;

                                    if (detalleIngresoReal.QtReal < 0)
                                        detalleIngresoReal.QtReal = 0;

                                    saldoAconsumir = 0;
                                }

                                movimientoConsumo.NuPrdcIngresoReal = detalleIngresoReal.NuPrdcIngresoReal;

                                detalleIngresoReal.NuTransaccion = nuTransaccion;

                                context.UpdateIngresoProduccionDetallesReales[detalleIngresoReal.NuPrdcIngresoReal] = detalleIngresoReal;

                                movimientoConsumo.Cantidad = cantidadMovimiento;

                                if (insumo.Consumible == "N")
                                {
                                    if (stock.ReservaSalida > movimientoConsumo.Cantidad)
                                        stock.ReservaSalida = (stock.ReservaSalida ?? 0) - movimientoConsumo.Cantidad;
                                    else
                                        stock.ReservaSalida = 0;
                                }

                                context.UpdateStocks[keyStock] = stock;
                            }

                            movimientoConsumo.Vencimiento = stock.Vencimiento;

                            var productoConsumidoMov = Map(movimientoConsumo, consumo.Empresa, serviceContext, consumo.ConfirmarMovimiento, nuTransaccion);
                            context.NewIngresoProduccionDetalleMov.Add(productoConsumidoMov);
                        }
                    }
                }
            }

            var idsDetalleIngresoMov = GetNewIdsDetalleIngresoMov(context.NewIngresoProduccionDetalleMov.Count, connection);

            foreach (var nuevoMovimiento in context.NewIngresoProduccionDetalleMov)
            {
                var id = idsDetalleIngresoMov.FirstOrDefault();

                nuevoMovimiento.Id = id;

                idsDetalleIngresoMov.Remove(id);
            }

            context.NotificarProduccion = (consumo.Insumos.Any(p => p.Motivo != TipoIngresoProduccion.MOT_CONS_ADS) && consumo.ConfirmarMovimiento);

            if (consumo.FinalizarProduccion)
            {
                var insumosConSaldo = detallesIngresoReal.Where(w => w.QtReal > 0).ToList();

                foreach (var insumo in insumosConSaldo)
                {
                    var saldoConsumir = (insumo.QtReal ?? 0);

                    insumo.QtReal = 0;
                    insumo.NuTransaccion = nuTransaccion;

                    context.UpdateIngresoProduccionDetallesReales[insumo.NuPrdcIngresoReal] = insumo;

                    var stock = stocks.FirstOrDefault(s => s.Ubicacion == espacioProducion.IdUbicacionProduccion &&
                        s.Empresa == insumo.Empresa &&
                        s.Producto == insumo.Producto &&
                        s.Faixa == insumo.Faixa &&
                        s.Identificador == insumo.Identificador);

                    if (stock != null)
                    {
                        if (stock.ReservaSalida > saldoConsumir)
                            stock.ReservaSalida = (stock.ReservaSalida ?? 0) - saldoConsumir;
                        else
                            stock.ReservaSalida = 0;

                        stock.NumeroTransaccion = nuTransaccion;
                        stock.FechaModificacion = DateTime.Now;

                        var keyStock = $"{espacioProducion.IdUbicacionProduccion}.{insumo.Empresa}.{insumo.Producto}.{insumo.Faixa}.{insumo.Identificador}";

                        context.UpdateStocks[keyStock] = stock;
                    }
                }

                if (context.NotificarProduccion)
                    ingreso.Situacion = SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL;
                else
                {
                    ingreso.Situacion = SituacionDb.PRODUCCION_FINALIZADA;
                    ingreso.FechaFinProduccion = DateTime.Now;

                    espacioProducion.NumeroTransaccion = nuTransaccion;

                    context.EspacioProduccion = espacioProducion;
                }
            }
            else
            {
                if (consumo.IniciarProduccion)
                {
                    espacioProducion.NumeroTransaccion = nuTransaccion;
                    context.EspacioProduccion = espacioProducion;
                }

                if (consumo.IniciarProduccion && consumo.Insumos.Count == 0)
                    ingreso.Situacion = SituacionDb.PRODUCCION_INICIADA;
                else if (context.NotificarProduccion)
                    ingreso.Situacion = SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_PARCIAL;
                else
                    ingreso.Situacion = SituacionDb.PRODUCIENDO;

            }

            ingreso.NuTransaccion = nuTransaccion;
            context.Ingreso = ingreso;

            return context;
        }

        public virtual List<long> GetNewIdsDetalleIngresoMov(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_INGRESO_REAL_MOV, count).ToList();
        }

        public virtual List<long> GetNewIdsDetalleIngresoReal(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<long>(connection, Secuencias.S_NU_PRDC_INGRESO_REAL, count).ToList();
        }

        public virtual async Task AddDetallesProductosConsumidosReales(DbConnection connection, DbTransaction tran, IEnumerable<IngresoProduccionDetalleReal> detallesReales)
        {
            string sql = @"INSERT INTO T_PRDC_DET_INGRESO_REAL
                            (NU_PRDC_INGRESO_REAL,
                            NU_PRDC_INGRESO,
                            CD_PRODUTO,
                            NU_IDENTIFICADOR,
                            CD_EMPRESA,
                            CD_FAIXA,
                            QT_REAL,
                            QT_NOTIFICADO,
                            NU_TRANSACCION,
                            DT_ADDROW,
                            NU_ORDEN,
                            QT_DESAFECTADA)
                        VALUES (
                            :NuPrdcIngresoReal,
                            :NuPrdcIngreso,
                            :Producto,
                            :Identificador,
                            :Empresa,
                            :Faixa,
                            :QtReal,
                            :QtNotificado,
                            :NuTransaccion,
                            :DtAddrow,
                            :NuOrden,
                            :QtDesafectado)";

            await _dapper.ExecuteAsync(connection, sql, detallesReales, transaction: tran);
        }

        public virtual async Task AddDetallesProductosConsumidosMov(DbConnection connection, DbTransaction tran, IEnumerable<IngresoProduccionDetalle> detallesProduccionMov)
        {
            string sql = @"INSERT INTO T_PRDC_DET_INGRESO_REAL_MOV
                    (NU_INGRESO_REAL_MOV,
                    NU_PRDC_INGRESO,
                    NU_PRDC_INGRESO_REAL,
                    CD_PRODUTO,
                    CD_EMPRESA,
                    CD_FAIXA,
                    CD_ENDERECO,
                    QT_ESTOQUE,
                    NU_IDENTIFICADOR,
                    NU_TRANSACCION,
                    DT_ADDROW,
                    DT_VENCIMIENTO,
                    FL_PENDIENTE_NOTIFICAR,
                    ND_MOTIVO) 
                   VALUES (
                    :Id,
                    :NuPrdcIngreso,                          
                    :NuPrdcIngresoReal,                          
                    :Producto,                     
                    :Empresa,                     
                    :Faixa,         
                    :Ubicacion,                      
                    :Cantidad,                
                    :Identificador,                        
                    :NuTransaccion,               
                    :FechaAlta,        
                    :Vencimiento,                     
                    :FlPendienteNotificar,                      
                    :Motivo)";

            await _dapper.ExecuteAsync(connection, sql, detallesProduccionMov, transaction: tran);
        }

        public virtual IEnumerable<IngresoProduccionDetalleReal> GetDetallesInsumos(string id)
        {
            IEnumerable<IngresoProduccionDetalleReal> resultado = new List<IngresoProduccionDetalleReal>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var sql = @"SELECT 
                            SI.CD_PRODUTO AS Producto,
                            SI.CD_EMPRESA AS Empresa,
                            SI.CD_FAIXA AS Faixa,
                            SI.NU_IDENTIFICADOR AS Identificador,
                            SI.QT_NOTIFICADO AS QtNotificado,
                            SI.QT_REAL AS QtReal,
                            SI.QT_REAL_ORIGINAL AS QtRealOriginal,
                            SI.QT_DESAFECTADA AS QtDesafectado,
                            SI.NU_ORDEN as NuOrden,
                            SI.NU_PRDC_INGRESO as NuPrdcIngreso,
                            SI.NU_PRDC_INGRESO_REAL as NuPrdcIngresoReal,
                            SI.DS_REFERENCIA as Referencia,
                            SI.FL_CONSUMIBLE as Consumible
                        FROM V_API_STOCK_INSUMOS SI
                        WHERE SI.NU_PRDC_INGRESO = :Id ";

                resultado = _dapper.Query<IngresoProduccionDetalleReal>(connection, sql, param: new { Id = id });

            }

            return resultado;
        }

        public virtual IEnumerable<IngresoProduccionDetalleReal> GetDetallesInsumos(string id, string idEspacio)
        {
            IEnumerable<IngresoProduccionDetalleReal> resultado = new List<IngresoProduccionDetalleReal>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                var sql = @"SELECT
                                STK.CD_PRODUTO AS Producto,
                                STK.CD_EMPRESA AS Empresa,
                                STK.CD_FAIXA AS Faixa,
                                STK.NU_IDENTIFICADOR  AS Identificador,
                                :qtNotificado  AS QtNotificado,
                                (STK.QT_ESTOQUE - STK.QT_RESERVA_SAIDA)  AS QtReal,
                                :qtRealOriginal  AS QtRealOriginal,
                                :qtDesafectada  AS QtDesafectado,
                                :orden  as NuOrden,
                                :nuIngreso as NuPrdcIngreso,
                                :qtReal as NuPrdcIngresoReal,
                                :dsReferencia as Referencia,
                                :flConsumible as Consumible
                            FROM
                                T_STOCK STK
                                INNER JOIN T_PRODUTO PRD ON STK.CD_EMPRESA = PRD.CD_EMPRESA
                                AND STK.CD_PRODUTO = PRD.CD_PRODUTO
                                INNER JOIN T_PRDC_LINEA LI ON LI.CD_ENDERECO_PRODUCCION = STK.CD_ENDERECO 
                            WHERE
                                (STK.QT_ESTOQUE - STK.QT_RESERVA_SAIDA) > 0
                                AND LI.CD_PRDC_LINEA = :idEspacio"
                ;

                resultado = _dapper.Query<IngresoProduccionDetalleReal>(connection, sql, param: new { idEspacio = idEspacio, qtNotificado = 0, qtRealOriginal = 0, qtDesafectada = 0, orden = 0, nuIngreso = id, qtReal = 0, dsReferencia = " ", flConsumible = "S" });

            }

            return resultado;
        }

        public virtual IngresoProduccionDetalle Map(IngresoProduccionDetalle detalle, int empresa, IConsumirProduccionServiceContext context, bool confirmarMovimiento, long nuTransaccion)
        {
            return new IngresoProduccionDetalle
            {
                NuPrdcIngreso = detalle.NuPrdcIngreso,
                Producto = detalle.Producto,
                Empresa = empresa,
                Faixa = detalle.Faixa,
                Ubicacion = detalle.Ubicacion,
                Cantidad = detalle.Cantidad,
                Identificador = detalle.Identificador,
                NuTransaccion = nuTransaccion,
                FechaAlta = detalle.FechaAlta,
                Vencimiento = detalle.Vencimiento,
                FlPendienteNotificar = confirmarMovimiento ? "S" : "N",
                Motivo = detalle.Motivo,
                NuPrdcIngresoReal = detalle.NuPrdcIngresoReal
            };
        }

        #endregion

        #region API Salida

        public virtual async Task<List<APITask>> GetProduccionesPendientesDeNotificar(CancellationToken cancelToken = default)
        {
            using (var connection = _dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);
                return GetKeysProduccionesPendientesDeNotificar(connection);
            }
        }

        public virtual List<APITask> GetKeysProduccionesPendientesDeNotificar(DbConnection connection)
        {
            var sql = @"SELECT 
                            ID_OPERACION AS Id,
                            DT_OPERACION AS Fecha
                        FROM V_CONFIRMACIONES_PENDIENTES
                        WHERE CD_INTERFAZ_EXTERNA = :cdInterfazExterna 
                            AND FL_HABILITADA = 'S'
                        ORDER BY 
                            DT_OPERACION ASC, 
                            ID_OPERACION ASC";

            return _dapper.Query<APITask>(connection, sql, param: new { cdInterfazExterna = CInterfazExterna.ConfirmacionProduccion }, commandType: CommandType.Text).ToList();
        }

        public virtual async Task<List<long>> GenerarInterfacesConfirmacionProduccion(string nuIngresoProduccion, Func<int?, string> GetGrupoConsulta, CancellationToken cancelToken = default)
        {
            using (var connection = _dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                long nuEjecucion = -2;
                long? nuTransaccion = null;

                try
                {
                    nuTransaccion = await _transaccionRepository.CreateTransaction($"Generación de interfaz de confirmación de producción. Nro. Ingreso producción: {nuIngresoProduccion}", connection, null, _application, _userId);
                }
                catch (Exception ex)
                {
                    nuTransaccion = 0;
                    logger.Error($"Confirmación de producción. Producción: {nuIngresoProduccion} - Error al generar la transacción: {ex}");
                }

                try
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        var dsReferencia = $"Confirmación de producción. Nro. Ingreso producción: {nuIngresoProduccion}";

                        logger.Debug(dsReferencia);

                        var datosProduccion = MapProduccion(nuIngresoProduccion, connection, tran, nuTransaccion.Value, out ConfirmacionProduccionBulkOperationContext context);

                        var interfazHabilitada = _parametroRepository.GetParamInterfazHabilitada(CInterfazExterna.ConfirmacionProduccion, datosProduccion.Empresa, connection, tran);

                        if (!interfazHabilitada)
                        {
                            logger.Debug($"La interfaz {CInterfazExterna.ConfirmacionProduccion} no esta habilitada para la empresa {datosProduccion.Empresa}.");
                            return new List<long>();
                        }

                        var grupoConsulta = GetGrupoConsulta(datosProduccion.Empresa);

                        nuEjecucion = await CrearEjecucion(datosProduccion, grupoConsulta, connection, tran, dsReferencia);
                        context.NroInterfazEjecucion = nuEjecucion;

                        await UpdateProduccion(context, connection, tran);

                        tran.Commit();

                        logger.Debug($"Interfaz de confirmación de producción terminada.");
                    }
                }
                catch (Exception ex)
                {
                    await UpdateProduccion(new ConfirmacionProduccionBulkOperationContext
                    {
                        NroIngresoProduccion = nuIngresoProduccion,
                        NuTransaccion = nuTransaccion.Value,
                        NroInterfazEjecucion = -2,

                    }, connection, tran: null, errorGenerado: true);

                    logger.Error($"Generación de interfaz de confirmación de producción. Producción: {nuIngresoProduccion} - Error: {ex}");
                }

                return new List<long>() { nuEjecucion };
            }
        }

        public virtual ConfirmacionProduccionResponse MapProduccion(string nroIngresoProduccion, DbConnection connection, DbTransaction tran, long nuTransaccion, out ConfirmacionProduccionBulkOperationContext context)
        {
            context = new ConfirmacionProduccionBulkOperationContext
            {
                NroIngresoProduccion = nroIngresoProduccion,
                NuTransaccion = nuTransaccion
            };

            var ingreso = GetIngresoProduccion(nroIngresoProduccion, connection, tran);

            var model = new ConfirmacionProduccionResponse
            {
                IdProduccionExterno = ingreso.IdProduccionExterno,
                Empresa = ingreso.Empresa.Value,
                Tipo = ingreso.Tipo,
                Predio = ingreso.Predio,
                EspacioProduccion = ingreso.IdEspacioProducion,
                FechaInicioProduccion = ingreso.FechaInicioProduccion?.ToString(CDateFormats.DATE_ONLY),
                FechaFinProduccion = ingreso.FechaFinProduccion?.ToString(CDateFormats.DATE_ONLY),
                Anexo1 = ingreso.Anexo1,
                Anexo2 = ingreso.Anexo2,
                Anexo3 = ingreso.Anexo3,
                Anexo4 = ingreso.Anexo4,
                Anexo5 = ingreso.Anexo5
            };

            var insumosPendientes = GetDetallesRealesPendientesNotificar(nroIngresoProduccion, connection, tran);

            foreach (var g in insumosPendientes.GroupBy(i => new
            {
                i.Producto,
                i.Identificador,
                i.Motivo
            }))
            {
                model.Insumos.Add(new InsumoProduccionResponse
                {
                    CodigoProducto = g.Key.Producto,
                    Identificador = g.Key.Identificador,
                    CantidadTeorica = g.Min(x => x.QtRealOriginal),
                    CantidadConsumida = g.Sum(x => x.Cantidad),
                    CodigoMotivo = g.Key.Motivo
                });
            }

            context.InsumosMovimiento.AddRange(insumosPendientes.Select(i => new
            {
                Id = i.Id,
                NuTransaccion = nuTransaccion
            }));

            foreach (var g in insumosPendientes.GroupBy(x => new
            {
                x.Producto,
                x.Identificador,
                x.Empresa,
                x.NuPrdcIngresoReal,
                x.NuPrdcIngreso
            }))
            {
                context.Insumos.Add(new
                {
                    CantidadNotificada = g.Sum(x => x.Cantidad),
                    NuPrdcIngresoReal = g.Key.NuPrdcIngresoReal,
                    NuTransaccion = nuTransaccion
                });
            }

            var salidasPendientes = GetDetallesSalidaRealesPendientesNotificar(nroIngresoProduccion, connection, tran);

            foreach (var g in salidasPendientes.GroupBy(s => new
            {
                s.Producto,
                s.Identificador,
                s.NuPrdcIngresoSalida,
                s.NuPrdcIngreso,
                s.Motivo
            }))
            {
                model.Productos.Add(new ProductoProduccionResponse
                {
                    CodigoProducto = g.Key.Producto,
                    Identificador = g.Key.Identificador,
                    Vencimiento = g.Min(x => x.Vencimiento?.ToString(CDateFormats.DATE_ONLY)),
                    CantidadTeorica = g.Min(x => x.CantidadTeorica),
                    CantidadProducida = g.Sum(x => x.Cantidad),
                    ModalidadCalculoLote = ingreso.IdModalidadLote,
                    CodigoMotivo = g.Key.Motivo,
                });

            }

            context.ProductosFinalesMovimiento.AddRange(salidasPendientes.Select(s => new
            {
                Id = s.Id,
                NuTransaccion = nuTransaccion
            }));


            foreach (var g in salidasPendientes.GroupBy(s => new
            {
                s.Producto,
                s.Identificador,
                s.Empresa,
                s.NuPrdcIngresoSalida,
                s.NuPrdcIngreso
            }))
            {
                context.ProductosFinales.Add(new
                {
                    CantidadNotificada = g.Sum(x => x.Cantidad),
                    NuPrdcIngresoSalida = g.Key.NuPrdcIngresoSalida,
                    NuTransaccion = nuTransaccion
                });
            }

            if (ingreso.Situacion == SituacionDb.PRODUCCION_PENDIENTE_NOTIFICACION_FINAL)
            {
                model.FinProduccion = "S";
                model.FechaFinProduccion = DateTime.Now.ToString(CDateFormats.DATE_ONLY);
                context.FechaFinProduccion = DateTime.Now;
                context.EspacioProduccion = ingreso.IdEspacioProducion;
                context.Situacion = SituacionDb.PRODUCCION_FINALIZADA;
            }
            else
            {
                model.FinProduccion = "N";
                context.FechaFinProduccion = null;
                context.EspacioProduccion = null;
                context.Situacion = SituacionDb.PRODUCCION_PARCIALMENTE_NOTIF;
            }

            return model;
        }

        public virtual async Task UpdateProduccion(ConfirmacionProduccionBulkOperationContext context, DbConnection connection, DbTransaction tran, bool errorGenerado = false)
        {
            if (!errorGenerado)
            {
                var sql = @"INSERT INTO T_PRDC_INGRESO_EJECUCION (
                                NU_PRDC_INGRESO,
                                NU_TRANSACCION, 
                                DT_ADDROW, 
                                NU_INTERFAZ_EJECUCION_SALIDA) 
                            VALUES (
                                :NroIngresoProduccion, 
                                :NuTransaccion, 
                                :FechaAlta, 
                                :NroInterfazEjecucion)";

                await _dapper.ExecuteAsync(connection, sql, param: new
                {
                    NroIngresoProduccion = context.NroIngresoProduccion,
                    NuTransaccion = context.NuTransaccion,
                    FechaAlta = DateTime.Now,
                    NroInterfazEjecucion = context.NroInterfazEjecucion,
                }, transaction: tran);


                sql = @"UPDATE T_PRDC_DET_INGRESO_REAL 
                        SET QT_NOTIFICADO = COALESCE(QT_NOTIFICADO,0) + :CantidadNotificada, 
                            NU_TRANSACCION = :NuTransaccion
                        WHERE NU_PRDC_INGRESO_REAL = :NuPrdcIngresoReal";

                await _dapper.ExecuteAsync(connection, sql, param: context.Insumos, transaction: tran);

                sql = @"UPDATE T_PRDC_DET_INGRESO_REAL_MOV 
                        SET FL_PENDIENTE_NOTIFICAR =  'N', 
                            NU_TRANSACCION = :NuTransaccion
                        WHERE NU_INGRESO_REAL_MOV = :Id";

                await _dapper.ExecuteAsync(connection, sql, param: context.InsumosMovimiento, transaction: tran);

                sql = @"UPDATE T_PRDC_DET_SALIDA_REAL 
                        SET QT_NOTIFICADO =  COALESCE(QT_NOTIFICADO,0) + :CantidadNotificada, 
                            NU_TRANSACCION = :NuTransaccion
                        WHERE NU_PRDC_SALIDA_REAL = :NuPrdcIngresoSalida";

                await _dapper.ExecuteAsync(connection, sql, param: context.ProductosFinales, transaction: tran);

                sql = @"UPDATE T_PRDC_DET_SALIDA_REAL_MOV 
                        SET FL_PENDIENTE_NOTIFICAR =  'N', 
                            NU_TRANSACCION = :NuTransaccion
                        WHERE NU_SALIDA_REAL_MOV = :Id";

                await _dapper.ExecuteAsync(connection, sql, param: context.ProductosFinalesMovimiento, transaction: tran);


                sql = @"UPDATE T_PRDC_INGRESO 
                        SET CD_SITUACAO = :situacion, 
                            NU_TRANSACCION = :NuTransaccion,
                            DT_UPDROW = :FechaModificacion,
                            DT_FIN_PRODUCCION = :FechaFinProduccion,
                            NU_ULT_INTERFAZ_EJECUCION= :NroInterfazEjecucion
                        WHERE NU_PRDC_INGRESO = :NroIngresoProduccion";

                await _dapper.ExecuteAsync(connection, sql, param: new
                {
                    NroIngresoProduccion = context.NroIngresoProduccion,
                    NuTransaccion = context.NuTransaccion,
                    Situacion = context.Situacion,
                    FechaModificacion = DateTime.Now,
                    FechaFinProduccion = context.FechaFinProduccion,
                    NroInterfazEjecucion = context.NroInterfazEjecucion,

                }, transaction: tran);

                sql = @"UPDATE T_PRDC_LINEA 
                        SET NU_PRDC_INGRESO = NULL, 
                            NU_TRANSACCION = :NuTransaccion,
                            DT_UPDROW = :FechaModificacion
                        WHERE CD_PRDC_LINEA = :EspacioProduccion";

                await _dapper.ExecuteAsync(connection, sql, param: new
                {
                    NuTransaccion = context.NuTransaccion,
                    FechaModificacion = DateTime.Now,
                    EspacioProduccion = context.EspacioProduccion
                }, transaction: tran);
            }
            else
            {
                var sql = @"UPDATE T_PRDC_INGRESO 
                            SET NU_TRANSACCION = :NuTransaccion,
                                DT_UPDROW = :FechaModificacion,                                
                                NU_ULT_INTERFAZ_EJECUCION= :NroInterfazEjecucion
                            WHERE NU_PRDC_INGRESO = :NroIngresoProduccion";

                await _dapper.ExecuteAsync(connection, sql, param: new
                {
                    NroIngresoProduccion = context.NroIngresoProduccion,
                    NuTransaccion = context.NuTransaccion,
                    FechaModificacion = DateTime.Now,
                    NroInterfazEjecucion = context.NroInterfazEjecucion,

                }, transaction: tran);
            }
        }

        public virtual IngresoProduccion GetIngresoProduccion(string nroIngresoProduccion, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            CD_EMPRESA as Empresa,
                            CD_FUNCIONARIO as Funcionario,
                            CD_PRDC_DEFINICION as IdFormula,
                            CD_PRDC_LINEA as IdEspacioProducion,
                            CD_PRODUTO_INSUMO_ANCLA_LOTE as ProductoInsumoAnclaLote,
                            CD_SITUACAO as Situacion,
                            DS_ANEXO1 as Anexo1,
                            DS_ANEXO2 as Anexo2,
                            DS_ANEXO3 as Anexo3,
                            DS_ANEXO4 as Anexo4,
                            DS_ANEXO5 as Anexo5,
                            DT_ADDROW as FechaAlta,
                            DT_UPDROW as FechaActualizacion,
                            FL_INGRESO_DIRECTO_A_PRODUCCION as IngresoDirectoProduccion,
                            FL_PERMITIR_AUTOASIGNAR_LINEA as PermitirAutoasignarLinea,
                            ID_GENERAR_PEDIDO as GeneraPedidoId,
                            ID_PRODUCCION_EXTERNO as IdProduccionExterno,
                            MODALIDAD_TRABAJO as ModalidadTrabajo,
                            ND_ASIGNACION_LOTE as IdModalidadLote,
                            ND_TIPO as Tipo,
                            NU_INTERFAZ_EJECUCION_ENTRADA as EjecucionEntrada,
                            NU_LOTE as Lote,
                            NU_POSICION_EN_COLA as PosicionEnCola,
                            NU_PRDC_INGRESO as Id,
                            NU_PRDC_ORIGINAL as NumeroProduccionOriginal,
                            NU_PREDIO as Predio,
                            NU_TRANSACCION as NuTransaccion,
                            QT_FORMULA as CantidadIteracionesFormula,
                            DT_INICIO_PRODUCCION as FechaInicioProduccion,
                            DT_FIN_PRODUCCION as FechaFinProduccion,
                            ID_MANUAL as IdManual,
                            NU_ULT_INTERFAZ_EJECUCION as NroUltInterfazEjecucion
                        FROM T_PRDC_INGRESO WHERE NU_PRDC_INGRESO = :nroIngresoProduccion";

            return _dapper.Query<IngresoBlackBox>(connection, sql, new
            {
                nroIngresoProduccion = nroIngresoProduccion
            }, CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual List<IngresoProduccionDetalle> GetDetallesRealesPendientesNotificar(string nroIngresoProduccion, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            irm.NU_INGRESO_REAL_MOV as Id ,
                            irm.NU_PRDC_INGRESO as NuPrdcIngreso,
                            irm.CD_ENDERECO as Ubicacion,
                            irm.CD_PRODUTO as Producto,
                            irm.NU_IDENTIFICADOR as Identificador,
                            irm.CD_EMPRESA as Empresa,
                            irm.CD_FAIXA as Faixa,
                            irm.QT_ESTOQUE as Cantidad,
                            irm.NU_TRANSACCION as NuTransaccion,
                            irm.DT_ADDROW as FechaAlta,
                            irm.DT_VENCIMIENTO as Vencimiento,
                            irm.ND_MOTIVO as Motivo,
                            irm.FL_PENDIENTE_NOTIFICAR as FlPendienteNotificar,
                            ir.NU_PRDC_INGRESO_REAL as NuPrdcIngresoReal,
                            COALESCE(ir.QT_REAL_ORIGINAL, 0) as QtRealOriginal
                        FROM T_PRDC_DET_INGRESO_REAL_MOV irm 
                        INNER JOIN T_PRDC_DET_INGRESO_REAL ir on irm.CD_EMPRESA = ir.CD_EMPRESA AND
                                irm.CD_FAIXA = ir.CD_FAIXA AND
                                irm.CD_PRODUTO = ir.CD_PRODUTO AND
                                irm.NU_IDENTIFICADOR = ir.NU_IDENTIFICADOR AND
                                irm.NU_PRDC_INGRESO = ir.NU_PRDC_INGRESO AND
                                irm.NU_PRDC_INGRESO_REAL = ir.NU_PRDC_INGRESO_REAL
                        WHERE irm.NU_PRDC_INGRESO = :nroIngresoProduccion AND irm.FL_PENDIENTE_NOTIFICAR = 'S'";

            return _dapper.Query<IngresoProduccionDetalle>(connection, sql, new
            {
                nroIngresoProduccion = nroIngresoProduccion
            }, CommandType.Text, transaction: tran).ToList();
        }

        public virtual List<SalidaProduccionDetalle> GetDetallesSalidaRealesPendientesNotificar(string nroIngresoProduccion, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                            SRM.NU_SALIDA_REAL_MOV as Id,
                            SRM.NU_PRDC_INGRESO as NuPrdcIngreso,
                            SRM.CD_ENDERECO as Ubicacion,
                            SRM.CD_PRODUTO as Producto,
                            SRM.NU_IDENTIFICADOR as Identificador,
                            SRM.CD_EMPRESA as Empresa,
                            SRM.CD_FAIXA as Faixa,
                            SRM.QT_ESTOQUE as Cantidad,
                            SRM.ND_MOTIVO as Motivo,
                            SRM.NU_TRANSACCION as NuTransaccion,
                            SRM.DT_ADDROW as FechaAlta,
                            SRM.DT_VENCIMIENTO as Vencimiento,
                            SRM.FL_PENDIENTE_NOTIFICAR as FlPendienteNotificar,
                            SR.NU_PRDC_SALIDA_REAL as NuPrdcIngresoSalida,
                            COALESCE(IT.QT_TEORICO, 0) as CantidadTeorica
                        FROM T_PRDC_DET_SALIDA_REAL_MOV SRM
                        INNER JOIN T_PRDC_DET_SALIDA_REAL SR on SRM.CD_EMPRESA = SR.CD_EMPRESA AND
                                SRM.CD_FAIXA = SR.CD_FAIXA AND
                                SRM.CD_PRODUTO = SR.CD_PRODUTO AND
                                SRM.NU_IDENTIFICADOR = SR.NU_IDENTIFICADOR AND
                                SRM.NU_PRDC_INGRESO = SR.NU_PRDC_INGRESO
                        LEFT JOIN T_PRDC_DET_INGRESO_TEORICO IT ON SR.NU_PRDC_DET_TEORICO = IT.NU_PRDC_DET_TEORICO
                        WHERE SRM.NU_PRDC_INGRESO = :nroIngresoProduccion AND SRM.FL_PENDIENTE_NOTIFICAR = 'S'";

            return _dapper.Query<SalidaProduccionDetalle>(connection, sql, new
            {
                nroIngresoProduccion = nroIngresoProduccion
            }, CommandType.Text, transaction: tran).ToList();
        }

        public virtual async Task<long> CrearEjecucion(ConfirmacionProduccionResponse model, string grupoConsulta, DbConnection connection, DbTransaction tran, string dsReferencia)
        {
            var ejecucionRepository = new EjecucionRepository(_dapper);

            var interfaz = new InterfazEjecucion
            {
                CdInterfazExterna = CInterfazExterna.ConfirmacionProduccion,
                Situacion = SituacionDb.ProcesadoPendiente,
                Comienzo = DateTime.Now,
                FechaSituacion = DateTime.Now,
                ErrorCarga = "N",
                ErrorProcedimiento = "N",
                Referencia = dsReferencia,
                Empresa = model.Empresa,
                GrupoConsulta = grupoConsulta
            };

            var data = JsonConvert.SerializeObject(model);
            var itfzData = new InterfazData
            {
                Alta = DateTime.Now,
                Data = Encoding.UTF8.GetBytes(data)
            };

            interfaz = await ejecucionRepository.AddEjecucion(interfaz, itfzData, connection, tran);

            return interfaz.Id;
        }

        #endregion

        public virtual async Task<long> CreateTransaction(string descripcion, DbConnection connection, DbTransaction tran)
        {
            var transaccionRepository = new TransaccionRepository(_context, _application, _userId, _dapper);
            return await transaccionRepository.CreateTransaction(descripcion, connection, tran, _application, _userId);
        }

        #endregion
    }
}
