using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Security
{
    public class Perfil
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int? Tipo { get; set; }
        public List<Resource> Recursos {get;set;}

        public Perfil()
        {
            this.Recursos = new List<Resource>();
        }
    }
}
