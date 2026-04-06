using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class UbicacionTipoRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly UbicacionTipoMapper _mapper;

        public UbicacionTipoRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new UbicacionTipoMapper();
        }

        #region Any
        public virtual bool AnyUbicacionTipo(short id)
        {
            return this._context.T_TIPO_ENDERECO.Any(d => d.CD_TIPO_ENDERECO == id);
        }

        #endregion

        #region Get
        public virtual List<UbicacionTipo> GetUbicacionTipos()
        {
            return _context.T_TIPO_ENDERECO.AsNoTracking()
                .Select(t => _mapper.MapToObject(t))
                .ToList();
        }

        public virtual UbicacionTipo GetUbicacionTipo(short id)
        {
            var entity = this._context.T_TIPO_ENDERECO.AsNoTracking()
                .Where(d => d.CD_TIPO_ENDERECO == id)
               .FirstOrDefault();

            return this._mapper.MapToObject(entity);
        }

        public virtual UbicacionTipo GetTipoByUbicacion(string idUbicacion)
        {
            return _context.T_ENDERECO_ESTOQUE
                .Include("T_TIPO_ENDERECO")
                .AsNoTracking()
                .Where(u => u.CD_ENDERECO == idUbicacion)
                .Select(u => _mapper.MapToObject(u.T_TIPO_ENDERECO))
                .FirstOrDefault();
        }

        public virtual List<UbicacionTipo> GetByNombreOrCodePartial(string value)
        {
            short codigoUbicacion;
            if (short.TryParse(value, out codigoUbicacion))
            {
                return this._context.T_TIPO_ENDERECO.AsNoTracking()
                    .Where(d => d.DS_TIPO_ENDERECO.ToLower().Contains(value.ToLower()) || d.CD_TIPO_ENDERECO == codigoUbicacion)
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }
            else
            {
                return this._context.T_TIPO_ENDERECO.AsNoTracking()
                    .Where(d => d.DS_TIPO_ENDERECO.ToLower().Contains(value.ToLower()))
                    .ToList().Select(d => this._mapper.MapToObject(d)).ToList();
            }

        }

        public virtual TipoDeEstructura GetTipoEstructura(short codigoEstrucutra)
        {
            return this._mapper.MapToObject(this._context.V_PAR050_TIPO_ESTRUTURA
                .AsNoTracking()
                .FirstOrDefault(x => x.CD_TP_ESTR == codigoEstrucutra));
        }

        public virtual List<TipoDeEstructura> GetTiposEstructuras()
        {
            return _context.V_PAR050_TIPO_ESTRUTURA
                .AsNoTracking()
                .Select(t => _mapper.MapToObject(t))
                .ToList();
        }

        #endregion

        #region Add
        public virtual void AddTipoUbicacion(UbicacionTipo tipoUbicacion)
        {
            T_TIPO_ENDERECO entity = this._mapper.MapToEntity(tipoUbicacion);
            this._context.T_TIPO_ENDERECO.Add(entity);
        }
        #endregion

        #region Update
        public virtual void UpdateTipoUbicacion(UbicacionTipo tipoUbicacion)
        {
            T_TIPO_ENDERECO entity = this._mapper.MapToEntity(tipoUbicacion);
            T_TIPO_ENDERECO attachedEntity = _context.T_TIPO_ENDERECO.Local
                .FirstOrDefault(c => c.CD_TIPO_ENDERECO == entity.CD_TIPO_ENDERECO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_TIPO_ENDERECO.Attach(entity);
                _context.Entry<T_TIPO_ENDERECO>(entity).State = EntityState.Modified;
            }
        }

        #endregion
    }
}
