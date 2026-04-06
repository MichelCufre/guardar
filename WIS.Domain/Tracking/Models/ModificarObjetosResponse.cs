using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Tracking.Models
{
    public class ModificarObjetosResponse
    {
        public List<ModificarObjetoResponse> Objetos { get; set; }

        public ModificarObjetosResponse()
        {
            Objetos = new List<ModificarObjetoResponse>();
        }

    }
    public class ModificarObjetoResponse
    {
        public string IdExternoObjeto { get; set; }
        public string Accion { get; set; }
        public bool ModificacionExitosa { get; set; }
        public string Mensaje { get; set; }
    }
}
