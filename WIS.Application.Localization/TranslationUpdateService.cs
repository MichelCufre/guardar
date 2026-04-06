using System;
using System.Collections.Generic;
using System.Linq;
using WIS.Domain.DataModel;
using WIS.Translation;

namespace WIS.Application.Localization
{
    public class TranslationUpdateService : ITranslationUpdateService
    {
        private static readonly string[] _nonUpdateableResources = { "General_Sec0_lbl_CustomerBrand" }; //TODO: Refactor, quitar de aca

        protected readonly IUnitOfWorkFactory _uowFactory;
        protected readonly ITranslationResourceProvider _resourceProvider;

        public TranslationUpdateService(IUnitOfWorkFactory uowFactory, ITranslationResourceProvider resourceProvider)
        {
            this._uowFactory = uowFactory;
            this._resourceProvider = resourceProvider;
        }

        public virtual List<TranslationVersion> GetAllVersions(string application, int userId)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.LocalizationRepository.GetAllVersions();
        }

        public virtual void UpdateDatabaseResources(string application, int userId)
        {
            List<TranslatedValue> resources = this._resourceProvider.GetResources();

            using var uow = this._uowFactory.GetUnitOfWork();
            
            List<TranslatedValue> baseResources = uow.LocalizationRepository.GetTranslationByLanguage(TranslationConfiguration.BaseLanguage);

            this.UpdateResources(uow, baseResources, resources);

            uow.SaveChanges();
        }

        public virtual List<TranslatedValue> GetResources(string application, int userId, string language)
        {
            using var uow = this._uowFactory.GetUnitOfWork();

            return uow.LocalizationRepository.GetTranslationByLanguage(language);
        }

        public virtual void UpdateUserLanguage(int userId, string language)
        {
            /*try
            {
                if (!string.IsNullOrEmpty(language))
                {
                    //RUsuario dbUsuario = new RUsuario();

                    using (var context = new WISBASEDB())
                    //using (WSEGDB context = new WSEGDB())
                    {
                        this.UpdateLanguage(context, userId, language);
                        //dbUsuario.UpdateLanguage(context, userId, language);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                //Do no harm
            }*/
        }

        public virtual void UpdateResources(IUnitOfWork uow, List<TranslatedValue> baseResources, List<TranslatedValue> resources)
        {
            bool change = false;
            foreach (var resource in resources.Where(d => !_nonUpdateableResources.Contains(d.Key)))
            {
                try
                {
                    TranslatedValue baseResource = baseResources.Where(d => d.Key == resource.Key).FirstOrDefault();

                    if (baseResource != null)
                    {
                        if (baseResource.Value != resource.Value)
                        {
                            uow.LocalizationRepository.UpdateTranslation(resource);

                            change = true;
                        }
                    }
                    else
                    {
                        uow.LocalizationRepository.AddTranslation(resource);

                        change = true;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(resource.Key + "$" + ex.InnerException?.Message ?? ex.Message);
                }
            }

            if (change)
                this.UpdateVersion(uow);

        }

        public virtual void UpdateVersion(IUnitOfWork uow)
        {
            TranslationVersion languageVersion = uow.LocalizationRepository.GetVersionByLanguage(TranslationConfiguration.BaseLanguage);

            if (languageVersion != null)
            {
                languageVersion.IncreaseVersion();
                uow.LocalizationRepository.UpdateTranslationVersion(languageVersion);
            }
        }
    }
}
