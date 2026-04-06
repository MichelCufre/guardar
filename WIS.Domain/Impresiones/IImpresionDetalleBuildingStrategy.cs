using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WIS.Domain.Impresiones
{
    public interface IImpresionDetalleBuildingStrategy
    {
        List<DetalleImpresion> Generar(Impresora impresora);
    }
}
