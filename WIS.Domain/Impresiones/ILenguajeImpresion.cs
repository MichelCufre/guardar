using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Impresiones
{
    public interface ILenguajeImpresion
    {
        string Id { get; set; }
        string Descripcion { get; set; }


        string CrearLenguajeImpresion(Dictionary<string, string> lenguaje);

    }
}
