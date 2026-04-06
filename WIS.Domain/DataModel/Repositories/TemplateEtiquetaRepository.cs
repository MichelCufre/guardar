using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.General.Configuracion;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class TemplateEtiquetaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly TemplateEtiquetaMapper _mapper;

        public TemplateEtiquetaRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new TemplateEtiquetaMapper();
        }

        #region ADD
        public virtual void AddTemplateEtiqueta(TemplateEtiqueta templateEtiqueta)
        {
            T_LABEL_TEMPLATE entity = this._mapper.MapToEntity(templateEtiqueta);
            this._context.T_LABEL_TEMPLATE.Add(entity);
        }
        #endregion

        #region Update
        public virtual void UpdateTemplateEtiqueta(TemplateEtiqueta templateEtiqueta)
        {
            T_LABEL_TEMPLATE entity = this._mapper.MapToEntity(templateEtiqueta);
            T_LABEL_TEMPLATE attachedEntity = _context.T_LABEL_TEMPLATE.Local
                .FirstOrDefault(w => w.CD_LABEL_ESTILO == entity.CD_LABEL_ESTILO 
                    && w.CD_LENGUAJE_IMPRESION == entity.CD_LENGUAJE_IMPRESION);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_LABEL_TEMPLATE.Attach(entity);
                _context.Entry<T_LABEL_TEMPLATE>(entity).State = EntityState.Modified;
            }

        }
        #endregion

        #region Remove
        public virtual void DeleteTemplateEtiqueta(string estilo, string lenguaje)
        {
            T_LABEL_TEMPLATE entity = this._context.T_LABEL_TEMPLATE
                .FirstOrDefault(x => x.CD_LABEL_ESTILO == estilo 
                    && x.CD_LENGUAJE_IMPRESION == lenguaje);
            T_LABEL_TEMPLATE attachedEntity = _context.T_LABEL_TEMPLATE.Local
                .FirstOrDefault(w => w.CD_LABEL_ESTILO == entity.CD_LABEL_ESTILO 
                    && w.CD_LENGUAJE_IMPRESION == entity.CD_LENGUAJE_IMPRESION);

            if (attachedEntity != null)
            {
                _context.T_LABEL_TEMPLATE.Remove(attachedEntity);
            }
            else
            {
                _context.T_LABEL_TEMPLATE.Remove(entity);
            }
        }

        #endregion

        #region Any
        public virtual bool ExisteTemplateEtiqueta(string estilo, string lenguaje)
        {
            return this._context.T_LABEL_TEMPLATE.AsNoTracking().Any(x => x.CD_LABEL_ESTILO == estilo && x.CD_LENGUAJE_IMPRESION == lenguaje);
        }
        #endregion

        #region GET
        public virtual TemplateEtiqueta GetEtiquetaEstilo(string estilo, string lenguaje)
        {
            return this._mapper.MapToObject(this._context.T_LABEL_TEMPLATE.AsNoTracking().FirstOrDefault(x => x.CD_LABEL_ESTILO == estilo && x.CD_LENGUAJE_IMPRESION == lenguaje));
        }
        #endregion


    }
}
