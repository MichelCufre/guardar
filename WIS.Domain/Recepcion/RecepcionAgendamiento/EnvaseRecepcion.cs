using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Recepcion.RecepcionAgendamiento
{
    public class EnvaseRecepcion
    {
        public string Codigo { get; set; }

        public decimal Faixa { get; set; }

        public int IdEmpresa { get; set; }

        public int IdAgenda { get; set; }

        public decimal Cantidad { get; set; }
    }
}
