using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Translation
{
    public class TranslationConfiguration
    {
        public const string BaseLanguage = "base";
        public const string FallbackCulture = "es";
        protected readonly string FileNameTemplate = "translation.json";
        protected readonly string LanguagesFileName = "languages.json";
        protected readonly string BasePath;
        protected readonly string TempPath;
        protected readonly string BackupPath;

        public TranslationConfiguration(string basePath)
        {
            this.BasePath = basePath;
            this.TempPath = Path.Combine(this.BasePath, "tmp");
            this.BackupPath = Path.Combine(this.BasePath, "bkp");
        }
        
        public string GetLanguageFileName()
        {
            return this.LanguagesFileName;
        }

        public string GetCultureName(CultureInfo culture)
        {
            return (culture != null && culture.Name.Length > 0 ? culture.Name : BaseLanguage);
        }

        public string GetCultureFileName(CultureInfo culture)
        {
            return (culture != null && !string.IsNullOrEmpty(culture.Name) ? culture.Name : BaseLanguage);
        }

        public string GetCultureFileLocation(CultureInfo culture)
        {
            return string.Join(@"\\", new List<string>
            {
                this.GetBasePath(),
                this.GetCultureFileName(culture),
                this.FileNameTemplate
            });
        }

        public string GetTempLocation(CultureInfo culture)
        {
            DateTime fechaActual = DateTime.Now;

            return string.Join(@"\\", new List<string>
            {
                this.GetTempPath(),
                this.GetCultureFileName(culture),
                this.FileNameTemplate + "." + fechaActual.ToString("yyyyMMdd_hms")
            });
        }

        public string GetBackupLocation(CultureInfo culture)
        {
            DateTime fechaActual = DateTime.Now;
            
            return string.Join(@"\\", new List<string>
            {
                this.GetBackupPath(),
                this.GetCultureFileName(culture),
                this.FileNameTemplate + "." + fechaActual.ToString("yyyyMMdd_hms")
            });
        }

        public string GetBasePath()
        {
            if (!Directory.Exists(this.BasePath))
                Directory.CreateDirectory(this.BasePath);

            return this.BasePath;
        }

        public string GetLanguageFileLocation()
        {
            return string.Join(@"\\", new List<string>{
                this.GetBasePath(),
                this.LanguagesFileName
            });
        }

        public string GetTempPath()
        {
            if (Directory.Exists(this.TempPath))
                Directory.Delete(this.TempPath,true);
                
            Directory.CreateDirectory(this.TempPath);

            return this.TempPath;
        }

        public string GetBackupPath()
        {
            if (Directory.Exists(this.BackupPath))
                Directory.Delete(this.BackupPath,true);
                
            Directory.CreateDirectory(this.BackupPath);
            
            return this.BackupPath;
        }
    }
}
