using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Inventario
{
    public class InventarioFiltros
    {
        public decimal NuInventario { get; set; }

        public int? Empresa { get; set; }

        public string Predio { get; set; }

        public bool ExcluirLpns { get; set; }

        public bool ExcluirSueltos { get; set; }

        public bool PermiteUbicacionesDeOtrosInventarios { get; set; }

        public Dictionary<int, string> AtributosCabezal { get; set; }

        public Dictionary<int, string> AtributosDetalle { get; set; }

        public InventarioFiltros()
        {
            AtributosCabezal = new Dictionary<int, string>();
            AtributosDetalle = new Dictionary<int, string>();
        }
    }
}
