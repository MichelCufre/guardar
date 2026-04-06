using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.General.Enums;

namespace WIS.Domain.General.Auxiliares
{
    public class AuxContenedor
    {
        public int NuContenedor { get; set; }

        public string TipoContenedor { get; set; }

        public int NuPreparacion { get; set; }

        public EstadoContenedor Estado { get; set; }

        public string Ubicacion { get; set; }

        public string NuIngresoProduccion { get; set; }

        public string IdExternoContenedor { get; set; }

        public string CodigoBarras { get; set; }

        public long? NroLpn { get; set; }

        public bool ExisteContenedorActivo { get; set; }

        public AuxContenedor(string codigoBarras)
        {
            CodigoBarras = codigoBarras;
            NuContenedor = -1;
            NuPreparacion = -1;
            ExisteContenedorActivo = false;
        }

    }
}
