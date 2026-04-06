using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class ClienteDiasValidezVentana
    {
        public string Cliente { get; set; }

        public int Empresa { get; set; }

        public short CantidadDiasValidezLiberacion { get; set; }

        public string VentanaLiberacion { get; set; }

        public DateTime? FechaModificacion { get; set; }

        public DateTime? FechaAlta { get; set; }
    }
}
