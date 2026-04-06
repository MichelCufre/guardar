using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Configuracion;

namespace WIS.Domain.Registro
{
    public class PuertaEmbarqueConfiguracion
    {
        public string PrefijoPuerta { get; set; }
        public string Clase { get; set; }
        public short Rotatividad { get; set; }
        public int FamiliaPrincipal { get; set; }

    }
}
