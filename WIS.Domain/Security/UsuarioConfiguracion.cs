using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Security
{
    public class UsuarioConfiguracion
    {
        public int IdUsuario { get; set; }                              //USERID
        public string AsignarAutoNuevasEmpresas { get; set; }           //FL_ASIG_AUTO_NUEVA_EMPRESA
        public DateTime? FechaAlta { get; set; }                        //DT_ADDROW
        public DateTime? FechaModificacion { get; set; }                //DT_UPDROW
    }
}
