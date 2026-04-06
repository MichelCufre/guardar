using Microsoft.EntityFrameworkCore;
using System.Linq;
using WIS.Domain.Configuracion;
using WIS.Domain.DataModel.Mappers;
using WIS.Persistence.Database;

namespace WIS.Domain.DataModel.Repositories
{
    public class ConfiguracionIdiomaRepository
    {
        protected readonly WISDB _context;
        protected readonly string _cdAplicacion;
        protected readonly int _userId;
        protected readonly ConfiguracionIdiomaMapper _mapper;

        public ConfiguracionIdiomaRepository(WISDB context, string cdAplicacion, int userId)
        {
            this._context = context;
            this._cdAplicacion = cdAplicacion;
            this._userId = userId;
            this._mapper = new ConfiguracionIdiomaMapper();
        }

        #region Any
        
        public virtual bool ExisteConfiguracionIdioma(string cdAplicacion, string bloque, string tipo, string clave, string idioma)
        {
            return this._context.T_LOCALIZACION
                .Any(w => w.CD_APLICACION == cdAplicacion && w.CD_BLOQUE == bloque
                    && w.CD_TIPO == tipo 
                    && w.CD_CLAVE == clave 
                    && w.CD_IDIOMA == idioma);
        }

        #endregion

        #region Get
        
        public virtual ConfiguracionIdioma GetConfiguracionIdioma(string cdAplicacion, string bloque, string tipo, string clave, string idioma)
        {
            return this._mapper.MapToObject(this._context.T_LOCALIZACION
                .FirstOrDefault(w => w.CD_APLICACION == cdAplicacion && w.CD_BLOQUE == bloque
                    && w.CD_TIPO == tipo 
                    && w.CD_CLAVE == clave 
                    && w.CD_IDIOMA == idioma));
        }
        
        #endregion

        #region Add
        
        public virtual void AddConfiguracionIdioma(ConfiguracionIdioma configuracionIdioma)
        {
            T_LOCALIZACION entity = this._mapper.MapToEntity(configuracionIdioma);
            this._context.T_LOCALIZACION.Add(entity);
        }
        
        #endregion

        #region Update

        public virtual void UpdateConfiguracionIdioma(ConfiguracionIdioma configuracionIdioma)
        {
            T_LOCALIZACION entity = this._mapper.MapToEntity(configuracionIdioma);
            T_LOCALIZACION attachedEntity = _context.T_LOCALIZACION.Local
                .FirstOrDefault(w => w.CD_APLICACION == entity.CD_APLICACION 
                    && w.CD_BLOQUE == entity.CD_BLOQUE 
                    && w.CD_TIPO == entity.CD_TIPO 
                    && w.CD_CLAVE == entity.CD_CLAVE 
                    && w.CD_IDIOMA == entity.CD_IDIOMA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                _context.T_LOCALIZACION.Attach(entity);
                _context.Entry<T_LOCALIZACION>(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove
        
        public virtual void DeleteLenguajeImpresion(string cdAplicacion, string bloque, string tipo, string clave, string idioma)
        {
            T_LOCALIZACION entity = this._context.T_LOCALIZACION
                .FirstOrDefault(w => w.CD_APLICACION == cdAplicacion 
                    && w.CD_BLOQUE == bloque
                    && w.CD_TIPO == tipo 
                    && w.CD_CLAVE == clave 
                    && w.CD_IDIOMA == idioma);

            T_LOCALIZACION attachedEntity = _context.T_LOCALIZACION.Local
                .FirstOrDefault(w => w.CD_APLICACION == entity.CD_APLICACION 
                    && w.CD_BLOQUE == entity.CD_BLOQUE
                    && w.CD_TIPO == entity.CD_TIPO 
                    && w.CD_CLAVE == entity.CD_CLAVE 
                    && w.CD_IDIOMA == entity.CD_IDIOMA);

            if (attachedEntity != null)
            {
                _context.T_LOCALIZACION.Remove(attachedEntity);
            }
            else
            {
                _context.T_LOCALIZACION.Remove(entity);
            }

        }

        #endregion
    }
}
