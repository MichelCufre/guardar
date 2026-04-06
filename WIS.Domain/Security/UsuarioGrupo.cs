using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Security
{
    public class UsuarioGrupo
    {
        public string Codigo { get; set; }
        public string Descripcion { get; set; }

        public UsuarioGrupo(string codigo, string descripcion)
        {
            Codigo = codigo;
            Descripcion = descripcion;
        }
    }
}
