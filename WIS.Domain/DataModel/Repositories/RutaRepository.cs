using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class RutaRepository
    {
        protected readonly WISDB _context;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly RutaMapper _mapper;
        protected readonly string _cdAplicacion;

        public RutaRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new RutaMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyRuta(short id)
        {
            return this._context.T_ROTA.Any(d => d.CD_ROTA == id);
        }

        public virtual bool AnyRuta(short id, string predio)
        {
            return this._context.T_ROTA
                .Include("T_ONDA")
                .Any(d => d.CD_ROTA == id && (d.T_ONDA.NU_PREDIO == predio || d.T_ONDA.NU_PREDIO == null));
        }

        public virtual bool AnyRutaSinPredio(short id)
        {
            return this._context.T_ROTA
                .Include("T_ONDA")
                .Any(d => d.CD_ROTA == id && d.T_ONDA.NU_PREDIO == null);
        }

        public virtual bool AnyRuta(string descripcion)
        {
            return this._context.T_ROTA.Any(d => d.DS_ROTA == descripcion);
        }

        public virtual bool AnyRutaZona(string zona)
        {
            return this._context.T_ROTA.Any(d => d.CD_ZONA == zona);
        }

        #endregion

        #region Get

        public virtual Ruta GetRutaGenerica()
        {
            //TODO ver si es necesario pasarlo a Parametro de sistema

            T_ROTA ruta = this._context.T_ROTA.AsNoTracking().FirstOrDefault(d => d.CD_ROTA == 1);

            return this._mapper.MapToObject(ruta);
        }

        public virtual Ruta GetRutaProcesosInternos()
        {
            //TODO ver si es necesario pasarlo a Parametro de sistema

            T_ROTA ruta = this._context.T_ROTA.AsNoTracking().FirstOrDefault(d => d.CD_ROTA == 0);

            return this._mapper.MapToObject(ruta);
        }

        public virtual Ruta GetRuta(short codigoRuta)
        {
            T_ROTA ruta = this._context.T_ROTA.AsNoTracking().FirstOrDefault(d => d.CD_ROTA == codigoRuta);

            return this._mapper.MapToObject(ruta);
        }

        public virtual Ruta GetRutaOnda(short codigoRuta)
        {
            T_ROTA ruta = this._context.T_ROTA.Include("T_ONDA").AsNoTracking().FirstOrDefault(d => d.CD_ROTA == codigoRuta);
            return this._mapper.MapToObject(ruta);
        }

        public virtual List<Ruta> GetByDescripcionOrCodePartial(string value, string predio)
        {
            var ignorarPredio = string.IsNullOrEmpty(predio);
            var query = this._context.T_ROTA.Include("T_ONDA").Where(d => (ignorarPredio || d.T_ONDA.NU_PREDIO == predio || d.T_ONDA.NU_PREDIO == null) && d.CD_SITUACAO == SituacionDb.Activo);

            if (short.TryParse(value, out short ruta))
                query = query.Where(d => (d.CD_ROTA == ruta || d.DS_ROTA.ToLower().Contains(value.ToLower())));
            else
                query = query.Where(d => d.DS_ROTA.ToLower().Contains(value.ToLower()));

            return query.Select(r => _mapper.MapToObject(r))
                .ToList();
        }

        public virtual Ruta GetRutaByZona(string cdZona, bool getRutaSinGeolocalizar)
        {
            T_ROTA ruta = this._context.T_ROTA.AsNoTracking().FirstOrDefault(d => d.CD_ZONA == cdZona);

            if (ruta == null && getRutaSinGeolocalizar)
                this._context.T_ROTA.AsNoTracking().FirstOrDefault(d => d.CD_ZONA == "S/G");

            return this._mapper.MapToObject(ruta);
        }

        public virtual short GetUltimaRuta()
        {
            return this._context.T_ROTA.AsNoTracking().OrderByDescending(r => r.CD_ROTA).FirstOrDefault().CD_ROTA;
        }

        public virtual IQueryable<Ruta> GetRutas()
        {
            return _context.T_ROTA.Include("T_ONDA").AsNoTracking()
                .Select(r => _mapper.MapToObject(r));
        }

        #endregion

        #region Add

        public virtual void AddRuta(Ruta ruta)
        {
            T_ROTA entity = this._mapper.MapToEntity(ruta);

            entity.DT_CADASTRAMENTO = DateTime.Now;
            entity.DT_ALTERACAO = DateTime.Now;
            entity.DT_SITUACAO = DateTime.Now;

            this._context.T_ROTA.Add(entity);

        }

        #endregion

        #region Update

        public virtual void UpdateRuta(Ruta ruta)
        {
            T_ROTA entity = this._mapper.MapToEntity(ruta);
            T_ROTA attachedEntity = _context.T_ROTA.Local
                .FirstOrDefault(r => r.CD_ROTA == entity.CD_ROTA);

            entity.DT_ALTERACAO = DateTime.Now;

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_ROTA.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Dapper
        public virtual async Task<Ruta> GetRutaByZona(string zona)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync();

                string sql = GetSqlSelectRuta() +
                    @" WHERE CD_ZONA = :zona ";

                return _dapper.Query<Ruta>(connection, sql, param: new { zona = zona }, commandType: CommandType.Text).FirstOrDefault();
            }
        }

        public virtual IEnumerable<short> GetRutas(IEnumerable<Ruta> rutas)
        {
            IEnumerable<short> resultado = new List<short>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_ROTA_TEMP (CD_ROTA) VALUES (:Id)";
                    _dapper.Execute(connection, sql, rutas, transaction: tran);

                    sql = @"SELECT R.CD_ROTA FROM T_ROTA R INNER JOIN T_ROTA_TEMP T ON R.CD_ROTA = T.CD_ROTA";

                    resultado = _dapper.Query<short>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }

        public static string GetSqlSelectRuta()
        {
            return @"SELECT 
	                    R.CD_ROTA as Id,
	                    R.CD_SITUACAO as EstadoId,
	                    R.CD_ONDA as OndaId,
	                    R.CD_TRANSPORTADORA as Transportista,
	                    R.DS_ROTA as Descripcion,
	                    R.CD_PORTA as PuertaEmbarqueId,
	                    R.DT_SITUACAO as FechaSituacion,
	                    R.DT_ALTERACAO as FechaModificacion,
	                    R.DT_CADASTRAMENTO as FechaAlta,
	                    R.ID_ORDEM_CARGA as ControlaOrdenDeCargaId,
	                    R.CD_ZONA as Zona
                    FROM T_ROTA R ";
        }

        public virtual async Task AddRutaByZona(Ruta ruta, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @" INSERT INTO T_ROTA (
                                        CD_ROTA,
                                        CD_SITUACAO,
                                        DS_ROTA,
                                        DT_SITUACAO,
                                        DT_ALTERACAO,
                                        DT_CADASTRAMENTO,
                                        ID_ORDEM_CARGA,
                                        CD_ZONA) 
                                    VALUES(
                                        :Id,
                                        :EstadoId,
                                        :Descripcion,
                                        :FechaSituacion,
                                        :FechaModificacion,
                                        :FechaAlta,
                                        'N',
                                        :Zona)";

                    await _dapper.ExecuteAsync(connection, sql, ruta, transaction: tran);

                    tran.Commit();
                }
            }
        }

        public virtual async Task UpdateRutaByZona(Ruta ruta, CancellationToken cancelToken = default)
        {
            using (var connection = this._dapper.GetDbConnection())
            {
                await connection.OpenAsync(cancelToken);

                using (var tran = connection.BeginTransaction())
                {

                    ruta.FechaModificacion = DateTime.Now;

                    string sql = @"
                        UPDATE T_ROTA SET                 
                        DS_ROTA= :Descripcion,
                        DT_ALTERACAO= :FechaModificacion
                        WHERE CD_ZONA = :Zona ";

                    await _dapper.ExecuteAsync(connection, sql, ruta, transaction: tran);

                    tran.Commit();
                }
            }
        }

        #endregion
    }
}
