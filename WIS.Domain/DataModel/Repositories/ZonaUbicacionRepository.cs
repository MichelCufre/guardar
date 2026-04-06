using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ZonaUbicacionRepository
    {
        protected WISDB _context;
        protected string _application;
        protected int _userId;
        protected readonly ZonaUbicacionMapper _mapper;
        protected readonly IDapper _dapper;

        public ZonaUbicacionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._context = context;
            this._application = application;
            this._userId = userId;
            this._mapper = new ZonaUbicacionMapper();
            this._dapper = dapper;
        }

        #region Any

        public virtual bool AnyZona(string id)
        {
            return _context.T_ZONA_UBICACION.Any(x => x.CD_ZONA_UBICACION == id.ToUpper());
        }

        public virtual bool AnyTpZona(string tp)
        {
            return _context.T_DET_DOMINIO.Any(d => d.CD_DOMINIO == CodigoDominioDb.TiposDeZonas && d.CD_DOMINIO_VALOR == tp);
        }

        public virtual bool AnyControlAcceso(string id)
        {
            return _context.T_CONTROL_ACCESO.Any(x => x.CD_CONTROL_ACCESO == id);
        }

        public virtual bool ZonaUtilizada(string cdZona, int idZona)
        {
            if (_context.T_ENDERECO_ESTOQUE.Any(e => e.CD_ZONA_UBICACION == cdZona))
                return true;

            if (_context.T_LPARAMETRO_CONFIGURACION.Any(e => e.DO_ENTIDAD_PARAMETRIZABLE == ParamManager.PARAM_ZONA
                && e.ND_ENTIDAD == $"{ParamManager.PARAM_ZONA}_{idZona}"))
                return true;

            return false;
        }

        #endregion

        #region Get

        public virtual ZonaUbicacion GetZona(string id)
        {
            var entity = this._context.T_ZONA_UBICACION.AsNoTracking().Where(d => d.CD_ZONA_UBICACION == id).FirstOrDefault();
            return this._mapper.MapToObject(entity);
        }

        public virtual ZonaUbicacion GetZonaByUbicacion(string endereco)
        {
            T_ZONA_UBICACION entity = null;
            var ubicacion = _context.T_ENDERECO_ESTOQUE.AsNoTracking().FirstOrDefault(d => d.CD_ENDERECO == endereco);
            if (ubicacion != null)
                entity = _context.T_ZONA_UBICACION.AsNoTracking().FirstOrDefault(d => d.CD_ZONA_UBICACION == ubicacion.CD_ZONA_UBICACION);

            return this._mapper.MapToObject(entity);
        }

        public virtual List<ZonaUbicacion> GetZonas()
        {
            var zonas = new List<ZonaUbicacion>();

            var entries = this._context.T_ZONA_UBICACION.AsNoTracking().ToList();

            foreach (var entry in entries)
            {
                zonas.Add(this._mapper.MapToObject(entry));
            }
            return zonas;
        }

        public virtual List<string> GetZonasNoUtilizadas()
        {
            var zonas = this._context.T_ZONA_UBICACION
                .GroupJoin(_context.T_ENDERECO_ESTOQUE,
                    zu => new { zu.CD_ZONA_UBICACION },
                    ee => new { ee.CD_ZONA_UBICACION },
                    (zu, ees) => new { Zona = zu, Ubicaciones = ees })
                .SelectMany(x => x.Ubicaciones.DefaultIfEmpty(), (x, u) => new { x.Zona, Ubicacion = u })
                .Where(x => x.Ubicacion == null)
                .AsNoTracking()
                .Select(x => x.Zona)
                .ToList();

            var zonasParametros = this._context.T_LPARAMETRO_CONFIGURACION
                .AsNoTracking()
                .Where(pc => pc.DO_ENTIDAD_PARAMETRIZABLE == ParamManager.PARAM_ZONA)
                .GroupBy(pc => pc.ND_ENTIDAD)
                .Select(x => x.Key)
                .ToList();

            return zonas
                .GroupJoin(zonasParametros,
                    z => $"{ParamManager.PARAM_ZONA}_{z.ID_ZONA_UBICACION}",
                    zp => zp,
                    (z, zps) => new { Zona = z, Parametros = zps })
                .SelectMany(x => x.Parametros.DefaultIfEmpty(), (x, zp) => new { x.Zona, Parametro = zp })
                .Where(x => x.Parametro == null)
                .GroupBy(x => x.Zona.CD_ZONA_UBICACION)
                .Select(x => x.Key)
                .ToList();
        }

        public virtual List<ZonaUbicacion> GetZonasHabilitadas()
        {
            var zonas = new List<ZonaUbicacion>();

            var entries = this._context.T_ZONA_UBICACION.AsNoTracking().Where(z => z.FL_HABILITADA == "S").ToList();

            foreach (var entry in entries)
            {
                zonas.Add(this._mapper.MapToObject(entry));
            }
            return zonas;
        }

        public virtual List<ZonaUbicacion> GetByZonaUbicacionNombreOrCodePartial(string value)
        {
            return this._context.T_ZONA_UBICACION.AsNoTracking()
                    .Where(d => d.DS_ZONA_UBICACION.ToLower().Contains(value.ToLower()) || d.CD_ZONA_UBICACION.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
        }

        public virtual List<ControlAcceso> GetControlAccesoByNombreOrCodePartial(string value)
        {
            return this._context.T_CONTROL_ACCESO.AsNoTracking()
                    .Where(d => d.DS_CONTROL_ACCESO.ToLower().Contains(value.ToLower()) || d.CD_CONTROL_ACCESO.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
        }

        public virtual List<ControlAcceso> GetControlesAcceso()
        {
            return this._context.T_CONTROL_ACCESO.AsNoTracking().ToList().Select(c => this._mapper.MapToObject(c)).ToList();
        }

        #endregion

        #region Add

        public virtual void AddZona(ZonaUbicacion zona)
        {
            zona.IdInterno = _context.GetNextSequenceValueInt(this._dapper, Secuencias.S_ID_ZONA_UBICACION);
            var entity = this._mapper.MapToEntity(zona);
            this._context.T_ZONA_UBICACION.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateZona(ZonaUbicacion zona)
        {
            var entity = this._mapper.MapToEntity(zona);
            var attachedEntity = _context.T_ZONA_UBICACION.Local
                .FirstOrDefault(x => x.CD_ZONA_UBICACION == entity.CD_ZONA_UBICACION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_ZONA_UBICACION.Attach(entity);
                _context.Entry<T_ZONA_UBICACION>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void RemoveZona(ZonaUbicacion zona)
        {
            var entity = this._mapper.MapToEntity(zona);
            var attachedEntity = this._context.T_ZONA_UBICACION.Local
                .FirstOrDefault(d => d.CD_ZONA_UBICACION == entity.CD_ZONA_UBICACION);

            if (attachedEntity != null)
            {
                this._context.T_ZONA_UBICACION.Remove(attachedEntity);
            }
            else
            {
                this._context.T_ZONA_UBICACION.Attach(entity);
                this._context.T_ZONA_UBICACION.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual IEnumerable<ZonaUbicacion> GetZonasByUbicaciones(IEnumerable<Ubicacion> pedidos)
        {
            IEnumerable<ZonaUbicacion> resultado = new List<ZonaUbicacion>();

            using (var connection = this._dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    string sql = @"INSERT INTO T_ENDERECO_ESTOQUE_TEMP (CD_ENDERECO) VALUES (:Id)";
                    _dapper.Execute(connection, sql, pedidos, transaction: tran);

                    sql = @"SELECT
                                Z.CD_ZONA_UBICACION as Id,
                                Z.DS_ZONA_UBICACION as Descripcion,
                                Z.TP_ZONA_UBICACION as TipoZonaUbicacion,
                                Z.CD_ZONA_UBICACION_PICKING as ZonaUbicacionPicking,
                                Z.CD_ESTACION as Estacion,
                                Z.CD_ESTACION_ALMACENAJE as EstacionAlmacenado,
                                Z.FL_HABILITADA as HabilitadaId,
                                Z.DT_ADDROW as Alta,
                                Z.DT_UPDROW as Modificacion,
                                Z.ID_ZONA_UBICACION as IdInterno,
                                E.CD_ENDERECO as Ubicacion
                            FROM T_ENDERECO_ESTOQUE E
                            INNER JOIN T_ENDERECO_ESTOQUE_TEMP T ON E.CD_ENDERECO = T.CD_ENDERECO
                            INNER JOIN T_ZONA_UBICACION Z ON E.CD_ZONA_UBICACION = Z.CD_ZONA_UBICACION";

                    resultado = _dapper.Query<ZonaUbicacion>(connection, sql, transaction: tran);

                    tran.Rollback();
                }
            }

            return resultado;
        }
        #endregion
    }
}
