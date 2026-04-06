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
using WIS.Domain.Interfaces;
using WIS.Domain.ManejoStock;
using WIS.Domain.Services.Interfaces;
using WIS.Exceptions;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class AjusteRepository
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly AjusteMapper _mapper;
        protected readonly IDapper _dapper;
        protected ParametroRepository _parametroRepository;

        public AjusteRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new AjusteMapper();
            this._dapper = dapper;
            this._parametroRepository = new ParametroRepository(_context, cdAplicacion, userId, dapper);

        }

        #region Add

        public virtual int Add(AjusteStock ajuste)
        {
            if (ajuste.NuAjusteStock == 0)
                ajuste.NuAjusteStock = GetNextNuAjuste();

            T_AJUSTE_STOCK entity = this._mapper.MapFromAjusteStock(ajuste);
            this._context.T_AJUSTE_STOCK.Add(entity);
            return ajuste.NuAjusteStock;
        }

        public virtual void AddAjusteStockDocumental(DocumentoAjusteStock ajuste, long numeroTransaccion)
        {
            var entity = this._mapper.MapFromAjusteDocumental(ajuste);

            entity.NU_TRANSACCION = numeroTransaccion;

            this._context.T_DOCUMENTO_AJUSTE_STOCK.Add(entity);
        }

        public virtual void AddAjusteStockDocumentalHistorico(DocumentoAjusteStockHistorico ajuste, long numeroTransaccion)
        {
            var entity = this._mapper.MapFromAjusteDocumentalHistorico(ajuste);

            entity.NU_TRANSACCION = numeroTransaccion;

            this._context.T_DOCUMENTO_AJUSTE_STOCK_HIST.Add(entity);
        }

        #endregion

        #region Remove

        public virtual void RemoveAjusteStockDocumental(DocumentoAjusteStock ajuste, long numeroTransaccion)
        {
            var entity = this._mapper.MapFromAjusteDocumental(ajuste);
            var attachedEntity = _context.T_DOCUMENTO_AJUSTE_STOCK.Local.FirstOrDefault(w => w.NU_AJUSTE_STOCK == entity.NU_AJUSTE_STOCK);

            entity.NU_TRANSACCION = numeroTransaccion;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                this._context.T_DOCUMENTO_AJUSTE_STOCK.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DOCUMENTO_AJUSTE_STOCK.Attach(entity);
                this._context.T_DOCUMENTO_AJUSTE_STOCK.Remove(entity);
            }
        }

        public virtual void EliminarAjusteStockDocumentalHistorico(DocumentoAjusteStockHistorico ajuste, long numeroTransaccion)
        {
            var entity = this._mapper.MapFromAjusteDocumentalHistorico(ajuste);
            var attachedEntity = _context.T_DOCUMENTO_AJUSTE_STOCK_HIST.Local.FirstOrDefault(w => w.NU_AJUSTE_STOCK == entity.NU_AJUSTE_STOCK);

            entity.NU_TRANSACCION = numeroTransaccion;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                this._context.T_DOCUMENTO_AJUSTE_STOCK_HIST.Remove(attachedEntity);
            }
            else
            {
                this._context.T_DOCUMENTO_AJUSTE_STOCK_HIST.Attach(entity);
                this._context.T_DOCUMENTO_AJUSTE_STOCK_HIST.Remove(entity);
            }
        }

        #endregion

        #region Update

        public virtual void UpdateAjusteStock(AjusteStock objInterfaz)
        {
            var entity = this._mapper.MapFromAjusteStock(objInterfaz);
            var attachedEntity = _context.T_AJUSTE_STOCK.Local.FirstOrDefault(w => w.NU_AJUSTE_STOCK == entity.NU_AJUSTE_STOCK);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_AJUSTE_STOCK.Attach(entity);
                _context.Entry<T_AJUSTE_STOCK>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateAjusteStockDocumental(DocumentoAjusteStock ajuste, long numeroTransaccion)
        {
            var entity = this._mapper.MapFromAjusteDocumental(ajuste);
            var attachedEntity = _context.T_DOCUMENTO_AJUSTE_STOCK.Local.FirstOrDefault(w => w.NU_AJUSTE_STOCK == entity.NU_AJUSTE_STOCK);

            entity.NU_TRANSACCION = numeroTransaccion;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_DOCUMENTO_AJUSTE_STOCK.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Any

        public virtual bool AnyMotivoAjuste(string motivoAjuste)
        {
            return this._context.T_MOTIVO_AJUSTE.Any(d => d.CD_MOTIVO_AJUSTE == motivoAjuste);
        }

        public virtual bool AnyAjuste(int nroMotivoAjuste)
        {
            return this._context.T_AJUSTE_STOCK.Any(d => d.NU_AJUSTE_STOCK == nroMotivoAjuste);
        }

        #endregion

        #region Get
        public virtual int GetNextNuAjuste()
        {
            return this._context.GetNextSequenceValueInt(_dapper, Secuencias.S_AJUSTE_STOCK);
        }

        public virtual AjusteStock GetAjusteStock(decimal nuAjusteStock)
        {
            var objEntity = this._context.T_AJUSTE_STOCK.Where(x => x.NU_AJUSTE_STOCK == nuAjusteStock)
                .AsNoTracking().FirstOrDefault();

            if (objEntity == null)
                throw new ValidationFailedException("General_Sec0_Error_Error05");

            return this._mapper.MapToAjusteStock(objEntity);
        }

        public virtual List<AjusteStock> GetAjustesStockDocumentalPendientes(int cantidad)
        {
            List<AjusteStock> ajustes = new List<AjusteStock>();
            var tpAjustes = new List<string>() { TipoAjusteDb.Inventario, TipoAjusteDb.Stock, TipoAjusteDb.Automatismo };

            var ajustesEntity = this._context.T_AJUSTE_STOCK
                                    .Where(d => d.NU_DOCUMENTO == "-1"
                                             && d.TP_DOCUMENTO == "-1"
                                             && tpAjustes.Contains(d.TP_AJUSTE))
                                    .AsNoTracking()
                                    .OrderBy(pa => pa.DT_REALIZADO)
                                    .Take(cantidad)
                                    .ToList();

            foreach (var ajuste in ajustesEntity)
            {
                ajustes.Add(this._mapper.MapToAjusteStock(ajuste));
            }

            return ajustes;
        }

        public virtual List<MotivoAjuste> GetsMotivosAjuste()
        {
            return _context.T_MOTIVO_AJUSTE
                .AsNoTracking()
                .Select(m => _mapper.MapToMotivoAjuste(m))
                .ToList();
        }

        public virtual MotivoAjuste GetMotivoAjuste(string codigo)
        {
            T_MOTIVO_AJUSTE entity = _context.T_MOTIVO_AJUSTE.AsNoTracking().FirstOrDefault(w => w.CD_MOTIVO_AJUSTE == codigo);
            return entity == null ? null : this._mapper.MapToMotivoAjuste(entity);
        }

        public virtual string GetNumeroOperacionAjuste()
        {
            return this._context.GetNextSequenceValueInt(_dapper, "S_OP_AJUSTE_DOC").ToString();
        }

        public virtual List<int> GetEmpresasConAjustesPendientes()
        {
            return this._context.T_DOCUMENTO_AJUSTE_STOCK
                .Where(das => das.CD_EMPRESA.HasValue)
                .AsNoTracking()
                .GroupBy(das => das.CD_EMPRESA.Value)
                .Select(grp => grp.Key)
                .ToList();
        }

        public virtual List<DocumentoAjusteStock> GetAjustesDocumentosPorEmpresa(int cdEmpresa)
        {
            List<DocumentoAjusteStock> ajustesDocumentales = new List<DocumentoAjusteStock>();

            var ajustesEntity = this._context.T_DOCUMENTO_AJUSTE_STOCK
                .Where(d => d.CD_EMPRESA == cdEmpresa)
                .AsNoTracking()
                .ToList();

            foreach (var ajuste in ajustesEntity)
            {
                ajustesDocumentales.Add(this._mapper.MapToAjusteDocumental(ajuste));
            }

            return ajustesDocumentales;
        }

        public virtual List<DocumentoAjusteStockHistorico> GetDocumentoAjustesStockHistoricoPorDocumento(string tipoDocumento, string numeroDocumento)
        {
            List<DocumentoAjusteStockHistorico> ajustes = new List<DocumentoAjusteStockHistorico>();

            var ajustesEntity = this._context.T_DOCUMENTO_AJUSTE_STOCK_HIST
                .Where(d => d.NU_DOCUMENTO == numeroDocumento
                    && d.TP_DOCUMENTO == tipoDocumento)
                .AsNoTracking()
                .ToList();

            foreach (var ajuste in ajustesEntity)
            {
                ajustes.Add(this._mapper.MapToAjusteDocumentalHistorico(ajuste));
            }

            return ajustes;
        }

        public virtual DocumentoAjusteStock GetAjustesDocumento(int nroAjuste)
        {
            DocumentoAjusteStock ajustesDocumental = null;

            var ajusteEntity = this._context.T_DOCUMENTO_AJUSTE_STOCK
                .Where(a => a.NU_AJUSTE_STOCK == nroAjuste)
                .AsNoTracking()
                .FirstOrDefault();

            if (ajusteEntity != null)
            {
                ajustesDocumental = this._mapper.MapToAjusteDocumental(ajusteEntity);
            }

            return ajustesDocumental;
        }

        public virtual List<DocumentoAjusteStock> GetAjustesDocumento(List<int> nrosAjuste)
        {
            List<DocumentoAjusteStock> ajustesDocumentales = new List<DocumentoAjusteStock>();

            var ajustesEntity = this._context.T_DOCUMENTO_AJUSTE_STOCK
                .Where(a => nrosAjuste.Contains(a.NU_AJUSTE_STOCK))
                .AsNoTracking()
                .ToList();

            foreach (var ajuste in ajustesEntity)
            {
                ajustesDocumentales.Add(this._mapper.MapToAjusteDocumental(ajuste));
            }

            return ajustesDocumentales;
        }

        #endregion

        #region Dapper

        public virtual List<int> GetNewIdsAjusteStock(int count, DbConnection connection)
        {
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_AJUSTE_STOCK, count).ToList();
        }

        public virtual List<int> GetNewIdsAjusteStock(int count)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            return _dapper.GetNextSequenceValues<int>(connection, Secuencias.S_AJUSTE_STOCK, count, tran).ToList();
        }

        public virtual async Task<List<APITask>> GetAjustesPendientesDeConfirmacion(CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var sql = @"SELECT 
                                ID_OPERACION AS Id,
                                DT_OPERACION AS Fecha
                            FROM V_CONFIRMACIONES_PENDIENTES
                            WHERE CD_INTERFAZ_EXTERNA = :cdInterfazExterna 
                              AND FL_HABILITADA= 'S'
                            ORDER BY 
                                DT_OPERACION ASC, 
                                ID_OPERACION ASC";

                return _dapper.Query<APITask>(connection, sql, param: new { cdInterfazExterna = CInterfazExterna.AjustesDeStock }, commandType: CommandType.Text).ToList();
            }
        }

        public virtual async Task<List<long>> GenerarInterfaces(int nuAjuste, Func<int?, string> GetGrupoConsulta, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                long nuEjecucion = -2;

                try
                {
                    using (var tran = connection.BeginTransaction())
                    {
                        logger.Debug($"Ajuste de Stock. Nro. Ajuste: {nuAjuste}");

                        var ajuste = Map(nuAjuste, connection, tran);
                        var empresa = ajuste.Ajustes.Select(a => a.Empresa).FirstOrDefault();

                        var interfazHabilitada = _parametroRepository.GetParamInterfazHabilitada(CInterfazExterna.AjustesDeStock, empresa, connection, tran);

                        if (!interfazHabilitada)
                        {
                            logger.Debug($"La interfaz {CInterfazExterna.AjustesDeStock} no esta habilitada para la empresa {empresa}.");
                            return new List<long>();
                        }

                        var grupoConsulta = GetGrupoConsulta(empresa);
                        var ejecucion = await CrearEjecucion(ajuste, grupoConsulta, connection, tran);

                        if (ajuste.Ajustes.FirstOrDefault().CantidadMovimiento != 0)
                            nuEjecucion = ejecucion.Id;

                        await UpdateAjusteStock(nuAjuste, nuEjecucion, connection, tran);

                        tran.Commit();

                        logger.Debug($"Ajuste actualizado. Nro InterfazEjecucion: {nuEjecucion}");
                    }
                }
                catch (Exception ex)
                {
                    nuEjecucion = -2;
                    logger.Error($"Ajuste de Stock. Nro. Ajuste: {nuAjuste} - Error: {ex}");
                    await UpdateAjusteStock(nuAjuste, nuEjecucion, connection, null);
                }

                return new List<long>() { nuEjecucion };
            }
        }

        public virtual AjusteStock GetAjuste(int nuAjuste, DbConnection connection, DbTransaction tran)
        {
            string sql = @"SELECT 
                        CD_APLICACAO as Aplicacion,
                        CD_EMPRESA as Empresa,
                        CD_ENDERECO as Ubicacion,
                        CD_FAIXA as Faixa,
                        CD_FUNCIONARIO as Funcionario,
                        CD_FUNC_MOTIVO as FuncionarioMotivo,
                        CD_MOTIVO_AJUSTE as CdMotivoAjuste,
                        CD_PRODUTO as Producto,
                        DS_MOTIVO as DescMotivo,
                        DT_FABRICACAO as FechaVencimiento,
                        DT_MOTIVO as FechaMotivo, 
                        DT_REALIZADO as FechaRealizado,
                        DT_UPDROW as FechaModificacion,
                        ID_AREA_AVERIA as IdAreaAveria,
                        ID_PROCESADO as IdProcesado,
                        ID_PROCESAR as IdProcesar,
                        NU_AJUSTE_STOCK as NuAjusteStock,
                        NU_DOCUMENTO as NuDocumento,
                        NU_IDENTIFICADOR as Identificador,
                        NU_INTERFAZ_EJECUCION as NuInterfazEjecucion,
                        NU_INVENTARIO_ENDERECO_DET as NuInventarioEnderecoDet,
                        NU_LOG_INVENTARIO as NuLogInventario,
                        NU_PREDIO as Predio,
                        NU_TRANSACCION as NuTransaccion,
                        QT_MOVIMIENTO as QtMovimiento,
                        TP_AJUSTE as TipoAjuste,
                        TP_DOCUMENTO as TpDocumento,
                        VL_SERIALIZADO as Serializado,
                        VL_ATRIBUTOS_LPN as Atributos
                FROM T_AJUSTE_STOCK WHERE NU_AJUSTE_STOCK = :nuAjuste";

            return _dapper.Query<AjusteStock>(connection, sql, new
            {
                nuAjuste = nuAjuste
            }, CommandType.Text, transaction: tran).FirstOrDefault();
        }

        public virtual AjustesResponse Map(int nuAjuste, DbConnection connection, DbTransaction tran)
        {
            var ajuste = GetAjuste(nuAjuste, connection, tran);
            var model = new AjustesResponse();

            model.Ajustes.Add(new AjusteResponse()
            {
                NumeroAjusteStock = ajuste.NuAjusteStock,
                Empresa = ajuste.Empresa,
                Ubicacion = ajuste.Ubicacion,
                FechaRealizacion = ajuste.FechaRealizado.ToString(CDateFormats.DATE_ONLY),
                Producto = ajuste.Producto,
                Faixa = ajuste.Faixa,
                Identificador = ajuste.Identificador,
                CantidadMovimiento = ajuste.QtMovimiento ?? 0,
                FechaVencimiento = ajuste.FechaVencimiento?.ToString(CDateFormats.DATE_ONLY),
                Predio = ajuste.Predio,
                TipoAjuste = ajuste.TipoAjuste,
                Motivo = ajuste.CdMotivoAjuste,
                DescripcionMotivo = ajuste.DescMotivo,
                FechaMotivo = ajuste.FechaMotivo?.ToString(CDateFormats.DATE_ONLY),
                AreaAveria = ajuste.IdAreaAveria,
                Aplicacion = ajuste.Aplicacion,
                Funcionario = ajuste.Funcionario,
                FechaUltimaModificacion = ajuste.FechaModificacion?.ToString(CDateFormats.DATE_ONLY),
                Serializado = ajuste.Serializado,
                NroLogInventario = ajuste.NuLogInventario,
                InterfazEjecucion = ajuste.NuInterfazEjecucion,
                Atributos = ajuste.Atributos,
            });

            return model;
        }

        public virtual async Task<InterfazEjecucion> CrearEjecucion(AjustesResponse ajuste, string grupoConsulta, DbConnection connection, DbTransaction tran)
        {
            var ejecucionRepository = new EjecucionRepository(_dapper);

            var interfaz = new InterfazEjecucion
            {
                CdInterfazExterna = CInterfazExterna.AjustesDeStock,
                Situacion = SituacionDb.ProcesadoPendiente,
                Comienzo = DateTime.Now,
                FechaSituacion = DateTime.Now,
                ErrorCarga = "N",
                ErrorProcedimiento = "N",
                Referencia = $"Interfaz de Ajuste de stock. Nro ajuste: {ajuste.Ajustes.FirstOrDefault().NumeroAjusteStock}",
                Empresa = ajuste.Ajustes.FirstOrDefault().Empresa,
                GrupoConsulta = grupoConsulta
            };

            var data = JsonConvert.SerializeObject(ajuste);
            var itfzData = new InterfazData
            {
                Alta = DateTime.Now,
                Data = Encoding.UTF8.GetBytes(data)
            };

            return await ejecucionRepository.AddEjecucion(interfaz, itfzData, connection, tran);
        }

        public virtual async Task UpdateAjusteStock(int nuAjuste, long nroEjecucion, DbConnection connection, DbTransaction tran)
        {
            var template = new
            {
                nuAjuste = nuAjuste,
                nroEjecucion = nroEjecucion,
                Updrow = DateTime.Now
            };

            var param = new DynamicParameters(template);
            string sql = @"UPDATE T_AJUSTE_STOCK 
                SET NU_INTERFAZ_EJECUCION = :nroEjecucion, 
                    DT_UPDROW = :Updrow
                WHERE NU_AJUSTE_STOCK = :nuAjuste";

            await _dapper.ExecuteAsync(connection, sql, param, transaction: tran);
        }

        public virtual void AddAjustesStock(List<AjusteStock> ajustes, IAjustesDeStockServiceContext serviceContext, out List<string> keysAjustes, CancellationToken cancelToken = default)
        {
            keysAjustes = new List<string>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.OpenAsync(cancelToken);

                var context = new AjusteBulkOperationContext();

                using (var tran = connection.BeginTransaction())
                {
                    List<int> ids = GetNewAjustesIds(ajustes.Count, connection, tran);

                    for (int i = 0; i < ajustes.Count; i++)
                    {
                        ajustes[i].NuAjusteStock = ids[i];
                    }

                    foreach (var ajuste in ajustes)
                    {
                        context.NewAjustes.Add(ajuste);
                        Producto prod = serviceContext.GetProducto(ajuste.Empresa, ajuste.Producto);
                        var stock = serviceContext.GetStock(ajuste);

                        if (stock == null)
                        {
                            context.NewStock.Add(ajuste);
                        }
                        else
                        {
                            if ((prod.ManejaFechaVencimiento() || prod.IsFifo()) && stock.Vencimiento < ajuste.FechaVencimiento)
                            {
                                ajuste.FechaVencimiento = stock.Vencimiento;
                            }

                            context.UpdateStock.Add(ajuste);
                        }
                    }

                    BulkInsertAjusteStock(connection, tran, context.NewAjustes);
                    BulkInsertStock(connection, tran, context.NewStock);
                    BulkUpdateStock(connection, tran, context.UpdateStock);

                    tran.Commit();

                    keysAjustes = ids.Select(n => n.ToString()).ToList();
                }
            }

        }

        public virtual async Task BulkUpdateStock(DbConnection connection, DbTransaction tran, List<object> updateStock)
        {
            string sql = @" UPDATE T_STOCK 
                            SET NU_TRANSACCION = :NuTransaccion, 
                                QT_ESTOQUE = QT_ESTOQUE + :QtMovimiento, 
                                DT_FABRICACAO = :FechaVencimiento
                            WHERE CD_ENDERECO = :Ubicacion
                                AND CD_PRODUTO = :Producto
                                AND CD_FAIXA = :Faixa
                                AND NU_IDENTIFICADOR = :Identificador 
                                AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(connection, sql, updateStock, transaction: tran);
        }

        public virtual async Task BulkInsertStock(DbConnection connection, DbTransaction tran, List<object> updateStock)
        {
            string sql = @" INSERT INTO T_STOCK (CD_ENDERECO,CD_EMPRESA,CD_PRODUTO,CD_FAIXA,NU_IDENTIFICADOR,QT_ESTOQUE,QT_RESERVA_SAIDA,
                                                    QT_TRANSITO_ENTRADA,DT_FABRICACAO,ID_AVERIA,ID_INVENTARIO,DT_INVENTARIO,ID_CTRL_CALIDAD,DT_UPDROW,NU_TRANSACCION) values 
                                                    ( :Ubicacion,:Empresa,:Producto,:Faixa,:Identificador,:QtMovimiento,0
                                                    ,0,:FechaVencimiento,:IdAreaAveria,'R',:FechaRealizado,'C',:FechaRealizado,:NuTransaccion)";

            await _dapper.ExecuteAsync(connection, sql, updateStock, transaction: tran);
        }

        public virtual async Task BulkInsertAjusteStock(DbConnection connection, DbTransaction tran, List<object> newAjustes)
        {
            string sql = @"INSERT INTO T_AJUSTE_STOCK 
                            (
                               CD_PRODUTO, CD_FAIXA, NU_IDENTIFICADOR, CD_EMPRESA, DT_REALIZADO,
                               TP_AJUSTE, QT_MOVIMIENTO, DS_MOTIVO, 
                               ID_PROCESAR, CD_MOTIVO_AJUSTE, NU_AJUSTE_STOCK, 
                               NU_TRANSACCION, NU_PREDIO, CD_FUNC_MOTIVO, 
                               DT_MOTIVO, CD_APLICACAO, CD_FUNCIONARIO, ID_AREA_AVERIA, CD_ENDERECO,
                               ID_PROCESADO, NU_INTERFAZ_EJECUCION, 
                               VL_SERIALIZADO, DT_FABRICACAO) 
                        VALUES 
                            (
                                :Producto,:Faixa,:Identificador,:Empresa,:FechaRealizado,
                                :TipoAjuste,:QtMovimiento,:DescMotivo,
                                :IdProcesar,:CdMotivoAjuste,:NuAjusteStock,:NuTransaccion,
                                :Predio,:FuncionarioMotivo,:FechaMotivo,:Aplicacion,:Funcionario,:IdAreaAveria,
                                :Ubicacion,:IdProcesado,:NuInterfazEjecucion,
                                :Serializado,:FechaVencimiento)";

            await _dapper.ExecuteAsync(connection, sql, newAjustes, transaction: tran);
        }

        public virtual void BulkInsertAjusteStock(DbConnection connection, DbTransaction tran, List<AjusteStock> newAjustes)
        {

            _dapper.BulkInsert(connection, tran, newAjustes, "T_AJUSTE_STOCK", new Dictionary<string, Func<AjusteStock, ColumnInfo>>
            {
                    { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                    { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                    { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Identificador)},
                    { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                    { "DT_REALIZADO", x => new ColumnInfo(x.FechaRealizado)},
                    { "TP_AJUSTE", x => new ColumnInfo(x.TipoAjuste)},
                    { "QT_MOVIMIENTO", x => new ColumnInfo(x.QtMovimiento,DbType.Decimal)},
                    { "DS_MOTIVO", x => new ColumnInfo(x.DescMotivo, DbType.String)},
                    { "ID_PROCESAR", x => new ColumnInfo(x.IdProcesar,DbType.String)},
                    { "CD_MOTIVO_AJUSTE", x => new ColumnInfo(x.CdMotivoAjuste,DbType.String)},
                    { "NU_AJUSTE_STOCK", x => new ColumnInfo(x.NuAjusteStock,DbType.Int32)},
                    { "NU_TRANSACCION", x => new ColumnInfo(x.NuTransaccion,DbType.Int64)},
                    { "NU_PREDIO", x => new ColumnInfo(x.Predio,DbType.String)},
                    { "CD_FUNC_MOTIVO", x => new ColumnInfo(x.FuncionarioMotivo,DbType.Int32)},
                    { "DT_MOTIVO", x => new ColumnInfo(x.FechaMotivo,DbType.DateTime)},
                    { "CD_APLICACAO", x => new ColumnInfo(x.Aplicacion,DbType.String)},
                    { "CD_FUNCIONARIO", x => new ColumnInfo(x.Funcionario,DbType.Int32)},
                    { "ID_AREA_AVERIA", x => new ColumnInfo(x.IdAreaAveria,DbType.String)},
                    { "CD_ENDERECO", x => new ColumnInfo(x.Ubicacion)},
                    { "ID_PROCESADO", x => new ColumnInfo(x.IdProcesado,DbType.String)},
                    { "NU_INTERFAZ_EJECUCION", x => new ColumnInfo(x.NuInterfazEjecucion,DbType.Int64)},
                    { "VL_SERIALIZADO", x => new ColumnInfo(x.Serializado,DbType.String)},
                    { "DT_FABRICACAO", x => new ColumnInfo(x.FechaVencimiento,DbType.DateTime)},
                    { "NU_DOCUMENTO", x => new ColumnInfo(x.NuDocumento,DbType.String)},
                    { "VL_ATRIBUTOS_LPN", x => new ColumnInfo(x.Atributos,DbType.String)},
            });
        }

        public virtual List<int> GetNewAjustesIds(int count, DbConnection connection, DbTransaction tran)
        {
            return _dapper.GetNextSequenceValues<int>(connection, "S_AJUSTE_STOCK", count, tran).ToList();
        }
        #endregion
    }
}
