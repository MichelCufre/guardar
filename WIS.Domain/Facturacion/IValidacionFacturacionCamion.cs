using System.Collections.Generic;
using WIS.Domain.Expedicion;

namespace WIS.Domain.Facturacion
{
    public interface IValidacionFacturacionCamion
    {
        List<ValidacionCamionResultado> Validar(Camion camion);
    }
}