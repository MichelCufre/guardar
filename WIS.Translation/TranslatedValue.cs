using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Translation
{
    public class TranslatedValue
    {
        public string ResourceType { get; set; }
        public string Key { get; set; }
        public string Language { get; set; }
        public string Value { get; set; }

        public TranslatedValue()
        {
        }
    }
}
