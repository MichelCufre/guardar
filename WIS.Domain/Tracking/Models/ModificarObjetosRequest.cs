using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WIS.Domain.Validation;

namespace WIS.Domain.Tracking.Models
{
    public class ModificarObjetosRequest
    {
        public List<ModificarObjetoRequest> Objetos { get; set; }

        public ModificarObjetosRequest()
        {
            Objetos = new List<ModificarObjetoRequest>();
        }
    }

    public class ModificarObjetoRequest
    {
        public string IdExternoObjeto { get; set; }
        public string Accion { get; set; }
        public ObjetoRequest Objeto { get; set; }

    }

    public class ObjetoRequest
    {
        public string Numero { get; set; }
        public string Descripcion { get; set; }
        public string Tipo { get; set; }
        public string CodigoBarras { get; set; }
        public decimal? Cantidad { get; set; }
        public decimal? Volumen { get; set; }
        public decimal? Peso { get; set; }
        public decimal? Alto { get; set; }
        public decimal? Largo { get; set; }
        public decimal? Profundidad { get; set; }
    }
}
