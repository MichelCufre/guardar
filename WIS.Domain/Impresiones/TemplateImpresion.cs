using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WIS.Domain.Impresiones
{
    public class TemplateImpresion
    {
        private const string FieldPrefixTag = "<WIS><CAMPO>";
        private const string FieldSuffixTag = "</CAMPO></WIS>";
        private const string TextPrefixTag = "<WIS><TEXTO>";
        private const string TextSuffixTag = "</TEXTO></WIS>";

        protected string Contenido { get; set; }
        public string CodigoLenguajeImpresion { get; set; }
        public string PreCuerpo { get; set; }
        public string PostCuerpo { get; set; }
        public decimal? Altura { get; set; }
        public decimal? Ancho { get; set; }
        public string EstiloEtiqueta { get; set; }

        public ILenguajeImpresion LenguajeImpresion { get; set; }
        public EtiquetaEstilo EtiquetaEstilo { get; set; }

        public TemplateImpresion(string contenido)
        {
            this.Contenido = contenido;
        }

        public virtual string GetContenido()
        {
            return this.Contenido;
        }

        public virtual string Parse(Dictionary<string, string> claves)
        {
            string contenido = this.ReplaceFieldTags(this.Contenido, claves);

            return this.ReplaceTextTags(contenido);
        }

        public virtual string ReplaceFieldTags(string contenido, Dictionary<string, string> claves)
        {
            string valorInsertar;

            foreach (var clave in claves)
            {
                valorInsertar = clave.Value;

                if (clave.Value == null)
                    valorInsertar = string.Empty;

                contenido = Regex.Replace(contenido, $"{FieldPrefixTag}{clave.Key}{FieldSuffixTag}", d => valorInsertar, RegexOptions.IgnoreCase);
            }

            return contenido;
        }
        public virtual string ReplaceTextTags(string content)
        {
            Regex regex = new Regex($"{TextPrefixTag}(.*){TextSuffixTag}");

            var matches = regex.Matches(content);

            string processedContent = content;

            foreach (Match match in matches)
            {
                Regex regexAux = new Regex(match.Groups[0].Value);

                processedContent = regexAux.Replace(processedContent, match.Groups[1].Value);
            }

            return processedContent;
        }
    }

}
