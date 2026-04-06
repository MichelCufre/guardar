using Microsoft.EntityFrameworkCore;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Extensions;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class GrupoRepository
    {
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected readonly WISDB _context;
        protected readonly int _userId;
        protected readonly IDapper _dapper;
        protected readonly string _cdAplicacion;
        protected readonly GrupoMapper _mapper;

        public GrupoRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            _context = context;
            _userId = userId;
            _dapper = dapper;
            _cdAplicacion = cdAplicacion;
            _mapper = new GrupoMapper();
        }

        #region Any
        public virtual bool AnyGrupo(string cdGrupo)
        {
            return _context.T_GRUPO
                .AsNoTracking()
                .Any(g => g.CD_GRUPO == cdGrupo);
        }
        public virtual bool GrupoDefault(string cdGrupo)
        {
            return _context.T_GRUPO
                .AsNoTracking()
                .Any(g => g.CD_GRUPO == cdGrupo && !string.IsNullOrEmpty(g.CD_CLASSE));

        }
        public virtual bool GrupoEliminable(string cdGrupo)
        {
            return _context.V_REG300_GRUPOS_ELIMINABLES
                .AsNoTracking()
                .Any(g => g.CD_GRUPO == cdGrupo);
        }

        #endregion

        #region Get

        public virtual List<Grupo> GetGrupos()
        {
            return _context.T_GRUPO
                .AsNoTracking()
                .Select(g => _mapper.MapToObject(g)).ToList();
        }

        public virtual Grupo GetGrupo(string cdGrupo)
        {
            var entity = _context.T_GRUPO
                .AsNoTracking()
                .FirstOrDefault(g => g.CD_GRUPO == cdGrupo);
            return _mapper.MapToObject(entity);
        }

        public virtual GrupoRegla GetGrupoRegla(long nuRegla)
        {
            var entity = _context.T_GRUPO_REGLA
                .AsNoTracking()
                .FirstOrDefault(g => g.NU_GRUPO_REGLA == nuRegla);

            return _mapper.MapToObject(entity);
        }

        public virtual int GetOrdenUltimaRegla()
        {
            return (_context.T_GRUPO_REGLA
                .AsNoTracking()
                .OrderByDescending(g => g.NU_ORDEN)
                .FirstOrDefault()?.NU_ORDEN) ?? 0;
        }

        public virtual GrupoRegla GetGrupoReglaForChangeOrder(int nuOrden)
        {
            var entity = _context.T_GRUPO_REGLA
                .AsNoTracking()
                .FirstOrDefault(g => g.NU_ORDEN == nuOrden);

            return _mapper.MapToObject(entity);
        }

        public virtual GrupoReglaParametro GetGrupoReglaParametro(long id)
        {
            var entity = _context.T_GRUPO_REGLA_PARAM
                .AsNoTracking()
                .FirstOrDefault(g => g.NU_GRUPO_REGLA_PARAM == id);

            return _mapper.MapToObject(entity);
        }

        public virtual List<GrupoReglaParametro> GetParametrosRegla(long nuRegla)
        {
            return _context.T_GRUPO_REGLA_PARAM
                .AsNoTracking()
                .Where(g => g.NU_GRUPO_REGLA == nuRegla)
                .Select(g => _mapper.MapToObject(g))
                .ToList();
        }

        public virtual List<GrupoRegla> GetReglasAgrupacion()
        {
            return _context.T_GRUPO_REGLA
                .AsNoTracking()
                .OrderBy(r => r.NU_ORDEN)
                .ThenBy(r => r.NU_GRUPO_REGLA)
                .Select(r => _mapper.MapToObject(r))
                .ToList();
        }

        public virtual Grupo GetDefaultGrupo(string cdClase)
        {
            return _context.T_GRUPO
                .AsNoTracking()
                .Where(g => g.CD_CLASSE == cdClase && g.FL_DEFAULT == "S")
                .Select(g => _mapper.MapToObject(g))
                .FirstOrDefault();
        }

        public virtual List<GrupoParametro> GetParametros()
        {
            return _context.T_GRUPO_PARAM
                .AsNoTracking()
                .Select(g => _mapper.MapToObject(g))
                .ToList();
        }

        public virtual List<GrupoReglaParametro> GetDefaultParamRegla(long nuRegla)
        {
            return _context.T_GRUPO_PARAM.AsNoTracking()
                .Select(g => new GrupoReglaParametro()
                {
                    NroRegla = nuRegla,
                    NroParametro = g.NU_PARAM,
                    Valor = g.VL_PARAM_DEFAULT,
                    FechaInsercion = DateTime.Now,

                }).ToList();
        }

        public virtual List<Grupo> GetGrupoByNameOrCodePartial(string value)
        {
            return _context.T_GRUPO
                .AsNoTracking()
                .Where(g => g.CD_GRUPO.ToLower() == value.ToLower() || g.DS_GRUPO.ToLower().Contains(value.ToLower()))
                .Select(g => _mapper.MapToObject(g))
                .ToList();
        }

        public virtual List<string> GetGruposModificables()
        {
            return _context.V_REG300_GRUPOS_ELIMINABLES.AsNoTracking().Select(g => g.CD_GRUPO).ToList();
        }
        #endregion

        #region Add
        public virtual void AddGrupo(Grupo grupo)
        {
            var entity = this._mapper.MapToEntity(grupo);
            this._context.T_GRUPO.Add(entity);
        }

        public virtual void AddGrupoRegla(GrupoRegla regla)
        {
            regla.Id = this._context.GetNextSequenceValueLong(_dapper, "S_NU_GRUPO_REGLA");

            var entity = this._mapper.MapToEntity(regla);
            this._context.T_GRUPO_REGLA.Add(entity);
        }

        public virtual void AddGrupoReglaParametro(GrupoReglaParametro param)
        {
            param.Id = this._context.GetNextSequenceValueLong(_dapper, "S_NU_GRUPO_REGLA_PARAM");
            var entity = this._mapper.MapToEntity(param);
            this._context.T_GRUPO_REGLA_PARAM.Add(entity);
        }
        #endregion

        #region Update
        public virtual void UpdateGrupo(Grupo grupo)
        {
            var entity = this._mapper.MapToEntity(grupo);
            var attachedEntity = _context.T_GRUPO.Local.FirstOrDefault(w => w.CD_GRUPO == entity.CD_GRUPO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_GRUPO.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateGrupoRegla(GrupoRegla regla)
        {
            var entity = this._mapper.MapToEntity(regla);
            var attachedEntity = _context.T_GRUPO_REGLA.Local.FirstOrDefault(w => w.NU_GRUPO_REGLA == entity.NU_GRUPO_REGLA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_GRUPO_REGLA.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateGrupoReglaParametro(GrupoReglaParametro param)
        {
            var entity = this._mapper.MapToEntity(param);
            var attachedEntity = _context.T_GRUPO_REGLA_PARAM.Local.FirstOrDefault(w => w.NU_GRUPO_REGLA_PARAM == entity.NU_GRUPO_REGLA_PARAM);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_GRUPO_REGLA_PARAM.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }
        #endregion

        #region Remove
        public virtual void RemoveGrupo(Grupo grupo)
        {
            var entity = this._mapper.MapToEntity(grupo);
            var attachedEntity = _context.T_GRUPO.Local.FirstOrDefault(w => w.CD_GRUPO == entity.CD_GRUPO);

            if (attachedEntity != null)
            {
                this._context.T_GRUPO.Remove(attachedEntity);
            }
            else
            {
                this._context.T_GRUPO.Attach(entity);
                this._context.T_GRUPO.Remove(entity);
            }
        }

        #endregion

        #region Dapper

        public virtual void EliminarRegla(long nuRegla)
        {
            using (var connection = _dapper.GetDbConnection())
            {
                connection.Open();

                using (var tran = connection.BeginTransaction())
                {
                    try
                    {
                        var sql = @"SELECT 
                                        NU_GRUPO_REGLA as Id,
                                        DS_REGLA as Descripcion,
                                        CD_GRUPO as CodigoGrupo,
                                        NU_ORDEN as Orden,
                                        DT_ADDROW as FechaInsercion,
                                        DT_UPDROW as FechaModificacion
                                        FROM T_GRUPO_REGLA 
                                        WHERE NU_GRUPO_REGLA = :nuRegla";
                        var regla = _dapper.Query<GrupoRegla>(connection, sql, param: new { nuRegla = nuRegla }, transaction: tran).FirstOrDefault();

                        if (regla != null)
                        {
                            sql = @"DELETE FROM T_GRUPO_REGLA_PARAM WHERE NU_GRUPO_REGLA = :nuRegla";
                            _dapper.Execute(connection, sql, param: new { nuRegla = nuRegla, }, transaction: tran);

                            sql = @"DELETE FROM T_GRUPO_REGLA WHERE NU_GRUPO_REGLA = :nuRegla";
                            _dapper.Execute(connection, sql, param: new { nuRegla = nuRegla, }, transaction: tran);

                            sql = @"SELECT 
                                        NU_GRUPO_REGLA as Id,
                                        DS_REGLA as Descripcion,
                                        CD_GRUPO as CodigoGrupo,
                                        NU_ORDEN as Orden,
                                        DT_ADDROW as FechaInsercion,
                                        DT_UPDROW as FechaModificacion
                                        FROM T_GRUPO_REGLA 
                                        WHERE CD_GRUPO = :cdGrupo AND NU_ORDEN > :nuOrden";

                            var reglasPosteriores = _dapper.Query<GrupoRegla>(connection, sql, param: new
                            {
                                cdGrupo = regla.CodigoGrupo,
                                nuOrden = regla.Orden
                            }, transaction: tran).ToList();

                            if (reglasPosteriores != null && reglasPosteriores.Count > 0)
                            {
                                var reglasUpdate = new List<object>();

                                reglasUpdate.AddRange(reglasPosteriores
                                .Select(g => new
                                {
                                    nuRegla = g.Id,
                                    nuOrden = g.Orden - 1,
                                    FechaModificacion = DateTime.Now,
                                }).ToList());


                                sql = @"UPDATE T_GRUPO_REGLA SET NU_ORDEN = :nuOrden, DT_UPDROW = :FechaModificacion WHERE NU_GRUPO_REGLA = :nuRegla";
                                _dapper.Execute(connection, sql, reglasUpdate, transaction: tran);

                            }
                            tran.Commit();
                        }

                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        throw ex;
                    }
                }
            }
        }


        #endregion
    }
}
