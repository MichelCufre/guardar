using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.Security
{
    public class Usuario
    {
        public int UserId { get; set; }//USERID
        public string Username { get; set; }//LOGINNAME
        public string Name { get; set; }//FULLNAME
        public string Email { get; set; }//EMAIL
        public bool IsEnabled { get; set; }//ISENABLED
        public string Language { get; set; }//LANGUAGE

        public List<UsuarioEmpresa> Empresas { get; set; }
        public List<UsuarioPermiso> Permisos { get; set; }
        public List<UsuarioGrupo> Grupos { get; set; }
        public string SessionTokenWeb { get; set; } //SESSIONTOKENWEB - Web
        public string SessionToken { get; set; } //SESSIONTOKEN - Colectores
        public bool SincronizacionRealizada { get; set; } //FL_SYNC_REALIZADA
        public string DomainName { get; set; }//DOMAINNAME
        public int? TipoUsuario { get; set; }//USERTYPEID

        public Usuario()
        {
            this.Empresas = new List<UsuarioEmpresa>();
            this.Permisos = new List<UsuarioPermiso>();
            this.Grupos = new List<UsuarioGrupo>();
        }
    }
}
