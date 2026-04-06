using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General
{
    public class PuertaEmbarque
    {
        public short Id { get; set; }
        public string Descripcion { get; set; }
        public short? Estado { get; set; }
        public string Tipo { get; set; }
        public string NumPredio { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string CodigoUbicacion { get; set; }

        public Ubicacion Ubicacion { get; set; }
        public Predio Predio { get; set; }
        public List<Ruta> Rutas { get; set; }

    }
}
