using System;
using System.Collections.Generic;
using System.Text;
using WIS.Translation;

namespace WIS.Application.Localization
{
    public interface ITranslationUpdateService
    {
        List<TranslationVersion> GetAllVersions(string application, int userId);
        void UpdateDatabaseResources(string application, int userId);
        List<TranslatedValue> GetResources(string application, int userId, string language);
        void UpdateUserLanguage(int userId, string language);
    }
}
