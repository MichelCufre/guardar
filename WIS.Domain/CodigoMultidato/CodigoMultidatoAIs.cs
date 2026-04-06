using System.Collections.Generic;

namespace WIS.Domain.CodigoMultidato
{
    public class CodigoMultidatoAIs
    {
        public string CodigoMultidato { get; set; }
        public int Empresa { get; set; }
        public string Aplicacion { get; set; }
        public Dictionary<string, object> AIs { get; set; }
    }
}
