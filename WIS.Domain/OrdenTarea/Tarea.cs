using System;
using System.Collections.Generic;
using System.Text;

namespace WIS.Domain.OrdenTarea
{
    public class Tarea
    {
        public string Id { get; set; }

        public string Descripcion { get; set; }

        public string TipoTarea { get; set; }

        public short CodigoSituacion { get; set; }

        public string NumeroComponente { get; set; }

        public string RegistroHoras { get; set; }

        public string RegistroManipuleo { get; set; }

        public string RegistroInsumos { get; set; }

        public string RegistroHorasEquipo { get; set; }
    }
}
