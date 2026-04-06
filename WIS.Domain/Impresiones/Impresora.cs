
using System;
using System.Collections.Generic;

namespace WIS.Domain.Impresiones
{
    public class Impresora
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public string Predio { get; set; }
        public string CodigoLenguajeImpresion { get; set; }
        public ILenguajeImpresion LenguajeImpresion { get; set; }
        public DateTime FechaAlta { get; set; }
        public int? Servidor { get; set; }

        public virtual string CrearDatosImpresion(Dictionary<string, string> DatosImpresion)
        {
            return string.Empty;
        }

        public virtual ILenguajeImpresion GetLenguaje()
        {
            return this.LenguajeImpresion;
        }
    }
}
