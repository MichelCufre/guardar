using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WIS.HttpLegacy.WebApi;
using WIS.Translation;
using WIS.Serialization;
using WIS.Translation.Serialization;

namespace WIS.LocalizationImportProcess.Logic
{
    public class LocalizationService
    {
        private readonly TranslationConfiguration _configuration;
        private readonly IWebApiClientLegacy _webClient;
        private readonly LocalizationSettings _settings;
        private readonly string VersionToken = "translationVersion";

        public LocalizationService(IWebApiClientLegacy webClient, LocalizationSettings settings)
        {
            this._configuration = new TranslationConfiguration(settings.LocalePath);
            this._webClient = webClient;
            this._settings = settings;
        }

        public async Task ImportResources(CancellationToken cancelToken)
        {
            try
            {

                var versions = await this.GetAllVersions(cancelToken);

                //Obtener diferencias de versiones
                TranslationVersion baseVerNew = this.GetDiffVersion(versions, null);

                if (baseVerNew != null)
                    this.GenerateFile(null, await this.GenerateTemp(null, baseVerNew.Version, cancelToken));

                foreach (var version in versions.Where(d => d.Language != "base"))
                {
                    var culture = CultureInfo.GetCultureInfo(version.Language);

                    TranslationVersion cultureVerNew = this.GetDiffVersion(versions, culture);

                    if (cultureVerNew != null)
                        this.GenerateFile(culture, await this.GenerateTemp(culture, cultureVerNew.Version, cancelToken));
                }

                this.GenerateLanguageList(versions);
            }
            catch (Exception ex)
            {
                throw ex;
                //Do no harm
            }
        }
        public async Task UpdateDatabaseResources(CancellationToken cancelToken)
        {
            var transferObject = new TranslationWrapper();

            transferObject.Application = "Translation";
            transferObject.User = 0;

            await this._webClient.PostAsync(this._settings.ServiceUri, "Translation", "UpdateDatabaseResources", transferObject, cancelToken);
        }

        private async Task<string> GenerateTemp(CultureInfo culture, int newVersion, CancellationToken cancelToken)
        {
            string tempFile = string.Empty;

            //Actualizar archivo de idioma base
            var translations = await this.GetResources(this._configuration.GetCultureName(culture), cancelToken);

            if (translations != null)
            {
                var content = new Dictionary<string, string>
                {
                    [this.VersionToken] = newVersion.ToString()
                };

                foreach (var translation in translations)
                {
                    content[translation.Key] = translation.Value;
                }

                tempFile = this._configuration.GetTempLocation(culture);

                new FileInfo(tempFile).Directory.Create();

                File.WriteAllText(tempFile, JsonConvert.SerializeObject(content), Encoding.UTF8);
            }

            return tempFile;
        }
        private void GenerateFile(CultureInfo culture, string tempFile)
        {
            if (!string.IsNullOrEmpty(tempFile))
            {
                string destinationFile = this._configuration.GetCultureFileLocation(culture);
                string backupPath = this._configuration.GetBackupLocation(culture);

                if (File.Exists(destinationFile))
                {
                    new FileInfo(backupPath).Directory.Create();

                    File.Replace(tempFile, destinationFile, backupPath);
                }
                else
                {
                    new FileInfo(destinationFile).Directory.Create();

                    File.Move(tempFile, destinationFile);
                }
            }
        }
        private void GenerateLanguageList(List<TranslationVersion> versions)
        {
            if (versions.Count > 0)
            {
                var languageList = new List<TranslationLanguage>();

                foreach (var version in versions.Where(d => d.Language != TranslationConfiguration.BaseLanguage))
                {
                    languageList.Add(new TranslationLanguage
                    {
                        Language = version.Language,
                        Name = new CultureInfo(version.Language)?.DisplayName
                    });
                }

                var content = JsonConvert.SerializeObject(new
                {
                    languages = languageList
                });

                File.WriteAllText(this._configuration.GetLanguageFileLocation(), content);
            }
        }

        private TranslationVersion GetDiffVersion(List<TranslationVersion> versions, CultureInfo culture)
        {
            int fileVersion = this.GetFileVersion(this._configuration.GetCultureFileLocation(culture));

            return versions.Where(v => v.Language == this._configuration.GetCultureName(culture) && v.Version > fileVersion).FirstOrDefault();
        }
        private int GetFileVersion(string filePath)
        {
            int version = -1;

            try
            {
                using (var fs = File.OpenRead(filePath))
                using (var textReader = new StreamReader(fs))
                using (var reader = new JsonTextReader(textReader))
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.PropertyName && this.VersionToken == (string)reader.Value)
                        {
                            reader.Read();

                            version = Convert.ToInt32((string)reader.Value);

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //TODO: Log
            }

            return version;
        }

        private async Task<List<TranslatedValue>> GetResources(string language, CancellationToken cancelToken)
        {
            var transferObject = new TranslationWrapper();

            transferObject.Application = "Translation";
            transferObject.User = 0;

            transferObject.SetData(language);

            var response = await this._webClient.PostAsync(this._settings.ServiceUri, "Translation", "GetResources", transferObject, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

            var result = JsonConvert.DeserializeObject<TranslationWrapper>(await response.Content.ReadAsStringAsync());

            return result.GetData<List<TranslatedValue>>();

        }
        private async Task<List<TranslationVersion>> GetAllVersions(CancellationToken cancelToken)
        {

            var transferObject = new TranslationWrapper();

            transferObject.Application = "Translation";
            transferObject.User = 0;

            var response = await this._webClient.PostAsync(this._settings.ServiceUri, "Translation", "GetAllTranslationVersions", transferObject, cancelToken);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Error, status: " + response.StatusCode + " - " + response.ReasonPhrase + "-" + await response.Content.ReadAsStringAsync());

            var result = JsonConvert.DeserializeObject<TranslationWrapper>(await response.Content.ReadAsStringAsync());

            return result.GetData<List<TranslationVersion>>();

        }
    }
}
