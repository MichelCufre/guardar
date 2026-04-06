using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Picking
{
    public class LiberacionOndaParametrosCalculados
    {
        public decimal CantidadLineas { get; set; }
        public decimal Peso { get; set; }
        public decimal VolumenTotal { get; set; }
        public decimal VolumenFinal { get; set; }
        public decimal Unidades { get; set; }
        public int FilasSeleccionadas { get; set; }

        public LiberacionOndaParametrosCalculados()
        {
            Peso = 0;
            Unidades = 0;
            VolumenTotal = 0;
            VolumenFinal = 0;
            CantidadLineas = 0;
            FilasSeleccionadas = 0;
        }
    }
}
