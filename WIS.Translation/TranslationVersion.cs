using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Translation
{
    public class TranslationVersion
    {
        public string Language { get; set; }
        public int Version { get; set; }
        public DateTime? LastEdited { get; set; }

        public void IncreaseVersion()
        {
            this.Version += 1;
            this.LastEdited = DateTime.Now;
        }
    }
}
