using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Security.Enums;

namespace WIS.Domain.Security
{
    public class UsuarioContraseniaHistorica
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public int PasswordUserId { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public FormatoContrasenia FormatoPassword { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public string Anexo { get; set; }



    }
}
