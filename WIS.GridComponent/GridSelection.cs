using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.GridComponent
{
    public class GridSelection
    {
        public const string Separator = "$";
        public bool AllSelected { get; set; }
        public List<string> Keys { get; set; }

        public GridSelection()
        {
            this.Keys = new List<string>();
        }

        public List<Dictionary<string, string>> GetSelection(List<string> keys)
        {
            var result = new List<Dictionary<string, string>>();

            foreach (string key in this.Keys)
            {
                string[] fragments = key.Split(Separator[0]);

                if (fragments.Length != keys.Count)
                    throw new InvalidOperationException("Cantidad de keys esperadas en la selección no coincide con cantidad obtenida");

                var dictionary = new Dictionary<string, string>();

                int index = 0;

                foreach (string fragmentKey in keys)
                {
                    dictionary[fragmentKey] = fragments[index];

                    index++;
                }

                result.Add(dictionary);
            }

            return result;
        }
        public List<T> GetSelection<T>(List<string> keys, Func<Dictionary<string, string>, T> method)
        {
            var result = new List<T>();

            List<Dictionary<string, string>> groups = this.GetSelection(keys);

            foreach (var group in groups)
            {
                result.Add(method(group));
            }

            return result;
        }
    }
}
