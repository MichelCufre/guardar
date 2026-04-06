using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class Pais
    {
        public string Id { get; set; } //CD_PAIS
        public string Nombre { get; set; } // DS_PAIS
        public string CodigoAlfa3 { get; set; } //CD_PAIS_ALFA3

        public DateTime? Alta { get; set; } //DT_ADDROW
        public DateTime? Modificacion { get; set; } //DT_UPDROW


        List<PaisSubdivision> Subdivisiones { get; set; }

        public Pais()
        {
            Subdivisiones = new List<PaisSubdivision>();
        }
    }
}
