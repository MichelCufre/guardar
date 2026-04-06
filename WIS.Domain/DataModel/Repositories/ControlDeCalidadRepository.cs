using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Domain.StockEntities;
using WIS.Persistence.Database;
using WIS.Persistence.General;

namespace WIS.Domain.DataModel.Repositories
{
    public class ControlDeCalidadRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly ControlDeCalidadMapper _mapper;

        public ControlDeCalidadRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ControlDeCalidadMapper();
            _dapper = dapper;
        }

        #region Any

        public virtual bool AnyControlDeCalidadClase(int id)
        {
            return this._context.T_TIPO_CTR_CALIDAD.Any(d => d.CD_CONTROL == id);
        }

        public virtual bool QuedaControlPendiente(int codigo)
        {
            return _context.T_CTR_CALIDAD_NECESARIO.Any(tccn => tccn.CD_CONTROL == codigo) || _context.T_CTR_CALIDAD_PENDIENTE.Any(tcc => tcc.CD_CONTROL == codigo);
        }

        public virtual bool AnyControlDeCalidadPendiente(int control, int empresa, string producto)
        {
            return this._context.T_CTR_CALIDAD_PENDIENTE
                .Any(d => d.CD_CONTROL == control && d.CD_EMPRESA == empresa && d.CD_PRODUTO == producto);
        }

        public virtual bool AnyControlDeCalidadPendienteSinAceptar(int control, int empresa, string producto)
        {
            return this._context.T_CTR_CALIDAD_PENDIENTE.Any(d => d.CD_CONTROL == control && d.CD_EMPRESA == empresa && d.CD_PRODUTO == producto && d.ID_ACEPTADO == "N");
        }

        public virtual bool AnyControlDeCalidadPendienteSinAceptar(int control, int nroInternoEtiqueta, int empresa, string producto, decimal faixa, string nroIdentificador)
        {
            return _context.T_CTR_CALIDAD_PENDIENTE.Any(d => d.CD_CONTROL == control && d.CD_PRODUTO == producto
                    && d.CD_EMPRESA == empresa && d.NU_IDENTIFICADOR == nroIdentificador && d.CD_FAIXA == faixa
                    && d.NU_ETIQUETA == nroInternoEtiqueta && d.ID_ACEPTADO == "N");
        }

        public virtual bool AnyControlPendiente(Stock stock, int controlExcluir = -1)
        {
            var controles = this._context.T_CTR_CALIDAD_PENDIENTE.Local.Where(d => d.CD_ENDERECO == stock.Ubicacion && d.CD_PRODUTO == stock.Producto && d.CD_EMPRESA == stock.Empresa
            && d.NU_IDENTIFICADOR == stock.Identificador && d.CD_FAIXA == stock.Faixa).ToList();

            var controlesAux = this._context.T_CTR_CALIDAD_PENDIENTE.AsNoTracking().AsEnumerable().Where(d => d.CD_ENDERECO == stock.Ubicacion
                && d.CD_PRODUTO == stock.Producto && d.CD_EMPRESA == stock.Empresa && d.NU_IDENTIFICADOR == stock.Identificador
                && d.CD_FAIXA == stock.Faixa && !controles.Any(x => x.NU_CTR_CALIDAD_PENDIENTE == d.NU_CTR_CALIDAD_PENDIENTE)).ToList();

            controles.AddRange(controlesAux);

            if (controles.Any(x => x.ID_ACEPTADO == "N"))
                return true;

            return false;
        }

        public virtual bool AnyControlDeCalidadProducto(int control, int empresa, string producto)
        {
            return this._context.T_CTR_CALIDAD_NECESARIO
                .Any(d => d.CD_CONTROL == control && d.CD_EMPRESA == empresa && d.CD_PRODUTO == producto);
        }

        public virtual bool AnyControlDeCalidadProducto(int idEmpresa, string codigoProducto)
        {
            return this._context.T_CTR_CALIDAD_NECESARIO.Any(d => d.CD_EMPRESA == idEmpresa && d.CD_PRODUTO == codigoProducto);
        }

        public virtual bool AnyControlDeCalidadPendienteDetalleLpn(long nuLpn, int idDetLpn)
        {
            return _context.T_CTR_CALIDAD_PENDIENTE
                .AsNoTracking()
                .Any(c => c.NU_LPN == nuLpn &&
                c.ID_LPN_DET == idDetLpn && c.ID_ACEPTADO == "N");

        }

        public virtual bool AnyInstancia(int instancia)
        {
            return _context.T_CTR_CALIDAD_PENDIENTE
                   .Any(x => x.NU_INSTANCIA_CONTROL == instancia);
        }

        #endregion

        #region Get

        public virtual ControlDeCalidadPendiente GetControlDeCalidadPendiente(string ubicacion, int empresa, string producto, decimal faixa, string identificador, long? nuLpn = null, int? idDetLpn = null, string estado = null)
        {
            var entity = _context.T_CTR_CALIDAD_PENDIENTE
                .AsNoTracking()
                .FirstOrDefault(c => c.CD_ENDERECO == ubicacion
                    && c.CD_EMPRESA == empresa
                    && c.CD_PRODUTO == producto
                    && c.CD_FAIXA == faixa
                    && c.NU_IDENTIFICADOR == identificador
                    && c.NU_LPN == nuLpn
                    && c.ID_LPN_DET == idDetLpn
                    && (!string.IsNullOrEmpty(estado) ? c.ID_ACEPTADO == estado : true));

            return _mapper.MapToObject(entity);
        }


        public virtual ControlDeCalidad GetTipoControlDeCalidad(int codigo)
        {
            var entity = this._context.T_TIPO_CTR_CALIDAD
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_CONTROL == codigo);

            return this._mapper.MapToObject(entity);
        }

        public virtual List<ControlDeCalidad> GetTiposControlesDeCalidad(List<int> codigos)
        {
            var entitys = this._context.T_TIPO_CTR_CALIDAD
                .AsNoTracking()
                .Where(d => codigos.Contains(d.CD_CONTROL)).ToList();

            return this._mapper.MapToObject(entitys);
        }

        public virtual List<ControlDeCalidad> GetByCodePartial(int codigo)
        {
            return this._context.T_TIPO_CTR_CALIDAD.AsNoTracking().Where(c => c.CD_CONTROL == codigo).ToList().Select(c => this._mapper.MapToObject(c)).ToList();
        }

        public virtual List<ControlDeCalidad> GetTiposControlCalidad()
        {
            return _context.T_TIPO_CTR_CALIDAD
                .AsNoTracking()
                .Select(t => _mapper.MapToObject(t))
                .ToList();
        }

        public virtual List<ControlDeCalidadPendiente> GetControlDeCalidadPendientes(string codigoProducto, decimal faixa, int idEmpresa, int numeroEtiqueta, string identificador)
        {
            var controles = new List<ControlDeCalidadPendiente>();

            var entities = this._context.T_CTR_CALIDAD_PENDIENTE
                .AsNoTracking()
                .Where(d => d.CD_PRODUTO == codigoProducto
                    && d.CD_FAIXA == faixa
                    && d.CD_EMPRESA == idEmpresa
                    && d.NU_ETIQUETA == numeroEtiqueta
                    && d.NU_IDENTIFICADOR == identificador)
                .ToList();

            foreach (var entity in entities)
            {
                controles.Add(this._mapper.MapToObject(entity));
            }

            return controles;
        }

        public virtual List<ControlDeCalidadPendiente> GetControles(List<int> idList)
        {
            var controles = new List<ControlDeCalidadPendiente>();

            var entities = this._context.T_CTR_CALIDAD_PENDIENTE.AsNoTracking().Where(d => idList.Contains(d.NU_CTR_CALIDAD_PENDIENTE))
                .GroupJoin(
                    this._context.T_STOCK.AsNoTracking(),
                    ccp => new { ccp.CD_ENDERECO, ccp.CD_PRODUTO, CD_EMPRESA = ccp.CD_EMPRESA ?? -1, ccp.NU_IDENTIFICADOR, CD_FAIXA = ccp.CD_FAIXA ?? -1 },
                    s => new { s.CD_ENDERECO, s.CD_PRODUTO, s.CD_EMPRESA, s.NU_IDENTIFICADOR, s.CD_FAIXA },
                    (ccp, s) => new { Control = ccp, Stock = s })
                .SelectMany(d => d.Stock.DefaultIfEmpty(), (d, Stock) => new { d.Control, Stock })
                .ToList();

            foreach (var entity in entities)
            {
                controles.Add(this._mapper.MapToObject(entity.Control, entity.Stock));
            }

            return controles;
        }

        public virtual ControlDeCalidadProducto GetControlDeCalidadProducto(int control, int empresa, string producto)
        {
            var entity = this._context.T_CTR_CALIDAD_NECESARIO.AsNoTracking()
                .Where(d => d.CD_CONTROL == control && d.CD_EMPRESA == empresa && d.CD_PRODUTO == producto)
                .FirstOrDefault();

            return this._mapper.MapToObject(entity);
        }

        public virtual List<ControlDeCalidadProducto> GetControlDeCalidadProducto(string producto, int empresa)
        {
            var entities = this._context.T_CTR_CALIDAD_NECESARIO.AsNoTracking().Where(d => d.CD_PRODUTO == producto && d.CD_EMPRESA == empresa)
                .ToList();

            var clases = new List<ControlDeCalidadProducto>();

            foreach (var entity in entities)
            {
                clases.Add(this._mapper.MapToObject(entity));
            }

            return clases;
        }

        public virtual long GetNextInstanciaSequenceValue()
        {
            return _context.GetNextSequenceValueLong(_dapper, "S_CTR_CALIDAD_INSTANCIA");
        }

        public virtual int GetNextIdSequenceValue()
        {
            return _context.GetNextSequenceValueInt(_dapper, "S_CTR_CALIDAD_PENDIENTE");
        }

        public virtual List<long> GetNextInstanciaSequenceValue(int cant)
        {
            List<long> toReturn = [];

            for (int i = 0; i < cant; i++)
            {
                toReturn.Add(this.GetNextInstanciaSequenceValue());
            }

            return toReturn;
        }

        public virtual List<int> GetNextIdSequenceValue(int cant)
        {
            List<int> toReturn = [];

            for (int i = 0; i < cant; i++)
            {
                toReturn.Add(this.GetNextIdSequenceValue());
            }

            return toReturn;
        }

        #endregion

        #region Add

        public virtual void AddTipoControlDeCalidad(ControlDeCalidad tipoCtrlCaliad)
        {
            var entity = this._mapper.MapToEntity(tipoCtrlCaliad);

            entity.DT_ADDROW = DateTime.Now;
            entity.DT_UPDROW = DateTime.Now;

            this._context.T_TIPO_CTR_CALIDAD.Add(entity);
        }

        public virtual int AddControlDeCalidadPendiente(ControlDeCalidadPendiente ctrlCalidadPendiente)
        {
            if (ctrlCalidadPendiente.Id <= 0)
                ctrlCalidadPendiente.Id = _context.GetNextSequenceValueInt(_dapper, Secuencias.S_CTR_CALIDAD_PENDIENTE);

            var entity = this._mapper.MapToEntity(ctrlCalidadPendiente);

            entity.DT_ADDROW ??= DateTime.Now;

            this._context.T_CTR_CALIDAD_PENDIENTE.Add(entity);

            return ctrlCalidadPendiente.Id;
        }

        public virtual void AddControlDeCalidadProducto(ControlDeCalidadProducto ctrlCalidadProducto)
        {
            var entity = this._mapper.MapToEntity(ctrlCalidadProducto);

            if (entity.DT_ADDROW == null)
                entity.DT_ADDROW = DateTime.Now;

            entity.DT_UPDROW = DateTime.Now;

            this._context.T_CTR_CALIDAD_NECESARIO.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateControlDeCalidadClase(ControlDeCalidad controlCalidadClase)
        {
            var entity = this._mapper.MapToEntity(controlCalidadClase);
            var attachedEntity = _context.T_TIPO_CTR_CALIDAD.Local
                .FirstOrDefault(w => w.CD_CONTROL == entity.CD_CONTROL);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_TIPO_CTR_CALIDAD.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateControlPendiente(ControlDeCalidadPendiente control)
        {
            var entity = this._mapper.MapToEntity(control);
            var attachedEntity = _context.T_CTR_CALIDAD_PENDIENTE.Local
                .FirstOrDefault(w => w.NU_CTR_CALIDAD_PENDIENTE == entity.NU_CTR_CALIDAD_PENDIENTE);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_CTR_CALIDAD_PENDIENTE.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateControlDeCalidadProducto(ControlDeCalidadProducto controlClase)
        {
            var entity = this._mapper.MapToEntity(controlClase);
            var attachedEntity = _context.T_CTR_CALIDAD_NECESARIO.Local
                .FirstOrDefault(w => w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_PRODUTO == entity.CD_PRODUTO
                    && w.CD_CONTROL == entity.CD_CONTROL);

            entity.DT_UPDROW = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_CTR_CALIDAD_NECESARIO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveControlDeCalidadClase(ControlDeCalidad controlCalidadClase)
        {
            var entity = this._mapper.MapToEntity(controlCalidadClase);
            var attachedEntity = _context.T_TIPO_CTR_CALIDAD.Local
                .FirstOrDefault(w => w.CD_CONTROL == entity.CD_CONTROL);

            if (attachedEntity != null)
            {
                this._context.T_TIPO_CTR_CALIDAD.Remove(attachedEntity);
            }
            else
            {
                this._context.T_TIPO_CTR_CALIDAD.Attach(entity);
                this._context.T_TIPO_CTR_CALIDAD.Remove(entity);
            }
        }

        public virtual void RemoveControlPendiente(ControlDeCalidadPendiente control)
        {
            var entity = this._mapper.MapToEntity(control);
            var attachedEntity = _context.T_CTR_CALIDAD_PENDIENTE.Local
                .FirstOrDefault(w => w.NU_CTR_CALIDAD_PENDIENTE == entity.NU_CTR_CALIDAD_PENDIENTE);

            if (attachedEntity != null)
            {
                this._context.T_CTR_CALIDAD_PENDIENTE.Remove(attachedEntity);
            }
            else
            {
                this._context.T_CTR_CALIDAD_PENDIENTE.Attach(entity);
                this._context.T_CTR_CALIDAD_PENDIENTE.Remove(entity);
            }
        }

        public virtual void RemoveControlDeCalidadProducto(ControlDeCalidadProducto controlClase)
        {
            var entity = this._mapper.MapToEntity(controlClase);
            var attachedEntity = _context.T_CTR_CALIDAD_NECESARIO.Local
                .FirstOrDefault(w => w.CD_EMPRESA == entity.CD_EMPRESA
                    && w.CD_PRODUTO == entity.CD_PRODUTO
                    && w.CD_CONTROL == entity.CD_CONTROL);

            if (attachedEntity != null)
            {
                this._context.T_CTR_CALIDAD_NECESARIO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_CTR_CALIDAD_NECESARIO.Attach(entity);
                this._context.T_CTR_CALIDAD_NECESARIO.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<ControlDeCalidadPendiente> GetControlesCalidadPendientesCriterios(IEnumerable<CriterioControlCalidadAPI> criterios)
        {
            IEnumerable<ControlDeCalidadPendiente> resultado = new List<ControlDeCalidadPendiente>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    _dapper.BulkInsert(connection, tran, criterios, "T_STOCK_PREDIO_TEMP", new Dictionary<string, Func<CriterioControlCalidadAPI, ColumnInfo>>
                    {
                        { "NU_PREDIO", x => new ColumnInfo(x.Predio)},
                        { "CD_PRODUTO", x => new ColumnInfo(x.Producto)},
                        { "CD_EMPRESA", x => new ColumnInfo(x.Empresa)},
                        { "NU_IDENTIFICADOR", x => new ColumnInfo(x.Lote)},
                        { "CD_FAIXA", x => new ColumnInfo(x.Faixa)},
                    });

                    string sql =
                            @" SELECT 
                                 CD_CONTROL
                                ,CTR.CD_EMPRESA
                                ,CTR.CD_ENDERECO
                                ,CTR.CD_FAIXA
                                ,CTR.CD_FUNCIONARIO_ACEPTO
                                ,CTR.CD_PRODUTO
                                ,CTR.DT_ADDROW
                                ,CTR.DT_UPDROW
                                ,CTR.ID_ACEPTADO
                                ,CTR.ID_LPN_DET
                                ,CTR.NU_CTR_CALIDAD_PENDIENTE
                                ,CTR.NU_ETIQUETA
                                ,CTR.NU_IDENTIFICADOR
                                ,CTR.NU_LPN
                                ,CTR.NU_PREDIO
                                FROM T_CTR_CALIDAD_PENDIENTE CTR 
                               INNER JOIN T_STOCK_PREDIO_TEMP TEMP ON TEMP.NU_PREDIO = CTR.NU_PREDIO
                               AND TEMP.CD_PRODUTO = CTR.CD_PRODUTO AND TEMP.CD_FAIXA = CTR.CD_FAIXA AND TEMP.CD_EMPRESA = CTR.CD_EMPRESA AND TEMP.NU_IDENTIFICADOR = CTR.NU_IDENTIFICADOR
                               WHERE CTR.ID_ACEPTADO = 'N'";

                    resultado = _dapper.Query<T_CTR_CALIDAD_PENDIENTE>(connection, sql, transaction: tran).Select(_mapper.MapToObject);

                    tran.Rollback();
                }
            }
            return resultado;
        }

        public virtual async Task AddOrUpdateControlDeCalidadPendiente(List<ControlDeCalidadPendiente> toAddControlesPendientes, List<ControlDeCalidadPendiente> toUpdateControlesPendientes, List<LpnDetalle> toUpdateLpnDetalle, List<Stock> toUpdateStock)
        {
            List<T_CTR_CALIDAD_PENDIENTE> insertControlesMapeados = _mapper.MapToEntity(toAddControlesPendientes);
            List<T_CTR_CALIDAD_PENDIENTE> updateControlesMapeados = _mapper.MapToEntity(toUpdateControlesPendientes);

            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        await this.BulkInsertControlCalidad(connection, transaction, insertControlesMapeados);
                        await this.BulkUpdateControlCalidad(connection, transaction, updateControlesMapeados);
                        await this.BulkUpdateControlCalidadDetallesLpn(connection, transaction, toUpdateLpnDetalle);
                        await this.BulkUpdateControlCalidadStock(connection, transaction, toUpdateStock);

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        public virtual async Task BulkUpdateControlCalidad(DbConnection conn, DbTransaction tran, List<T_CTR_CALIDAD_PENDIENTE> controles)
        {
            string sqlUpdateControles = @"
                UPDATE T_CTR_CALIDAD_PENDIENTE SET
                    ID_ACEPTADO = :ID_ACEPTADO,
                    DT_UPDROW = :DT_UPDROW,
                    CD_FUNCIONARIO_ACEPTO = :CD_FUNCIONARIO_ACEPTO
                WHERE NU_CTR_CALIDAD_PENDIENTE = :NU_CTR_CALIDAD_PENDIENTE
            ";

            await _dapper.ExecuteAsync(conn, sqlUpdateControles, controles, transaction: tran);
        }

        public virtual async Task BulkUpdateControlCalidadStock(DbConnection conn, DbTransaction tran, List<Stock> stock)
        {
            string sql = @"
                UPDATE T_STOCK SET 
                    ID_CTRL_CALIDAD = :ControlCalidad,
                    DT_UPDROW = :FechaModificacion,
                    NU_TRANSACCION = :NumeroTransaccion
                WHERE CD_ENDERECO = :Ubicacion 
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador
                    AND CD_EMPRESA = :Empresa ";

            await _dapper.ExecuteAsync(conn, sql, stock, transaction: tran);
        }

        public virtual async Task BulkUpdateControlCalidadDetallesLpn(DbConnection conn, DbTransaction tran, List<LpnDetalle> detallesLpn)
        {
            string sql = @"
                UPDATE T_LPN_DET SET 
                    NU_TRANSACCION = :NumeroTransaccion,                  
                    ID_CTRL_CALIDAD = :IdCtrlCalidad
                WHERE
                    ID_LPN_DET = :Id AND
                    NU_LPN = :NumeroLPN AND
                    CD_PRODUTO = :CodigoProducto AND
                    CD_FAIXA = :Faixa AND
                    CD_EMPRESA = :Empresa AND
                    NU_IDENTIFICADOR = :Lote
            ";

            await _dapper.ExecuteAsync(conn, sql, detallesLpn, transaction: tran);
        }

        public virtual async Task BulkInsertControlCalidad(DbConnection connection, DbTransaction tran, List<T_CTR_CALIDAD_PENDIENTE> controles)
        {
            string sqlInsertControles = @"
                INSERT INTO T_CTR_CALIDAD_PENDIENTE (
                    CD_CONTROL, CD_ENDERECO, NU_ETIQUETA, CD_EMPRESA, CD_PRODUTO, ID_ACEPTADO, DT_ADDROW, DT_UPDROW,
                    CD_FAIXA, NU_IDENTIFICADOR, CD_FUNCIONARIO_ACEPTO, NU_CTR_CALIDAD_PENDIENTE, NU_PREDIO, NU_LPN,
                    ID_LPN_DET, DS_CONTROL, NU_INSTANCIA_CONTROL
                ) VALUES (
                    :CD_CONTROL, :CD_ENDERECO, :NU_ETIQUETA, :CD_EMPRESA, :CD_PRODUTO, :ID_ACEPTADO, :DT_ADDROW, :DT_UPDROW,
                    :CD_FAIXA, :NU_IDENTIFICADOR, :CD_FUNCIONARIO_ACEPTO, :NU_CTR_CALIDAD_PENDIENTE, :NU_PREDIO, :NU_LPN,
                    :ID_LPN_DET, :DS_CONTROL, :NU_INSTANCIA_CONTROL
                )
            ";

            await _dapper.ExecuteAsync(connection, sqlInsertControles, controles, transaction: tran);
        }

        public virtual void UpdateControlCalidadStock(Stock stock)
        {
            string sql = @"
                UPDATE T_STOCK SET 
                    ID_CTRL_CALIDAD = :ControlCalidad,
                    DT_UPDROW = :FechaModificacion,
                    NU_TRANSACCION = :NumeroTransaccion
                WHERE CD_ENDERECO = :Ubicacion 
                    AND CD_PRODUTO = :Producto 
                    AND CD_FAIXA = :Faixa 
                    AND NU_IDENTIFICADOR = :Identificador
                    AND CD_EMPRESA = :Empresa ";

            _dapper.Execute(_context.Database.GetDbConnection(), sql, stock, transaction: _context.Database.CurrentTransaction?.GetDbTransaction());
        }

        #endregion
    }
}
