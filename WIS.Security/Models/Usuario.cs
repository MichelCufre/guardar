using System.Collections.Generic;

namespace WIS.Security.Models
{
    public class Usuario
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsEnabled { get; set; }
        public string Language { get; set; }
        public int Equipo { get; set; }
        public string Predio { get; set; }
        public List<UsuarioEmpresa> Empresas { get; set; }
        public List<UsuarioPermiso> Permisos { get; set; }
        public List<UsuarioGrupo> Grupos { get; set; }
        public List<UsuarioPredio> Predios { get; set; }
        public string SessionTokenWeb { get; set; } //Web
        public string SessionToken { get; set; } //Colectores

        public Usuario()
        {
            this.Empresas = new List<UsuarioEmpresa>();
            this.Permisos = new List<UsuarioPermiso>();
            this.Grupos = new List<UsuarioGrupo>();
            this.Predios = new List<UsuarioPredio>();
        }
    }
}
