using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Security.Enums;

namespace WIS.Domain.Security
{
    public class UsuarioData
    {
        public int UserId { get; set; }//USERID
        public bool IsLocked { get; set; } //ISLOCKED
        public string Password { get; set; }//PASSWORD
        public string IdTenant { get; set; }//ID_TENANT
        public string PasswordSalt { get; set; }//PASSWORDSALT
        public decimal? CausaBloqueo { get; set; }//LOCKCAUSE
        public DateTime? FechaUltimoLogin { get; set; }//LASTLOGINDATETIME
        public DateTime? FechaUltimoBloqueo { get; set; }//LASTLOCKOUTDATETIME
        public FormatoContrasenia FormatoPassword { get; set; }//PASSWORDFORMAT
        public DateTime? FechaUltimoCambioPassword { get; set; }//LASTPASSWORDCHANGEDATETIME
        public decimal? NroIntentosContraseñaErronea { get; set; }//FAILEDPASSWORDATTEMPTCOUNT
        public DateTime? FechaUltimoIntentoIncorrectoLogin { get; set; }//FAILEDPASSWORDATTEMPTWINSTART


    }
}
