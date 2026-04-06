using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class PaisSubdivision
    {
        public string Id { get; set; } //CD_SUBDIVISION
        public string Nombre { get; set; } //NM_SUBDIVISION
        public string IdPais { get; set; } //CD_PAIS
        public Pais Pais { get; set; }
        List<PaisSubdivisionLocalidad> Localidades { get; set; }

        public PaisSubdivision()
        {
            Localidades = new List<PaisSubdivisionLocalidad>();
        }
    }
}
