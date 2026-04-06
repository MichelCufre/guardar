using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel.Mappers;
using WIS.Domain.DataModel.Mappers.Constants;
using WIS.Domain.General;
using WIS.Domain.Services.Interfaces;
using WIS.Persistence.Database;
using WIS.Translation;

namespace WIS.Domain.DataModel.Repositories
{
    public class LocalizacionRepository
    {
        protected readonly WISDB _context;
        protected readonly string _application;
        protected readonly int _userId;
        protected readonly LocalizacionMapper _mapper;
        protected DominioRepository _dominioRepository;
        protected readonly IDapper _dapper;

        public LocalizacionRepository(WISDB context, string application, int userId, IDapper dapper)
        {
            this._dapper = dapper;
            this._userId = userId;
            this._context = context;
            this._application = application;
            this._mapper = new LocalizacionMapper();
            this._dominioRepository = new DominioRepository(_context, application, userId, _dapper);
        }

        #region Any

        #endregion

        #region Get

        public virtual TranslationVersion GetVersionByLanguage(string language)
        {
            var entityVersion = this._context.T_LOCALIZACION_VERSION
                .AsNoTracking()
                .FirstOrDefault(d => d.CD_IDIOMA == language);

            return this._mapper.MapToObject(entityVersion);
        }

        public virtual List<TranslationVersion> GetAllVersions()
        {
            List<TranslationVersion> versions = new List<TranslationVersion>();

            var entityVersions = this._context.T_LOCALIZACION_VERSION
                .AsNoTracking()
                .ToList();

            foreach (var entity in entityVersions)
            {
                versions.Add(this._mapper.MapToObject(entity));
            }

            return versions;
        }

        public virtual List<TranslatedValue> GetTranslationByLanguage(string language)
        {
            var translations = new List<TranslatedValue>();

            var entityTranslations = this._context.T_LOCALIZACION
                .AsNoTracking()
                .Where(d => d.CD_IDIOMA == language)
                .ToList();

            foreach (var entity in entityTranslations)
            {
                translations.Add(this._mapper.MapToObject(entity));
            }

            return translations;
        }

        public virtual List<TranslatedValue> GetTranslation(List<string> keys, string language)
        {
            List<TranslatedValue> resources = this.GetTranslationByKeyAndLanguage(keys, language);
            List<TranslatedValue> baseResources = this.GetTranslationByKeyAndLanguage(keys, "base");

            return resources.Union(baseResources, new TranslationResourceComparer()).ToList();
        }

        public virtual List<TranslatedValue> GetTranslationByKeyAndLanguage(List<string> keys, string language)
        {
            return _context.T_LOCALIZACION
                .AsNoTracking()
                .Where(d => keys.Contains(d.CD_APLICACION + this._mapper.Separator + d.CD_BLOQUE + this._mapper.Separator + d.CD_TIPO + this._mapper.Separator + d.CD_CLAVE)
                    && d.CD_IDIOMA == language)
                .Select(d => _mapper.MapToObject(d))
                .ToList();
        }

        public virtual TranslatedValue GetTranslation(string key, string language)
        {
            var resource = this.GetTranslationByKeyAndLanguage(key, language);
            var baseResource = this.GetTranslationByKeyAndLanguage(key, "base");

            return resource ?? baseResource;
        }

        public virtual TranslatedValue GetTranslationByKeyAndLanguage(string key, string language)
        {
            var valores = key.Split("_");
            var app = valores[0];
            var bloque = valores[1];
            var tipo = valores[2];
            var clave = string.Join("_", valores.Skip(3));

            return _context.T_LOCALIZACION
                .AsNoTracking()
                .Where(d => d.CD_APLICACION == app && d.CD_BLOQUE == bloque && d.CD_TIPO == tipo && d.CD_CLAVE == clave
                    && d.CD_IDIOMA == language)
                .Select(d => _mapper.MapToObject(d))?
                .FirstOrDefault();
        }

        public virtual List<DominioDetalle> GetAllIdiomas()
        {
            return _dominioRepository.GetDominios(CodigoDominioDb.Idioma);

        }

        public virtual DominioDetalle GetIdioma(string id)
        {
            return _dominioRepository.GetDominios(CodigoDominioDb.Idioma)
                .FirstOrDefault(i => i.Valor == id);
        }

        #endregion

        #region Add

        public virtual void AddTranslation(TranslatedValue translation)
        {
            var translationEntity = this._mapper.MapToEntity(translation);

            this._context.T_LOCALIZACION.Add(translationEntity);
        }

        public virtual void AddTranslationVersion(TranslationVersion version)
        {
            var versionEntity = this._mapper.MapToEntity(version);

            this._context.T_LOCALIZACION_VERSION.Add(versionEntity);
        }

        #endregion

        #region Update

        public virtual void UpdateTranslation(TranslatedValue translation)
        {
            var entity = this._mapper.MapToEntity(translation);
            var attachedEntity = _context.T_LOCALIZACION.Local
                .FirstOrDefault(l => l.CD_APLICACION == entity.CD_APLICACION
                    && l.CD_BLOQUE == entity.CD_BLOQUE
                    && l.CD_TIPO == entity.CD_TIPO
                    && l.CD_CLAVE == entity.CD_CLAVE
                    && l.CD_IDIOMA == entity.CD_IDIOMA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_LOCALIZACION.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void UpdateTranslationVersion(TranslationVersion version)
        {
            var entity = this._mapper.MapToEntity(version);
            var attachedEntity = _context.T_LOCALIZACION_VERSION.Local
                .FirstOrDefault(l => l.CD_IDIOMA == entity.CD_IDIOMA);

            if (attachedEntity != null)
            {
                var attachedEntry = _context.Entry(attachedEntity);
                attachedEntry.CurrentValues.SetValues(entity);
                attachedEntry.State = EntityState.Modified;
            }
            else
            {
                this._context.T_LOCALIZACION_VERSION.Attach(entity);
                this._context.Entry(entity).State = EntityState.Modified;
            }
        }

        #endregion

        #region Remove

        #endregion

        #region Dapper

        #endregion
    }
}
