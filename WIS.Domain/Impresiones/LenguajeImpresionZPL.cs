using System;
using System.Collections.Generic;

namespace WIS.Domain.Impresiones
{
    public class LenguajeImpresionZPL : ILenguajeImpresion
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }

        public virtual string CrearLenguajeImpresion(Dictionary<string, string> lenguaje)
        {
            throw new NotImplementedException();
        }
    }
}
