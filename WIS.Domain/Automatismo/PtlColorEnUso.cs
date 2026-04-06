using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Automatismo
{
    public class PtlColorEnUso
    {
        public string Color { get; set; }
        public int UserId { get; set; }
        public int Ptl { get; set; }

        public DateTime FechaRegistro { get; set; }
        public DateTime FechaUltimaAccion { get; set; }
        public long Transaccion { get; set; }
    }
}
