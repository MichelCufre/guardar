using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class TipoContenedor
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public int RangoInicial { get; set; }
        public int RangoFinal { get; set; }
        public int? UltimaSecuencia { get; set; }
        public bool ClientePredefinido { get; set; }
        public string TipoEnvase { get; set; }
        public bool Retornable { get; set; }
        public string TpObjetoTracking { get; set; }
        public bool Habilitado { get; set; }
        public string  NombreSecuencia { get; set; }
    }
}
