using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.Impresiones;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class EstiloEtiquetaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly EstiloEtiquetaMapper _mapper;

        public EstiloEtiquetaRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new EstiloEtiquetaMapper();
        }

        #region Any

        public virtual bool ExisteEtiquetaEstilo(string idEstiloEtiqueta)
        {
            return this._context.T_LABEL_ESTILO.Any(cb => cb.CD_LABEL_ESTILO == idEstiloEtiqueta);
        }

        #endregion

        #region Get

        public virtual List<EtiquetaEstilo> GetEtiquetaEstilos()
        {
            return _context.T_LABEL_ESTILO
                .Select(l => _mapper.MapToObject(l))
                .ToList();
        }

        public virtual EtiquetaEstilo GetEtiquetaEstilo(string idEstiloEtiqueta)
        {
            return this._mapper.MapToObject(this._context.T_LABEL_ESTILO.AsNoTracking().FirstOrDefault(x => x.CD_LABEL_ESTILO == idEstiloEtiqueta));
        }

        public virtual List<EtiquetaEstilo> GetEtiquetaEstilos(string tipo, string lenguaje = null)
        {
            if (!string.IsNullOrEmpty(lenguaje))
            {
                return (from es in this._context.T_LABEL_ESTILO.AsNoTracking()
                        join tem in this._context.T_LABEL_TEMPLATE.AsNoTracking() on es.CD_LABEL_ESTILO equals tem.CD_LABEL_ESTILO
                        where tem.CD_LENGUAJE_IMPRESION == lenguaje
                        && es.TP_LABEL == tipo
                        select es).Select(s => _mapper.MapToObject(s)).ToList();
            }
            else
            {
                return this._context.T_LABEL_ESTILO
                    .Where(x => x.TP_LABEL == tipo)
                    .Select(x => _mapper.MapToObject(x))
                    .ToList();
            }
        }

        #endregion

        #region Add

        public virtual void AddEtiquetaEstilo(EtiquetaEstilo etiquetaEstilo)
        {
            var entity = this._mapper.MapToEntity(etiquetaEstilo);
            this._context.T_LABEL_ESTILO.Add(entity);
        }

        #endregion

        #region Update

        public virtual void UpdateEstiloEtiqueta(EtiquetaEstilo etiquetaEstilo)
        {
            var entity = this._mapper.MapToEntity(etiquetaEstilo);
            var attachedEntity = _context.T_LABEL_ESTILO.Local
                .FirstOrDefault(w => w.CD_LABEL_ESTILO == entity.CD_LABEL_ESTILO);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_LABEL_ESTILO.Attach(entity);
                _context.Entry<T_LABEL_ESTILO>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        public virtual void DeleteEtiquetaEstilo(string idEstiloEtiqueta)
        {
            var entity = this._context.T_LABEL_ESTILO
                .FirstOrDefault(x => x.CD_LABEL_ESTILO == idEstiloEtiqueta);
            var attachedEntity = _context.T_LABEL_ESTILO.Local
                .FirstOrDefault(w => w.CD_LABEL_ESTILO == entity.CD_LABEL_ESTILO);

            if (attachedEntity != null)
                _context.T_LABEL_ESTILO.Remove(attachedEntity);
            else
                _context.T_LABEL_ESTILO.Remove(entity);
        }

        #endregion
    }
}
