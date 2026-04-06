using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class PredioRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly PredioMapper _mapper;
        protected readonly IDapper _dapper;

        public PredioRepository(WISDB context, string cdAplicacion, int userId, IDapper dapper)
        {
            this._dapper = dapper;
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new PredioMapper();
        }

        #region Any
        public virtual bool AnyPredio(string numero)
        {
            return this._context.T_PREDIO.AsNoTracking().Any(w => w.NU_PREDIO == numero);
        }
        public virtual bool AnyIdExternoPredio(string idExterno)
        {
            return this._context.T_PREDIO.AsNoTracking().Any(w => w.ID_EXTERNO == idExterno);
        }
        public virtual bool AnyPrediosUsuario(string numeroPredio, int idUsuario)
        {
            return this._context.T_PREDIO_USUARIO.Any(s => s.NU_PREDIO == numeroPredio && s.USERID == idUsuario);
        }
        public virtual bool AnyPredioConAsignacionUsuario(string nuPredio, int userId)
        {
            return this._context.T_PREDIO_USUARIO.Any(x => x.NU_PREDIO == nuPredio && x.USERID != userId);
        }

        #endregion

        #region Get
        public virtual Predio GetPredio(string numero)
        {
            return _mapper.MapPredioToObject(this._context.T_PREDIO.AsNoTracking().FirstOrDefault(w => w.NU_PREDIO == numero));
        }

        public virtual List<Predio> GetPrediosUsuario(int idUsuario)
        {
            var predios = this._context.T_PREDIO.AsNoTracking().ToList();
            var prediosUsuario = this.GetPredioUser(idUsuario);
            var prediosObj = new List<Predio>();

            foreach (var entity in predios)
            {
                if (prediosUsuario.Contains(entity.NU_PREDIO))
                    prediosObj.Add(this._mapper.MapPredioToObject(entity));
            }

            return prediosObj;
        }

        public virtual List<string> GetPrediosPicking()
        {
            var res = _context.V_PREDIOS_PICKING_PRE050.GroupBy(x => x.NU_PREDIO).Select(s => s.Key).ToList();

            return res;
        }
        public virtual List<string> GetPredioUser(int userId)
        {
            var res = _context.T_PREDIO_USUARIO.AsNoTracking().Where(x => x.USERID == userId).Select(x => x.NU_PREDIO).ToList();

            return res;
        }

        public virtual string GetPuntoEntregaPredio(string nuPredio)
        {
            return _context.T_PREDIO.AsNoTracking().FirstOrDefault(w => w.NU_PREDIO == nuPredio)?.CD_PUNTO_ENTREGA;
        }


        public virtual List<Predio> GetPredios()
        {
            return this._context.T_PREDIO.ToList().Select(w => this._mapper.MapPredioToObject(w)).OrderBy(x => x.Numero).ToList();
        }

        public virtual List<Predio> GetPredioByKeysPartial(string valueSearch)
        {
            return this._context.T_PREDIO.AsNoTracking()
                   .Where(w => w.NU_PREDIO.ToLower().Contains(valueSearch.ToLower())
                        && w.NU_PREDIO.ToLower().Contains(valueSearch.ToLower())
                    ).ToList()
                   .Select(w => this._mapper.MapPredioToObject(w)).ToList();

        }


        public virtual List<Predio> GetPrediosSinSincronizar()
        {
            return this._context.T_PREDIO.Where(p => p.DS_ENDERECO != null && p.FL_SYNC_REALIZADA != "S" && p.NU_PREDIO != GeneralDb.PredioSinDefinir).ToList()
                .Select(w => this._mapper.MapPredioToObject(w)).OrderBy(x => x.Numero).ToList();
        }
        #endregion

        #region Add
        public virtual void AddPredio(Predio obj)
        {
            T_PREDIO entity = this._mapper.MapPredioToEntity(obj);
            this._context.T_PREDIO.Add(entity);
        }
        public virtual void AddPredioUsuario(int userId, string predio)
        {
            this._context.T_PREDIO_USUARIO.Add(new T_PREDIO_USUARIO
            {
                USERID = userId,
                NU_PREDIO = predio,
            });
        }
        public virtual void AsignarPredioUsuarios(Predio obj, List<int> usuarios)
        {
            usuarios?.Distinct().ToList().ForEach(w =>
            {
                if (!this._context.T_PREDIO_USUARIO.Any(x => x.NU_PREDIO == obj.Numero && x.USERID == w))
                    _context.T_PREDIO_USUARIO.Add(new T_PREDIO_USUARIO { NU_PREDIO = obj.Numero, USERID = w });
            });
        }
        public virtual void AsignarPredioUsuarios(string predio, List<int> usuarios)
        {
            usuarios?.Distinct().ToList().ForEach(w =>
            {
                if (!this._context.T_PREDIO_USUARIO.Any(x => x.NU_PREDIO == predio && x.USERID == w))
                    _context.T_PREDIO_USUARIO.Add(new T_PREDIO_USUARIO { NU_PREDIO = predio, USERID = w });
            });
        }

        #endregion

        #region Update

        public virtual void UpdatePredio(Predio predio)
        {
            T_PREDIO entity = this._mapper.MapPredioToEntity(predio);
            T_PREDIO attachedEntity = _context.T_PREDIO.Local.FirstOrDefault(x => x.NU_PREDIO == entity.NU_PREDIO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.State = EntityState.Modified;
                attachedEntry.CurrentValues.SetValues(entity);
            }
            else
            {
                _context.T_PREDIO.Attach(entity);
                _context.Entry<T_PREDIO>(entity).State = EntityState.Modified;
            }
        }
        #endregion

        #region Remove

        public virtual void RemovePredio(Predio obj)
        {
            T_PREDIO entity = this._mapper.MapPredioToEntity(obj);
            T_PREDIO attachedEntity = _context.T_PREDIO.Local.FirstOrDefault(x => x.NU_PREDIO == entity.NU_PREDIO);

            if (attachedEntity != null)
            {
                _context.T_PREDIO.Remove(attachedEntity);
            }
            else
            {
                _context.T_PREDIO.Attach(entity);
                _context.T_PREDIO.Remove(entity);
            }
        }
        public virtual void RemoverPredioUsuarios(string predio, List<int> usuarios)
        {
            usuarios?.Distinct().ToList().ForEach(w =>
            {
                var entity = this._context.T_PREDIO_USUARIO
                    .FirstOrDefault(s => s.NU_PREDIO == predio && s.USERID == w);

                if (entity != null)
                {
                    var attachedEntity = this._context.T_PREDIO_USUARIO.Local
                        .FirstOrDefault(s => s.NU_PREDIO == predio && s.USERID == w);

                    if (attachedEntity != null)
                    {
                        _context.T_PREDIO_USUARIO.Remove(attachedEntity);
                    }
                    else
                    {
                        _context.T_PREDIO_USUARIO.Attach(entity);
                        _context.T_PREDIO_USUARIO.Remove(entity);
                    }
                }
            });
        }
        #endregion

    }
}
