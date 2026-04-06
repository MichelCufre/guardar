using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class PaisSubdivisionLocalidad
    {
        public long Id { get; set; } //ID_LOCALIDAD
        public string Codigo { get; set; } //CD_LOCALIDAD
        public string CodigoSubDivicion { get; set; } //CD_SUBDIVISION
        public string Nombre { get; set; } //NM_LOCALIDAD
        public string CodigoIATA { get; set; } //CD_IATA
        public string CodigoPostal { get; set; }//CD_POSTAL

        public PaisSubdivision Subdivision { get; set; }
    }
}
