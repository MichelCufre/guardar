using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WIS.Domain.General
{
    public class Aplicacion
    {
        public string Codigo { get; set; }          //CD_APLICACION
        public string Descripcion { get; set; }     //DS_APLICACION
        public bool ManejaRecorrido { get; set; }   //FL_RECORRIDO
    }
}
