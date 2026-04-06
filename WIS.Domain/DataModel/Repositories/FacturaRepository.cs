using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General.API.Bulk.Facturas;
using WIS.Domain.Recepcion;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class FacturaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly Logger _logger = LogManager.GetCurrentClassLogger();
        protected readonly IDapper _dapper;
        protected readonly FacturaMapper _mapper;

        public FacturaRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._dapper = dapper;
            this._mapper = new FacturaMapper();
        }

        #region Any

        public virtual bool AnyFacturaActivaBySerieYNum(int empresa, string cliente, string numFactura, string serie)
        {
            return this._context.T_RECEPCION_FACTURA
                .Any(d => d.CD_EMPRESA == empresa
                    && d.CD_CLIENTE == cliente
                    && d.NU_FACTURA == numFactura
                    && d.NU_SERIE == serie
                    && d.ND_ESTADO != EstadoFacturaDb.Cancelada);
        }

        public virtual bool AnyFacturaAsociadaAgenda(int nroAgenda)
        {
            return this._context.T_RECEPCION_FACTURA
                .AsNoTracking()
                .Any(f => f.NU_AGENDA == nroAgenda);
        }

        #endregion

        #region Get

        public virtual Factura GetFactura(int idFactura, bool mapDetalle = true)
        {
            T_RECEPCION_FACTURA factura = this._context.T_RECEPCION_FACTURA
                .Where(d => d.NU_RECEPCION_FACTURA == idFactura)
                .AsNoTracking()
                .FirstOrDefault();

            List<T_RECEPCION_FACTURA_DET> detalles = null;
            if (mapDetalle)
                detalles = this._context.T_RECEPCION_FACTURA_DET
                .AsNoTracking()
                .Where(d => d.NU_RECEPCION_FACTURA == idFactura)
                .ToList();

            if (factura == null)
                return null;

            return this._mapper.MapToObject(factura, detalles);
        }

        public virtual FacturaDetalle GetFacturaDetalle(int numeroFactura, int idEmpresa, string codigoProducto, decimal faixa, string identificador)
        {
            var detalle = this._context.T_RECEPCION_FACTURA_DET.AsNoTracking().Where(d => d.NU_RECEPCION_FACTURA == numeroFactura
                                                                             && d.CD_EMPRESA == idEmpresa
                                                                             && d.CD_PRODUTO == codigoProducto
                                                                             && d.CD_FAIXA == faixa
                                                                             && d.NU_IDENTIFICADOR == identificador)
                                                                    .FirstOrDefault();
            if (detalle == null)
                return null;

            return this._mapper.MapToObject(detalle);
        }

        public virtual List<Factura> GetFacturasByAgenda(int agenda)
        {
            List<T_RECEPCION_FACTURA> entityfacturas = this._context.T_RECEPCION_FACTURA
                .Include("T_RECEPCION_FACTURA_DET")
                .Where(d => d.NU_AGENDA == agenda)
                .AsNoTracking()
                .ToList();

            List<Factura> colFacturas = new List<Factura>();

            foreach (var item in entityfacturas)
            {
                colFacturas.Add(this._mapper.MapFacturaToObject(item));
            }

            return colFacturas;
        }

        public virtual Factura GetFacturaCabezal(int idFactura)
        {
            T_RECEPCION_FACTURA factura = this._context.T_RECEPCION_FACTURA.Include(s => s.T_RECEPCION_FACTURA_DET).AsNoTracking().Where(d => d.NU_RECEPCION_FACTURA == idFactura).FirstOrDefault();

            if (factura == null)
                return null;

            return this._mapper.MapToObject(factura, factura.T_RECEPCION_FACTURA_DET);
        }

        #endregion

        #region Add

        public virtual void AddFactura(Factura factura)
        {
            factura.Id = this._context.GetNextSequenceValueInt(_dapper, "S_RECEPCION_FACTURA");

            T_RECEPCION_FACTURA entity = this._mapper.MapToEntity(factura);

            this._context.T_RECEPCION_FACTURA.Add(entity);

            if (factura.Detalles != null)
            {
                foreach (var detalle in factura.Detalles)
                {
                    this.AddFacturaDetalle(detalle);
                }
            }
        }

        public virtual void AddFacturaDetalle(FacturaDetalle detalle)
        {
            detalle.Id = this._context.GetNextSequenceValueInt(_dapper, "S_RECEPCION_FACTURA_DET");

            T_RECEPCION_FACTURA_DET entity = this._mapper.MapToEntity(detalle);
            entity.NU_TRANSACCION = _context.GetTransactionNumber();
            this._context.T_RECEPCION_FACTURA_DET.Add(entity);
        }

        #endregion

        #region Update

        public virtual void AddUpdateIdAgendaFactura(Factura factura, bool updateDetalles = true)
        {
            T_RECEPCION_FACTURA entity = this._mapper.MapToEntity(factura);

            T_RECEPCION_FACTURA attachedEntity = this._context.T_RECEPCION_FACTURA.Local
                .Where(d => d.NU_RECEPCION_FACTURA == factura.Id)
                .FirstOrDefault();

            if (updateDetalles)
                this.AddUpdateIdAgendaFacturaDetalles(factura.Detalles);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_RECEPCION_FACTURA.Attach(entity);
                _context.Entry<T_RECEPCION_FACTURA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void AddUpdateIdAgendaFacturaDetalles(List<FacturaDetalle> detalles)
        {
            foreach (var detalle in detalles)
            {
                T_RECEPCION_FACTURA_DET entity = this._mapper.MapToEntity(detalle);

                T_RECEPCION_FACTURA_DET attachedEntity = this._context.T_RECEPCION_FACTURA_DET.Local
                    .Where(d => d.NU_RECEPCION_FACTURA == detalle.IdFactura
                        && d.NU_RECEPCION_FACTURA_DET == detalle.Id)
                    .FirstOrDefault();

                if (attachedEntity != null)
                {
                    var attachedEntry = _context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                    attachedEntry.State = EntityState.Modified;
                }
                else
                {
                    _context.T_RECEPCION_FACTURA_DET.Attach(entity);
                    _context.Entry<T_RECEPCION_FACTURA_DET>(entity).State = EntityState.Modified;
                }
            }
        }

        public virtual void UpdateFactura(Factura factura)
        {
            T_RECEPCION_FACTURA entity = this._mapper.MapFacturaSinDetallesToEntity(factura);
            T_RECEPCION_FACTURA attachedEntity = this._context.T_RECEPCION_FACTURA.Local.Where(d => d.NU_RECEPCION_FACTURA == factura.Id).FirstOrDefault();

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_RECEPCION_FACTURA.Attach(entity);
                _context.Entry<T_RECEPCION_FACTURA>(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateFacturaDetalle(FacturaDetalle detalle)
        {
            T_RECEPCION_FACTURA_DET entity = this._mapper.MapToEntity(detalle);
            T_RECEPCION_FACTURA_DET attachedEntity = this._context.T_RECEPCION_FACTURA_DET.Local.Where(d => d.NU_RECEPCION_FACTURA_DET == detalle.Id).FirstOrDefault();

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_RECEPCION_FACTURA_DET.Attach(entity);
                _context.Entry<T_RECEPCION_FACTURA_DET>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteFacturaDetalle(FacturaDetalle detalle)
        {
            T_RECEPCION_FACTURA_DET entity = this._context.T_RECEPCION_FACTURA_DET.Local.Where(d => d.NU_RECEPCION_FACTURA_DET == detalle.Id).FirstOrDefault();
            if (entity == null)
            {
                entity = this._mapper.MapToEntity(detalle);

                this._context.T_RECEPCION_FACTURA_DET.Attach(entity);
            }

            this._context.T_RECEPCION_FACTURA_DET.Remove(entity);
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<Factura> GetFacturas(IEnumerable<Factura> facturas)
        {
            IEnumerable<Factura> resultado = new List<Factura>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_RECEPCION_FACTURA_TEMP (NU_FACTURA, CD_EMPRESA, NU_SERIE, CD_CLIENTE) VALUES (:NumeroFactura, :IdEmpresa, :Serie, :CodigoInternoCliente)";
                    _dapper.Execute(connection, sql, facturas, transaction: tran);

                    sql = @"SELECT RF.NU_RECEPCION_FACTURA AS Id
                        ,RF.NU_FACTURA AS NumeroFactura
                        ,RF.NU_SERIE AS Serie
                        ,RF.TP_FACTURA as TipoFactura
                        ,RF.CD_EMPRESA AS IdEmpresa
                        ,RF.CD_CLIENTE AS CodigoInternoCliente
                        ,RF.NU_PREDIO AS IdPredio                        
                        ,RF.ND_ESTADO AS Estado
                        FROM T_RECEPCION_FACTURA RF
                        INNER JOIN T_RECEPCION_FACTURA_TEMP T ON RF.NU_FACTURA = T.NU_FACTURA
                            AND RF.CD_EMPRESA = T.CD_EMPRESA
                            AND RF.NU_SERIE = T.NU_SERIE
                            AND RF.CD_CLIENTE = T.CD_CLIENTE";

                    resultado = _dapper.Query<Factura>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public virtual async Task AddFacturas(List<Factura> facturas, IFacturaServiceContext context, CancellationToken cancelToken = default)
        {
            await AddFacturas(GetBulkOperationContext(facturas, context), cancelToken);
        }

        public virtual async Task AddFacturas(FacturaBulkOperationContext context, CancellationToken cancelToken)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    await BulkInsertFacturas(connection, tran, context.NewFacturas);
                    await BulkInsertDetalles(connection, tran, context.NewDetalles);

                    tran.Commit();
                }
            }
        }

        public virtual FacturaBulkOperationContext GetBulkOperationContext(List<Factura> facturas, IFacturaServiceContext serviceContext)
        {
            var context = new FacturaBulkOperationContext();

            foreach (var factura in facturas)
            {
                context.NewFacturas.Add(GetFacturaEntity(factura));

                foreach (var linea in factura.Detalles)
                {
                    linea.NumeroTransaccion = factura.NumeroTransaccion;

                    context.NewDetalles.Add(GetFacturaDetalleEntity(linea, factura));
                }
            }

            return context;
        }

        public virtual object GetFacturaEntity(Factura factura)
        {
            return new
            {
                Factura = factura.NumeroFactura,
                IdFactura = factura.Id,
                Serie = factura.Serie,
                TipoFactura = factura.TipoFactura,
                Empresa = factura.IdEmpresa,
                Anexo1 = factura.Anexo1,
                Anexo2 = factura.Anexo2,
                Anexo3 = factura.Anexo3,
                Predio = factura.Predio,
                Observacion = factura.Observacion,
                Cliente = factura.CodigoInternoCliente,
                FechaInsercion = DateTime.Now,
                Situacion = SituacionDb.Activo,
                Moneda = factura.CodigoMoneda,
                FechaEmision = factura.FechaEmision,
                Estado = EstadoFacturaDb.Pendiente,
                FechaVencimiento = factura.FechaVencimiento,
                Transaccion = factura.NumeroTransaccion,
                IdOrigen = "I",
                TotalDigitado = factura.TotalDigitado,
                IvaBase = factura.IvaBase,
                IvaMinimo = factura.IvaMinimo
            };
        }

        public virtual async Task BulkInsertFacturas(DbConnection connection, DbTransaction tran, List<object> facturas)
        {
            string sql = @"INSERT INTO T_RECEPCION_FACTURA 
                    (CD_CLIENTE, 
                    CD_EMPRESA,
                    CD_MONEDA,
                    CD_SITUACAO,
                    DS_ANEXO1,
                    DS_ANEXO2,
                    DS_ANEXO3,
                    DS_OBSERVACION,
                    DT_ADDROW,
                    DT_EMISION,
                    DT_VENCIMIENTO,
                    ND_ESTADO,
                    NU_PREDIO,
                    NU_RECEPCION_FACTURA,
                    NU_FACTURA,
                    TP_FACTURA,
                    NU_SERIE,
                    NU_TRANSACCION,
                    IM_TOTAL_DIGITADO,
                    IM_IVA_BASE,
                    IM_IVA_MINIMO,
                    ID_ORIGEN) 
                    VALUES 
                    (:Cliente,
                    :Empresa,
                    :Moneda,
                    :Situacion,
                    :Anexo1,
                    :Anexo2,
                    :Anexo3,
                    :Observacion,
                    :FechaInsercion,
                    :FechaEmision,
                    :FechaVencimiento,
                    :Estado,
                    :Predio,
                    :IdFactura,
                    :Factura,
                    :TipoFactura,
                    :Serie,
                    :Transaccion,
                    :TotalDigitado,
                    :IvaBase,
                    :IvaMinimo,
                    :IdOrigen)";

            await _dapper.ExecuteAsync(connection, sql, facturas, transaction: tran);
        }

        public virtual object GetFacturaDetalleEntity(FacturaDetalle detalle, Factura factura)
        {
            return new
            {
                Empresa = detalle.IdEmpresa,
                Faixa = detalle.Faixa,
                CodigoProducto = detalle.Producto,
                FechaInsercion = DateTime.Now,
                FechaVencimiento = detalle.FechaVencimiento,
                ImporteUnitario = detalle.ImporteUnitario,
                Identificador = string.IsNullOrEmpty(detalle.Identificador) ? "(AUTO)" : detalle.Identificador.Trim(),
                IdFactura = detalle.IdFactura,
                IdFacturaDetalle = detalle.Id,
                CantidadFacturada = detalle.CantidadFacturada,
                CantidadValidada = 0,
                CantidadRecibida = 0,
                Transaccion = factura.NumeroTransaccion,
                Factura = factura.NumeroFactura,
                Serie = factura.Serie,
                Cliente = factura.CodigoInternoCliente,
                Anexo1 = detalle.Anexo1,
                Anexo2 = detalle.Anexo2,
                Anexo3 = detalle.Anexo3,
                Anexo4 = detalle.Anexo4,
            };
        }

        public virtual async Task BulkInsertDetalles(DbConnection connection, DbTransaction tran, List<object> detalles)
        {
            var sql = @" INSERT INTO T_RECEPCION_FACTURA_DET
                            (CD_EMPRESA,
                            CD_FAIXA,
                            CD_PRODUTO,
                            DT_ADDROW,
                            DT_VENCIMIENTO,
                            IM_UNITARIO_DIGITADO,
                            NU_IDENTIFICADOR,
                            NU_RECEPCION_FACTURA,
                            NU_RECEPCION_FACTURA_DET,
                            QT_FACTURADA,
                            QT_VALIDADA,
                            QT_RECIBIDA,
                            NU_TRANSACCION,
                            DS_ANEXO1,
                            DS_ANEXO2,
                            DS_ANEXO3,
                            DS_ANEXO4) 
                        SELECT 
                            :Empresa,
                            :Faixa,
                            :CodigoProducto,
                            :FechaInsercion,
                            :FechaVencimiento,
                            :ImporteUnitario,
                            :Identificador,
                            RF.NU_RECEPCION_FACTURA,
                            :IdFacturaDetalle,
                            :CantidadFacturada,
                            :CantidadValidada,
                            :CantidadRecibida,
                            :Transaccion,
                            :Anexo1,
                            :Anexo2,
                            :Anexo3,
                            :Anexo4
                        FROM T_RECEPCION_FACTURA RF
                        WHERE RF.NU_FACTURA = :Factura
                            AND RF.CD_EMPRESA = :Empresa
                            AND RF.NU_SERIE = :Serie
                            AND RF.CD_CLIENTE = :Cliente
                            AND (RF.ND_ESTADO IS NULL OR RF.ND_ESTADO <> 'CANCELADA')";

            await _dapper.ExecuteAsync(connection, sql, detalles, transaction: tran);
        }

        public virtual async Task<Factura> GetFacturaOrNull(string nuFactura, int empresa, string serie, string tipoAgente, string codigoAgente, CancellationToken cancelToken = default)
        {
            var agente = new AgenteRepository(_context, _application, _userId, _dapper).GetAgenteOrNull(empresa, codigoAgente, tipoAgente).Result;

            if (agente == null)
                return null;

            return await GetFacturaOrNull(nuFactura, empresa, serie, agente.CodigoInterno, cancelToken);
        }

        public virtual async Task<Factura> GetFacturaOrNull(string nuFactura, int empresa, string serie, string cliente, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                var model = GetFactura(new Factura()
                {
                    NumeroFactura = nuFactura,
                    IdEmpresa = empresa,
                    Serie = serie,
                    CodigoInternoCliente = cliente
                }, connection);

                Fill(connection, model);

                return model;
            }
        }

        public virtual Factura GetFactura(Factura model, DbConnection connection, DbTransaction tran = null)
        {
            var param = new DynamicParameters(new
            {
                idFactura = model.Id,
                nuFactura = model.NumeroFactura,
                cdEmpresa = model.IdEmpresa,
                nuSerie = model.Serie,
                cdCliente = model.CodigoInternoCliente
            });

            string sql = @"SELECT 
                    NU_RECEPCION_FACTURA as Id,
                    CD_CLIENTE as CodigoInternoCliente,
                    CD_EMPRESA as IdEmpresa,
                    CD_MONEDA as CodigoMoneda,
                    CD_SITUACAO as Situacion,
                    DS_ANEXO1 as Anexo1,
                    DS_ANEXO2 as Anexo2,
                    DS_ANEXO3 as Anexo3,
                    DS_OBSERVACION as Observacion,
                    DT_ADDROW as FechaCreacion,
                    DT_EMISION as FechaEmision,
                    DT_UPDROW as FechaModificacion,
                    DT_VENCIMIENTO as FechaVencimiento,
                    ID_ORIGEN as IdOrigen,
                    IM_IVA_BASE as IvaBase,
                    IM_IVA_MINIMO as IvaMinimo,
                    IM_TOTAL_DIGITADO as TotalDigitado,
                    ND_ESTADO as Estado,
                    NU_AGENDA as Agenda,
                    NU_PREDIO as Predio,
                    NU_FACTURA as NumeroFactura,
                    TP_FACTURA as TipoFactura,
                    NU_REFERENCIA as Referencia,
                    NU_REMITO as Remito,
                    NU_SERIE as Serie
                FROM T_RECEPCION_FACTURA ";

            if (string.IsNullOrEmpty(model.NumeroFactura))
            {
                sql += @"WHERE NU_RECEPCION_FACTURA = :idFactura";
            }
            else
            {
                sql += @"WHERE NU_FACTURA = :nuFactura AND CD_EMPRESA = :cdEmpresa
                AND NU_SERIE = :nuSerie AND CD_CLIENTE = :cdCliente";
            }

            return _dapper.Query<Factura>(connection, sql, param: param, commandType: CommandType.Text, transaction: tran)
                .OrderBy(f => f.Estado == EstadoFacturaDb.Cancelada ? 1 : 0)
                .FirstOrDefault();
        }

        public virtual void Fill(DbConnection connection, Factura model)
        {
            if (model != null)
            {
                model.Detalles = GetDetallesFactura(connection, new FacturaDetalle()
                {
                    IdFactura = model.Id
                });
            }
        }

        public virtual List<FacturaDetalle> GetDetallesFactura(DbConnection connection, FacturaDetalle model)
        {
            var param = new DynamicParameters(new
            {
                idFactura = model.IdFactura,
                producto = model.Producto,
                empresa = model.IdEmpresa,
                identificador = model.Identificador
            });

            string sql = GetSqlSelectFacturaDetalle() +
                @"WHERE RFD.NU_RECEPCION_FACTURA = :idFactura ";

            if (!string.IsNullOrEmpty(model.Producto))
            {
                sql += @"AND RFD.CD_EMPRESA = :empresa 
                    AND RFD.CD_PRODUTO = :producto 
                    AND RFD.NU_IDENTIFICADOR = :identificador ";
            }

            return _dapper.Query<FacturaDetalle>(connection, sql, param: param, commandType: CommandType.Text).ToList();
        }

        public static string GetSqlSelectFacturaDetalle()
        {
            return @"SELECT 
                        RFD.CD_EMPRESA as IdEmpresa,
                        RFD.CD_FAIXA as Faixa,
                        RFD.CD_PRODUTO as Producto,
                        RFD.DT_ADDROW as FechaCreacion,
                        RFD.DT_UPDROW as FechaModificacion,
                        RFD.DT_VENCIMIENTO as FechaVencimiento,
                        RFD.IM_UNITARIO_DIGITADO as ImporteUnitario,
                        RFD.NU_IDENTIFICADOR as Identificador,
                        RFD.NU_RECEPCION_FACTURA as IdFactura,
                        RFD.NU_RECEPCION_FACTURA_DET as Id,
                        RFD.QT_FACTURADA as CantidadFacturada,
                        RFD.QT_VALIDADA as CantidadValidada,
                        RFD.QT_RECIBIDA as CantidadRecibida,
                        RFD.DS_ANEXO1 as Anexo1,
                        RFD.DS_ANEXO2 as Anexo2,
                        RFD.DS_ANEXO3 as Anexo3,
                        RFD.DS_ANEXO4 as Anexo4
                    FROM T_RECEPCION_FACTURA_DET RFD ";
        }

        public virtual bool EsPosibleDesvincularFactura(int idAgenda, int idFactura)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                var sql = @$"select rf.NU_RECEPCION_FACTURA
                                    from t_recepcion_factura rf
                                    inner join t_recepcion_factura_det rfd on rf.NU_RECEPCION_FACTURA = rfd.NU_RECEPCION_FACTURA
                                    inner join t_agenda a on rf.nu_agenda = a.nu_agenda
                                        where rf.nu_agenda = {idAgenda}
                                            and rf.nu_recepcion_factura = {idFactura}
                                            and a.cd_situacao = 4
                                        group by rf.NU_RECEPCION_FACTURA
                                        having sum(rfd.qt_validada) = 0";

                return connection.Query<int>(sql).ToList().Count() > 0;
            }
        }

        public virtual void ValidarFacturas(int nuAgenda)
        {
            string sql = "PR_VALIDAR_FACTURA_RECEPCION";
            _dapper.Query<object>(_context.Database.GetDbConnection(), sql, new
            {
                P_Agenda = nuAgenda
            }, commandType: CommandType.StoredProcedure, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        public virtual List<long> GetFacturasNoUsadasEnAgenda(int nuAgenda)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();

            var sql = @"SELECT NU_RECEPCION_FACTURA FROM V_QUITAR_AGENDA_FACTURAS WHERE NU_AGENDA = :nuAgenda";

            return _dapper.Query<long>(connection, sql, new { nuAgenda }, commandType: CommandType.Text, transaction: tran).ToList();
        }

        public virtual void LiberarFacturas(long nuRecepcionFactura, long nuTransaccion)
        {
            var connection = _context.Database.GetDbConnection();
            var tran = _context.Database.CurrentTransaction?.GetDbTransaction();
            var parametros = new
            {
                nuFactura = nuRecepcionFactura,
                nuTransaccion = nuTransaccion,
                FechaModificacion = DateTime.Now,
            };

            var sql = @"UPDATE T_RECEPCION_FACTURA 
                        SET NU_AGENDA = null, 
                            NU_TRANSACCION = :nuTransaccion,
                            DT_UPDROW = :FechaModificacion
                        WHERE NU_RECEPCION_FACTURA = :nuFactura";

            _dapper.Execute(connection, sql, param: parametros, transaction: tran);

            sql = @"UPDATE T_RECEPCION_FACTURA_DET 
                    SET QT_VALIDADA = 0,
                        NU_TRANSACCION = :nuTransaccion,
                        DT_UPDROW = :FechaModificacion
                    WHERE NU_RECEPCION_FACTURA = :nuFactura";

            _dapper.Execute(connection, sql, param: parametros, transaction: tran);
        }

        #endregion
    }
}
