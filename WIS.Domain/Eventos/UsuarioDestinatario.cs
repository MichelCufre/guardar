using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Eventos
{
    public partial class UsuarioDestinatario : Destinatario
    {
        public UsuarioDestinatario()
        {
            this.TipoDestinatario = TipoDestinatario.Funcionario;
        }

        public string NombreUsuario { get; set; }
        public string Email { get; set; }
        public bool IsEnabled { get; set; }
        public string Language { get; set; }
        public int Equipo { get; set; }
        public string Predio { get; set; }
        public string SessionTokenWeb { get; set; } //Web
        public string SessionToken { get; set; } //Colectores

    }
}
