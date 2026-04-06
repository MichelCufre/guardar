using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.General
{
    public class Dominio
    {
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public string FlInterno { get; set; }

        public List<DominioDetalle> Detalles { get; set; }

        public Dominio()
        {
            Detalles = new List<DominioDetalle>();
        }
    }
}
