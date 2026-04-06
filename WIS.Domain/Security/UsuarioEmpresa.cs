using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Security
{
    public class UsuarioEmpresa
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public UsuarioEmpresa(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
    }
}
